import isVisible from "@nice-digital/wdio-cucumber-steps/lib/support/check/isVisible";

export const validateRecordForm = () => {
     browser.pause(2000);
     isVisible("//input[@name='title']", false);     
 };

 export default validateRecordForm