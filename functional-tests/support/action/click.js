import clickElement from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";

export const emailInput = () => {	
	clickElement("click", "element", "//*[@name='email']");	
};
export const passwordInput = () => {
	clickElement("click", "element", "//*[@name='password']");	
};
export const submitButton = () => {
	clickElement("doubleClick", "element", "//BUTTON[@type='submit'][text()='Sign In']");
	browser.pause(2000);	
};
export const signOut = () => {
	clickElement("doubleClick", "element", "//SPAN[@class='octicon octicon-sign-out']");	
};
export const itemsButton = () => {
	clickElement("click", "element", "//div[@class='dashboard-group__list-label'][contains(text(),'Items')]");	
};
export const createItem = () => {
	clickElement("click", "element", "//SPAN[text()='Create Item']");	
};
export const createButton = () => {
	clickElement("click", "element", "//BUTTON[@type='submit'][text()='Create']");	
};
export const selectRecordItem = () => {
	browser.pause(2000);
	clickElement("click", "element", "//a[contains(text(),'NSAIDs Automated test again')]");	
};
export const selectEvidenceType = () => {
	clickElement("click", "element",  "[for=evidenceType] .Select-control .Select-placeholder");
};
export const save = () => {
	clickElement("click", "element", "//BUTTON[@data-button='update']");
	browser.pause(5000);
};

export default emailInput;