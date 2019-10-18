var keystone = require("keystone");
var Types = keystone.Field.Types;

const http = require("http");

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
	// This should probably be a POST and use a library like request to make it easier without having to use the low-levek node API
	console.log("Post save, sending request...", doc);

	// Use host.docker.internal as it's host address on Docker for Windows
	// TODO: Make this configurable via ENV var?
	var options = {
		host: "host.docker.internal",
		port: 63963,
		path: "/api/content/" + doc._id,
		method: "PUT",
		headers: {
			host: "localhost"
		}
	};

	var req = http.request(options, res => {
		console.log(`response statusCode: ${res.statusCode}`);

		var body = "";
		res.on("data", function(d) {
			body += d;
		});
		res.on("end", function() {
			console.log("response body", body);

			// Data reception is done, do whatever with it!
			var parsed = JSON.parse(body);
			console.log("loaded data", parsed);

			next();
		});
	});

	req.on("error", err => {
		console.error("Error sending post publish: " + err.message);

		next();
	});

	req.end();

	console.log("...sent PUT request", options);
});

Item.defaultColumns = "title, source|20%, specialities|20%";
Item.register();
