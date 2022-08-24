Feature: Accessibility testing on MAS specialities page
  As a user of the Keystone CMS 
  We can check accessibility on specialities page

Background:
   Given I open the url "/keystone/signin"        
      
Scenario: Accessibility testing on MAS specialities page
  Given I am logged in to Keystone CMS with username and password 
  When I click on the specialities button
  Then the page should have no accessibility issues