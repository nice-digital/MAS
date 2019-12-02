var keystone = require("keystone");
var Types = keystone.Field.Types;

const https = require("https");
var log4js = require("log4js");

var logger = log4js.getLogger("Item.js");
logger.level = "debug";

var Item = new keystone.List("Item", {
	map: { name: "title" },
	autokey: { path: "slug", from: "title", unique: true }
});

Item.add({
	title: { 
		type: Types.Text, 
		required: true, 
		initial: true
	 },
	source: {
		type: Types.Relationship,
		ref: "Source",
		many: false,
		required: true,
		initial: true
	},
	shortSummary: { 
		type: Types.Textarea,
		required: true,
		initial: true,
		max: 280,
		label: "Short summary",
	},
	publicationDate: { 
		type: Types.Datetime, 
		label: "Publication date",
	},
	evidenceType: {
		type:Types.Text, 
		label: "Evidence type",
	},
	speciality: {
		type: Types.Relationship,
		ref: "Speciality",
		many: true,
	},
	resourceLinks: {
		type: Types.Url, 
		label: "Resource links",
	},
	UKMiComment: {
		type: Types.Html,
		wysiwyg: true,
		label: "UKMi comment",
	},
	createdDate: { 
		type: Types.Datetime, 
		default: Date.now,
		label: "Created date",
	},
	isInitial: { 
		type: Types.Boolean ,
		hidden: true,
		default: true,
	},
	// relevancyScore: { 
	// 	type: Types.Select, 
	// 	options: '1,2,3', 
	//  label: "Relevancy score",
	//  },
});

Item.schema.pre('validate', function(next) {

	if(this.isInitial){
		this.isInitial = false;
		next();
	}	
	
	else {
		if (!this.publicationDate) {
			next(Error('Publication date is required.'));
		}
		else if (!this.evidenceType) {
			next(Error('Evidence type is required.'));
		}
		else if (!this.speciality || String(this.speciality).match(/^\s*$/) !== null) {
			next(Error('Speciality is required.'));
		}
		else if (!this.resourceLinks) {
			next(Error('Resource links is required.'));
		}
		else if (!this.UKMiComment) {
			next(Error('UKMi comment is required.'));
		}
		else if (!this.createdDate) {
			next(Error('Created date is required.'));
		}
		else {
			next();
		}
	}
    
});

// Post save hook to trigger a lambda with the document details
Item.schema.post("save", function(doc, next) {
	logger.info("Post save, sending request...", doc);

	var item = "";

	keystone.list('Item').model.findById(doc._id)
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

	var data = JSON.stringify(item); 
	
	var options = {
		hostname: hostname,
		path: contentpath,
		secureProtocol: "TLSv1_2_method",
		method: "PUT",
		headers: {
			"Content-Type": "application/json"
		}
	};

	const req = https.request(options, res => {
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
