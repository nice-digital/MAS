const keystone = require("keystone"),
	moment = require("moment"),
	https = require("https"),
	http = require("http"),
	log4js = require("log4js"),
	utils = require("keystone-utils"),
	_ = require("lodash");

const Types = keystone.Field.Types;

const logger = log4js.getLogger("Item.js");
logger.level = "debug";

const Item = new keystone.List("Item", {
	map: { name: "title" },
	track: true,
	autokey: { path: "slug", from: "title", unique: true, fixed: true },
	defaultSort: "-createdAt"
});

Item.add({
	title: {
		type: Types.Text,
		required: true,
		initial: true
	},
	url: {
		type: Types.Url,
		label: "URL",
		initial: true,
		note: "The URL needs to contain http:// or https://"
	},
	source: {
		type: Types.Relationship,
		ref: "Source",
		many: false,
		required: true,
		initial: true
	},
	evidenceType: {
		type: Types.Relationship,
		ref: "EvidenceType",
		initial: true,
		required: true,
		many: false
	},
	shortSummary: {
		type: Types.Textarea,
		required: false,
		initial: false,
		max: 281,
		label: "Short summary"
	},
	publicationDate: {
		type: Types.Date,
		label: "Publication date"
	},
	specialities: {
		type: Types.Relationship,
		ref: "Speciality",
		many: true,
		label: "Specialities"
	},
	comment: {
		type: Types.Html,
		label: "SPS comment",
		wysiwyg: true
	},
	resourceLinks: {
		type: Types.Html,
		label: "Resource links",
		wysiwyg: true,
		required: false
	},
	isInitial: {
		type: Types.Boolean,
		hidden: true,
		default: true
	},
	relevancy: {
		type: Types.Select,
		numeric: true,
		options: [
			{ value: 1, label: "High" },
			{ value: 2, label: "Medium" },
			{ value: 3, label: "Low" }
		],
		label: "Weekly relevancy score"
	},
	weekly: {
		type: Types.Relationship,
		ref: "Weekly",
		label: "Weekly newsletter",
		note: "For use by the NICE MPT only"
	},
	commentUrl: {
		type: Types.Text,
		watch: "title",
		value: function() {
			return (
				process.env.STATIC_SITE_PATH +
				(this.slug || utils.slug(this.title)) +
				".html"
			);
		},
		noedit: true,
		label: "SPS comment URL"
	}
});

Item.fullResponseFields = [
	"_id",
	"title",
	"slug",
	"url",
	"shortSummary",
	"comment",
	"resourceLinks",
	"commentUrl",
	"source._id",
	"source.title",
	"specialities",
	"evidenceType._id",
	"evidenceType.title",
	"evidenceType.key",
	"evidenceType.broaderTitle",
	"publicationDate",
	"updatedAt",
	"createdAt"
];

Item.schema.pre("validate", function(next) {
	if (this.isInitial) {
		this.isInitial = false;
		next();
	} else {
		if (!this.shortSummary) {
			next(Error("Short summary is required."));
		} else if (!this.relevancy) {
			next(Error("Relevancy score is required."));
		} else {
			next();
		}
	}
});

const createWeeklyIfNeeded = async () => {
	const sendDateMoment = moment()
		.startOf("isoweek")
		.add(7, "days");

	const WeeklyModel = keystone.list("Weekly").model;

	const weeklyEntity = await WeeklyModel.findOne({
		sendDate: sendDateMoment.toDate()
	}).exec();

	if (!weeklyEntity) {
		const startDateMoment = sendDateMoment.clone().subtract(1, "week"),
			endDateMoment = startDateMoment.clone().add(4, "days"),
			dateFormat = "Do MMMM YYYY";

		var newWeekly = new WeeklyModel({
			title:
				startDateMoment.format(dateFormat) +
				" to " +
				endDateMoment.format(dateFormat),
			sendDate: sendDateMoment.toDate(),
			startDate: startDateMoment.toDate(),
			endDate: endDateMoment.toDate()
		});
		await newWeekly.save();
	}
};

// Post save hook to trigger a lambda with the document details
Item.schema.post("save", async function(doc, next) {
	await createWeeklyIfNeeded();

	logger.info("Post save, sending request...");

	let item;

	try {
		item = await keystone
			.list("Item")
			.model.findById(doc._id)
			.select(
				"_id title slug url shortSummary comment resourceLinks commentUrl publicationDate updatedAt createdAt"
			)
			.populate({ path: "source", select: "_id title" })
			.populate("evidenceType")
			.populate({ path: "specialities", select: "_id title key broaderTitle" })
			.exec();
	} catch (err) {
		logger.error("An error occurred finding item: ", err.message);
		return next(new Error(`An error occurred finding item: ${err.message}`));
	}

	const contentpath = process.env.CONTENT_PATH;
	const hostname = process.env.HOST_NAME;
	const hostport = process.env.HOST_PORT;

	const data = JSON.stringify(_.pick(item, Item.fullResponseFields));
	logger.debug("Sending: ", data);

	var options = {
		hostname: hostname,
		port: hostport,
		path: contentpath,
		method: "PUT",
		headers: {
			"Content-Type": "application/json"
		}
	};

	if (hostport === "443") options.secureProtocol = "TLSv1_2_method";
	else options.headers.host = "localhost";

	const req = (hostport === "443" ? https : http).request(options, res => {
		if (res.statusCode >= 200 && res.statusCode <= 299) {
			next();
		} else {
			logger.error("Post save PUT request error: Status code ", res.statusCode);
			next(new Error(`An error has occurred. Status code ${res.statusCode}`));
		}
	});

	req.on("error", error => {
		logger.error("Post save PUT request error :", error);
		next(new Error(`An error has occurred : ${error}`));
	});

	req.write(data);
	req.end();

	logger.info("...sent PUT request", options);
});

Item.defaultColumns = "title, source, relevancy, createdAt";
Item.register();
