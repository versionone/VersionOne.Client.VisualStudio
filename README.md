# VersionOne Client for Visual Studio

VersionOne.Client.VisualStudio is a Visual Studio extension that connects To build this code you'll need:

# Build Prerequisites

* [Install Visual Studio 2012](http://msdn.microsoft.com/en-US/library/vstudio/e2h7fzkw.aspx)
* [Install Update 3 for Visual Studio 2012](http://support.microsoft.com/kb/2835600)
* [Install Visual Studio 2012 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=30668) -- *includes project templates, tools, tests, and reference assemblies that are required to build extensions for Visual Studio 2012*
* [Install NuGet 2.7](http://docs.nuget.org/docs/release-notes/nuget-2.7) or greater. -- **most importantly, this allows a simplified package restore workflow for NuGet packages that this code requires via `packages.config` files in each project. [See this post](http://docs.nuget.org/docs/release-notes/nuget-2.7) for all the details.**
* **Install Bash shell** -- our `build.sh` and other scripts are written in Bash, so you need a good Bash shell to execute them. People on the VersionOne team use both [http://git-scm.com/download/win](Git Bash) and 
[Cygwin with the Bash package](http://www.cygwin.com/) successfully.

## Build known issues and solutions or workarounds

### Symptom: Build says failed, but with 0 apparent errors

### Likely cause

If you have already installed the extension before, and then tried to uninstall it, sometimes Visual Studio has a stray "pending deletion" in the registry.

### Remedy

While we do not know yet how to prevent this issue, see the issue [Build completes with FAILED message, despite no visible Errors, but Detailed logging shows Task "EnableExtension" FAILED](https://github.com/versionone/VersionOne.Client.VisualStudio/issues/10) which documents the resolution for when it happens.