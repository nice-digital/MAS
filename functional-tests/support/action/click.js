import clickElement from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";

export const emailInput = () => {
	clickElement("click", "element", "//*[@name='email']");	
};
export const passwordInput = () => {
	clickElement("click", "element", "//*[@name='password']");	
};
export const submitButton = () => {
	clickElement("doubleClick", "element", "//BUTTON[@type='submit'][text()='Sign In']");	
};
export const signOut = () => {
	clickElement("doubleClick", "element", "//SPAN[@class='octicon octicon-sign-out']");	
};

export default emailInput;