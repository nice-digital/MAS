const keystone = require("keystone"),
	log4js = require("log4js");

const utils = require("./utils");

const niceLoggingConfig = require("./nice-logging.json"),
	log4jsConfig = require("./log4js.json");

const logger = log4js.getLogger();

// Configure log4js for logging.
// Usage: require("./logging").configure();
exports.configure = () => {
	// Handle any uncaught global application errors
	process
		.on("unhandledRejection", (reason, p) => {
			logger.fatal(reason, "Unhandled Rejection at Promise", p);
		})
		.on("uncaughtException", err => {
			logger.fatal(err, "Uncaught Exception thrown");
			process.exit(1);
		});

	// Overriding the keystone 500 handler means we can log errors
	keystone.set("500", (err, req, res, next) => {
		const {
			hostname,
			method,
			originalUrl,
			params,
			path,
			protocol,
			query,
			xhr
		} = req;
		err.request = {
			hostname,
			method,
			originalUrl,
			params,
			path,
			protocol,
			query,
			xhr,
			url: req.protocol + "://" + req.get("host") + req.originalUrl
		};
		logger.error(err.message, err);

		const isJsonResponse = res.get("Content-Type") === "application/json";

		// This error method comes from custom middleware that add this method onto the response object
		return res.error(err, isJsonResponse);
	});

	// Pass default keystone logging through our log4js logging
	keystone.set("logger options", {
		stream: {
			write: function(str) {
				// Use debug level as most of the time, request logging isn't very useful
				logger.debug(str);
			}
		}
	});

	const config = { ...log4jsConfig };

	// Use the normal nice logging config to be consistent with other projects
	// This also has the benefit of transforming nice-logging.json with our
	// normal Octo library variable set
	const {
		RabbitMQHost,
		RabbitMQVHost,
		RabbitMQPort,
		RabbitMQUsername,
		RabbitMQPassword,
		RabbitMQExchangeName,
		RabbitMQExchangeType
	} = niceLoggingConfig.Logging;

	if (RabbitMQHost !== "") {
		Object.assign(config.appenders.niceLogging, {
			host: RabbitMQHost,
			port: RabbitMQPort,
			exchange: RabbitMQExchangeName,
			mq_type: RabbitMQExchangeType,
			vhost: RabbitMQVHost,
			username: RabbitMQUsername,
			password: RabbitMQPassword
		});
	} else {
		// If Rabbit isn't configured, then remove those appenders from log4js config
		// This stops log4js trying to connect to rabbit (even if we're not using that appender)
		delete config.appenders.niceLogging;
		delete config.appenders.niceLoggingFilter;
		config.categories.default.appenders = config.categories.default.appenders.filter(
			f => !f.includes("niceLogging")
		);
	}

	// We need a custom log format to match with the rest of nice logging
	log4js.addLayout("niceLoggingLayout", utils.niceLoggingLayout);

	log4js.configure(config);
};
