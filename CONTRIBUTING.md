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

## Updating Generated Code

Generated code can be updated by running the following code:

```
$ git submodule init
$ git submodule update --remote --recursive
$ cd stone
$ python setup.py install
$ cd ..
$ python generate.py
```

Note: the `buildall.ps1` file also will update generated code so unless you are looking to explicitely test something new, this step is generally unnecessary

## Testing the Code

Tests live under the Dropbox.Api.Tests Project.  Please fill in the dropbox.runsettings file with test tokens in order to successfully make calls to the Dropbox servers.

The tests are run as a part of the build script we use, this can be run the following way: 

```
powershell -ExecutionPolicy Bypass -File buildall.ps1 -testSettings <PATH_TO_TEST_SETTINGS> 
```

Where test settings is the `dropbox.runsettings` file mentioned above.

If you need to test the documentation, you can add the `-doc` flag to the build command to also generate that.

```
powershell -ExecutionPolicy Bypass -File buildall.ps1 -testSettings <PATH_TO_TEST_SETTINGS> -doc
```

[issues]: https://github.com/dropbox/dropbox-sdk-dotnet/issues
[pr]: https://github.com/dropbox/dropbox-sdk-dotnet/pulls
[coc]: https://github.com/dropbox/dropbox-sdk-dotnet/blob/master/CODE_OF_CONDUCT.md
[license]: https://github.com/dropbox/dropbox-sdk-dotnet/blob/master/LICENSE
[cla]: https://opensource.dropbox.com/cla/
