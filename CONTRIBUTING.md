# Contributing to the Dropbox SDK for DotNet
We value and rely on the feedback from our community. This comes in the form of bug reports, feature requests, and general guidance. We welcome your issues and pull requests and try our hardest to be timely in both response and resolution. Please read through this document before submitting issues or pull requests to ensure we have the necessary information to help you resolve your issue.

## Filing Bug Reports
You can file a bug report on the [GitHub Issues][issues] page.

1. Search through existing issues to ensure that your issue has not been reported. If it is a common issue, there is likely already an issue.

2. Please ensure you are using the latest version of the SDK. While this may be a valid issue, we only will fix bugs affecting the latest version and your bug may have been fixed in a newer version.

3. Provide as much information as you can regarding the language version, SDK version, and any other relevant information about your environment so we can help resolve the issue as quickly as possible.

## Submitting Pull Requests

We are more than happy to recieve pull requests helping us improve the state of our SDK. You can open a new pull request on the [GitHub Pull Requests][pr] page.

1. Please ensure that you have read the [License][license], [Code of Conduct][coc] and have signed the [Contributing License Agreement (CLA)][cla].

2. Please add tests confirming the new functionality works. Pull requests will not be merged without passing continuous integration tests unless the pull requests aims to fix existing issues with these tests.

## Working with the SDK

This guide is geared towards developing on a Linux machine. Visual Studio may or may not behave as expected.

You'll need to install the following tools.
* .NET 5.0
* PowerShell

### Building

Building using the `dotnet` CLI is straightforward.

```sh
dotnet build dropbox-sdk-dotnet/Dropbox.Api/
```

### Running tests

Testing is also straightforward. Make sure to create `dropbox-sdk-dotnet/Dropbox.Api.Integration.Tests/settings.json` (see `settings.json.example` for a template) before running integration tests.

```sh
dotnet test dropbox-sdk-dotnet/Dropbox.Api.Unit.Tests/
dotnet test dropbox-sdk-dotnet/Dropbox.Api.Integration.Tests/
```

### Linting

You can use [dotnet-format](https://github.com/dotnet/format) to lint from the command line.

```sh
# Install a recent dotnet-format build
dotnet tool install -g dotnet-format --add-source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json
# Optionally omit `--check` to auto-fix lint issues
dotnet format --check --fix-whitespace --fix-style info --fix-analyzers info dropbox-sdk-dotnet/
```

### Updating Generated Code

Install PowerShell and execute `./scripts/generate_stone.ps1` to regenerate Stone types.

```sh
git submodule init
git submodule update --remote --recursive
./scripts/generate_stone.ps1
```

### Cutting New Versions (for Dropboxers)

To cut a new version, create a new GitHub release using `vX.Y.Z` as the tag name. GitHub Actions will automatically build the SDK and publish it to NuGet as version `X.Y.Z`.

[issues]: https://github.com/dropbox/dropbox-sdk-dotnet/issues
[pr]: https://github.com/dropbox/dropbox-sdk-dotnet/pulls
[coc]: https://github.com/dropbox/dropbox-sdk-dotnet/blob/main/CODE_OF_CONDUCT.md
[license]: https://github.com/dropbox/dropbox-sdk-dotnet/blob/main/LICENSE
[cla]: https://opensource.dropbox.com/cla/
