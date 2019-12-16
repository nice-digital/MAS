const keystone = require("keystone"),
	async = require("async"),
	log4js = require("log4js"),
	fs = require("fs"),
	path = require("path");

const logger = log4js.getLogger("0.0.2-specialities.js");
logger.level = "debug";

const SpecialityList = keystone.list("Speciality");

const updateSpeciality = async (element, done) => {
	const title = element.prefLabel["@value"],
		key = element["@id"];

	const specialityDbEntity = await SpecialityList.model
		.findOne({
			title
		})
		.exec();

	if (!specialityDbEntity) {
		const errMsg = `Couldn't find speciality with title "${title}"`;
		loggger.error(errMsg);
		throw new Error(errMsg);
	}

	logger.info(
		`Replacing key "${specialityDbEntity.key}" with "${key}" for speciality "${title}" ("${specialityDbEntity.id}")`
	);

	SpecialityList.updateItem(
		specialityDbEntity,
		{
			key
		},
		{
			ignoreNoEdit: true
		},
		err => {
			if (err) {
				logger.error(err);
				throw err;
			} else {
				logger.info(
					`Replaced speciality key for "${title}" ("${specialityDbEntity.id}")`
				);
				done();
			}
		}
	);
};

exports = module.exports = function(done) {
	fs.readFile(
		path.resolve(__dirname, "0.0.2-specialities.jsonld"),
		"UTF-8",
		function(err, contents) {
			if (err) {
				loggger.error(err);
				throw err;
			}

			const specialitiesJsonLd = JSON.parse(contents);

			const specialities = specialitiesJsonLd["@graph"].filter(
				element => element.broader
			);

			async.forEach(specialities, updateSpeciality, done);
		}
	);
};
