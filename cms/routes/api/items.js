var keystone = require("keystone"),
	_ = require("lodash");

var Items = keystone.list("Item");

exports.single = function(req, res) {
	Items.model
		.findById(req.params.itemId)
		.populate("source")
		.populate("evidenceType")
		.populate("speciality")
		.exec(function(err, item) {
			if (err) return res.err(err);

			if (!item) return res.notfound("Item not found");

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
exports.list = function(req, res) {
	// TODO: Pagination
	Items.model
		.find()
		.populate("source")
		.populate("evidenceType")
		.populate("speciality")
		.exec(function(err, items) {
			if (err) return res.json({ err: err });

			res.json(items);
		});
};
