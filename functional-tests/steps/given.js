import "@nice-digital/wdio-cucumber-steps/lib/given";

//Uncomment below section to write custom step definitions

import { Given } from "cucumber";


Given(
    /^I open the homepage$/,
    () => {browser.url ("/keystone")}
);
Given(
    /^I am logged in to Keystone CMS with username and password$/, ()=> 
      {
        browser.setValue("//*[@name='email']", "test@test.co.uk");
        browser.setValue("//*[@name='password']","Password1" );
        browser.doubleClick("//BUTTON[@type='submit'][text()='Sign In']");
        browser.pause(2000);
    }
);