module.exports = {
	testResultsProcessor: "jest-teamcity-reporter",
	collectCoverageFrom: [
		"<rootDir>/models/**/*.{js,jsx}",
		"<rootDir>/routes/**/*.{js,jsx}"
	],
	collectCoverage: process.env.TEAMCITY_VERSION ? true : false
};
