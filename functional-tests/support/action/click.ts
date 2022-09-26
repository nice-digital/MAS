import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {scroll} from "@nice-digital/wdio-cucumber-steps/lib/support/action/scroll";
import {setInputField } from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";


export async function emailInput(): Promise<void> {
	await clickElement("click", "selector", "//*[@name='email']");
};

export async function passwordInput(): Promise<void> {
	await clickElement("click", "selector", "//*[@name='password']");
};

export async function submitButton(): Promise<void> {
	await clickElement("doubleClick", "selector", "//BUTTON[@type='submit'][text()='Sign In']");
	await pause("2000");
};

export async function signOut(): Promise<void> {
	await clickElement("doubleClick", "selector", "//SPAN[@class='octicon octicon-sign-out']");
};

export async function itemsButton(): Promise<void> {
	await pause("5000");
	await clickElement("click", "selector", "//div[@class='dashboard-group__list-label'][contains(text(),'Items')]");
	await pause("5000");
};

export async function createItem(): Promise<void> {
	await clickElement("click", "selector", "//SPAN[text()='Create Item']");
};

export async function createButton(): Promise<void> {
	await clickElement("click", "selector", "button.css-h629qq");
	await pause("5000");
};

export async function selectRecordItem(): Promise<void> {
	await pause("2000");
	await clickElement("click", "selector", "//a[contains(text(),'NSAIDs Automated test again')]");
};

export async function selectEvidenceType(): Promise<void> {
	await clickElement("click", "selector", "[for=evidenceType] .Select-control .Select-placeholder");
	await pause("2000");
	await setInputField("set", "Policy", "[for='evidenceType'] .Select-control .Select-placeholder");
    await pause("2000");
    await browser.keys(['Enter'])
};

export async function save(): Promise<void> {
	await clickElement("click", "selector", "//BUTTON[@data-button='update']");
	await pause("15000");
};

export async function manageButton(): Promise<void> {
	await clickElement("click", "selector", "//BUTTON[@type][text()='Manage']");
};

export async function deleteButton(): Promise<void> {
	await scroll("//BUTTON[@data-button='delete']");
	await clickElement("click", "selector", "//BUTTON[@data-button='delete']");
	await pause("2000");
};

export async function deleteItem(): Promise<void> {
	await clickElement("click", "selector", "//BUTTON[@data-button-type='confirm']");
	await pause("1000");
};

export async function itemRecordAdded(): Promise<void> {
	await scroll("//a[contains(text(),'NSAIDs Automated test again')]");
	await pause("1000");
	await clickElement("click", "selector", "//BUTTON[@type][text()='Delete']");
};

export async function weekliesButton(): Promise<void> {
	await clickElement("click", "selector", "//div[@class='dashboard-group__list-label'][contains(text(),'Weeklies')]");
};

export async function sourcesButton(): Promise<void> {
	await clickElement("click", "selector", "//div[@class='dashboard-group__list-label'][contains(text(),'Sources')]");
};

export async function specialitiesButton(): Promise<void> {
	await clickElement("click", "selector", "//div[@class='dashboard-group__list-label'][contains(text(),'Specialities')]");
};

export async function evidenceTypesButton(): Promise<void> {
	await clickElement("click", "selector", "//div[@class='dashboard-group__list-label'][contains(text(),'Evidence Types')]");
};

export async function usersButton(): Promise<void> {
	await clickElement("click", "selector", "//div[@class='dashboard-group__list-label'][contains(text(),'Users')]");
};

export async function selectRecordAccessibility(): Promise<void> {
	await pause("2000");
	await clickElement("click", "selector", "//a[contains(text(),'this ia another test')]");
};

export async function selectNewItem(): Promise<void> {
	await pause("2000");
	await clickElement("click", "selector", "//a[contains(text(),'Regression Automated to be deleted')]");
};

export async function navigateItemPage(): Promise<void> {
	await pause("2000");
	await clickElement("click", "selector", ".active a");
};

export async function navigateHomePage(): Promise<void> {
	await pause("5000");
	await clickElement("click", "selector", ".primary-navbar__link .octicon-home");
	await pause("5000");
};

export default emailInput;