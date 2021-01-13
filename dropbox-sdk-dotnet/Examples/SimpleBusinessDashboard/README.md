# Simple Business Dashboard

A example web app built using Dropbox API v2 .NET SDK. This app allow you to post blogs which will be automatically saved to your Dropbox. It can help you go througth the server app OAuth flow and file operations APIs.

## Setup
Create `appsettings.json` using `appsettings.json.example` as a template. You will need to supply your own app credentials. You can find them from the App Console at https://www.dropbox.com/developers/apps. Be sure to add http://localhost:5000/Home/Auth to the redirect URL list in the app console.

## Run

Launch the demo by running `dotnet run` and navigating to http://localhost:5000.

## Caveats

This demo serves only to demonstrate the Dropbox .NET SDK and should not be used as an example of how to implement a ASP.NET Core project.
