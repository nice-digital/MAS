import "@nice-digital/wdio-cucumber-steps/lib/when";
import { When } from "cucumber";
import emailInput, { passwordInput, submitButton, signOut, createItem, selectSource, createButton, selectRecordItem, selectEvidenceType, selectSpecialities, save, itemsButton } from "../support/action/click";
import { addEmail, addPassword, addTitle, addShortSummary, addSource } from "../support/action/addInput";

When(
    /^I click the email input field$/,     
    emailInput  
);
When(
    /^I add the email address$/, 
    addEmail   
);
When(
    /^I click the password input field$/,      
    passwordInput
);
When(
    /^I add the password$/, 
    addPassword    
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
    /^I click on the items button$/, 
    itemsButton
);
When(
    /^I click on the create item button$/, 
    createItem   
);
When(
    /^I add a Title$/,
    addTitle   
);
When(
    /^I add the Source$/, 
    addSource          
);
When(
    /^I click to select the Evidence type$/,
    selectEvidenceType         
);
When(
    /^I click on the create button$/, 
    createButton
);
When(
    /^I click on an item record$/,
    selectRecordItem       
);
When(
    /^I click on the Save button$/,
        save   
);

