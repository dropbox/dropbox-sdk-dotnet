# Simple Blog Demo

An example web app built using Dropbox API v2 .NET SDK. This app allows you to post blogs which will be automatically saved to your Dropbox. It can help you go through the server app OAuth flow and file operations APIs.

## Setup

Create `appsettings.json` using `appsettings.json.example` as a template. The example settings use SQLite as a backing store for the demo. You will need to supply your own app credentials. You can find them from the App Console at https://www.dropbox.com/developers/apps. Be sure to add http://localhost:5000/Home/Auth to the redirect URL list in the app console.

To set up database tables, run the following:
```
dotnet tool install --global dotnet-ef
dotnet ef database update
```

## Run

Launch the demo by running `dotnet run` and navigating to http://localhost:5000. You can sign up for a test account by clicking the sign up button (make sure you put a blog name here, it will link to article creation page).

After registration, you can click the connect button which will kick out the OAuth flow. Once connected, all articles you create will be saved under /{Your Dropbox App Name} folder.


## Caveats

This demo serves only to demonstrate the Dropbox .NET SDK and should not be used as an example of how to implement a ASP.NET Core project.
