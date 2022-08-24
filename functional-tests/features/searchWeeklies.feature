Feature: MAS CMS Weeklies
  As a user of the Keystone CMS 
  I want to be able to search weeklies in the CMS.

Background:
   Given I open the url "/keystone/signin"        
      
Scenario: Search weeklies in the CMS
  Given I am logged in to Keystone CMS with username and password 
  When I click on the weeklies button
  And I search the weekly title 4th July 2022 to 8th July 2022
  Then I expect the weekly title is visible on the page

  