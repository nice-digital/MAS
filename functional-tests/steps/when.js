import "@nice-digital/wdio-cucumber-steps/lib/when";
import { When } from "cucumber";
// import submitForm from "@nice-digital/wdio-cucumber-steps/lib/support/action/submitForm";
import emailInput, { passwordInput, submitButton, signOut } from "../support/action/click";
import { addEmail, addPassword } from "../support/action/addInput";

When(
    /^I click the email input field$/,
        () => {
       emailInput
    }
);
When(
    /^I click the password input field$/,
        () => {
       passwordInput
    }
);
When(
    /^I click the Submit button$/,
      submitButton
    
);
When(
    /^I click the Sign Out button$/,
      signOut
    
);
When(
    /^I add the email address$/, 
        addEmail   
);
When(
    /^I add the set password$/, 
        addPassword    
);
