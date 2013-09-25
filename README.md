# VersionOne Client for Visual Studio

If you are interested in contributing to this project, please contact [Ian Buchanan](mailto:ian.buchanan@versionone.com).

# Build requirements

VersionOne.Client.VisualStudio is a Visual Studio extension. To build this code you'll need:

* [Install Visual Studio 2012](http://msdn.microsoft.com/en-US/library/vstudio/e2h7fzkw.aspx)
* [Install Update 3 for Visual Studio 2012](http://support.microsoft.com/kb/2835600)
* [Install Visual Studio 2012 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=30668) -- *includes project templates, tools, tests, and reference assemblies that are required to build extensions for Visual Studio 2012*
* [Install NuGet 2.7](http://docs.nuget.org/docs/release-notes/nuget-2.7) or greater. -- **most importantly, this allows a simplified package restore workflow for NuGet packages that this code requires. [See this post](http://docs.nuget.org/docs/release-notes/nuget-2.7) for all the details.**