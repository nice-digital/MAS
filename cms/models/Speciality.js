var keystone = require("keystone");

var Speciality = new keystone.List("Speciality", {
	map: { name: "title" },
	// Specialities should only be added via a migration, because the names need
	// to correspond to MailChimp group names for subscribers to get personalised emails
	nodelete: true,
	nocreate: true,
	noedit: true
});

Speciality.add({
	title: { type: String, required: true },
	key: { type: String, required: true }
});

Speciality.relationship({
	ref: "Items",
	path: "items",
	refPath: "specialities"
});


Speciality.defaultColumns = "title, key";

Speciality.register();
