// Simulate config options from your production environment by
// customising the .env file in your project's root folder.
require("dotenv").config();

// Require keystone
const keystone = require("keystone");

// Initialise Keystone with your project's configuration.
// See http://keystonejs.com/guide/config for available options
// and documentation.

keystone.init({
	name: "Medicines Awareness Service",
	brand: "Medicines Awareness Service",
	favicon: "public/favicon.ico",
	mongo: process.env.MONGO_URL,
	port: process.env.PORT,
	"auto update": true,
	session: true,
	auth: true,
	"user model": "User",
	static: "public"
});

// Load your project's Models
keystone.import("models");

// Setup common locals for your templates. The following are required for the
// bundled templates and layouts. Any runtime locals (that should be set uniquely
// for each request) should be added to ./routes/middleware.js
keystone.set("locals", {
	_: require("lodash"),
	env: keystone.get("env"),
	utils: keystone.utils,
	editable: keystone.content.editable
});

// Load your project's Routes
keystone.set("routes", require("./routes"));

// Configure the navigation bar in Keystone's Admin UI
keystone.set("nav", {
	content: ["Item", "Weekly"],
	admin: ["Source", "Speciality", "EvidenceType"],
	users: "User"
});

keystone.set("wysiwyg override toolbar", true);
keystone.set(
	"wysiwyg additional buttons",
	"bold italic | bullist numlist | link | code"
);

keystone.set("wysiwyg additional options", {
	// Remove the 'p' etc from status bar at the bottom of the editor
	// See https://www.tiny.cloud/docs-4x/configure/editor-appearance/#elementpath
	elementpath: false,
	// Remove title and target from the link dialog
	// See https://www.tiny.cloud/docs-4x/plugins/link/#link_title
	link_title: false,
	target_list: false,
	// Default options for "styleselect" if added to buttons on a per-field basis
	// See https://www.tiny.cloud/docs-4x/configure/content-formatting/#style_formats
	style_formats: [
		{ title: "Header 2", format: "h2" },
		{ title: "Header 3", format: "h3" },
		{ title: "Header 4", format: "h4" },
		{ title: "Header 5", format: "h5" },
		{ title: "Header 6", format: "h6" },
		{ title: "Paragraph", format: "p" }
	]
});

keystone.set("signin logo", "/images/both-logos.png");
keystone.set("adminui custom styles", "./public/styles/keystone.less");

// Start Keystone to connect to your database and initialise the web server

keystone.start();
