var keystone = require("keystone");
var Types = keystone.Field.Types;

const request = require("request");

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
	// This should probably be a POST
	console.log("Post save, sending request...", doc);

	// Use host.docker.internal as it's host address on Docker for Windows
	// TODO: Make this configurable via ENV var?

	var contentpath = process.env.CONTENT_PATH;
	var hostname = process.env.HOST_NAME;

	var data = JSON.stringify(this); 
	var options = {
		uri: hostname + contentpath,
		method: "PUT",
		headers: {
			host: "localhost"
		},
		body: data,
		json: true
	};

	request(options, function (error, response, body) {
		if (error) {
			console.log('Error sending post publish :', error);
		}
		next();
	});

	console.log("...sent PUT request", options);
});



Item.defaultColumns = "title, source|20%, specialities|20%";
Item.register();
