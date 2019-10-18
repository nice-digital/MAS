var keystone = require("keystone");

/**
 * PostCategory Model
 * ==================
 */

var ItemCateogry = new keystone.List("ItemCategory", {
	autokey: { from: "name", path: "key", unique: true }
});

ItemCateogry.add({
	name: { type: String, required: true }
});

ItemCateogry.relationship({
	ref: "Items",
	path: "items",
	refPath: "categories"
});

ItemCateogry.register();
