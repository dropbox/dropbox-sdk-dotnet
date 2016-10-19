# Simple Business Dashboard

A example web app built using Dropbox API v2 .NET SDK. This app allow you to post blogs which will be automatically saved to your Dropbox. It can help you go througth the server app OAuth flow and file operations APIs.

## Setup

To run this web app, load Dropbox.Api.sln in Visual Studio and set SimpleBusinessDashboard as startup project.

In appSettings section of Web.config, set DropboxAppKey and DropboxAppSecret to be your app key and secret. You can find them from the App Console at https://www.dropbox.com/developers/apps

Add http://localhost:5001/Home/Auth to the redirect URL list in the app console.

## Run

In Visual Studio, launch the web app in your desired browser. You can sign up for a test account by clicking the sign up button (make sure you put a blog name here, it will link to article creation page).

After registration, you can click the connect button which will kick out the OAuth flow. Once connected, all articles you create will be saved under /{Your Dropbox App Name} folder.
