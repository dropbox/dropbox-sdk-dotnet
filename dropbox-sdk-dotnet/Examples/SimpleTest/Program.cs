// <copyright file="Program.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>

namespace SimpleTest
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Dropbox.Api;
    using Dropbox.Api.Common;
    using Dropbox.Api.Files;
    using Dropbox.Api.Team;

    internal partial class Program
    {
        // Add an ApiKey (from https://www.dropbox.com/developers/apps) here
        private const string ApiKey = "XXXXXXXXXXXXXXX";

        // This loopback host is for demo purpose. If this port is not
        // available on your machine you need to update this URL with an unused port.
        private const string LoopbackHost = "http://127.0.0.1:52475/";

        // URL to receive OAuth 2 redirect from Dropbox server.
        // You also need to register this redirect URL on https://www.dropbox.com/developers/apps.
        private readonly Uri redirectUri = new Uri(LoopbackHost + "authorize");

        // URL to receive access token from JS.
        private readonly Uri jSRedirectUri = new Uri(LoopbackHost + "token");

        private readonly string settingsPath = Path.Join(Directory.GetCurrentDirectory(), "settings.json");

        [STAThread]
        private static int Main(string[] args)
        {
            Console.WriteLine("SimpleTest");
            var instance = new Program();
            try
            {
                var task = Task.Run((Func<Task<int>>)instance.Run);

                task.Wait();

                return task.Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void WriteSettings(Settings settings)
        {
            File.WriteAllText(
                this.settingsPath,
                JsonSerializer.Serialize(settings));
        }

        private Settings ReadSettings()
        {
            return JsonSerializer.Deserialize<Settings>(File.ReadAllText(this.settingsPath));
        }

        private async Task<int> Run()
        {
            DropboxCertHelper.InitializeCertPinning();

            var accessToken = await this.GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                return 1;
            }

            // Specify socket level timeout which decides maximum waiting time when no bytes are
            // received by the socket.
            var httpClient = new HttpClient(new SocketsHttpHandler())
            {
                // Specify request level timeout which decides maximum time that can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20),
            };

            try
            {
                var config = new DropboxClientConfig("SimpleTestApp")
                {
                    HttpClient = httpClient,
                };

                var client = new DropboxClient(accessToken, config);
                await this.RunUserTests(client);

                // Tests below are for Dropbox Business endpoints. To run these tests, make sure the ApiKey is for
                // a Dropbox Business app and you have an admin account to log in.

                /*
                var client = new DropboxTeamClient(accessToken, userAgent: "SimpleTeamTestApp", httpClient: httpClient);
                await RunTeamTests(client);
                */
            }
            catch (HttpException e)
            {
                Console.WriteLine("Exception reported from RPC layer");
                Console.WriteLine("    Status code: {0}", e.StatusCode);
                Console.WriteLine("    Message    : {0}", e.Message);
                if (e.RequestUri != null)
                {
                    Console.WriteLine("    Request uri: {0}", e.RequestUri);
                }
            }

            return 0;
        }

        /// <summary>
        /// Run tests for user-level operations.
        /// </summary>
        /// <param name="client">The Dropbox client.</param>
        /// <returns>An asynchronous task.</returns>
        private async Task RunUserTests(DropboxClient client)
        {
            await this.GetCurrentAccount(client);

            var path = "/DotNetApi/Help";
            var folder = await this.CreateFolder(client, path);
            var list = await this.ListFolder(client, path);

            var firstFile = list.Entries.FirstOrDefault(i => i.IsFile);
            if (firstFile != null)
            {
                await this.Download(client, path, firstFile.AsFile);
            }

            var pathInTeamSpace = "/Test";
            await this.ListFolderInTeamSpace(client, pathInTeamSpace);

            await this.Upload(client, path, "Test.txt", "This is a text file");

            await this.ChunkUpload(client, path, "Binary");
        }

        /// <summary>
        /// Run tests for team-level operations.
        /// </summary>
        /// <param name="client">The Dropbox client.</param>
        /// <returns>An asynchronous task.</returns>
        private async Task RunTeamTests(DropboxTeamClient client)
        {
            var members = await client.Team.MembersListAsync();

            var member = members.Members.FirstOrDefault();

            if (member != null)
            {
                // A team client can perform action on a team member's behalf. To do this,
                // just pass in team member id in to AsMember function which returns a user client.
                // This client will operates on this team member's Dropbox.
                var userClient = client.AsMember(member.Profile.TeamMemberId);
                await this.RunUserTests(userClient);
            }
        }

        /// <summary>
        /// Handles the redirect from Dropbox server. Because we are using token flow, the local
        /// http server cannot directly receive the URL fragment. We need to return a HTML page with
        /// inline JS which can send URL fragment to local server as URL parameter.
        /// </summary>
        /// <param name="http">The http listener.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task HandleOAuth2Redirect(HttpListener http)
        {
            var context = await http.GetContextAsync();

            // We only care about request to RedirectUri endpoint.
            while (context.Request.Url.AbsolutePath != this.redirectUri.AbsolutePath)
            {
                context = await http.GetContextAsync();
            }

            context.Response.ContentType = "text/html";

            // Respond with a page which runs JS and sends URL fragment as query string
            // to TokenRedirectUri.
            using (var file = File.OpenRead("index.html"))
            {
                file.CopyTo(context.Response.OutputStream);
            }

            context.Response.OutputStream.Close();
        }

        /// <summary>
        /// Handle the redirect from JS and process raw redirect URI with fragment to
        /// complete the authorization flow.
        /// </summary>
        /// <param name="http">The http listener.</param>
        /// <returns>The <see cref="OAuth2Response"/>.</returns>
        private async Task<OAuth2Response> HandleJSRedirect(HttpListener http)
        {
            var context = await http.GetContextAsync();

            // We only care about request to TokenRedirectUri endpoint.
            while (context.Request.Url.AbsolutePath != this.jSRedirectUri.AbsolutePath)
            {
                context = await http.GetContextAsync();
            }

            var redirectUri = new Uri(context.Request.QueryString["url_with_fragment"]);

            var result = DropboxOAuth2Helper.ParseTokenFragment(redirectUri);

            return result;
        }

        /// <summary>
        /// Gets the dropbox access token.
        /// <para>
        /// This fetches the access token from the applications settings, if it is not found there
        /// (or if the user chooses to reset the settings) then the UI in <see cref="LoginForm"/> is
        /// displayed to authorize the user.
        /// </para>
        /// </summary>
        /// <returns>A valid access token or null.</returns>
        private async Task<string> GetAccessToken()
        {
            Console.Write("Reset settings (Y/N) ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                this.WriteSettings(new Settings());
            }

            Console.WriteLine();

            var settings = this.ReadSettings();

            if (string.IsNullOrEmpty(settings.AccessToken))
            {
                try
                {
                    Console.WriteLine("Waiting for credentials.");
                    var state = Guid.NewGuid().ToString("N");
                    var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, ApiKey, this.redirectUri, state: state);
                    var http = new HttpListener();
                    http.Prefixes.Add(LoopbackHost);

                    http.Start();

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        System.Diagnostics.Process.Start($"\"{authorizeUri}\"");
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        System.Diagnostics.Process.Start("xdg-open", $"\"{authorizeUri}\"");
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        System.Diagnostics.Process.Start("open", $"\"{authorizeUri}\"");
                    }

                    // Handle OAuth redirect and send URL fragment to local server using JS.
                    await this.HandleOAuth2Redirect(http);

                    // Handle redirect from JS and process OAuth response.
                    var result = await this.HandleJSRedirect(http);

                    if (result.State != state)
                    {
                        // The state in the response doesn't match the state in the request.
                        return null;
                    }

                    Console.WriteLine("and back...");

                    var accessToken = result.AccessToken;
                    var uid = result.Uid;
                    Console.WriteLine("Uid: {0}", uid);

                    settings.AccessToken = accessToken;
                    settings.Uid = uid;

                    this.WriteSettings(settings);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                    return null;
                }
            }

            return settings.AccessToken;
        }

        /// <summary>
        /// Gets information about the currently authorized account.
        /// <para>
        /// This demonstrates calling a simple rpc style api from the Users namespace.
        /// </para>
        /// </summary>
        /// <param name="client">The Dropbox client.</param>
        /// <returns>An asynchronous task.</returns>
        private async Task GetCurrentAccount(DropboxClient client)
        {
            var full = await client.Users.GetCurrentAccountAsync();

            Console.WriteLine("Account id    : {0}", full.AccountId);
            Console.WriteLine("Country       : {0}", full.Country);
            Console.WriteLine("Email         : {0}", full.Email);
            Console.WriteLine("Is paired     : {0}", full.IsPaired ? "Yes" : "No");
            Console.WriteLine("Locale        : {0}", full.Locale);
            Console.WriteLine("Name");
            Console.WriteLine("  Display  : {0}", full.Name.DisplayName);
            Console.WriteLine("  Familiar : {0}", full.Name.FamiliarName);
            Console.WriteLine("  Given    : {0}", full.Name.GivenName);
            Console.WriteLine("  Surname  : {0}", full.Name.Surname);
            Console.WriteLine("Referral link : {0}", full.ReferralLink);

            if (full.Team != null)
            {
                Console.WriteLine("Team");
                Console.WriteLine("  Id   : {0}", full.Team.Id);
                Console.WriteLine("  Name : {0}", full.Team.Name);
            }
            else
            {
                Console.WriteLine("Team - None");
            }
        }

        /// <summary>
        /// Creates the specified folder.
        /// </summary>
        /// <remarks>This demonstrates calling an rpc style api in the Files namespace.</remarks>
        /// <param name="client">The Dropbox client.</param>
        /// <param name="path">The path of the folder to create.</param>
        /// <returns>The result from the ListFolderAsync call.</returns>
        private async Task<FolderMetadata> CreateFolder(DropboxClient client, string path)
        {
            Console.WriteLine("--- Creating Folder ---");
            var folderArg = new CreateFolderArg(path);
            try
            {
                var folder = await client.Files.CreateFolderV2Async(folderArg);

                Console.WriteLine("Folder: " + path + " created!");

                return folder.Metadata;
            }
            catch (ApiException<CreateFolderError> e)
            {
                if (e.Message.StartsWith("path/conflict/folder"))
                {
                    Console.WriteLine("Folder already exists... Skipping create");
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Lists the items within a folder inside team space. See
        /// https://www.dropbox.com/developers/reference/namespace-guide for details about
        /// user namespace vs team namespace.
        /// </summary>
        /// <param name="client">The Dropbox client.</param>
        /// <param name="path">The path to list.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task ListFolderInTeamSpace(DropboxClient client, string path)
        {
            // Fetch root namespace info from user's account info.
            var account = await client.Users.GetCurrentAccountAsync();

            if (!account.RootInfo.IsTeam)
            {
                Console.WriteLine("This user doesn't belong to a team with shared space.");
            }
            else
            {
                try
                {
                    // Point path root to namespace id of team space.
                    client = client.WithPathRoot(new PathRoot.Root(account.RootInfo.RootNamespaceId));
                    await this.ListFolder(client, path);
                }
                catch (PathRootException ex)
                {
                    Console.WriteLine(
                        "The user's root namespace ID has changed to {0}",
                        ex.ErrorResponse.AsInvalidRoot.Value);
                }
            }
        }

        /// <summary>
        /// Lists the items within a folder.
        /// </summary>
        /// <remarks>This demonstrates calling an rpc style api in the Files namespace.</remarks>
        /// <param name="client">The Dropbox client.</param>
        /// <param name="path">The path to list.</param>
        /// <returns>The result from the ListFolderAsync call.</returns>
        private async Task<ListFolderResult> ListFolder(DropboxClient client, string path)
        {
            Console.WriteLine("--- Files ---");
            var list = await client.Files.ListFolderAsync(path);

            // show folders then files
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                Console.WriteLine("D  {0}/", item.Name);
            }

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                var file = item.AsFile;

                Console.WriteLine(
                    "F{0,8} {1}",
                    file.Size,
                    item.Name);
            }

            if (list.HasMore)
            {
                Console.WriteLine("   ...");
            }

            return list;
        }

        /// <summary>
        /// Downloads a file.
        /// </summary>
        /// <remarks>This demonstrates calling a download style api in the Files namespace.</remarks>
        /// <param name="client">The Dropbox client.</param>
        /// <param name="folder">The folder path in which the file should be found.</param>
        /// <param name="file">The file to download within <paramref name="folder"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task Download(DropboxClient client, string folder, FileMetadata file)
        {
            Console.WriteLine("Download file...");

            using (var response = await client.Files.DownloadAsync(folder + "/" + file.Name))
            {
                Console.WriteLine("Downloaded {0} Rev {1}", response.Response.Name, response.Response.Rev);
                Console.WriteLine("------------------------------");
                Console.WriteLine(await response.GetContentAsStringAsync());
                Console.WriteLine("------------------------------");
            }
        }

        /// <summary>
        /// Uploads given content to a file in Dropbox.
        /// </summary>
        /// <param name="client">The Dropbox client.</param>
        /// <param name="folder">The folder to upload the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileContent">The file content.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task Upload(DropboxClient client, string folder, string fileName, string fileContent)
        {
            Console.WriteLine("Upload file...");

            using (var stream = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(fileContent)))
            {
                var response = await client.Files.UploadAsync(folder + "/" + fileName, WriteMode.Overwrite.Instance, body: stream);

                Console.WriteLine("Uploaded Id {0} Rev {1}", response.Id, response.Rev);
            }
        }

        /// <summary>
        /// Uploads a big file in chunk. The is very helpful for uploading large file in slow network condition
        /// and also enable capability to track upload progerss.
        /// </summary>
        /// <param name="client">The Dropbox client.</param>
        /// <param name="folder">The folder to upload the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task ChunkUpload(DropboxClient client, string folder, string fileName)
        {
            Console.WriteLine("Chunk upload file...");

            // Chunk size is 128KB.
            const uint chunkSize = 128 * 1024;

            // Create a random file of 1MB in size.
            var fileContent = new byte[1024 * 1024];
            new Random().NextBytes(fileContent);

            using (var stream = new MemoryStream(fileContent))
            {
                ulong numChunks = (ulong)Math.Ceiling((double)stream.Length / chunkSize);

                byte[] buffer = new byte[chunkSize];
                string sessionId = null;

                for (ulong idx = 0; idx < numChunks; idx++)
                {
                    Console.WriteLine("Start uploading chunk {0}", idx);
                    var byteRead = stream.Read(buffer, 0, (int)chunkSize);

                    using (MemoryStream memStream = new MemoryStream(buffer, 0, byteRead))
                    {
                        if (idx == 0)
                        {
                            var result = await client.Files.UploadSessionStartAsync(body: memStream);
                            sessionId = result.SessionId;
                        }
                        else
                        {
                            UploadSessionCursor cursor = new UploadSessionCursor(sessionId, (ulong)(chunkSize * idx));

                            if (idx == numChunks - 1)
                            {
                                await client.Files.UploadSessionFinishAsync(cursor, new CommitInfo(folder + "/" + fileName), memStream);
                            }
                            else
                            {
                                await client.Files.UploadSessionAppendV2Async(cursor, body: memStream);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// List all members in the team.
        /// </summary>
        /// <param name="client">The Dropbox team client.</param>
        /// <returns>The result from the MembersListAsync call.</returns>
        private async Task<MembersListResult> ListTeamMembers(DropboxTeamClient client)
        {
            var members = await client.Team.MembersListAsync();

            foreach (var member in members.Members)
            {
                Console.WriteLine("Member id    : {0}", member.Profile.TeamMemberId);
                Console.WriteLine("Name         : {0}", member.Profile.Name);
                Console.WriteLine("Email        : {0}", member.Profile.Email);
            }

            return members;
        }

        private class Settings
        {
            public string AccessToken { get; set; } = string.Empty;

            public string Uid { get; set; } = string.Empty;
        }
    }
}
