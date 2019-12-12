var keystone = require("keystone");

/**
 * PostCategory Model
 * ==================
 */

var ItemCategory = new keystone.List("ItemCategory", {
	autokey: { from: "name", path: "key", unique: true },
	plural: "Categories"
});

ItemCategory.add({
	name: { type: String, required: true }
});

ItemCategory.relationship({
	ref: "Items",
	path: "items",
	refPath: "categories"
});

ItemCategory.register();
