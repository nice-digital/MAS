# MAS CMS

> KeystoneJS CMS for managing content within MAS

- [MAS CMS](#mas-cms)
- [Running the CMS](#running-the-cms)
- [Configuring the CMS](#configuring-the-cms)
- [Tests](#tests)
	- [Watch mode](#watch-mode)
	- [Coverage](#coverage)
- [Logging](#logging)
	- [Error handling](#error-handling)
		- [Global error handling](#global-error-handling)
		- [Custom error handling](#custom-error-handling)
			- [Error rendering methods](#error-rendering-methods)

# Running the CMS

Mongo on machine or via Docker. `cd cms` then `npm i` then `npm start`

# Configuring the CMS

Configure the CMS through environment variables.

Running the CMS as part of the Docker stack from the root of this repo will automatically set these for you.

- `MONGO_URL` - The URL for connecting to Mongo. Defaults to _mongodb://localhost:27017_
- `PORT` - The port to run under. Defaults to _3010_.
- `COOKIE_SECRET`

# Tests

There are unit tests for custom logic and validation within the CMS. These use Jest under the hood.

Run the tests (and linting) on the command line with:

```
npm test
```

Alternatively, run `npm run test:unit` to just run the unit tests without linting.

## Watch mode

Run the tests in watch mode when developing to automatically re-run changed tests:

```
npm run test:unit:watch
```

## Coverage

Collect coverage by running the following command:

```
npm run test:unit:coverage
```

This reports coverage on the command line, and as HTML within the _coverage_ folder. It also reports coverage to TeamCity automatically when run on a build agent (or when the `TEAMCITY_VERSION` environment variable is set).

- `MONGO_URL` - The URL for connecting to Mongo. Defaults to _mongodb://localhost:27017_
- `PORT` - The port to run under. Defaults to _3010_.
- `COOKIE_SECRET`

# Logging

We use [log4js](https://log4js-node.github.io/log4js-node/) for logging. Use it like:

```js
const log4js = require("log4js");
const logger = log4js.getLogger();

logger.debug("Some debug messages");
logger.error("An error!");
```

Log4js is configured to log info to the console and debug messages to _logs/keystone.log_ by default. In production it will log to our logstash (Kibana) instance, via a rabbit message queue. It gets the settings for this queue from _logging/nice-logging.json_. To test this logging locally, set the values in _nice-logging.json_ file (`RabbitMQHost` etc) with values for our Rabbit MQ. These setting are in the 'Logging' variable set in the Octo library.

## Error handling

There are 2 ways to handle and log errors with custom routes.

### Global error handling

The first, and simplest way to handle errors, is to catch an exception, and leave the logging and server response up to the default middleware within keystone's express app. This renders an HTML error message by default:

```js
exports.someMethod = function(req, res, next) {
	const anErrorFromSomewhere = new Error(
		"All we are is dust in the wind, dude"
	);
	return next(anErrorFromSomewhere);
};
```

> Note: This works because we've overidden the _500_ option in keystone via `keystone.set("500", ...)`.

Set the response content type to _application/json_ first to serve a JSON response:

```js
exports.someMethod = function(req, res, next) {
	const anErrorFromSomewhere = new Error("Be excellent to each other");
	res.type("application/json");
	return next(anErrorFromSomewhere);
};
```

### Custom error handling

The second option is to catch an error, log it yourself and provide your own server response. This gives you more find-grained control but it does require you to provide your own response:

```js
const { serializeError } = require("serialize-error");
exports.someMethod = function(req, res, next) {
	var anErrorFromSomewhere = new Error("Party on, dude!");

	logger.error(anErrorFromSomewhere);
	return res.status(500).json({ error: serializeError(err) });
};
```

The above example creates its own response, however it's easier to use one of the custom error rendering methods:

#### Error rendering methods

There are 2 methods on `res` for sending errors: `res.error` and `res.notfound`, added by custom middleware:

```js
return res.error(err); // Outputs HTML and 500 response
return res.error(err, true); // Serializes the error message into JSON and 500 response

return res.notfound("Not found", "Sorry, item with id 123 could not be found"); // Outputs HTML and 404 response
return res.notfound(
	"Not found",
	"Sorry, item with id 123 could not be found",
	true
); // Outputs JSON and 404 response
```
