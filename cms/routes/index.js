/**
 * This file is where you define your application routes and controllers.
 *
 * Start by including the middleware you want to run for every request;
 * you can attach middleware to the pre('routes') and pre('render') events.
 *
 * For simplicity, the default setup for route controllers is for each to be
 * in its own file, and we import all the files in the /routes/views directory.
 *
 * Each of these files is a route controller, and is responsible for all the
 * processing that needs to happen for the route (e.g. loading data, handling
 * form submissions, rendering the view template, etc).
 *
 * Bind each route pattern your application should respond to in the function
 * that is exported from this module, following the examples below.
 *
 * See the Express application routing documentation for more information:
 * http://expressjs.com/api.html#app.VERB
 */

const keystone = require("keystone");

const {
	initErrorHandlers,
	initLocals,
	flashMessages,
	securityHeaders
} = require("./middleware");

const importRoutes = keystone.importer(__dirname);

// Common Middleware
keystone.pre("routes", initErrorHandlers);
keystone.pre("routes", initLocals);
keystone.pre("render", flashMessages);
keystone.pre("admin", securityHeaders);

// Import Route Controllers
const routes = {
	views: importRoutes("./views"),
	api: importRoutes("./api")
};

// Setup Route Bindings
exports = module.exports = function(app) {
	// Views
	app.get("/", routes.views.index);
	app.get("/api/items/:itemId", routes.api.items.single);
	app.get("/api/items/daily/:date", routes.api.items.daily);
	app.get("/api/items", routes.api.items.list);
	app.get("/api/items/month/:date", routes.api.items.month);
	app.get("/api/yearMonths", routes.api.items.yearMonths);
	app.get("/api/specialities/:specialityId", routes.api.specialities.single);
	app.get("/api/specialities", routes.api.specialities.list);
	app.get(
		"/api/weeklies/bysenddate/:sendDate",
		routes.api.weeklies.singleBySendDate
	);

	// NOTE: To protect a route so that only admins can see it, use the requireUser middleware:
	// app.get('/protected', middleware.requireUser, routes.views.protected);
};
