var keystone = require("keystone");
var async = require("async");
var Speciality = keystone.list("Speciality");

const getSpecialities = function() {
	// TODO: We should get these from TopBraid so mimic a promise
	return Promise.resolve([
		{
			key: "023caa1d-543e-4cd4-a89d-8eaa2c0ec513",
			title: "Allergy and immunology"
		},
		{
			key: "be1c2e2f-745e-4a82-b5aa-d4cef4d31a1b",
			title: "Anaesthesia and pain"
		},
		{ key: "8f13726e-5635-471f-ad3c-fc910a6ac2b1", title: "Cancers" },
		{
			key: "c3e39a56-de18-4679-8db0-58f3a098c750",
			title: "Cardiovascular system disorders"
		},
		{
			key: "53fc67a4-46d8-4171-9447-7fcf216c8749",
			title: "Complementary and alternative therapies"
		},
		{ key: "3d41226d-6fd4-420a-9ae6-62b36e2f365c", title: "Critical care" },
		{ key: "0a7bdd5b-d027-48a9-813f-707b8f7e2d8f", title: "Diabetes" },
		{
			key: "99d0f92c-964c-4030-8f09-70780fbab2ac",
			title: "Ear, nose and throat disorders"
		},
		{
			key: "ea782323-1ebb-44d6-ba9e-2d97e78b8cf5",
			title: "Emergency medicine and urgent care"
		},
		{
			key: "3cc45ece-e3d4-40fc-960b-5f2c436c1ed2",
			title: "Endocrine system disorders"
		},
		{ key: "63ae3f54-a830-4931-857a-81ffd126133e", title: "Eyes and vision" },
		{ key: "ec13dcdc-548b-4f77-b526-a09111bfb57a", title: "Family planning" },
		{
			key: "324813be-a93e-485a-89bd-8464e4a412e0",
			title: "Gastrointestinal disorders"
		},
		{ key: "04935977-4565-445f-ab0f-6f1e5ad823fd", title: "Genetics" },
		{
			key: "6c632dfa-b6a2-4b3a-ac31-4c4ebe22a2d2",
			title: "Haematological disorders"
		},
		{
			key: "f48a5fc4-8228-47ec-ba84-e6c8e22ded59",
			title: "Infection and infectious diseases"
		},
		{ key: "cb0faa85-1781-484e-a265-2715621c1be9", title: "Later life" },
		{
			key: "fb8766ce-2c88-4db6-a56d-906ee2ee2603",
			title: "Learning disabilities"
		},
		{ key: "e7c0f8c0-3fea-47ed-a4c2-6f1084b3d2b4", title: "Liver disorders" },
		{
			key: "9e36e9bc-220b-41d6-9d5c-600921d39e72",
			title: "Mental health and illness"
		},
		{
			key: "ab8b4819-55dc-4ab6-99e1-0178e38cd210",
			title: "Musculo-skeletal disorders"
		},
		{
			key: "793637c7-9615-42b1-905c-4827a6fab04c",
			title: "Neurological disorders"
		},
		{
			key: "3bc103c8-14ae-4068-acf1-ae7f4257f103",
			title: "Nutritional and metabolic disorders"
		},
		{
			key: "06944b54-b666-4f00-8c80-74eca60efcbc",
			title: "Obstetrics and gynaecology"
		},
		{
			key: "55117a7f-908f-4b68-af57-f40f82389551",
			title: "Oral and dental health"
		},
		{
			key: "26854420-0bd0-4998-934d-95d393d2bfd2",
			title: "Other / Unclassified"
		},
		{
			key: "141aeb17-6599-4c05-a6fe-a64e5b994141",
			title: "Paediatric and neonatal medicine"
		},
		{
			key: "228c0a56-8d82-4efa-9ae2-bb370d06ffe7",
			title: "Palliative and End of Life Care"
		},
		{
			key: "5a18afa9-e3b0-4f40-a131-2bd007c6bab7",
			title: "Policy, Commissioning and Managerial"
		},
		{
			key: "67588b95-7ebb-4283-99aa-a6bddfa7ea43",
			title: "Renal and urologic disorders"
		},
		{
			key: "eb492904-ca53-4abb-9e61-15871d3b1cb9",
			title: "Respiratory disorders"
		},
		{ key: "5228bed0-6785-4534-95f4-b5e10bf1b4c8", title: "Sexual health" },
		{ key: "3665f6cd-25c6-4c1b-9a81-3f61a93a68e4", title: "Skin disorders" },
		{ key: "34486ab5-d29c-440c-8a43-3f7c72b01043", title: "Sports medicine" },
		{ key: "d0ea592c-932a-462f-b206-cf454e40fa20", title: "Stroke" },
		{ key: "fa73b654-cce0-4cae-ae27-b87463aff958", title: "Surgery" },
		{ key: "e52e495c-c963-43fc-92f4-47061994e034", title: "Travel medicine" },
		{ key: "5a1195de-2f22-4ac5-8ccc-b5b3d292da87", title: "Vaccination" },
		{ key: "55d7201c-9fd0-4db5-92b3-b354ac0c07a8", title: "Wounds and injuries" }
	]);
};

function createSpeciality(speciality, done) {
	var newSpeciality = new Speciality.model(speciality);

	newSpeciality.save(function(err) {
		if (err) {
			console.error(
				"Error adding speciality " + speciality.title + " to the database:"
			);
			console.error(err);
		} else {
			console.log("Added speciality " + speciality.title + " to the database.");
		}
		done(err);
	});
}

exports = module.exports = function(done) {
	getSpecialities().then(specialities => {
		async.forEach(specialities, createSpeciality, done);
	});
};
