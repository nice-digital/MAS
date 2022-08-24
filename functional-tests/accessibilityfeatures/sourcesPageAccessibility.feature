Feature: Accessibility testing on MAS sources page
  As a user of the Keystone CMS 
  We can check accessibility on sources page

Background:
   Given I open the url "/keystone/signin"        
      
Scenario: Accessibility testing on MAS sources page
  Given I am logged in to Keystone CMS with username and password 
  When I click on the sources button
  Then the page should have no accessibility issues