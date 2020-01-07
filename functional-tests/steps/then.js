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
                value = "//DIV[@class='css-mv6v0s']"
                break; 
            case "Your changes have been saved successfully":
                value = "//DIV[@class='css-ctpeu']"
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
        var form = ("//DIV[@class='css-s2cbvv']");
        browser.isExisting(form);
        browser.getValue(form);
    }          
);
Then(
    /^I can add an Evidence type$/, () => { 
        browser.click( "[for=evidenceType] .Select-control .Select-placeholder");    
        browser.waitForVisible("#react-select-3--list", 5000);
        browser.click("#react-select-3--option-16");
        browser.pause(5000);
    }          
);
Then(
    /^I expect the new record is added to the list of items$/, () => {
        var item = ("(//A[@class='ItemList__value ItemList__value--text ItemList__link--interior ItemList__link--padded ItemList__value--truncate'][text()='NSAIDs Automated test again'][text()='NSAIDs Automated test again'])[3]");
        browser.isExisting(item);        
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
        var speciality = $ ("[for=speciality] .Select-control .Select-placeholder");
        speciality.scroll(250, 250);
        browser.pause(5000);
        browser.click( "[for=speciality] .Select-control .Select-arrow");
        browser.click("#react-select-4--option-4");       
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
        var relevancy = $ ("[for=relevancy] .Select-control .Select-placeholder");
        relevancy.scroll(950, 750);
        browser.click( "[for=relevancy] .Select-control .Select-arrow");
        browser.click("#react-select-5--option-0");       
    }                
);
