// Simulate config options from your production environment by
// customising the .env file in your project's root folder.
require("dotenv").config();

// Require keystone
var keystone = require("keystone");
var handlebars = require("express-handlebars");

// Initialise Keystone with your project's configuration.
// See http://keystonejs.com/guide/config for available options
// and documentation.

keystone.init({
	name: "MAS",
	brand: "MAS",
	mongo: process.env.MONGO_URL,
	port: process.env.PORT,
	"auto update": true,
	session: true,
	auth: true,
	"user model": "User"
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
	users: "users",
	items: ["items", "specialities"]
});

keystone.set("wysiwyg override toolbar", true);
keystone.set("wysiwyg additional buttons", "bold italic | bullist numlist | link | code");

keystone.set("wysiwyg additional options", {
	// Remove the 'p' etc from status bar at the bottom of the editor
	// See https://www.tiny.cloud/docs-4x/configure/editor-appearance/#elementpath
	elementpath: false,
	// Remove title and target from the link dialog
	// See https://www.tiny.cloud/docs-4x/plugins/link/#link_title
	link_title: false,
	target_list: false
});

// Start Keystone to connect to your database and initialise the web server

keystone.start();
