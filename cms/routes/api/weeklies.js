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
		return res.status(500).json({
			error: err
		});
	}

	if (!weekly) {
		return res.status(404).json({ error: "Weekly could not be found" });
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
