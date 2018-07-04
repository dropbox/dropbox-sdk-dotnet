# Dropbox.NET

A .NET SDK for integrating with the Dropbox API v2.

## Support Platforms
 - .NET Framework 4+
 - Windows Store Apps
 - Windows Phone 8 and 8.1 Apps
 - Silverlight 5.0
 - .NET Standard 1.1+ (Note: If you are using project.json, you need to add `portable-net45+win8` to imports under you frameworks section. If you are using .csproj file, you need to add `<PackageTargetFallback>portable-net45+win8</PackageTargetFallback>` element to your project.)
## Setup

To get started with Dropbox.NET, we recommend you add it to your project using NuGet.

To install `Dropbox.Api`, run the following command in the Package Manager Console:

```PM> Install-Package Dropbox.Api```

## Creating an application

You need to create an Dropbox Application to make API requests.

- Go to https://dropbox.com/developers/apps.

## Obtaining an access token

All requests need to be made with an OAuth 2 access token. To get started, once
you've created an app, you can go to the app's console and generate an access
token for your own Dropbox account.

## Examples

Several examples can be found in the [examples](https://github.com/dropbox/dropbox-sdk-dotnet/tree/master/dropbox-sdk-dotnet/Examples) directory:
* [SimpleBlogDemo](https://github.com/dropbox/dropbox-sdk-dotnet/tree/master/dropbox-sdk-dotnet/Examples/SimpleBlogDemo) — An ASP.NET MVC application that creates a simple blogging
  platform, this shows how to upload and download files.
* [SimpleBusinessDashboard](https://github.com/dropbox/dropbox-sdk-dotnet/tree/master/dropbox-sdk-dotnet/Examples/SimpleBusinessDashboard) — An ASP.NET MVC application that creates a simple business
  dashboard, showcasing the Dropbox Business endpoints.
* [SimpleTest](https://github.com/dropbox/dropbox-sdk-dotnet/tree/master/dropbox-sdk-dotnet/Examples/SimpleTest) — A windows console application that demonstrates basic use of the SDK;
  this also contains code that connects with OAuth2 using WPF.
* [UniversalDemo](https://github.com/dropbox/dropbox-sdk-dotnet/tree/master/dropbox-sdk-dotnet/Examples/UniversalDemo/UniversalDemo) — A slide show app for both Windows Store and
  Windows Phone 8.1

## Documentation

You can find out more details in the [full documentation for the Dropbox.NET SDK](http://dropbox.github.io/dropbox-sdk-dotnet/html/R_Project_DotNetApiDocumentation.htm).

## Building from source
You can also build the SDK or create local nuget package from source code directly.
### Basic Setup

1. Prerequisites:
   - Visual Studio 2013 or above.
   - Python 2.7 or above.
   - [Optional] Sandcastle Help File Builder installed (https://github.com/EWSoftware/SHFB/releases). This is only required for doc generation.
   - [Optional] Visual Studio 2017 or above. This is required to generate .Net Standard package

2. Clone the repository and update submodules.
   ```
   git clone https://github.com/dropbox/dropbox-sdk-dotnet.git
   cd dropbox-sdk-dotnet
   git submodule init    
   git submodule update # also do this after every "git checkout" and "git pull"
   ```
3. Install stone and its dependencies by running
   ```
   cd stone
   python setup.py install
   ```

### Generate latest source code

1. Inside `dropbox-sdk-dotnet` repo, run `generate.py` script to generate class for latest data types. This will also generate all csproj files.
   ```
   python generate.py
   ```

2. Open up the `Dropbox.Api.sln` in Visual Studio and run
   the included examples as a sanity check.

### Create nuget package (This needs to be done on Windows)
1. Edit buildall.ps1 and update major version, release version and release notes.
2. In Visual Studio Developer Command Prompt run
   ```
   powershell -ExecutionPolicy Bypass -File buildall.ps1 -testSettings <PATH_TO_TEST_SETTINGS> 
   ```
   A .nukpg file will be generated in `Dropbox.Api` directory. Checkout [here](dropbox-sdk-dotnet/Dropbox.Api.Tests/dropbox.runsettings)
   for the format of test settings file.

### Generating Docs
1. In Visual Studio Developer Command Prompt, run
   ```
   powershell -ExecutionPolicy Bypass -File buildall.ps1 -testSettings <PATH_TO_TEST_SETTINGS> -doc
   ```
