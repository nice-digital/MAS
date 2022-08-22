Feature: MAS CMS Item Form
  As a user of the Keystone CMS 
  I want to be able to edit an item in the CMS.

Background:
  Given I open the url "/keystone/signin"      
  And I am logged in to Keystone CMS with username and password   
      
Scenario: Edit core and add non-core fields
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
  And I pause for 5000ms
  And I navigate to home page
  And I pause for 5000ms
  When I click on the items button
  Then I expect the new record is added to the list of items
  And I click on an item record
  And I can add a URL   
  And I can add a Short summary   
  And I can add a Speciality type
  And I can add a SPS comment
  And I can add a Resource link
  And I can add a Weekly relevancy score
  When I click on the Save button
  Then I expect the page to contain the text "Your changes have been saved successfully"
  And I click on the delete button
  Then I can see the modal dialog box
  When I click the confirm delete button on the dialog box
  Then I expect that the url is "https://cms-mas.test.nice.org.uk/keystone/items"


  