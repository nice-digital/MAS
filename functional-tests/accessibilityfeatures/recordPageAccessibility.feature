Feature: Accessibility testing on MAS item record page
  As a user of the Keystone CMS 
  We can check accessibility on item record page

Background:
   Given I open the url "/keystone/signin"        
      
Scenario: Accessibility testing on MAS item record page
  Given I am logged in to Keystone CMS with username and password 
  When I click on the items button
  And I click on an item record and check accesibility issues
  Then the page should have no accessibility issues