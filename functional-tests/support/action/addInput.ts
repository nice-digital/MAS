import {setInputField } from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {clearInputField} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clearInputField";


export async function addEmail(): Promise<void> {
    await pause("2000");
    await setInputField("add", "test@test.co.uk", "//*[@name='email']");
};

export async function addPassword(): Promise<void> {
    await pause("2000");
    await setInputField("add", "Password1!", "//*[@name='password']");
};

export async function addTitle(): Promise<void> {
    await setInputField("add", "NSAIDs Automated test again", ".FormField__inner.field-size-full  [name=title]");
    await pause("2000");
};

export async function addSource(): Promise<void> {
    await clickElement("click", "selector", "[for='source'] .Select-control .Select-placeholder");
    await pause("2000");
    await setInputField("set", "ABL Health Ltd", "[for='source'] .Select-control .Select-placeholder");
    await pause("2000");
    await browser.keys(['Enter'])
};

export async function addShortSummary(): Promise<void> {
    await pause("2000");
    await setInputField("add", "Nonsteroidal anti-inflammatory drugs (NSAIDs) are members of a drug class that reduces pain, decreases fever, prevents blood clots", "//*[@name='shortSummary']");
    await pause("2000");
};

export async function addUrl(): Promise<void> {
    await pause("2000");
    await clearInputField("//*[@name='url']")
    await setInputField("add", "https://bnfc.nice.org.uk/drug/abacavir.html", "//*[@name='url']");
    await pause("2000");
};

export async function addPublicationDate(): Promise<void> {
    await pause("2000");
    await clickElement("click", "selector", "//BUTTON[@type][text()='Today']");
};

export async function addSpsComment(): Promise<void> {
    await clickElement("doubleClick", "selector", "[for='comment'] iframe");
    await pause("2000");
    await setInputField("set", "To access content relevant to you, create an account and sign in each time you use our website", "[for='comment'] iframe");
    await pause("2000");
};

export async function addResourceLink(): Promise<void> {
    const elem = await $("[for='resourceLinks'] iframe");
    await elem.scrollIntoView();
    await clickElement("click", "selector", "[for='resourceLinks'] iframe");
    await pause("2000");
    await setInputField("set", "To access content relevant to you, create an account and sign in each time you use our website", "[for='resourceLinks'] iframe");
    await pause("2000");
};

export default addEmail;