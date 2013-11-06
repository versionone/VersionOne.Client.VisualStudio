Feature: Install the VersionOne Client for Visual Studio
  In order to use the VersionOne Client for Visual Studio
  Developers should be able to install it

# Mark's steps:
# Select Tools->Options to open configuration form
# Enter VersionOne URL and Password
# Click “Test Connection”
# Click “OK” and Restart Visual Studio
# Monitor Visual Studio for errors loading extensions

  Scenario: Valid URL and credentials
    Given I have navigated to the configuration form
    And I have provided "https://www14.v1hosting.com/v1sdktesting/" as the URL
    And I have provided "remote" as the username
    And I have provided "remote" as the password
    When I test the connection
    Then I see confirmation that the client can connect to VersionOne.

  Scenario: URL without https

  Scenario: URL without a trailing slash

  Scenario: URL with Default.aspx

  Scenario: Invalid username

  Scenario: Invalid password
