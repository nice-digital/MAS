jest.mock("keystone", () => {
	const item = {
		model: {
			find: jest.fn()
		}
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
		json = jest.fn();
		response = { json, status: jest.fn(() => ({ json })) };
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

			expect(response.status).toHaveBeenCalledWith(500);
			expect(json).toHaveBeenCalledWith({ error: error });
		});

		it("should return a 404 JSON response when a weekly isn't found", async () => {
			Weekly.model.findOne.mockImplementation(() => ({
				exec: () => null
			}));

			await singleBySendDate(request, response);

			expect(response.status).toHaveBeenCalledWith(404);
			expect(json).toHaveBeenCalledWith({ error: "Weekly could not be found" });
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

		it("should populate the source, evidenceType and speciality fields on the items", async () => {
			Weekly.model.findOne.mockImplementation(() => ({
				exec: () => ({
					toObject: jest.fn()
				})
			}));

			const unusedPopulate = jest.fn();
			const thirdPopulate = jest.fn(() => ({
				populate: unusedPopulate
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

			expect(response.status).toHaveBeenCalledWith(500);
			expect(json).toHaveBeenCalledWith({ error: error });
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
				exec: () => [{ b: 2 }]
			};
			Item.model.find.mockImplementation(() => populate);

			await singleBySendDate({ params: {} }, response);

			expect(response.status).not.toHaveBeenCalled();
			expect(json).toHaveBeenCalledWith({ a: 1, items: [{ b: 2 }] });
		});
	});
});
