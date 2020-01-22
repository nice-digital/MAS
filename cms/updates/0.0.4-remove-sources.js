const keystone = require("keystone"),
	async = require("async"),
	log4js = require("log4js");
	newSources = require("./sources2.json");

const Source = keystone.list("Source");

const logger = log4js.getLogger();

const removeSource = async (source, done) => {
	logger.info(`Source is ${source.title} with id ${source._id}`);

	let existsInNewSourceList = false;

	newSources.forEach(element => {if(element.oldEPiServerId == source.oldEPiServerId){existsInNewSourceList = true}})

	if(existsInNewSourceList == true){
		logger.info(`Not deleting ${source.title} as it exists in the new source list`)
	}else{
		logger.info(
			`Deleting source ${source.title} as it doesn't exist in the new source list`
		);
	
		try {
			await Source.model.findById(source._id)
						.remove(function(err) {
							if(err){
								logger.error("Error deleting `${}`", err);	
							}else{
								logger.info(`Successfully deleted ${source.title}`)
							}
						});
		} catch (e) {
			logger.error("Error deleting `${}`", e);
			throw e;
		}
	}
	done();
};

exports = module.exports = async function(done) {
	logger.info(`Removing sources that aren't on the new source list..`);

	let existingSources;
	try {
		existingSources = await Source.model
		.find({ oldEPiServerId: { $gt: 0 } })
		.exec();
	} catch (e) {
		logger.error("Error fetching sources with entities in update", e);
		throw e;
	}

	logger.info(
		`Found ${existingSources.length} sources to update`
	);

	async.forEach(
		existingSources,
		removeSource,
		done
	);
};
