Feature: MAS CMS Admin tabs
  As a user of the Keystone CMS 
  I want to be able to search Sources, Specialities, Evidence Types

Background:
   Given I open the url "/keystone/signin"        
      
Scenario: Search admin tabs; Sources, Specialities, Evidence Types
  Given I am logged in to Keystone CMS with username and password 
  When I click on the sources button
  And I search the sources title ABL Health Ltd
  Then I expect the sources title is visible on the page
  And I navigate to home page
  When I click on the specialities button
  And I search the specialities title Allergy and immunology
  Then I expect the specialities title is visible on the page
  And I navigate to home page
  When I click on the evidence types button
  And I search the evidence types title Evidence summaries - Medicines Q&A
  Then I expect the evidence types title is visible on the page



  