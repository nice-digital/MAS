const keystone = require("keystone"),
	_ = require("lodash");

const Weekly = keystone.list("Weekly"),
	Item = keystone.list("Item");

exports.singleBySendDate = async function(req, res) {
	let weekly;
	try {
		weekly = await Weekly.model
			.findOne({ sendDate: req.params.sendDate })
			.exec();
	} catch (err) {
		return res.error(err, true);
	}

	if (!weekly) {
		return res.notfound(
			"Weekly could not be found",
			`Weekly with send date of ${req.params.sendDate} could not be found`,
			true
		);
	}

	let items;
	try {
		items = await Item.model
			.find({ weekly: weekly._id })
			.populate("source")
			.populate("evidenceType")
			.populate("specialities")
			.select(Item.fullResponseFields.join(" "))
			.exec();
	} catch (err) {
		return res.error(err, true);
	}

	items = _.map(items, _.partialRight(_.pick, Item.fullResponseFields));
	res.json({ ...weekly.toObject(), items });
};
