const keystone = require("keystone");

const Types = keystone.Field.Types;

const EvidenceType = new keystone.List("EvidenceType", {
	map: { name: "title" },
	nodelete: true,
	nocreate: true,
	noedit: true
});

EvidenceType.add({
	title: { type: String, required: true },
	key: { type: String, required: true },
	oldEPiServerId: {
		label: "Old EPiServer ID",
		type: Types.Number,
		required: false
	}
});

EvidenceType.relationship({
	ref: "Item",
	path: "items",
	refPath: "evidenceType"
});

EvidenceType.defaultColumns = "title, oldEPiServerId, key";

EvidenceType.register();
