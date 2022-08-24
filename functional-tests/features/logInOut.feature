Feature: MAS Homepage
  As a user of the Keystone CMS 
  I want to be able to log in and out of the CMS.

Background:
  Given I have a screen that is 1366 by 768 pixels
  Given I open the url "/keystone/signin"

 Scenario: Log in and out of MAS Keystone
  Given I am logged in to Keystone CMS with username and password 
  Then I expect the page to contain the text "Medicines Awareness Service"
  When I click the Sign Out button 
  Then I expect the page to contain the text "You have been signed out."