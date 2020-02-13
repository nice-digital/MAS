const keystone = require("keystone"),
	async = require("async"),
	log4js = require("log4js"),
	fs = require("fs");


const logger = log4js.getLogger();
const evidenceTypeList = keystone.list("EvidenceType");

const deleteEvidenceType = async (evidenceType, done) => {
	let eTitle = evidenceType.title;
	logger.info(`Deleting evidence type: ${etitle}`);

	try {
		await evidenceTypeList.model.findOne({ title: { "$regex": etitle, "$options": "i" } }).remove();
		done();
	} catch (err) {
		logger.error(`Error deleting ${etitle}`, err);
		done(err);
	}

	logger.info(`Successfully deleted ${etitle}`);
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

exports = module.exports = function (done) {
	logger.info(`Started update 05.`);

	fs.readFile(
		path.resolve(__dirname, "0.0.5-evidence-types-update.jsonld"),
		"UTF-8",
		function(err, contents){
			const evidenceTypesJsonLd = JSON.parse(contents),
				latestEvidenceTypes = evidenceTypesJsonLd["@graph"];
			
			let callback = function (err) {
				if (err != null) {
					logger.error(err);
				}
			}
			
			let latestEvidenceTypesIds = latestEvidenceTypes.map(a => a["@id"]);
			let evidenceTypesToDelete = evidenceTypeList.find( { key: { $nin: latestEvidenceTypesIds } } );
			let deletes = async.forEach(evidenceTypesTodelete, deleteEvidenceType, callback);
			
			let latestEvidenceTypesNames = content.map(a => a.prefLabel["@value"]);
			evidenceTypeList.find( { key: { $in: latestEvidenceTypesIds }, title: { $ne: latestEvidenceTypesNames } } );
			let updates = async.forEach(evidenceTypesTodelete, renameEvidenceType, callback); 

			let newEvidenceTypes = content.map(a => !latestEvidenceTypesNames.includes(a.prefLabel["@value"]));
			evidenceTypeList.find( { key: { $in: latestEvidenceTypesIds }, title: { $ne: latestEvidenceTypesNames } } );
			let adds = async.forEach(newEvidenceTypes, addNewEvidenceType, callback); 

			Promise.all([deletes, updates, adds]).then(() => {
				logger.info(`Finished update 05.`);
				done();
			});
		}
	);
};
