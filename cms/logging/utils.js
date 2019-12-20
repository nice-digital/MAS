const moment = require("moment"),
	os = require("os"),
	{ serializeError } = require("serialize-error");

// Log4js levels are all uppercase but we want levels like "Error" to match nice logging
const getNiceLogLevelStr = logLevel =>
	logLevel[0] + logLevel.substr(1).toLowerCase();

const getMessage = dataArray =>
	dataArray
		.map(v => {
			if (v instanceof Error) {
				return v.message;
			} else if (typeof v == "string") {
				return v;
			}
			return "";
		})
		.join(" ");

const niceLoggingLayout = config => {
	return log4jsEvent => {
		const {
			startTime,
			data, // An array of strings/objects E.g. [ 'An error', Error ]
			level, // E.g. { level: 40000, levelStr: 'ERROR', colour: 'red' }
			context,
			functionName,
			fileName,
			lineNumber,
			columnNumber,
			callStack
		} = log4jsEvent;

		const niceLogEvent = {
			// Capitalized field names to match nice logging
			Timestamp: moment(startTime).toISOString(true),
			NodeEnv: process.env.NODE_ENV,
			Environment: "test",
			Application: "MAS-CMS",
			MachineName: os.hostname(),
			Message: getMessage(data),
			Level: getNiceLogLevelStr(level.levelStr),
			Properties: {
				RawData: JSON.stringify(data, null, 2),
				FunctionName: functionName,
				FileName: fileName,
				LineNumber: lineNumber,
				ColumnNumber: columnNumber,
				CallStack: callStack
			}
		};

		if (level.levelStr === "ERROR") {
			const error = data.find(l => l instanceof Error);

			if (error) {
				niceLogEvent.Exception = serializeError(error);
			}
		}

		return JSON.stringify(niceLogEvent);
	};
};

module.exports = {
	getNiceLogLevelStr,
	niceLoggingLayout
};
