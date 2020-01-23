const keystone = require("keystone"),
	async = require("async"),
	log4js = require("log4js");
	newSources = require("./sources2.json");

const Source = keystone.list("Source");

const logger = log4js.getLogger();

const removeSource = async (source, done) => {
	const { title, _id } = source;

	const existsInNewSourceList = newSources.some(element => element.oldEPiServerId == source.oldEPiServerId);

	if(existsInNewSourceList){
		logger.debug(`Not deleting ${title} as it exists in the new source list`)
		done();
		return;
	}
	
	logger.debug(
		`Deleting source ${title} as it doesn't exist in the new source list`
	);

	try {
		await Source.model.findById(_id)
				.remove(function(err) {
					if(err){
						logger.error(`Error deleting ${title}`, err);	
					}else{
						logger.info(`Successfully deleted ${title}`)
					}
				});
	} catch (err) {
		logger.error(`Error deleting ${title}`, err);
		throw err;
	}
	
	done();
};

exports = module.exports = async function(done) {
	logger.info(`Removing sources that aren't on the new source list..`);

	let existingSources;
	try {
		existingSources = await Source.model
		.find()
		.exec();
	} catch (err) {
		logger.error(`Error fetching sources with entities in update`, err);
		throw err;
	}

	logger.debug(
		`Found ${existingSources.length} sources to update`
	);

	async.forEach(
		existingSources,
		removeSource,
		done
	);
};
