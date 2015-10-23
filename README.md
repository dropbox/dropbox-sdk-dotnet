# Dropbox.NET

A .NET SDK for integrating with the Dropbox API v2 (preview version).

## Setup

To get started with Dropbox.NET, we recommend you add it to your project using NuGet.

To install `Dropbox.Api`, run the following command in the Package Manager Console:

```PM> Install-Package Dropbox.Api -Pre```

## Creating an application

You need to create an Dropbox Application to make API requests.

- Go to https://dropbox.com/developers/apps.

## Obtaining an access token

All requests need to be made with an OAuth 2 access token. To get started, once
you've created an app, you can go to the app's console and generate an access
token for your own Dropbox account.

## Examples

Several examples can be found in the examples directory:
* SimpleBlogDemo — An ASP.NET MVC application that creates a simple blogging
  platform, this shows how to upload and download files.
* SimpleTest — A windows console application that demonstrates basic use of the SDK;
  this also contains code that connects with OAuth2 using WPF.
* UniversalDemo — A slide show app for both Windows Store and
  Windows Phone 8.1

## Documentation

You can find out more details in the [full documentation for the Dropbox.NET SDK](http://dropbox.github.io/dropbox-sdk-dotnet/html/R_Project_DotNetApiDocumentation.htm).
