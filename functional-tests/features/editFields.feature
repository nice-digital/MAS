Feature: MAS CMS Item Form
  As a user of the Keystone CMS 
  I want to be able to edit an item in the CMS.

Background:
   Given I open the homepage            
      
Scenario: Edit core and add non-core fields
Given I am logged in to Keystone CMS with username and password 
  When I click on the items button
    And I click on an item record
  Then I expect the record form is visible
    And I can add a URL   
    And I can add a Short summary   
    And I can add a Speciality type
    And I can add a SPS comment
    And I can add a Resource link
    And I pause for 2000ms
    And I can add a Weekly relevancy score
    #And I can add a Publication date
  When I click on the Save button
  Then I expect the page to contain the text "Your changes have been saved successfully"

Scenario: Delete record item 
  When I click on the items button
  Then I expect to see a list of items
  When I click on an item record
    And I click on the delete button
  Then I can see the modal dialog box
  When I click the confirm delete button on the dialog box  
  Then I expect that the url is "https://cms-mas.test.nice.org.uk/keystone/items"
  