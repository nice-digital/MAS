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
		await evidenceTypeList.model.findOne({ title: { "$regex": eTitle, "$options": "i" } }).remove();
		done();
	} catch (err) {
		logger.error(`Error deleting ${eTitle}`, err);
		done(err);
	}

	logger.info(`Successfully deleted ${eTitle}`);
};

const renameEvidenceType = async (evidenceType, done) => {
	let evidenceTypeInKeystone = evidenceTypeList.model.findOne({key: evidenceType["@id"] });
	let newTitle = evidenceType.prefLable["@value"];
	let oldTitle = evidenceTypeInKeystone.title;

	if(newTitle != evidenceTypeInKeystone.title){
		logger.info(`Renaming evidence type ${oldTitle} to ${newTitle}`);

		try {
			await evidenceTypeList.model.updateOne(
				{ key: evidenceType.id},
				{ $set: { title: newTitle } }
			);
			done();
		} catch (err) {
			logger.error(`Error renaming ${oldTitle} to ${newTitle}`, err);
			done(err);
		}
	
		logger.info(`Successfully renamed ${oldTitle} to ${newTitle}`);
	}	
}

const addNewEvidenceType = async (evidenceType, done) => {
	const {
		broader,
		title
	} = evidenceType;

	logger.info(`Adding new evidence type: ${title}`);

	const newEvidenceTypeDbModel = new evidenceTypeList.model({
		title: title,
		broaderTitle: broader,
		key: encodeURIComponent(title)
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
				latestEvidenceTypes = evidenceTypesJsonLd["@graph"];
			
			let callback = function (err) {
				if (err != null) {
					logger.error(err);
				}
			}

			// let b;
			// try {
			// 	b = await evidenceTypeList.model.find().exec();
			// 	logger.info('b:');
			// 	logger.info(b);
			// } catch (err) {
			// 	logger.error(`Error:`, err);
			// 	throw err;
			// }
			
			//Deletes
			let latestEvidenceTypesIds = latestEvidenceTypes.map(a => a["@id"]);
			let evidenceTypesToDelete;
			try {
				evidenceTypesToDelete = await evidenceTypeList.model.find( { key: { $nin: latestEvidenceTypesIds } }).exec();
				logger.info('evidenceTypesToDelete:');
				logger.info(evidenceTypesToDelete);
			} catch (err) {
				logger.error(`Error:`, err);
				throw err;
			}
			let deletes = async.forEach(evidenceTypesToDelete, deleteEvidenceType, callback);
			
			//Renames
			// let latestEvidenceTypesNames = latestEvidenceTypes.filter(a => a.prefLabel != undefined && a.prefLabel["@value"] != undefined);
			// logger.info(latestEvidenceTypesNames);
			// let evidenceTypesToRename;
			// try {
			// 	evidenceTypesToRename = await evidenceTypeList.model.find( { key: { $in: latestEvidenceTypesIds }}).exec();
			// 	logger.info('evidenceTypesToRename:');
			// 	logger.info(evidenceTypesToRename);
			// } catch (err) {
			// 	logger.error(`Error:`, err);
			// 	throw err;
			// }
			logger.info(latestEvidenceTypesIds);
			let latestEvidenceTypesx = latestEvidenceTypes.filter(a => a.prefLabel != undefined && a.prefLabel["@value"] != undefined).map(a => a.prefLabel["@value"]);
			logger.info(latestEvidenceTypesx);
			let updates = async.forEach(latestEvidenceTypesx, renameEvidenceType, callback); 


			// //Additions
			// let newEvidenceTypes = content.map(a => !latestEvidenceTypesNames.includes(a.prefLabel["@value"]));
			// evidenceTypeList.model.find( { key: { $in: latestEvidenceTypesIds }, title: { $ne: latestEvidenceTypesNames } } );
			// let adds = async.forEach(newEvidenceTypes, addNewEvidenceType, callback); 

			Promise.all([deletes, updates/*, adds*/]).then(() => {
				logger.info(`Finished update 05.`);
				done();
			});
		}
	);
};
