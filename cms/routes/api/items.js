var keystone = require("keystone");

var Items = keystone.list("Item");

exports.single = function(req, res) {
	Items.model.findById(req.params.itemId).exec(function(err, item) {
		if (err) return res.err(err);

		if (!item) return res.notfound('Item not found');

		res.json(item);
	});
};

/**
 * List Items
 */
exports.list = function(req, res) {
	// TODO: Pagination
	Items.model.find(function(err, items) {
		if (err) return res.json({ err: err });

		res.json(items);
	});
};
