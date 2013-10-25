Feature: Install the VersionOne Client for Visual Studio
  In order to use the VersionOne Client for Visual Studio
  Developers should be able to install it

  Scenario: Install using VSIX file from zip
    Given a zip file distribution available from the VersionOne App Catalog
    And I have unzipped that distribution to extract the VSIX file
    When I double-click the VSIX
    Then it installs VersionOne Client for Visual Studio into only those targets listed as supported in the user documentation.

  Scenario: Install using Chocolatey

  Scenario: Install using Microsoft Visual Studio Gallery