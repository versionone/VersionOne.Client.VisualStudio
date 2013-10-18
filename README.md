# VersionOne Client for Visual Studio

VersionOne.Client.VisualStudio is a Visual Studio extension that connects to VersionOne.

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
