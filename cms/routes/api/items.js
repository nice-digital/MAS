const keystone = require("keystone"),
	_ = require("lodash"),
	Items = keystone.list("Item");

const log4js = require("log4js"),
	logger = log4js.getLogger();

exports.single = function (req, res, next) {
	Items.model
		.findById(req.params.itemId)
		.populate("source")
		.populate("evidenceType")
		.populate("specialities")
		.exec(function (err, item) {
			if (err) {
				logger.error(`Error getting item with id ${req.params.itemId}`, err);
				return res.error(err, true);
			}

			if (!item) {
				const notFoundMsg = `Couldn't find item with id ${req.params.itemId}`;
				logger.info(notFoundMsg);
				return res.notfound("Item not found", notFoundMsg, true);
			}

			const obj = _.pick(item, [
				"_id",
				"updatedAt",
				"createdAt",
				"slug",
				"shortSummary",
				"source._id",
				"source.title",
				"url",
				"title",
				"comment",
				"publicationDate",
				"resourceLinks",
				"speciality",
				"evidenceType._id",
				"evidenceType.title",
				"evidenceType.key",
				"evidenceType.broaderTitle"]);

			return res.json(obj);
		});
};

/**
 * List Items
 */
exports.list = function (req, res) {
	// TODO: Pagination
	Items.model
		.find()
		.populate("source")
		.populate("evidenceType")
		.populate("specialities")
		.exec(function (err, items) {
			if (err) {
				logger.error(`Failed to get list of items`, err);
				return res.error(err, true);
			}

			res.json(items);
		});
};
