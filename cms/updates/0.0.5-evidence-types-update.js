const keystone = require("keystone"),
	async = require("async"),
	log4js = require("log4js"),
	fs = require("fs"),
	path = require("path");

const logger = log4js.getLogger();
const evidenceTypeList = keystone.list("EvidenceType");

const deleteEvidenceType = async (evidenceType, done) => {
	let eTitle = evidenceType.title;
	logger.info(`Deleting evidence type: ${eTitle}`);

	try {
		await evidenceTypeList.model
			.findOne({ title: { $regex: eTitle, $options: "i" } })
			.remove();
		done();
	} catch (err) {
		logger.error(`Error deleting '${eTitle}'`, err);
		done(err);
	}

	logger.info(`Successfully deleted '${eTitle}'`);
};

const updateEvidenceType = async (jsonldEvidenceType, done) => {
	let masEvidenceType = await evidenceTypeList.model
		.findOne({ key: jsonldEvidenceType["@id"] })
		.exec();
	if (!masEvidenceType) return;

	let newTitleShort = jsonldEvidenceType["prefLabel"]["@value"];
	let newTitle = masEvidenceType.broaderTitle + " - " + newTitleShort;
	let oldTitle = masEvidenceType.title;

	if (newTitle != oldTitle && !oldTitle.includes(newTitleShort)) {
		logger.info(`Updating evidence type '${oldTitle}' to '${newTitle}'`);

		try {
			var docToUpdate = await evidenceTypeList.model.findOne({
				key: jsonldEvidenceType["@id"]
			});
			docToUpdate.title = newTitle;
			docToUpdate.key = jsonldEvidenceType["@id"];
			await docToUpdate.save();
			done();
		} catch (err) {
			logger.error(`Error updating '${oldTitle}' to '${newTitle}'`, err);
			done(err);
		}

		logger.info(`Successfully updated '${oldTitle}' to '${newTitle}'`);
	}
};

const addNewMasEvidenceType = async (masEvidenceType, done) => {
	logger.info(`Adding new evidence type: ${masEvidenceType.title}`);

	masEvidenceType.save(function(err) {
		if (err) {
			logger.error(
				`Error adding evidence type '${masEvidenceType.title}' to the database`,
				err
			);
			throw err;
		} else {
			logger.info(
				`Successfully added evidence type '${masEvidenceType.title}' to the database.`
			);
		}
		done();
	});
};

exports = module.exports = async function(done) {
	logger.info(`Started update 05.`);
	fs.readFile(
		path.resolve(__dirname, "0.0.5-evidence-types-update.jsonld"),
		"UTF-8",
		async function(err, contents) {
			const jsonldObj = JSON.parse(contents),
				jsonldEvidenceTypes = jsonldObj["@graph"].filter(
					a => a.prefLabel != undefined && a.prefLabel["@value"] != undefined
				);

			let callback = function(err) {
				if (err != null) {
					logger.error(err);
				}
			};

			//Deletes
			let jsonldEvidenceTypesIds = jsonldEvidenceTypes.map(a => a["@id"]);
			let evidenceTypesToDelete;
			try {
				evidenceTypesToDelete = await evidenceTypeList.model
					.find({ key: { $nin: jsonldEvidenceTypesIds } })
					.exec();
			} catch (err) {
				logger.error(`Error:`, err);
				throw err;
			}
			let deletes = async.forEach(
				evidenceTypesToDelete,
				deleteEvidenceType,
				callback
			);

			//Renames
			let updates = async.forEach(
				jsonldEvidenceTypes,
				updateEvidenceType,
				callback
			);

			//Additions
			function createMasEvidenceType(jsonldEvidenceType) {
				let broaderTitle = jsonldEvidenceTypes.find(
					x => x["@id"] == jsonldEvidenceType.broader
				).prefLabel["@value"];

				return new evidenceTypeList.model({
					title: broaderTitle + " - " + jsonldEvidenceType.prefLabel["@value"],
					key: jsonldEvidenceType["@id"]
				});
			}

			let additionsFromJsonld = ["Safety support material"];
			let newEvidenceTypes = jsonldEvidenceTypes
				.filter(et => additionsFromJsonld.includes(et.prefLabel["@value"]))
				.map(et => createMasEvidenceType(et));

			let adds = async.forEach(
				newEvidenceTypes,
				addNewMasEvidenceType,
				callback
			);

			Promise.all([deletes, updates, adds]).then(() => {
				logger.info(`Finished update 05.`);
				done();
			});
		}
	);
};
