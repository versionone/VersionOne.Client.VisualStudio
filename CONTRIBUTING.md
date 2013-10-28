# Contributing to VersionOne Client for Visual Studio

 1. [Getting Involved](#getting-involved)
 2. [Reporting Bugs](#reporting-bugs)
 3. [Contributing Code](#contributing-code)
 4. [Quality Bands](#quality-bands)

## Getting Involved

We need your help to make VersionOne Client for Visual Studio a useful integration. You don't have to write code to help! You can help the project by discovering and [reporting issues](#reporting-bugs).

## Reporting Bugs

Before reporting an [issue][issues], search the already reported issues for similar cases, and if it has been reported already, just add any additional details in the comments. Whether you are writing a new issue or adding comments, here are some tips for creating a helpful report that will make fixing it much easier and quicker:

 * Write a **descriptive, specific title**.
 * Write a description from the **user perspective**. What were you trying to accomplish? How does the failure affect you?
 * Include **steps to reproduce** the issue. How can someone else recreate the problem?
 * Include the **error message**. If the error has a stack trace, include as much as you can.

## Contributing Code

### Making Changes to Source

If you are not yet familiar with the way GitHub works (forking, pull requests, etc.), be sure to read [the article about forking](https://help.github.com/articles/fork-a-repo) on the GitHub Help website; it will get you started quickly. You should always write each batch of changes (feature, bugfix, etc.) in **its own topic branch**. Please do not commit to the `master` branch, or your unrelated changes will go into the same pull request. You should also follow the code style and whitespace conventions of the original codebase.

### Open Source Licenses and Attribution

Regardless of whether attribution is required by a dependency, we want to acknowledge the work that we depend up on and make it easy for people to evaluate the legal implications of using this project. Therefore, we require all dependencies be attributed in the [ACKNOWLEDGEMENTS.md](https://github.com/versionone/VersionOne.SDK.NET.APIClient/blob/master/ACKNOWLEDGEMENTS.md). This should include the persons or organizations who contributed the libraries, a link to the source code, and a link to the underlying license (even when this project sub-licenses under the modified BSD license).

## Quality Bands

Open source software evolves over time. A young project may be little more than some ideas and a kernel of unstable code. As a project matures, source code, UI, tests, and APIs will become more stable. To help consumers understand what they are getting, we characterize every release with one of the following quality bands.

### Seed

A seed has a lot of potential, but expect to put in your own time, expertise, and materials. We put this release into the wild in order to get feedback and to attract contributors. Use with caution. Please expect to work with developers to use and maintain this release.

### Sapling

The product is undergoing rapid growth. The code works. Test coverage is on the rise. Documentation is firming up. Some APIs may be public but are subject to change. Please expect to inform developers where information is insufficient to self-serve.

### Mature

The product is stable. The code will continue to evolve with minimum breaking changes. Documentation is sufficient for self-service. APIs are stable.

[issues]: https://github.com/versionone/VersionOne.Client.VisualStudio/issues