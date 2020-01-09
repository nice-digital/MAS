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
    browser.click("[for='source'] .Select-control .Select-placeholder");
    browser.waitForVisible("[for='source'] .Select-menu", 5000)
    browser.click("[for='source'] .Select-menu .Select-option#react-select-2--option-1");
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
    browser.pause(2000);
    setInputField("set", "2020-01-20", "//INPUT[@id='_DateInput_1']") 
};
export const addSpsComment = () => {
    setTextFieldValue("[for='comment'] iframe","The first stop for professional medicines advice.",0,0);
};
export const addResourceLink = () => { 
    setTextFieldValue("[for='resourceLinks'] iframe", "To access content relevant to you, create an account and sign in each time you use our website", 580, 556);
};

// Perhaps move to https://github.com/nice-digital/wdio-cucumber-steps if not application specific
function setTextFieldValue(selector, value, scrollX, scrollY){
    if(scrollX && scrollY){
        browser.scroll(scrollX,scrollY);
    }
    var frameID = browser.getAttribute(selector, 'id');
    browser.pause(5000);    
    browser.frame(frameID);
    browser.waitForExist('#tinymce');      
    var editorBody = $('#tinymce > p');
    editorBody.click();
    editorBody.keys(value);
    browser.frameParent();
};

export default addEmail;