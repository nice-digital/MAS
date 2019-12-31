var keystone = require("keystone");

var Weekly = keystone.list("Weekly");

exports.single = function(req, res) {
	Weekly.model
		.findById(req.params.weeklyId)
		.exec(function(err, weekly) {
			if (err) return res.err(err);

			if (!weekly) return res.notfound("Weekly not found");

			res.json(weekly);
		});
};


/**
 * List Items
 */
exports.list = function(req, res) {
	// TODO: Pagination
	Weekly.model
		.find()
		.exec(function(err, weekly) {
			if (err) return res.json({ err: err });

			res.json(weekly);
		});
};