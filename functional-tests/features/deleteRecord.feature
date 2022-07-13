Feature: MAS CMS Item Form
  As a user of the Keystone CMS 
  I want to be able to delete an item in the CMS.

Background:
   Given I open the url "/keystone/signin"    
   And I am logged in to Keystone CMS with username and password     
      
Scenario: Delete record item 
  Then I create a new item record
  And I navigate to home page
  When I click on the items button
  When I click on the new item record
  And I click on the delete button
  Then I can see the modal dialog box
  When I click the confirm delete button on the dialog box  
  Then I expect that the url is "https://cms-mas.test.nice.org.uk/keystone/items"