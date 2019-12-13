const keystone = require("keystone"),
	moment = require("moment"),
	https = require("https"),
	http = require("http"),
	log4js = require("log4js");;

const Types = keystone.Field.Types;

const logger = log4js.getLogger("Item.js");
logger.level = "debug";

const Item = new keystone.List("Item", {
	map: { name: "title" },
	autokey: { path: "slug", from: "title", unique: true }
});

var shouldPostLambda = false;

if (this.isInitial === false) {
	shouldPostLambda = true;
}

Item.add({
	title: { 
		type: Types.Text, 
		required: true, 
		initial: true,
	},
	url: {
		type: Types.Url, 
		label: "URL",
		initial: true,
	},
	source: {
		type: Types.Relationship,
		ref: "Source",
		many: false,
		required: true,
		initial: true,
	},
	shortSummary: { 
		type: Types.Textarea,
		required: true,
		initial: true,
		max: 280,
		label: "Short summary",
	},
	publicationDate: { 
		type: Types.Date, 
		label: "Publication date",
	},
	evidenceType: {
		type: Types.Text, 
		label: "Evidence type",
	},
	speciality: {
		type: Types.Relationship,
		ref: "Speciality",
		many: true,
	},
	comment: {
		type: Types.Html,
		label: "SPS comment",
		wysiwyg: true,
	},
	resourceLinks: {
		type: Types.Html,
		label: "Resource links",
		wysiwyg: true,
	},
	isInitial: { 
		type: Types.Boolean,
		hidden: true,
		default: true,
	},
	relevancy: { 
		type: Types.Select,
		numeric: true,
		options:  [
			{ value: 1, label: "High" },
			{ value: 2, label: "Medium" },
			{ value: 3, label: "Low" }
		], 
		label: "Weekly relevancy score",
	},
	createdDate: { 
		type: Types.Datetime, 
		default: Date.now,
		label: "Created date",
		noedit: true
	},
	modifiedDate: { 
		type: Types.Datetime, 
		default: Date.now,
		label: "Modified date",
		noedit: true,
		watch: true,
		value: Date.now
	},
	weekly: {
		type: Types.Relationship,
		ref: "Weekly",
		label: "Weekly newsletter",
		note: "For use by the NICE MPT only"
	},
});

Item.schema.pre('validate', function(next) {
	if(this.isInitial){
		this.isInitial = false;
		next();
	}	
	
	else {
		if (!this.evidenceType) {
			next(Error('Evidence type is required.'));
		}
		else if (!this.speciality || String(this.speciality).match(/^\s*$/) !== null) {
			next(Error('Speciality is required.'));
		}
		else if (!this.resourceLinks) {
			next(Error('Resource links is required.'));
		}
		else if (!this.createdDate) {
			next(Error('Created date is required.'));
		}
		else {
			next();
		}
	}
});

const createWeeklyIfNeeded = async () => {
	const sendDateMoment = moment().startOf("isoweek").add(7, "days");

	const WeeklyModel = keystone.list("Weekly").model;

	const weeklyEntity = await WeeklyModel.findOne({
		sendDate: sendDateMoment.toDate()
	}).exec();

	if(!weeklyEntity) {
		const startDateMoment = sendDateMoment.clone().subtract(1, "week"),
			endDateMoment = startDateMoment.clone().add(4, "days"),
			dateFormat = "Do MMMM YYYY";

		var newWeekly = new WeeklyModel({
			title: startDateMoment.format(dateFormat) + " to " + endDateMoment.format(dateFormat),
			sendDate: sendDateMoment.toDate(),
			startDate: startDateMoment.toDate(),
			endDate: endDateMoment.toDate(),
		});
		await newWeekly.save();
	}
};

// Post save hook to trigger a lambda with the document details
Item.schema.post("save", async function(doc, next) {
	await createWeeklyIfNeeded();

	if(!shouldPostLambda){
		shouldPostLambda = !this.isInitial;
		next();
	}	

	logger.info("Post save, sending request...", doc);

	var item = "";

	await keystone.list("Item").model.findById(doc._id)
	.populate("source")
	.then((source) => {
		item = source;
	})
	.catch((err) => {
		logger.error("An error occurred finding source: ", err);
		next(new Error(`An error occurred finding source: ${err}`));
	})

	var contentpath = process.env.CONTENT_PATH;
	var hostname = process.env.HOST_NAME;
	var hostport = process.env.HOST_PORT;

	var data = JSON.stringify(item); 
	
	var options = {
		hostname: hostname,
		port: hostport,
		path: contentpath,
		method: "PUT",
		headers: {
			"Content-Type": "application/json"
		}
	};

	 if (hostport === "443")
	 	options.secureProtocol = "TLSv1_2_method";
	 else
	 	options.headers.host = "localhost";
		

	const req = (hostport === "443" ? https : http).request(options, res => {
		if (res.statusCode == "200") {
			next();
		}
		else {
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

Item.defaultColumns = "title, source|20%, specialities|20%";
Item.register();
