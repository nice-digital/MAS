var keystone = require("keystone");
var Types = keystone.Field.Types;

const request = require("request");

var Item = new keystone.List("Item", {
	map: { name: "title" },
	autokey: { path: "slug", from: "title", unique: true }
});

Item.add({
	publicationDate: { 
		type: Types.Datetime, 
		default: Date.now,
		required: true,
		initial: true
	},
	createdDate: { 
		type: Types.Datetime, 
		default: Date.now,
		required: true,
		initial: true
	},
	category: {
		type:Types.Text, 
		required: true,
		initial: true
	},
	title: { 
		type: Types.Text, 
		required: true, 
		initial: true
	 },
	source: {
		type: Types.Relationship,
		ref: "Source",
		many: false,
		initial: true
	},
	geographicalCoverage:{
		type: Types.Select, 
		options: 'International, UK',
		required: true,
		initial: true
	},
	speciality: {
		type: Types.Relationship,
		ref: "Speciality",
		many: true,
		initial: true
	},
	shortSummary: { 
		type: Types.Textarea,
		required: true,
		initial: true,
		min: 10, 
		max: 280
	 },
	resourceLinks: {
		type: Types.Url, 
		required: false, 
		initial: true 
	},
	UKMiComment: {
		type: Types.Textarea,
		required: true,
		initial: true,
		height: 400
	},
	MAWScore: { 
		type:Types.Number, 
		required:true,
		initial: true
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

	var options = {
		uri: hostname + contentpath + doc._id,
		method: "PUT",
		headers: {
			host: "localhost"
		}
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
