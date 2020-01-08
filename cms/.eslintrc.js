module.exports = {
	parser: "babel-eslint",
	extends: [
		"@nice-digital/eslint-config/es6",
		"plugin:prettier/recommended",
		"prettier/standard"
	],
	parserOptions: {
		ecmaFeatures: {
			jsx: true,
			modules: true
		}
	},
	env: {
		es6: true,
		jest: true
	}
};
