Feature: MAS CMS Item Form
  As a user of the Keystone CMS 
  I want to be able to edit an item in the CMS.

Background:
   Given I open the url "/keystone/signin"      
   And I am logged in to Keystone CMS with username and password   
      
Scenario: Edit core and add non-core fields
  Then I create a new item record
  And I navigate to home page
  When I click on the items button
  When I click on the new item record
  And I can add a URL   
  And I can add a Short summary   
  And I can add a Speciality type
 # And I can add a SPS comment
 # And I can add a Resource link
  And I can add a Weekly relevancy score
  When I click on the Save button
  Then I expect the page to contain the text "Your changes have been saved successfully"
  And I click on the delete button
  Then I can see the modal dialog box
  When I click the confirm delete button on the dialog box


  