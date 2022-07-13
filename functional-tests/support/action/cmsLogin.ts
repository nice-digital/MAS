import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {setInputField } from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";

export async function cmsLogin(): Promise<void> {

    await setInputField("add", "test@test.co.uk", "//*[@name='email']");
    await pause("2000");
     await setInputField("add", "Password1!", "//*[@name='password']");
    await pause("2000");
    await clickElement("click", "element", "//BUTTON[@type='submit'][text()='Sign In']");
    await pause("2000");
};

export default cmsLogin;