jest.mock("keystone", () => {
	const list = {
		add: jest.fn(),
		schema: {
			pre: jest.fn()
		},
		relationship: jest.fn(),
		register: jest.fn()
	};

	return {
		// Use a normal func and not an arrow so it can be used as a constructor
		List: function() {
			return list;
		},
		Field: {
			Types: {}
		}
	};
});

describe("Weekly", () => {
	let keystone, pre;

	beforeEach(() => {
		jest.resetModules();
		require("../../models/Weekly");
		keystone = require("keystone");

		pre = keystone.List("Weekly").schema.pre;
	});

	it("should set up a pre-validate function", () => {
		expect(pre).toHaveBeenCalledWith("validate", expect.any(Function));
	});

	it("should call next with no errors when validation succeeds", () => {
		const validatorFn = pre.mock.calls[0][1];

		const next = jest.fn();
		validatorFn.call({}, next);

		expect(next).toHaveBeenCalledWith();
	});

	it.each`
		field                  | filledIn     | emptyFields
		${"commentaryTitle"}   | ${"title"}   | ${"summary and body"}
		${"commentarySummary"} | ${"summary"} | ${"title and body"}
		${"commentaryBody"}    | ${"body"}    | ${"title and summary"}
	`(
		"should validate commentary $emptyFields when $filledIn is set",
		({ field, filledIn, emptyFields }) => {
			const pre = keystone.List("Weekly").schema.pre;

			expect(pre).toHaveBeenCalledWith("validate", expect.any(Function));

			const validatorFn = pre.mock.calls[0][1];

			const next = jest.fn();
			validatorFn.call(
				{
					[field]: "Test content"
				},
				next
			);

			const errorMessage = `Commentary ${emptyFields} are both required if commentary ${filledIn} is set`;

			expect(next).toHaveBeenCalledWith(Error(errorMessage));
		}
	);
});
