const keystone = require("keystone"),
	async = require("async"),
	he = require("he"),
	log4js = require("log4js");

const Source = keystone.list("Source");

const logger = log4js.getLogger();

const htmlEntityRegexStr = "&[^&;\\s]{2,6};";

const replaceHtmlEntitiesInSource = async (source, done) => {
	const oldTitle = source.title,
		// Treat nbsp as a special case because they look weird in git bash
		newTitle = he.decode(source.title.replace(/&nbsp;/g, " "));

	logger.info(
		`Updating '${oldTitle}' to '${newTitle}' for source '${source._id}'`
	);

	source.title = newTitle;

	try {
		await source.save();
	} catch (e) {
		logger.error("Error saving `${}`", e);
		throw e;
	}

	done();
};

exports = module.exports = async function(done) {
	logger.info(`Updating sources containg HTML entities in their title...`);

	let sourcesWithHtmlEntitiesInTitle;
	try {
		sourcesWithHtmlEntitiesInTitle = await Source.model
			.find({ title: { $regex: htmlEntityRegexStr } })
			.exec();
	} catch (e) {
		logger.error("Error fetching sources with entities in update", e);
		throw e;
	}

	logger.info(
		`Found ${sourcesWithHtmlEntitiesInTitle.length} sources to update`
	);

	async.forEach(
		sourcesWithHtmlEntitiesInTitle,
		replaceHtmlEntitiesInSource,
		done
	);
};
