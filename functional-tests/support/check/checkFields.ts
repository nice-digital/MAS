import {setInputField } from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";


export async function searchWeeklies(): Promise<void> {

    await clickElement("click", "selector", ".css-13azwyo input");
    await setInputField("set", "4th July 2022 to 8th July 2022", ".css-13azwyo input");

};

export async function searchSources(): Promise<void> {

    await clickElement("click", "selector", ".css-13azwyo input");
    await setInputField("set", "ABL Health Ltd", ".css-13azwyo input");

};

export async function searchSpecialities(): Promise<void> {

    await clickElement("click", "selector", ".css-13azwyo input");
    await setInputField("set", "Allergy and immunology", ".css-13azwyo input");

};

export async function searchEvidenceType(): Promise<void> {

    await clickElement("click", "selector", ".css-13azwyo input");
    await setInputField("set", "Evidence summaries - Medicines Q&A", ".css-13azwyo input");

};

export default searchWeeklies;