import setInputField from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";

export const addEmail = () => {
	setInputField("add", "test@test.co.uk", "//*[@name='email']");	
};

export const addPassword = () => {
    setInputField("add", "Password1", "//*[@name='password']");
};

export default addEmail;