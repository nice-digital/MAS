var keystone = require("keystone");
var Types = keystone.Field.Types;

var Source = new keystone.List("Source", {
	map: { name: "title" },
	nodelete: true
});

Source.add({
	title: { type: String, required: true, initial: true },
	oldEPiServerId: {
		label: "Old EPiServer ID",
		type: Types.Number,
		required: false
	}
});

Source.relationship({
	ref: "Item",
	path: "items",
	refPath: "source"
});

Source.defaultColumns = "title";

Source.register();
