# Installation (No Proxy)

## Preparation

1. Create a Story A – Owner A
2. Create a Test A – Owner A
3. Create a Story B Owner B
4. Create a Test B Owner B

## Steps

1. Navigate to staging catalog entry for Visual Studio: http://appcatalogstage.azurewebsites.net/app/index.html#/Details/VersionOne.Client.VisualStudio
2. Select Tools->Options to open configuration form
3. Enter VersionOne URL (https://www14.v1host.com/v1sdktesting/) and Credentials (admin/admin)
4. Click "Test Connection"
5. Click "OK" and Restart Visual Studio
6. Monitor Visual Studio for errors loading extensions

# Usage

## Steps

1. Open VersionOne Projects window. Select View->Other Windows
2. Select a project with workitems that has been scheduled in an active iteration
3. Open VersionOne Workitems window SelectView->Other Windows
4. Check each field in form to see if it is editable
  * a. Change Title
  * b. Owner
  * c. Change Status
  * d. Change Estimate
  * e. Change Detail Estimate
  * f. Change todo 
  * g. Save by clicking disk icon
  * h. Click refresh icon
  * i. Confirm that fields have been successfully changed
5. Click “Show only my Tasks Icon”
6. Observe the reduction of workitems down to the owner indicated in the configuration
7. Click Add defect icon

# Context Menu

## Steps

1. Create Defect
2. Create a Task
  * a. Leave Title Empty and observe attempt to save
  * b. Fill in Title and observe attempt to save
  * c. Check this asset in b. against VersionOne to see if successfully created
3. Create a Story and click revert to see if it removes the story
