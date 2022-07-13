import {setInputField } from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";


export async function newRecordDelete(): Promise<void> {

    await clickElement("click", "selector", "//div[@class='dashboard-group__list-label'][contains(text(),'Items')]");
    await clickElement("click", "selector", "//SPAN[text()='Create Item']");
    await setInputField("add", "Regression Automated to be deleted", ".FormField__inner.field-size-full");
    await pause("2000");
    await clickElement("click", "selector", "[for='source'] .Select-control .Select-placeholder");
    await setInputField("set", "ABL Health Ltd", "[for='source'] .Select-control .Select-placeholder");
    await pause("2000");
    await browser.keys(['Enter'])
    await clickElement("click", "selector", "[for='evidenceType'] .Select-control .Select-input");
    await setInputField("set", "Guidance and advice - Guidance", "[for='evidenceType'] .Select-control .Select-input");
    await pause("2000");
    await browser.keys(['Enter'])
    await clickElement("click", "selector", "//BUTTON[@type='submit'][text()='Create']");
    await pause("5000");
};


export default newRecordDelete;