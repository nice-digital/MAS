const keystone = require("keystone"),
	async = require("async"),
	log4js = require("log4js"),
	migrationList = require("./evidence-types-update-list.js");

const logger = log4js.getLogger();
const EvidenceTypeList = keystone.list("EvidenceType");

const deleteEvidenceType = async (title, done) => {
	logger.info(`Deleting evidence type: ${title}`);

	try {
		await EvidenceTypeList.model.find({ title: title }).remove();
		logger.info(`Successfully deleted ${title}`);
		done();
	} catch (err) {
		logger.error(`Error deleting ${title}`, err);
		done(err);
	}
};

const renameEvidenceType = async (evidenceType, done) => {
	const {
		oldTitle,
		newTitle
	} = evidenceType;

	logger.info(`Renaming evidence type ${oldTitle} to ${newTitle}`);

	try {
		let item = await EvidenceTypeList.model.find({ title: oldTitle });
		item.title = newTitle;
		logger.info(`Successfully renamed ${oldTitle} to ${newTitle}`);
		done();
	} catch (err) {
		logger.error(`Error renaming ${oldTitle} to ${newTitle}`, err);
		done(err);
	}
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
