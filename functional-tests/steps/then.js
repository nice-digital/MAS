import "@nice-digital/wdio-cucumber-steps/lib/then";
import { Then } from "cucumber";
import checkContainsText from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import validateRecordForm from "../support/check/validate";
import setInputField from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import { addUrl, addPublicationDate, addEvidenceType, addSpsComment, addShortSummary, addResourceLink } from "../support/action/addInput";
import { selectEvidenceType, selectSpecialities, relevancy } from "../support/action/click";

Then(
    /^I expect the page to contain the text "([^"]*)?"$/, (text) => {
        var value = "";
        switch(text) {
            case "Powered by KeystoneJS":
                value = "//DIV[@class='auth-footer']"
                break;
            case "MAS":
                value = "//DIV[@class='dashboard-heading'][text()='Medicines Awareness Service']"
                break;
            case "You have been signed out.":
                value = "[data-alert-type='info']";
                break; 
            case "Your changes have been saved successfully":
                value = "[data-alert-type='success']";
                break;          
            default:
                value = "div.dashboard-header";
          }
          checkContainsText("element", value, false, text);
    }
);
Then(
    /^I expect to see a list of items$/, () => {
        var table = ("table.Table.ItemList");
        browser.isExisting(table);
        browser.pause(2000);
    }          
);
Then(
    /^I expect a create a new item form pops up$/, () => {
        var form = ("[data-screen-id='modal-dialog']");
        browser.isExisting(form);
        browser.getValue(form);
    }          
);
Then(
    /^I can add an Evidence type$/, () => { 
        browser.click("[for='evidenceType'] .Select-control .Select-placeholder");    
        browser.waitForVisible("[for='evidenceType'] .Select-menu ", 5000);
        browser.click("[for='evidenceType'] .Select-menu .Select-option#react-select-3--option-3");
        browser.pause(5000);
    }          
);
Then(
    /^I expect the new record is added to the list of items$/, () => {
        browser.pause(3000);
        browser.click("[data-list-path='items'] a");
        browser.waitForVisible("//A[text()='NSAIDs Automated test again']");      
    }          
);
Then(
    /^I expect the record form is visible$/, 
    validateRecordForm
);
Then(
    /^I can add a URL$/, 
    addUrl
);
Then(
    /^I can add a Short summary$/, 
    addShortSummary
);
Then(
    /^I can add a Publication date$/, 
    addPublicationDate
);
Then(
    /^I can add a Speciality type$/, () => {  
        var speciality = $ ("[for='specialities'] .Select-control .Select-placeholder");
        speciality.scroll(250, 250);
        browser.pause(5000);
        browser.click("[for='specialities'] .Select-control .Select-arrow");
        browser.click("[for='specialities'] .Select-menu .Select-option#react-select-4--option-4")
              
    }                
);
Then(
    /^I can add a SPS comment$/, 
    addSpsComment
);
Then(
    /^I can add a Resource link$/, 
    addResourceLink
);
Then(
    /^I can add a Weekly relevancy score$/, () => {  
        var relevancy = $ ("[for='relevancy'] .Select-control .Select-placeholder");
        relevancy.scroll(950, 750);
        browser.click("[for='relevancy'] .Select-control .Select-arrow");
        browser.click("[for='relevancy'] .Select-menu .Select-option#react-select-5--option-0")
            
    }                
);
