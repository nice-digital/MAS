import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {checkContainsText} from '@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText';


 export async function validateRecordForm(): Promise<void> {

	await pause("2000");
	await waitForDisplayed("//input[@name='title']", "");
};
export async function validateWeeklyTitle(): Promise<void> {

	await pause("2000");
	await waitForDisplayed(".ItemList__col a", "");
	await checkContainsText("element", ".ItemList__col a", "", '4th July 2022 to 8th July 2022'); 
};

export async function validateSourcesTitle(): Promise<void> {

	await pause("2000");
	await waitForDisplayed(".ItemList__col a", "");
	await checkContainsText("element", ".ItemList__col a", "", 'ABL Health Ltd'); 
};

export async function validateSpecialitiesTitle(): Promise<void> {

	await pause("2000");
	await waitForDisplayed(".ItemList__col a", "");
	await checkContainsText("element", ".ItemList__col a", "", 'Allergy and immunology'); 
};

export async function validateEvidenceTypeTitle(): Promise<void> {

	await pause("2000");
	await waitForDisplayed(".ItemList__col a", "");
	await checkContainsText("element", ".ItemList__col a", "", 'Evidence summaries - Medicines Q&A'); 
	await pause("2000");
};

export default validateRecordForm;
