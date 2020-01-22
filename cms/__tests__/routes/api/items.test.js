jest.mock("keystone", () => {
	const item = {
		model: {
			find: jest.fn()
		},
		fullResponseFields: ["_id", "title", "slug"]
	};

	return {
		list: () => item
	};
});

describe("items", () => {
	let keystone, daily, Item, request, response, json;

	beforeEach(() => {
		jest.resetModules();

		keystone = require("keystone");
		Item = keystone.list("Item");
		daily = require("../../../routes/api/items").daily;
		request = { params: {} };
		json = jest.fn();
		response = {
			json,
			badRequest: jest.fn(),
			error: jest.fn()
		};
	});

	describe("daily", () => {});

	it("should return a 400 bad request when the date format is invalid", async () => {
		const date = "not a date";
		await daily({ ...request, ...{ params: { date } } }, response);

		expect(response.badRequest).toHaveBeenCalledWith(
			"Couldn't get daily items",
			"Date 'not a date' is not in the format YYYY-M-D",
			true
		);
	});

	it("should search for daily items between start and end of the given date", async () => {
		const date = "2020-01-09";
		await daily({ ...request, ...{ params: { date } } }, response);

		expect(Item.model.find).toHaveBeenCalledWith({
			createdAt: {
				$gte: new Date(Date.parse("2020-01-09")),
				$lt: new Date(Date.parse("2020-01-09 23:59:59.999Z"))
			}
		});
	});

	it("should return a 500 JSON error response when there's an error getting the daily items", async () => {
		const error = new Error("An error getting daily items");

		Item.model.find.mockImplementation(() => {
			throw error;
		});

		await daily(
			{ ...request, ...{ params: { date: "2020-01-09" } } },
			response
		);

		expect(response.error).toHaveBeenCalledWith(error, true);
	});

	it("should return the found item as json with whitelist of fields", async () => {
		const items = [{ title: "test", excludedField: "not used" }];

		Item.model.find.mockImplementation(() => {
			return {
				populate: () => ({
					populate: () => ({
						populate: () => ({
							select: () => ({
								exec: () => items
							})
						})
					})
				})
			};
		});

		await daily(
			{ ...request, ...{ params: { date: "2020-01-09" } } },
			response
		);

		expect(json).toHaveBeenCalledWith([{ title: "test" }]);
	});
});
