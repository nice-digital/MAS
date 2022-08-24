Feature: Accessibility testing on MAS users page
  As a user of the Keystone CMS 
  We can check accessibility on users page

Background:
   Given I open the url "/keystone/signin"        
      
Scenario: Accessibility testing on MAS users page
  Given I am logged in to Keystone CMS with username and password 
  When I click on the users button
  Then the page should have no accessibility issues