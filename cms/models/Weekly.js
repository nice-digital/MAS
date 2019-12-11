var keystone = require("keystone");
var Types = keystone.Field.Types;

var Weekly = new keystone.List("Weekly", {
	map: { name: "title" }
});

Weekly.add({
	title: { 
		type: Types.Text, 
		required: true, 
		initial: true,
	},
	// items: {
	// 	type: Types.Relationship,
	// 	ref: "Item",
	// 	many: true,
	// 	label: "Daily items",
	// },
	weekCommencing: { 
		type: Types.Datetime, 
		default: Date.now,
		label: "Created date",
	},
	createdDate: { 
		type: Types.Datetime, 
		default: Date.now,
		label: "Created date",
	},
	commentary: {
		type: Types.Html,
		wysiwyg: true
	}
});

Weekly.relationship({
	ref: "Item",
	path: "items",
	refPath: "weekly"
});



Weekly.defaultColumns = "title";
Weekly.register();
