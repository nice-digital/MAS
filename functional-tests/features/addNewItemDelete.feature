Feature: MAS CMS Item Form
  As a user of the Keystone CMS 
  I want to be able to add an item to the CMS.
  I want to be able to delete an item to the CMS.

Background:
  Given I open the url "/keystone/signin"
  And I am logged in to Keystone CMS with username and password 
      

 Scenario: Create new item using core fields and delete item record
  When I click on the items button
  Then I expect to see a list of items    
  When I click on the create item button
  Then I expect a create a new item form pops up   
  When I add a Title  
  Then I can add a URL
  And I add the Source   
  And I click to select the Evidence type   
  Then I can add an Evidence type   
  And I pause for 2000ms 
  When I click on the create button
  And I pause for 20000ms  
  And I navigate to home page
  And I pause for 5000ms
  When I click on the items button
  Then I expect the new record is added to the list of items
  And I click on an item record
  And I click on the delete button
  Then I can see the modal dialog box
  When I click the confirm delete button on the dialog box
  Then I expect that the url is "https://cms-mas.test.nice.org.uk/keystone/items"



    