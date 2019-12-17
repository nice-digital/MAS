const keystone = require("keystone"),
	moment = require("moment"),
	async = require("async");

const trainingContentItems = require("./0.0.3-training-content.json");

const Source = keystone.list("Source");
const Item = keystone.list("Item");
const EvidenceType = keystone.list("EvidenceType");

const createItem = (item, done) => {
	var newItem = new Item.model(item);

	newItem.save(function(err) {
		if (err) {
			console.error("Error adding item " + item.title + " to the database:");
			console.error(err);
		} else {
			console.log("Added item " + item.title + " to the database.");
		}
		done(err);
	});
};

exports = module.exports = function(done) {
	Source.model.find().exec(function(err, sources) {
		if (err) throw err;

		EvidenceType.model.find().exec(function(err, evidenceTypes) {
			if (err) throw err;

			const items = trainingContentItems.map((element, i) => ({
				...element,
				source: sources.find(s => s.title === element.source).id,
				evidenceType: evidenceTypes.find(e => e.title === element.evidenceType)
					.id,
				createdAt: moment()
					.subtract(i, "days")
					.toDate(),
				relevancy: Math.floor(Math.random() * 3) + 1
			});

			async.forEach(items, createItem, done);
		});
	});
};
