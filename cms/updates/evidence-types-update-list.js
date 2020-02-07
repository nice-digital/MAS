const evidenceTypeUpdates = {
	evidenceTypesTodelete: [
		"Commissioning guides",
		"Health technology assessments",
		"Medicines evidence commentaries",
		"Evidence updates",
		"Eyes on Evidence Commentaries",
		"Ongoing or unpublished research",
		"Care pathways",
		"Drug horizon scanning",
		"Drug regulatory and marketing",
		"Evidence-based management reports",
		"Patient decision aids",
		"Drug Costs",
		"Implementation support tools"
	],
	evidenceTypesToRename: [
		{
			oldTitle: "Other economic evaluations",
			newTitle: "Health economics"
		}
	],
	newEvidenceTypes: [
		{
			broader: "afety alerts",
			title: "Safety support material"
		}
	]
}

exports = module.exports = function () {
	return evidenceTypeUpdates;
}
