jest.mock("keystone", () => {
	const item = {
		model: {
			find: jest.fn()
		},
		fullResponseFields: ["_id", "title", "slug"]
	};

	const weekly = {
		model: {
			findOne: jest.fn(() => ({
				exec: jest.fn()
			}))
		}
	};

	return {
		list: name => {
			switch (name) {
				case "Item":
					return item;
				case "Weekly":
					return weekly;
				default:
					throw new Error("listName must be either Item or Weekly");
			}
		}
	};
});

describe("weeklies", () => {
	let keystone, singleBySendDate, Weekly, Item, request, response, json;

	beforeEach(() => {
		jest.resetModules();

		keystone = require("keystone");
		Weekly = keystone.list("Weekly");
		Item = keystone.list("Item");
		singleBySendDate = require("../../../routes/api/weeklies").singleBySendDate;
		request = { params: {} };
		response = { json: jest.fn(), error: jest.fn(), notfound: jest.fn() };
	});

	describe("singleBySendDate", () => {
		it("should search for a weekly based on the given send date parameter", async () => {
			const sendDate = "2020-01-09";
			await singleBySendDate(
				{ ...request, ...{ params: { sendDate: sendDate } } },
				response
			);

			expect(Weekly.model.findOne).toHaveBeenCalledWith({ sendDate: sendDate });
		});

		it("should return a 500 JSON error response when there's an error getting the weekly", async () => {
			const error = new Error("An error getting a weekly");

			Weekly.model.findOne.mockImplementation(() => {
				throw error;
			});

			await singleBySendDate({ params: {} }, response);

			expect(response.error).toHaveBeenCalledWith(error, true);
		});

		it("should return a 404 JSON response when a weekly isn't found", async () => {
			Weekly.model.findOne.mockImplementation(() => ({
				exec: () => null
			}));

			await singleBySendDate(request, response);

			expect(response.notfound).toHaveBeenCalledWith(
				"Weekly could not be found",
				expect.any(String),
				true
			);
		});

		it("should request items for the found weekly's id", async () => {
			const weeklyId = 1234;
			Weekly.model.findOne.mockImplementation(() => ({
				exec: () => ({
					_id: weeklyId,
					toObject: jest.fn()
				})
			}));

			await singleBySendDate(request, response);

			expect(Item.model.find).toHaveBeenCalledWith({ weekly: weeklyId });
		});

		it("should populate the required fields on the items", async () => {
			Weekly.model.findOne.mockImplementation(() => ({
				exec: () => ({
					toObject: jest.fn()
				})
			}));

			const select = jest.fn(() => ({
				exec: jest.fn()
			}));
			const unusedPopulate = jest.fn();
			const thirdPopulate = jest.fn(() => ({
				populate: unusedPopulate,
				select: select
			}));
			const secondPopulate = jest.fn(() => ({
				populate: thirdPopulate
			}));
			const firstPopulate = jest.fn(() => ({
				populate: secondPopulate
			}));

			Item.model.find.mockImplementation(() => ({
				populate: firstPopulate
			}));

			await singleBySendDate(request, response);

			expect(firstPopulate).toHaveBeenCalledWith("source");
			expect(secondPopulate).toHaveBeenCalledWith("evidenceType");
			expect(thirdPopulate).toHaveBeenCalledWith("specialities");
			expect(unusedPopulate).not.toHaveBeenCalledWith();
			expect(select).toHaveBeenCalledWith(Item.fullResponseFields.join(" "));
		});

		it("should return a 500 JSON error response when there's an error getting the items", async () => {
			Weekly.model.findOne.mockImplementation(() => ({
				exec: () => ({})
			}));

			const error = new Error("An error getting items");
			Item.model.find.mockImplementation(() => {
				throw error;
			});

			await singleBySendDate({ params: {} }, response);

			expect(response.error).toHaveBeenCalledWith(error, true);
		});

		it("should return the weekly and items as JSON", async () => {
			Weekly.model.findOne.mockImplementation(() => ({
				exec: () => ({
					toObject: () => ({
						a: 1
					})
				})
			}));

			const populate = {
				populate: () => populate,
				select: () => populate,
				exec: () => [{ title: "test", something: false }]
			};
			Item.model.find.mockImplementation(() => populate);

			await singleBySendDate({ params: {} }, response);

			expect(response.json).toHaveBeenCalledWith({
				a: 1,
				items: [{ title: "test" }]
			});
		});
	});
});
