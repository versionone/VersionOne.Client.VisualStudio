Feature: Install the VersionOne Client for Visual Studio
  In order to use the VersionOne Client for Visual Studio
  Developers should be able to install it

  Scenario: Install using VSIX from the VersionOne App Catalog:
    Given the VSIX file from the VersionOne App Catalog
    When I try to install the VSIX to Visual Studio $VERSION $EDITION 
    Then I get $RESULT
    | Version |  Edition  | Result |
    | Any     |  Express  | Failure message |
    | 2010    |  Any      | Failure message |
    | 2012    |  Pro      | Success and functionality works | 
    | 2012    |  Premium  | Success and functionality works | 
    | 2012    |  Ultimate | Success and functionality works | 
    | 2013    |  Pro      | Success and functionality works | 
    | 2013    |  Premium  | Success and functionality works | 
    | 2013    |  Ultimate | Success and functionality works |

  Note: Download at http://platform.versionone.com.s3.amazonaws.com/downloads/VersionOne.Client.VisualStudio_9.0.0.47.zip

  Scenario: Install using Chocolatey

  Scenario: Install using Microsoft Visual Studio Gallery
