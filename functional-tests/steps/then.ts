import "@nice-digital/wdio-cucumber-steps/lib/then";
import { Then } from '@cucumber/cucumber';
import {validateRecordForm, validateWeeklyTitle, validateSourcesTitle, validateSpecialitiesTitle, validateEvidenceTypeTitle } from "../support/check/validate";
import { addUrl, addPublicationDate, addEvidenceType, addSpsComment, addShortSummary, addResourceLink } from "../support/action/addInput";
import { selectEvidenceType, selectSpecialities, relevancy, deleteButton, deleteItem } from "../support/action/click";
import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {scroll} from "@nice-digital/wdio-cucumber-steps/lib/support/action/scroll";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {isExisting} from "@nice-digital/wdio-cucumber-steps/lib/support/check/isExisting";
import {checkContainsText} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import {setInputField } from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import selectByVisibleText from "webdriverio/build/commands/element/selectByVisibleText";
import {searchWeeklies, searchSources, searchSpecialities, searchEvidenceType } from "../support/check/checkFields"


Then(
    /^I expect the page to contain the text "([^"]*)?"$/, async (text) => {

        
        var value = "";

        switch( text ) {
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
                value = ".dashboard-header";
          }
          await pause("2000");
          return value;
    }
);
Then(
    /^I expect to see a list of items$/, async () => {
        await isExisting("table.Table.ItemList", "");
        await pause("2000");
    }          
);
Then(
    /^I expect a create a new item form pops up$/, async () => {
        const form = await $("[data-screen-id='modal-dialog']");
        await isExisting("[data-screen-id='modal-dialog']", "");
        await form.getValue
    }          
);
Then(
    /^I can add an Evidence type$/, async () => { 
        
        await clickElement("click", "selector", "[for='evidenceType'] .Select-control .Select-input");
        await pause("2000");
        await setInputField("set", "Guidance and advice - Guidance", "[for='evidenceType'] .Select-control .Select-input");
        await pause("2000");
        await browser.keys(['Enter'])      
    }         
);
Then(
    /^I expect the new record is added to the list of items$/, async () => {
        await waitForDisplayed("//A[text()='NSAIDs Automated test again']", "");
        await pause("2000");
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
    /^I can add a Speciality type$/, async () => {  

        await pause("2000");
        await clickElement("click", "selector", "[for='specialities'] .Select-control .Select-placeholder");
        await pause("2000");
        await setInputField("set", "Cancers", "[for='evidenceType'] .Select-control .Select-arrow");
        await browser.keys(['Enter'])
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
    /^I can add a Weekly relevancy score$/, async () => {  
        await pause("2000");
        await clickElement("click", "selector", "[for='relevancy'] .Select-control .Select-placeholder");
        await pause("2000");
        await setInputField("set", "High", "[for='relevancy'] .Select-control .Select-placeholder");
        await browser.keys(['Enter']);
    }                
);
Then(
    /^I can click on the delete button$/, 
    deleteButton
);
Then( 
    /^I can see the modal dialog box$/, async () => {
    const confirmDelete = await $("[data-screen-id='modal-dialog']");
    confirmDelete.isExisting();
    }
);
Then(
    /^I search the weekly title 4th July 2022 to 8th July 2022$/, 
    searchWeeklies
);
Then(
    /^I expect the weekly title is visible on the page$/, 
    validateWeeklyTitle
);
Then(
    /^I search the sources title ABL Health Ltd$/, 
    searchSources
);
Then(
    /^I expect the sources title is visible on the page$/, 
    validateSourcesTitle
);
Then(
    /^I search the specialities title Allergy and immunology$/, 
    searchSpecialities
);
Then(
    /^I expect the specialities title is visible on the page$/, 
    validateSpecialitiesTitle
);
Then(
    /^I search the evidence types title Evidence summaries - Medicines Q&A$/, 
    searchEvidenceType
);
Then(
    /^I expect the evidence types title is visible on the page$/, 
    validateEvidenceTypeTitle
);