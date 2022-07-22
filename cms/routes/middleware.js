/**
 * This file contains the common middleware used by your routes.
 *
 * Extend or replace these functions as your application requires.
 *
 * This structure is not enforced, and just a starting point. If
 * you have more middleware you may want to group it as separate
 * modules in your project's /lib directory.
 */
const keystone = require("keystone"),
	keystoneUtils = require("keystone-utils"),
	_ = require("lodash"),
	{ serializeError } = require("serialize-error");

/**
	Initialises the standard view locals

	The included layout depends on the navLinks array to generate
	the navigation in the header, you may wish to change this array
	or replace it with your own templates / logic.
*/
exports.initLocals = function(req, res, next) {
	res.locals.navLinks = [
		{ label: "Home", key: "home", href: "/" },
		{ label: "Blog", key: "blog", href: "/blog" },
		{ label: "Contact", key: "contact", href: "/contact" }
	];
	res.locals.user = req.user;
	next();
};

/**
	Fetches and clears the flashMessages before a view is rendered
*/
exports.flashMessages = function(req, res, next) {
	var flashMessages = {
		info: req.flash("info"),
		success: req.flash("success"),
		warning: req.flash("warning"),
		error: req.flash("error")
	};
	res.locals.messages = _.some(flashMessages, function(msgs) {
		return msgs.length;
	})
		? flashMessages
		: false;
	next();
};

/**
	Prevents people from accessing protected pages when they're not signed in
 */
exports.requireUser = function(req, res, next) {
	if (!req.user) {
		req.flash("error", "Please sign in to access this page.");
		res.redirect("/keystone/signin");
	} else {
		next();
	}
};

/**
    Inits the error handler functions into `res`
*/
exports.initErrorHandlers = function(req, res, next) {
	// Mimic the default keystone error handler
	// See https://github.com/keystonejs/keystone-classic/blob/master/server/bindErrorHandlers.js#L46-L73
	res.error = function(err, useJson) {
		const isDevEnv = keystone.get("env") === "development";

		if (useJson || req.is("json")) {
			const serializedError = isDevEnv
				? serializeError(err)
				: { message: err.message };

			return res.status(500).json({ error: serializedError });
		}

		var msg = "";
		if (isDevEnv) {
			if (err instanceof Error) {
				if (err.type) {
					msg += "<h2>" + err.type + "</h2>";
				}
				msg += keystoneUtils.textToHTML(err.message);
			} else if (typeof err === "object") {
				msg += "<code>" + JSON.stringify(err) + "</code>";
			} else if (err) {
				msg += err;
			}
		}

		return res
			.status(500)
			.send(
				keystone.wrapHTMLError(
					"Sorry, an error occurred loading the page (500)",
					msg
				)
			);
	};

	res.notfound = function(title, message, useJson) {
		if (useJson || req.is("json")) {
			return res.status(404).json({ title, message });
		}

		return res.status(404).send(keystone.wrapHTMLError(title, message));
	};

	res.badRequest = function(title, message, useJson) {
		if (useJson || req.is("json")) {
			return res.status(400).json({ title, message });
		}

		return res.status(400).send(keystone.wrapHTMLError(title, message));
	};

	next();
};

exports.securityHeaders = function(req, res, next) {
	res.set("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
	res.set("Content-Security-Policy", "frame-ancestors 'self';");
	res.set("X-XSS-Protection", "1; mode=block");
	res.set("X-Content-Type-Options", "nosniff");
	res.set("X-Frame-Options", "DENY");
	res.set("Permissions-Policy", "interest-cohort=()");

	next();
};
