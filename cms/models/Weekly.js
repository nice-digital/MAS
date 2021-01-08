var keystone = require("keystone");
var Types = keystone.Field.Types;

var Weekly = new keystone.List("Weekly", {
	map: { name: "title" },
	track: true,
	nocreate: true,
	nodelete: true,
	defaultSort: "-sendDate"
});

Weekly.add({
	title: {
		type: Types.Text,
		required: true
	},
	startDate: {
		type: Types.Date,
		label: "Start date",
		noedit: true,
		hidden: true
	},
	endDate: {
		type: Types.Date,
		label: "End date",
		noedit: true,
		hidden: true
	},
	sendDate: {
		type: Types.Date,
		label: "Send date",
		format: "ddd Do MMM YYYY",
		noedit: true,
		note: "The Monday after, when the weekly email is actually sent"
	},
	commentaryTitle: {
		type: Types.Text,
		label: "Commentary title",
		note:
			'Optional.<br>E.g. "New MHRA drug safety advice: September to November 2019"'
	},
	commentarySummary: {
		type: Types.Html,
		wysiwyg: true,
		height: 200,
		label: "Commentary summary",
		note:
			"Short summary (a few paragraphs) for the weekly's commentary.<br>Required if you've set a commentary title."
	},
	commentaryBody: {
		type: Types.Html,
		height: 500,
		label: "Full commentary",
		note:
			"The full commentary.<br>Required if you've set a commentary title and summary.",
		wysiwyg: {
			overrideToolbar: true,
			additionalButtons:
				"styleselect | bold italic | bullist numlist | link | accreditation | code "
		}
	}
});

Weekly.schema.pre("validate", function(next) {
	if (this.commentaryTitle && (!this.commentarySummary || !this.commentaryBody))
		return next(
			Error(
				"Commentary summary and body are both required if commentary title is set"
			)
		);
	if (this.commentarySummary && (!this.commentaryTitle || !this.commentaryBody))
		return next(
			Error(
				"Commentary title and body are both required if commentary summary is set"
			)
		);
	if (this.commentaryBody && (!this.commentaryTitle || !this.commentarySummary))
		return next(
			Error(
				"Commentary title and summary are both required if commentary body is set"
			)
		);

	next();
});

Weekly.relationship({
	ref: "Item",
	path: "items",
	refPath: "weekly"
});

Weekly.defaultColumns = "title, sendDate";
Weekly.register();
