exports.create = {
	User: [
		{
			"name.first": "Admin",
			"name.last": "User",
			email: process.env.KEYSTONE_ADMIN_EMAIL,
			password: process.env.KEYSTONE_ADMIN_PASSWORD,
			isAdmin: true
		}
	]
};
