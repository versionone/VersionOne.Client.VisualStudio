IssueReference : https://github.com/versionone/VersionOne.Client.VisualStudio/issues/7

[Volatile]
Reference: https://www14.v1host.com/v1sdktesting/meta.v1/Member?xsl=api.xsl

Background:
  Given a Member with
    Name : "Ian Inactive"
    Nickname : "IanIn"
    Username : "ianinactive"
  And a Story named "Story with Inactive Member" with "Ian Inactive as the Owner,
  And "Ian Inactive" has been Inactivated,

Scenario: View Workitem where owner Member is Inactive
  When I look at "Story with Inactivated Member" 
  Then I see "IanIn" with a strike-through in the Owners field
-- TODO: add issue to pluralize Owner to Owners
-- TODO: generalize into a tabular form with examples for Story, Defect, Task, Test.
-- See example: https://github.com/versionone/VersionOne.AppCatalog.Web/blob/master/features/links_section.feature


[Volatile]
Reference: https://www14.v1host.com/v1sdktesting/meta.v1/Workitem?xsl=api.xsl

Scenario: Save Workitem "Story with Inactive Member"
  Given I set Status to "Future" (Which should be there, regardless of Workspace caveat)
  When I "Save"
  Then I should see "Future" Status for "Story with Inactive Member" in the VersionOne Web UI

When I try to sign up for "Story with Inactivated Member"
Then ...



When I open the Owners drop-down for "Story with Inactivated Member"
Then ...
