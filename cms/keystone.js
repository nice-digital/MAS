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
	users: "users",
	items: ["items", "specialities"]
});

keystone.set("signin logo", "/images/sps-logo.png");
keystone.set("adminui custom styles", "./public/styles/keystone.less");

// Start Keystone to connect to your database and initialise the web server

keystone.start();
