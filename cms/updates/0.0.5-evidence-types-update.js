const keystone = require("keystone"),
	async = require("async"),
	log4js = require("log4js"),
	fs = require("fs"),
	path = require("path"),
	util = require("util");

const logger = log4js.getLogger();
const evidenceTypeList = keystone.list("EvidenceType");

const deleteEvidenceType = async (evidenceType, done) => {
	let eTitle = evidenceType.title;
	logger.info(`Deleting evidence type: ${eTitle}`);

	try {
		await evidenceTypeList.model.findOne({ title: { "$regex": eTitle, "$options": "i" } }).remove();
		done();
	} catch (err) {
		logger.error(`Error deleting ${eTitle}`, err);
		done(err);
	}

	logger.info(`Successfully deleted ${eTitle}`);
};

const updateEvidenceType = async (evidenceType, done) => {
	let evidenceTypeInKeystone = await evidenceTypeList.model.findOne({key: evidenceType["@id"] }).exec();
	if(!evidenceTypeInKeystone)
		return;

	let newTitle = evidenceTypeInKeystone.broaderTitle + " - " + evidenceType["prefLabel"]["@value"];
	let oldTitle = evidenceTypeInKeystone.title;

	if(newTitle != oldTitle){
		logger.info(`Renaming evidence type ${oldTitle} to ${newTitle}`);

		try {
			var docToUpdate = await evidenceTypeList.model.findOne({ key: evidenceType["@id"] });
			docToUpdate.title = newTitle;
			docToUpdate.key = evidenceType["@id"];
			await docToUpdate.save();
			done();
		} catch (err) {
			logger.error(`Error renaming ${oldTitle} to ${newTitle}`, err);
			done(err);
		}
	
		logger.info(`Successfully renamed ${oldTitle} to ${newTitle}`);
	}	
}

const addNewEvidenceType = async (evidenceType, done) => {
	let title = evidenceType.prefLabel["@value"];
	let key = evidenceType["@id"];

	logger.info(`Adding new evidence type: ${title}`);

	const newEvidenceTypeDbModel = new evidenceTypeList.model({
		title: title,
		key: key
	});

	newEvidenceTypeDbModel.save(function (err) {
		if (err) {
			logger.error(`Error adding evidence type ${title} to the database`, err);
			throw err;
		} else {
			logger.info(`Added evidence type ${title} to the database.`);
		}
		done();
	});
};

exports = module.exports = async function (done) {
	logger.info(`Started update 05.`);
	fs.readFile(
		path.resolve(__dirname, "0.0.5-evidence-types-update.jsonld"),
		"UTF-8",
		async function(err, contents){
			const evidenceTypesJsonLd = JSON.parse(contents),
				latestEvidenceTypes = evidenceTypesJsonLd["@graph"]
					.filter(a => a.prefLabel != undefined && a.prefLabel["@value"] != undefined);
			
			let callback = function (err) {
				if (err != null) {
					logger.error(err);
				}
			}

			//Deletes
			let latestEvidenceTypesIds = latestEvidenceTypes.map(a => a["@id"]);
			let evidenceTypesToDelete;
			try {
				evidenceTypesToDelete = await evidenceTypeList.model.find( { key: { $nin: latestEvidenceTypesIds } }).exec();
			} catch (err) {
				logger.error(`Error:`, err);
				throw err;
			}
			let deletes = async.forEach(evidenceTypesToDelete, deleteEvidenceType, callback);
			
			//Renames
			let updates = async.forEach(latestEvidenceTypes, updateEvidenceType, callback); 

			//Additions
			let newEvidenceTypes = latestEvidenceTypes.filter(et => et.prefLabel["@value"] == "Safety support material");
			//TODO: Ensure the title for the new types incorporates the broader title eg saferty alters - saferty support material
			let adds = async.forEach(newEvidenceTypes, addNewEvidenceType, callback); 
			
			Promise.all([deletes, updates, adds]).then(() => {
				logger.info(`Finished update 05.`);
				done();
			});
		}
	);
};
