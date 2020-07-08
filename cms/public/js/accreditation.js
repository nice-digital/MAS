tinymce.PluginManager.add('accreditation', function (editor, url) {
    // Add a button that inserts the accreditation image withing an img tag.
    editor.addButton('accreditation', {
        tooltip: 'Insert accreditation image',
        icon: 'icon-accreditation',
        onclick: function () {
            editor.insertContent('<img style="display: inline-block;" src="https://www.medicinesresources.nhs.uk/nhs_accreditation.jpg" alt="nhs accreditation logo" width="20" height="20" />');
        }
    });
    return {
        getMetadata: function () {
            return {
                name: "Insert accreditation image"
            };
        }
    };
});
