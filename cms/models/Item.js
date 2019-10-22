var keystone = require("keystone");
var Types = keystone.Field.Types;

const request = require("request");

var Item = new keystone.List("Item", {
	map: { name: "title" },
	autokey: { path: "slug", from: "title", unique: true }
});

Item.add({
	title: { type: String, required: true, initial: true },
	url: { type: Types.Url, required: true, initial: true },
	summary: { type: Types.Textarea, required: true, initial: true },
	specialities: {
		type: Types.Relationship,
		ref: "Speciality",
		many: true,
		initial: true
	},
	source: {
		type: Types.Relationship,
		ref: "Source",
		many: false,
		initial: true
	},
	UKMiComment: {
		type: Types.Html,
		wysiwyg: true,
		required: false,
		height: 400
	},
	state: {
		type: Types.Select,
		options: "draft, published, archived",
		default: "draft",
		index: true
	},
	createdDate: { type: Date, default: Date.now },
	author: { type: Types.Relationship, ref: "User", index: true },
	publishedDate: {
		type: Types.Date,
		index: true,
		dependsOn: { state: "published" }
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
