Feature: Accessibility testing on MAS home page
  As a user of the Keystone CMS 
  We can check accessibility on home page

Background:
   Given I open the url "/keystone/signin"        
      
Scenario: Accessibility testing on MAS home page
  Given I am logged in to Keystone CMS with username and password 
  Then the page should have no accessibility issues