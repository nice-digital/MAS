var keystone = require("keystone");
var Types = keystone.Field.Types;

const https = require("https");

var Item = new keystone.List("Item", {
	map: { name: "title" },
	autokey: { path: "slug", from: "title", unique: true }
});

Item.add({
	// publicationDate: { 
	// 	type: Types.Datetime, 
	// 	required: true,
	// 	initial: true
	// },
	// createdDate: { 
	// 	type: Types.Datetime, 
	// 	default: Date.now,
	// 	required: true,
	// 	initial: true
	// },
	// category: {
	// 	type:Types.Text, 
	// 	required: true,
	// 	initial: true
	// },
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
	// speciality: {
	// 	type: Types.Relationship,
	// 	ref: "Speciality",
	// 	many: true,
	// 	initial: true
	// },
	shortSummary: { 
		type: Types.Textarea,
		required: true,
		initial: true,
		min: 10, 
		max: 280
	 },
	// resourceLinks: {
	// 	type: Types.Url, 
	// 	required: false, 
	// 	initial: true 
	// },
	// UKMiComment: {
	// 	type: Types.Html,
	// 	wysiwyg: true,
	// 	required:true, 
	// 	initial: true 
	// },
	// RelevancyScore: { 
	// 	type: Types.Select, 
	// 	options: '1,2,3',
	// 	required:true, 
	// 	initial: true 
	//  }
});

// Post save hook to trigger a lambda with the document details
Item.schema.post("save", function(doc, next) {
	// This should probably be a POST
	console.log("Post save, sending request...", doc);

	var contentpath = process.env.CONTENT_PATH;
	var hostname = process.env.HOST_NAME;

	var data = JSON.stringify(this); 

	var options = {
		hostname: hostname,
		path: contentpath,
		secureProtocol: "TLSv1_2_method",
		method: "PUT",
		headers: {
			"Content-Type": "application/json",
			"Content-Length": "data.length"
		}
	};

	const req = https.request(options, res => {
		console.log("statusCode: ", res.statusCode);
		res.on('data', d => {
	    	process.stdout.write(d);
		});
	});

	req.on('error', error => {
	  console.log("Error sending post publish :", error);
	});

	req.end();

	next();

	console.log("...sent PUT request", options);
});

Item.defaultColumns = "title, source|20%, specialities|20%";
Item.register();
