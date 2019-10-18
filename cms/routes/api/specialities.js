var keystone = require("keystone");

var Speciality = keystone.list("Speciality");

exports.single = function(req, res) {
	Speciality.model.findById(req.params.itemId).exec(function(err, item) {
		if (err) return res.err(err);

		if (!item) return res.notfound("Speciality not found");

		res.json(item);
	});
};

exports.list = function(req, res) {
	Speciality.model.find(function(err, specialities) {
		if (err) return res.json({ err: err });

		res.json(specialities);
	});
};
