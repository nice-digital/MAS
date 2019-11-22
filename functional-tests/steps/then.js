import "@nice-digital/wdio-cucumber-steps/lib/then";
import { Then } from "cucumber";
import checkContainsText from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";

Then(
    /^I expect the page to contain the text "([^"]*)?"$/, (text) => {
        var value = "";
        switch(text) {
            case "Powered by KeystoneJS":
                value = "//DIV[@class='auth-footer']"
                break;
            case "MAS":
                value = "//DIV[@class='dashboard-heading'][text()='MAS']"
                break;
            case "You have been signed out.":
                value = "//DIV[@class='alert_1wamaxc-o_O-info_1lss0us']"
                break;             
            default:
                value = "div.dashboard-header";
          }
          checkContainsText("element", value, false, text);
    }
);
