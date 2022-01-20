const keystone = require("keystone"),
	_ = require("lodash"),
	moment = require("moment"),
	Items = keystone.list("Item");

const log4js = require("log4js"),
	logger = log4js.getLogger();

/**
 * Single item, given an id
 * /api/items/5e205dadc12944a35f24572b
 */
exports.single = function(req, res) {
	Items.model
		.findById(req.params.itemId)
		.populate("source")
		.populate("evidenceType")
		.populate("specialities")
		.select(Items.fullResponseFields.join(" "))
		.exec(function(err, item) {
			if (err) {
				logger.error(`Error getting item with id ${req.params.itemId}`, err);
				return res.error(err, true);
			}

			if (!item) {
				const notFoundMsg = `Couldn't find item with id ${req.params.itemId}`;
				logger.info(notFoundMsg);
				return res.notfound("Item not found", notFoundMsg, true);
			}

			const obj = _.pick(item, Items.fullResponseFields);

			return res.json(obj);
		});
};

/**
 * List of all items
 * /api/items
 */
exports.list = function(req, res) {
	Items.model
		.find()
		.limit(0)
		.select("title slug updatedAt")
		.exec(function(err, items) {
			if (err) {
				logger.error(`Failed to get list of items`, err);
				return res.error(err, true);
			}

			res.json(items);
		});
};

/**
 * Daily items for a given date
 * /api/items/daily/2020-01-16
 */
exports.daily = async function(req, res) {
	const dateStr = req.params.date,
		date = moment(dateStr, "YYYY-M-D", true);

	if (!date.isValid()) {
		const errorMessage = `Date '${dateStr}' is not in the format YYYY-M-D`;
		logger.error(errorMessage);
		return res.badRequest("Couldn't get daily items", errorMessage, true);
	}

	const startOfDay = date.clone().startOf("day"),
		endOfDay = date.clone().endOf("day");

	let items;
	try {
		items = await Items.model
			.find({
				createdAt: { $gte: startOfDay.toDate(), $lt: endOfDay.toDate() }
			})
			.populate("source")
			.populate("evidenceType")
			.populate("specialities")
			.select(Items.fullResponseFields.join(" "))
			.exec();
	} catch (err) {
		logger.error(`Error getting daily items for date ${dateStr}`, err);
		return res.error(err, true);
	}

	if (items.length === 0)
		logger.warn(`Zero daily items found for date ${dateStr}`);

	const obj = _.map(items, _.partialRight(_.pick, Items.fullResponseFields));

	res.json(obj);
};

/**
 * List of items for a given month
 * /api/items/month/2020-01
 */
exports.month = async function(req, res) {
	const start = Date.now();
	const dateStr = req.params.date,
		date = moment(dateStr, "YYYY-M", true);

	if (!date.isValid()) {
		const errorMessage = `Date '${dateStr}' is not in the format YYYY-M`;
		logger.error(errorMessage);
		return res.badRequest("Couldn't get months items", errorMessage, true);
	}

	const startOfMonth = date.clone().startOf("month"),
		endOfMonth = date.clone().endOf("month");

	let items;
	try {
		items = await Items.model
			.find({
				createdAt: { $gte: startOfMonth.toDate(), $lt: endOfMonth.toDate() }
			})
			.select("title slug updatedAt")
			.exec();
	} catch (err) {
		logger.error(`Error getting items for month ${dateStr}`, err);
		return res.error(err, true);
	}

	if (items.length === 0) logger.warn(`Zero items found for month ${dateStr}`);

	const obj = _.map(items, _.partialRight(_.pick, Items.fullResponseFields));

	res.json(obj);
	const millis = Date.now() - start;
	console.log(`seconds elapsed = ${Math.floor(millis / 1000)}`);
};

/**
 * List of all the months with items
 * /api/yearMonths
 */
exports.yearMonths = function(req, res) {
	Items.model
		.aggregate([
			{
				$group: {
					_id: { $dateToString: { format: "%Y-%m", date: "$createdAt" } }
				}
			}
		])
		.cursor()
		.exec()
		.toArray(function(err, items) {
			if (err) {
				logger.error(`Failed to get list of YearMonths`, err);
				return res.error(err, true);
			}

			res.json(items);
		});
};
