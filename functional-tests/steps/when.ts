import "@nice-digital/wdio-cucumber-steps/lib/when";
import { When } from "@cucumber/cucumber";
import emailInput, { passwordInput, submitButton, signOut, createItem, selectSource, createButton, selectRecordItem, selectEvidenceType, navigateItemPage, selectNewItem, selectRecordAccessibility, specialitiesButton, usersButton, evidenceTypesButton, selectSpecialities, sourcesButton, weekliesButton, save, itemsButton, manageButton, itemRecordAdded, deleteButton, deleteItem, navigateHomePage } from "../support/action/click";
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
When(
    /^I click the manage button$/,
    manageButton   
);
When(
    /^I click on the item record added$/,
    itemRecordAdded     
);
When(
    /^I click on the delete button$/, 
    deleteButton
);
When( 
    /^I click the confirm delete button on the dialog box$/,
    deleteItem
);

When(
    /^I click on the weeklies button$/, 
    weekliesButton
);

When(
    /^I click on the sources button$/, 
    sourcesButton
);

When(
    /^I click on the specialities button$/, 
    specialitiesButton
);

When(
    /^I click on the evidence types button$/, 
    evidenceTypesButton
);

When(
    /^I click on the users button$/, 
    usersButton
);
When(
    /^I click on an item record and check accesibility issues$/,
    selectRecordAccessibility       
);
When(
    /^I navigate to the Items page$/,
    navigateItemPage       
);
When(
    /^I navigate to home page$/, 
    navigateHomePage
);