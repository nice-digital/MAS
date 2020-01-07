Feature: Homepage
  As a user of the Keystone CMS 
  I want to be able to log in and out of the CMS.

Background:
  Given I have a screen that is 1366 by 768 pixels

 Scenario: Navigate to find guidance page
  Given I open the homepage   
  When I click the email input field
    And I add the email address 
    And I click the password input field
    And I add the password 
    And I click the Submit button  
  Then I expect the page to contain the text "Medicines Awareness Service"
  When I click the Sign Out button 
  Then I expect the page to contain the text "You have been signed out."
  

  Scenario: Check page has no accessibility issues
    Then the page should have no accessibility issues