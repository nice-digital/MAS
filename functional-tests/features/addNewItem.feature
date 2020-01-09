Feature: Homepage
  As a user of the Keystone CMS 
  I want to be able to add an item to the CMS.

Background:
  Given I open the homepage
    And I am logged in to Keystone CMS with username and password 
      

 Scenario: Create new item using core fields
  When I click on the items button
  Then I expect to see a list of items    
  When I click on the create item button
  Then I expect a create a new item form pops up   
  When I add a Title  
    And I add the Source 
    And I click to select the Evidence type   
  Then I can add an Evidence type   
  And I pause for 2000ms 
  When I click on the create button
  And I pause for 2000ms
  Then I expect the new record is added to the list of items



    