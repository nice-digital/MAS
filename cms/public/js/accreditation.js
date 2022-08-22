window.tinymce.PluginManager.add("accreditation", function(editor) {
	// Add a button that inserts the accreditation image withing an img tag.
	editor.ui.registry.addButton("accreditation", {
		tooltip: "Insert accreditation image",
		icon: "template",
		onAction: function() {
			editor.insertContent(
				'<img style="display: inline-block;" src="https://www.medicinesresources.nhs.uk/nhs_accreditation.jpg" alt="nhs accreditation logo" width="20" height="20" />'
			);
		}
	});
	return {
		getMetadata: function() {
			return {
				name: "Insert accreditation image"
			};
		}
	};
});
