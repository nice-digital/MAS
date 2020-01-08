const keystone = require("keystone"),
	async = require("async"),
	log4js = require("log4js"),
	fs = require("fs"),
	path = require("path");

const oldEpiServerIdMappings = require("./0.0.2-evidence-types-episerver-ids.json");

const logger = log4js.getLogger("0.0.2-evidence-types.js");
logger.level = "debug";

const ConceptType = "http://www.w3.org/2004/02/skos/core#Concept";
const EvidenceTypeConceptId = "mas_evidence_types:CONCEPT-Evidence_type";

const EvidenceTypeList = keystone.list("EvidenceType");

const createEvidenceType = (element, done) => {
	const {
		"@id": key,
		prefLabel: { "@value": leafTitle },
		broaderConcept
	} = element;

	const oldEPiServerId = oldEpiServerIdMappings[key];

	const {
		"@id": broaderId,
		prefLabel: { "@value": broaderTitle }
	} = broaderConcept;

	const title =
		broaderId === EvidenceTypeConceptId
			? leafTitle
			: `${broaderTitle} - ${leafTitle}`;

	const newEvidenceTypeDbModel = new EvidenceTypeList.model({
		title,
		key,
		oldEPiServerId
	});

	newEvidenceTypeDbModel.save(function(err) {
		if (err) {
			logger.error(`Error adding evidence type ${title} to the database`, err);
			throw err;
		} else {
			logger.info(`Added evidence type ${title} to the database.`);
		}
		done();
	});
};

exports = module.exports = function(done) {
	fs.readFile(
		path.resolve(__dirname, "0.0.2-evidence-types.jsonld"),
		"UTF-8",
		function(err, contents) {
			if (err) {
				logger.error(err);
				throw err;
			}

			const evidenceTypesJsonLd = JSON.parse(contents),
				evidenceTypes = evidenceTypesJsonLd["@graph"];

			const isLeafConcept = ({ "@id": id, "@type": type }) =>
				type === ConceptType &&
				!evidenceTypes.some(({ broader }) => broader === id);

			const withBroaderConcept = element => {
				const broaderConcept = evidenceTypes.find(
					el => el["@id"] === element.broader
				);
				return { ...element, broaderConcept };
			};

			const evidenceTypeLeafNodes = evidenceTypes
				.filter(isLeafConcept)
				.map(withBroaderConcept);

			async.forEach(evidenceTypeLeafNodes, createEvidenceType, done);
		}
	);
};
