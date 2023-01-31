import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {setInputField } from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";

export async function cmsLogin(username: string, password: string): Promise<void> {

    await pause("2000");
    await setInputField("add", process.env[username], "//*[@name='email']");
    await pause("2000");
     await setInputField("add", process.env[password], "//*[@name='password']");
    await pause("2000");
    await clickElement("click", "element", "//BUTTON[@type='submit'][text()='Sign In']");
    await pause("2000");
};

export default cmsLogin;