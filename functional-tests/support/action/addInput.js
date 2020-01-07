import setInputField from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import clearInputField from "@nice-digital/wdio-cucumber-steps/lib/support/action/clearInputField";

export const addEmail = () => {
	setInputField("add", "test@test.co.uk", "//*[@name='email']");	
};
export const addPassword = () => {
    setInputField("add", "Password1", "//*[@name='password']");
};
export const addTitle = () => {
    setInputField("add", "NSAIDs Automated test again", ".FormField__inner.field-size-full  [name=title]");
};
export const addSource = () => {
    browser.click("[for=source] .Select-control .Select-placeholder");
    browser.waitForVisible("#react-select-2--list", 5000);     
    browser.click("#react-select-2--option-1");
    browser.pause(5000);  
};
export const addShortSummary = () => {
    setInputField("add", "Nonsteroidal anti-inflammatory drugs (NSAIDs) are members of a drug class that reduces pain, decreases fever, prevents blood clots", "//*[@name='shortSummary']");
};
export const addUrl = () => {
   clearInputField("//*[@name='url']");
   setInputField("add", "https://bnfc.nice.org.uk/drug/abacavir.html", "//*[@name='url']");
};
export const addPublicationDate = () => {
    browser.click("//*[@name='publicationDate']");
    browser.click(".DayPicker-Week:nth-child(2) > .DayPicker-Day:nth-child(3)");  
    //setInputField("add", "2019-12-05", "//*[@name='publicationDate']");
};
export const addSpsComment = () => {
    //setInputField("add", "TThe first stop for professional medicines advice.", "iframe#keystone-html-0_ifr");
    var spsComment = browser.getAttribute("[for='comment'] iframe", 'id');
    browser.pause(5000)
    console.log(spsComment);
    browser.frame(spsComment);
    browser.waitForExist('#tinymce');
    console.log($('#tinymce'));    
    var editorBody = $('#tinymce > p');
    editorBody.click();
    editorBody.keys("The first stop for professional medicines advice.");
    browser.frameParent("tinyMCE.activeEditor.insertContent('<p>hi</p>')");
}
export const addResourceLink = () => {
    var resourceLink = browser.getAttribute("[for='resourceLinks'] iframe", 'id');
    browser.pause(5000)
    console.log(resourceLink);
    browser.frame(resourceLink);
    browser.waitForExist('#tinymce');
    console.log($('#tinymce'));    
    var editorBody = $('#tinymce > p');
    editorBody.click();
    editorBody.keys("To access content relevant to you, create an account and sign in each time you use our website");
    browser.frameParent("tinyMCE.activeEditor.insertContent('<p>hi</p>')");
}

export default addEmail;