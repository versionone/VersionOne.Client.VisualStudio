# VersionOne Client for Visual Studio

VersionOne.Client.VisualStudio is a Visual Studio extension that connects to VersionOne.

# Download a build

You can find the latest public build for the extension in the [VersionOne App Catalog](http://v1appcatalog.azurewebsites.net/app/index.html#/Details/VersionOne.Client.VisualStudio).

# Build Prerequisites

## Step 1: Install or upgrade to the latest NuGet

[Install NuGet 2.7](http://docs.nuget.org/docs/release-notes/nuget-2.7) or greater.

We use NuGet to manage external dependencies that this project requires. -- **Most importantly, as of NuGet 2.7, there is a simplified package restore workflow for NuGet packages that this code requires via `packages.config` files in each project.** [See this post](http://docs.nuget.org/docs/release-notes/nuget-2.7) for all the details.

## Step 2: Automate the installation of required developer tools

We build with Visual Studio 2012 Professional and Premium, and several other tools, all of which are listed in
the Chocolatey [packages.config](packages.config) file.

## Step 3: Install Chocolatey

Not familiar with Chocolatey? It's a package manager for Windows, similar to apt-get in the Linux world. It actually uses NuGet internally. To installl Chocolatey:

* First, see [Chocolatey's requirements](https://github.com/chocolatey/chocolatey/wiki)
* Next, assuming you already Cloned or Downloaded this repository from GitHub into `C:\Projects\VersionOne.Client.VisualStudio`, open an `Admininstrator` command prompt in that folder and run `install_chocolatey.bat`

## Step 4: Use Chocolatey to install the developer tools

If the Chocolatey install worked, then:

* **First:** if you already have Visual Studio 2012 installed without using Chocolatey, you can open up [packages.config](packages.config) and remove the line for it. That will avoid downloading the large file over the internet.
* Close the command prompt and open a new `Administrator` command prompt so that you get an updated PATH environment variable and navigate back to the repository folder.
* Run `install_dev_tools.bat`

This should start downloading and automatically installling the tools listed in [packages.config](packages.config).

## Alternatively: If you don't want or cannot use Chocolatey, you can manually install developer tools

* [Install Visual Studio 2012 Professional or higher](http://msdn.microsoft.com/en-US/library/vstudio/e2h7fzkw.aspx)
* [Install Update 3 for Visual Studio 2012](http://support.microsoft.com/kb/2835600)
* [Install Visual Studio 2012 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=30668) -- *includes project templates, tools, tests, and reference assemblies that are required to build extensions for Visual Studio 2012*
* **Install Bash shell** -- our `build.sh` and other scripts are written in Bash, so you need a good Bash shell to execute them. People on the VersionOne team use both [http://git-scm.com/download/win](Git Bash) and 
[Cygwin with the Bash package](http://www.cygwin.com/) successfully.

# How to Build

Assuming you have followed the previous steps and your environment is all setup correctly now:

* Open a Git Bash prompt as `Administrator`
* Change directory to `/c/Projects/VersionOne.Client.VisualStudio`
* Type `./build.sh`

This should build the client successfully. TODO: what about tests?

# Build known issues and solutions or workarounds

## Symptom: Build says failed, but with 0 apparent errors

### Likely cause

If you have already installed the extension before, and then tried to uninstall it, sometimes Visual Studio has a stray "pending deletion" in the registry.

### Remedy

While we do not know yet how to prevent this issue, see the issue [Build completes with FAILED message, despite no visible Errors, but Detailed logging shows Task "EnableExtension" FAILED](https://github.com/versionone/VersionOne.Client.VisualStudio/issues/10) which documents the resolution for when it happens.

# Advanced Builds

## How to Build with Visual Studio 2013

The steps below assume you have Chocolatey installed. If you do not, you can find the downloads on Microsoft's web site.

* Install Visual Studio 2013: `cinst VisualStudio2013Professional`. Other editions exist as Chocolatey packages too.
* Install the Visual Studio 2013 SDK: `cinst VS2013SDK`
* Using a text editor, open `VersionOne.VisualStudio.VSPackage\VersionOne.VisualStudio.VSPackage.csproj`
* Change `11.0` to `12.0` in the line that reads `<MinimumVisualStudioVersion>11.0</MinimumVisualStudioVersion>` -- this tells Visual Studio that the project can work with Visual Studio 2013, which happens to be version 12.
* You should now be able to open the solution with Visual Studio and compile the VSIX package.
* **NOTE:** We have also tested that when you build with 2013, you can still successfully install the extension into 2012.

## How to target lower Visual Studio editions (like 2010)

The VersionOne policy is to support the current and current - 1 product versions for the integrations that we formally support. However, this code is open source, and we welcome your assistance in building the projecct with targets for other editions if you will also test them and demo your findings with us. If it appears that all test cases work, then you can send us a pull request for the additional target.

Since we have not been able to fully test against 2010, we do not currently have the installation target for it. But, if you'd like to help this project by building and testing against 2010, please contact us because we can work with you to make this happen. We have the previous versions of the code that did target 2010, and as we understand it [from this post on StackOverflow](http://stackoverflow.com/questions/12499133/develop-vsix-for-vs2010-under-vs2012), it may be possible to still target 2010 from 2012 or higher.
