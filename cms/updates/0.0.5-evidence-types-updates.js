const keystone = require("keystone"),
	async = require("async"),
	log4js = require("log4js"),
	migrationList = require("./evidence-types-update-list.js");

const logger = log4js.getLogger();
const EvidenceTypeList = keystone.list("EvidenceType");

const deleteEvidenceType = async (etitle, done) => {
	logger.info(`Deleting evidence type: ${etitle}`);

	try {
		await EvidenceTypeList.model.findOne({ title: { "$regex": etitle, "$options": "i" } }).remove();
		done();
	} catch (err) {
		logger.error(`Error deleting ${etitle}`, err);
		done(err);
	}

	logger.info(`Successfully deleted ${etitle}`);
};

const renameEvidenceType = async (evidenceType, done) => {
	const {
		oldTitle,
		newTitle
	} = evidenceType;

	logger.info(`Renaming evidence type ${oldTitle} to ${newTitle}`);

	try {
		await EvidenceTypeList.model.updateOne(
			{ title: { "$regex": oldTitle, "$options": "i" } },
			{ $set: { title: newTitle } }
		);
		done();
	} catch (err) {
		logger.error(`Error renaming ${oldTitle} to ${newTitle}`, err);
		done(err);
	}

	logger.info(`Successfully renamed ${oldTitle} to ${newTitle}`);
};

const addNewEvidenceType = async (evidenceType, done) => {
	const {
		broader,
		title
	} = evidenceType;

	logger.info(`Adding new evidence type: ${title}`);

	const newEvidenceTypeDbModel = new EvidenceTypeList.model({
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
	const {
		evidenceTypesTodelete,
		evidenceTypesToRename,
		newEvidenceTypes
	} = migrationList();

	let callback = function (err) {
		if (err != null) {
			logger.error(err);
		}
	}

	let deletes = async.forEach(evidenceTypesTodelete, deleteEvidenceType, callback);
	let updates = async.forEach(evidenceTypesToRename, renameEvidenceType, callback);
	let adds = async.forEach(newEvidenceTypes, addNewEvidenceType, callback);

	Promise.all([deletes, updates, adds]).then(() => {
		logger.info(`Finished update 05.`);
		done();
	});
};
