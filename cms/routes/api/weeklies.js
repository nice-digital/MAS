var keystone = require("keystone");

var Weekly = keystone.list("Weekly");

exports.single = function(req, res) {
	Weekly.model
		.findOne()
		.sort("sendDate" -1)
		.exec(function(err, weekly) {
			if (err) return res.json({ err: err });

			res.json(weekly);
		});
};