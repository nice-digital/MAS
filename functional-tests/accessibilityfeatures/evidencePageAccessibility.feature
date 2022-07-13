Feature: Accessibility testing on MAS evidence types page
  As a user of the Keystone CMS 
  We can check accessibility on evidence types page

Background:
   Given I open the url "/keystone/signin"        
      
Scenario: Accessibility testing on MAS evidence types page
  Given I am logged in to Keystone CMS with username and password 
  When I click on the evidence types button
  Then the page should have no accessibility issues