const keystone = require("keystone"),
	fs = require("fs"),
	path = require("path");

// Load the JSONLD for evidence types on application load
// so we don't have to hit the file system again and again
let evidenceTypesJsonLd;
fs.readFile(
	path.resolve(__dirname, "../evidence-types.jsonld"),
	"UTF-8",
	function(err, contents) {
		if (err) {
			loggger.error(err);
			throw err;
		}

		evidenceTypesJsonLd = JSON.parse(contents);
	}
);

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
	oldEPiServerId: { label: "Old EPiServer ID", type: Types.Number, required: false },
});

EvidenceType.relationship({
	ref: "Item",
	path: "items",
	refPath: "evidenceType"
});

EvidenceType.schema.virtual("broaderTitle").get(function() {
	const key = this.key,
		evidenceTypes = evidenceTypesJsonLd["@graph"],
		concept = evidenceTypes.find(e => e["@id"] === key);

	// Assume only 2 levels
	const broaderConcept = evidenceTypes.find(e => e["@id"] === concept.broader);

	if(broaderConcept.broader)
		return broaderConcept.prefLabel["@value"];
	else
		return concept.prefLabel["@value"];
});

EvidenceType.defaultColumns = "title, oldEPiServerId, key";

EvidenceType.register();
