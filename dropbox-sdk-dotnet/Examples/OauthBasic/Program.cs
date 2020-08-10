namespace OauthTest
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    using Dropbox.Api;

    partial class Program
    {
        // Add an ApiKey (from https://www.dropbox.com/developers/apps) here
        private const string ApiKey = "XXXXXXXXXXXXXXX";

        // Add an ApiSecret (from https://www.dropbox.com/developers/apps) here
        private const string ApiSecret = "XXXXXXXXXXXXXXX";

        // This loopback host is for demo purpose. If this port is not
        // available on your machine you need to update this URL with an unused port.
        private const string LoopbackHost = "http://127.0.0.1:52475/";

        // URL to receive OAuth 2 redirect from Dropbox server.
        // You also need to register this redirect URL on https://www.dropbox.com/developers/apps.
        private readonly Uri RedirectUri = new Uri(LoopbackHost + "authorize");

        // URL to receive access token from JS.
        private readonly Uri JSRedirectUri = new Uri(LoopbackHost + "token");


        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [STAThread]
        static int Main(string[] args)
        {
            var instance = new Program();
            try
            {
                Console.WriteLine("Example OAuth Application");
                var task = Task.Run((Func<Task<int>>)instance.Run);

                task.Wait();

                return task.Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        private async Task<int> Run()
        {
            DropboxCertHelper.InitializeCertPinning();

            string[] scopeList = new string[3] { "files.metadata.read", "files.content.read", "account_info.read" };
            var uid = await this.AcquireAccessToken(scopeList, IncludeGrantedScopes.None);
            if (string.IsNullOrEmpty(uid))
            {
                return 1;
            }

            // Specify socket level timeout which decides maximum waiting time when no bytes are
            // received by the socket.
            var httpClient = new HttpClient(new WebRequestHandler { ReadWriteTimeout = 10 * 1000 })
            {
                // Specify request level timeout which decides maximum time that can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            };

            try
            {
                var config = new DropboxClientConfig("SimpleOAuthApp")
                {
                    HttpClient = httpClient
                };

                var client = new DropboxClient(Settings.Default.AccessToken, Settings.Default.RefreshToken, ApiKey, ApiSecret, config);

                // This call should succeed since the correct scope has been acquired
                await GetCurrentAccount(client);
                
                Console.WriteLine("Refreshing without scope account_info.read");
                var newScopes = new string[] { "files.metadata.read", "files.content.read" };
                await client.RefreshAccessToken(newScopes);
                try
                {
                    // This should fail since token does not have "account_info.read" scope  
                    await GetCurrentAccount(client);
                }
                catch (Exception)
                {
                    Console.WriteLine("Correctly failed with invalid scope");
                }
                Console.WriteLine("Attempting to try again with include_granted_scopes");
                await this.AcquireAccessToken(newScopes, IncludeGrantedScopes.User);
                var clientNew = new DropboxClient(Settings.Default.AccessToken, Settings.Default.RefreshToken, ApiKey, ApiSecret, config);
                await GetCurrentAccount(clientNew);

                Console.WriteLine("Oauth Test Complete!");
                Console.WriteLine("Exit with any key");
                Console.ReadKey();
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
        /// Handles the redirect from Dropbox server. Because we are using token flow, the local
        /// http server cannot directly receive the URL fragment. We need to return a HTML page with
        /// inline JS which can send URL fragment to local server as URL parameter.
        /// </summary>
        /// <param name="http">The http listener.</param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task HandleOAuth2Redirect(HttpListener http)
        {
            var context = await http.GetContextAsync();

            // We only care about request to RedirectUri endpoint.
            while (context.Request.Url.AbsolutePath != RedirectUri.AbsolutePath)
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
        /// <returns>The <see cref="OAuth2Response"/></returns>
        private async Task<Uri> HandleJSRedirect(HttpListener http)
        {
            var context = await http.GetContextAsync();

            // We only care about request to TokenRedirectUri endpoint.
            while (context.Request.Url.AbsolutePath != JSRedirectUri.AbsolutePath)
            {
                context = await http.GetContextAsync();
            }

            var redirectUri = new Uri(context.Request.QueryString["url_with_fragment"]);
            
            return redirectUri;
        }

        /// <summary>
        /// Acquires a dropbox access token and saves it to the default settings for the app.
        /// <para>
        /// This fetches the access token from the applications settings, if it is not found there
        /// (or if the user chooses to reset the settings) then the UI in <see cref="LoginForm"/> is
        /// displayed to authorize the user.
        /// </para>
        /// </summary>
        /// <returns>A valid uid if a token was acquired or null.</returns>
        private async Task<string> AcquireAccessToken(string[] scopeList, IncludeGrantedScopes includeGrantedScopes)
        {
            Console.Write("Reset settings (Y/N) ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Settings.Default.Reset();
            }
            Console.WriteLine();

            var accessToken = Settings.Default.AccessToken;
            var refreshToken = Settings.Default.RefreshToken;

            if (string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    Console.WriteLine("Waiting for credentials.");
                    var state = Guid.NewGuid().ToString("N");
                    var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code, ApiKey, RedirectUri, state: state, tokenAccessType : TokenAccessType.Offline, scopeList : scopeList, includeGrantedScopes: includeGrantedScopes);
                    var http = new HttpListener();
                    http.Prefixes.Add(LoopbackHost);

                    http.Start();

                    System.Diagnostics.Process.Start(authorizeUri.ToString());

                    // Handle OAuth redirect and send URL fragment to local server using JS.
                    await HandleOAuth2Redirect(http);

                    // Handle redirect from JS and process OAuth response.
                    var redirectUri = await HandleJSRedirect(http);

                    Console.WriteLine("Exchanging code for token");
                    var tokenResult = await DropboxOAuth2Helper.ProcessCodeFlowAsync(redirectUri, ApiKey, ApiSecret, RedirectUri.ToString(), state);
                    Console.WriteLine("Finished Exchanging Code for Token");
                    // Bring console window to the front.
                    SetForegroundWindow(GetConsoleWindow());
                    accessToken = tokenResult.AccessToken;
                    refreshToken = tokenResult.RefreshToken;
                    var uid = tokenResult.Uid;
                    Console.WriteLine("Uid: {0}", uid);
                    Console.WriteLine("AccessToken: {0}", accessToken);
                    if (tokenResult.RefreshToken != null)
                    {
                        Console.WriteLine("RefreshToken: {0}", refreshToken);
                        Settings.Default.RefreshToken = refreshToken;
                    }
                    if (tokenResult.ExpiresAt != null)
                    {
                        Console.WriteLine("ExpiresAt: {0}", tokenResult.ExpiresAt);
                    }
                    if (tokenResult.ScopeList != null)
                    {
                        Console.WriteLine("Scopes: {0}", String.Join(" ", tokenResult.ScopeList));
                    }
                    Settings.Default.AccessToken = accessToken;
                    Settings.Default.Uid = uid;
                    Settings.Default.Save();
                    http.Stop();
                    return uid;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                    return null;
                }
            }

            return null;
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
            try
            {
                Console.WriteLine("Current Account:");
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
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
