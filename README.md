[![Logo][logo]][repo]

[![NuGet](https://img.shields.io/badge/Frameworks-.NetFramework%204.5%20%7C%20.NetStandard%202.0-blue)](https://www.nuget.org/packages/Dropbox.Api)
[![NuGet](https://img.shields.io/nuget/v/Dropbox.Api.svg)](https://www.nuget.org/packages/Dropbox.Api)
[![codecov](https://codecov.io/gh/dropbox/dropbox-sdk-dotnet/branch/master/graph/badge.svg)](https://codecov.io/gh/dropbox/dropbox-sdk-dotnet)

The offical Dropbox SDK for DotNet.

Documentation can be found on [Github Pages][documentation]

## Installation

Create an app via the [Developer Console][devconsole]

Install via [NuGet](https://www.nuget.org/)

```
PM> Install-Package Dropbox.Api
```

After installation, follow one of our [Examples][examples] or read the [Documentation][documentation].

You can also view our [OAuth guide][oauthguide].

## Examples

We provide [Examples][examples] to help get you started with a lot of the basic functionality in the SDK.

- **OAuth**
    - [OAuth Basic](https://github.com/dropbox/dropbox-sdk-dotnet/tree/master/dropbox-sdk-dotnet/Examples/OauthBasic) - Simple application that runs through a basic OAuth flow to acquire a token and make a call to users/get_current_account.
    - [OAuth PKCE](https://github.com/dropbox/dropbox-sdk-dotnet/tree/master/dropbox-sdk-dotnet/Examples/OAuthPKCE) - Simple application that runs through an OAuth flow using [PKCE](https://oauth.net/2/pkce/) and acquires a token to make a call to users/get_current_account.
- **Other Examples**
    - [Simple Test](https://github.com/dropbox/dropbox-sdk-dotnet/tree/master/dropbox-sdk-dotnet/Examples/SimpleTest) - This is a simple test which validates some simple functionality (This is a good place to start to see how the SDK can be used)
    - [Simple Blog Demo](https://github.com/dropbox/dropbox-sdk-dotnet/tree/master/dropbox-sdk-dotnet/Examples/SimpleBlogDemo) - This is a simple demo of how the Dropbox SDK can be used to create a simple blog with backed up blog posts
    - [Simple Business Dashboard](https://github.com/dropbox/dropbox-sdk-dotnet/tree/master/dropbox-sdk-dotnet/Examples/SimpleBusinessDashboard) - This is a demo of a business dashboard
    - [Universal Demo](https://github.com/dropbox/dropbox-sdk-dotnet/tree/master/dropbox-sdk-dotnet/Examples/UniversalDemo/UniversalDemo) - This is an example of how to use the SDK across multiple platforms

## Getting Help

If you find a bug, please see [CONTRIBUTING.md][contributing] for information on how to report it.

If you need help that is not specific to this SDK, please reach out to [Dropbox Support][support].

## License

This SDK is distributed under the MIT license, please see [LICENSE][license] for more information.

[logo]: https://cfl.dropboxstatic.com/static/images/sdk/dotnet_banner.png
[repo]: https://github.com/dropbox/dropbox-sdk-dotnet
[documentation]: http://dropbox.github.io/dropbox-sdk-dotnet/html/R_Project_DotNetApiDocumentation.htm
[examples]: https://github.com/dropbox/dropbox-sdk-dotnet/tree/master/dropbox-sdk-dotnet/Examples
[license]: https://github.com/dropbox/dropbox-sdk-dotnet/blob/master/LICENSE
[contributing]: https://github.com/dropbox/dropbox-sdk-dotnet/blob/master/CONTRIBUTING.md
[devconsole]: https://dropbox.com/developers/apps
[oauthguide]: https://www.dropbox.com/lp/developers/reference/oauth-guide
[support]: https://www.dropbox.com/developers/contact
