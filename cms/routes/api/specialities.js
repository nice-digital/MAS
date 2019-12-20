var keystone = require("keystone");

var Speciality = keystone.list("Speciality");

exports.single = function(req, res) {
	Speciality.model.findById(req.params.itemId).exec(function(err, item) {
		if (err) {
			logger.error(
				`Error getting speciality with id ${req.params.itemId}`,
				err
			);
			return res.error(err, true);
		}

		if (!item) {
			const notFoundMsg = `Couldn't find speciality with id ${req.params.itemId}`;
			logger.info(notFoundMsg);
			return res.notfound("Speciality not found", notFoundMsg, true);
		}

		res.json(item);
	});
};

exports.list = function(req, res) {
	Speciality.model.find(function(err, specialities) {
		if (err) {
			logger.error(`Failed to get list of specialities`, err);
			return res.error(err, true);
		}

		res.json(specialities);
	});
};
