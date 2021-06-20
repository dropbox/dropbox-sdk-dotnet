namespace OauthPKCE
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    using Dropbox.Api;
    using OAuthPKCE;

    partial class Program
    {
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
        static async Task<int> Main()
        {
            return await Task.Run(new Program().Run);
        }

        private async Task<int> Run()
        {
            Console.WriteLine("Example OAuth PKCE Application");
            DropboxCertHelper.InitializeCertPinning();

            var uid = await AcquireOAuthTokens(null, IncludeGrantedScopes.None);
            if (string.IsNullOrEmpty(uid))
            {
                return 1;
            }

            // Specify socket level timeout which decides maximum waiting time when no bytes are
            // received by the socket.
            using HttpClientHandler httpClientHandler = new HttpClientHandler();
            using var httpClient = new HttpClient(httpClientHandler)
            {
                // Specify request level timeout which decides maximum time that can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            };

            try
            {
                var config = new DropboxClientConfig("SimplePKCEOAuthApp")
                {
                    HttpClient = httpClient
                };

                var client = new DropboxClient(Settings.Default.RefreshToken, Settings.Default.ApiKey, config);

                // This call should succeed since the correct scope has been acquired
                await GetCurrentAccount(client);

                Console.WriteLine("Oauth PKCE Test Complete!");
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
        /// Acquires a dropbox OAuth tokens and saves them to the default settings for the app.
        /// <para>
        /// This fetches the OAuth tokens from the applications settings, if it is not found there
        /// (or if the user chooses to reset the settings) then the UI in <see cref="LoginForm"/> is
        /// displayed to authorize the user.
        /// </para>
        /// </summary>
        /// <returns>A valid uid if a token was acquired or null.</returns>
        private async Task<string> AcquireOAuthTokens(string[] scopeList, IncludeGrantedScopes includeGrantedScopes)
        {
            Settings.Default.Upgrade();
            Console.Write("Reset settings (Y/N) ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Settings.Default.Reset();
            }
            Console.WriteLine();

            string accessToken = Settings.Default.AccessToken;
            string uid = Settings.Default.Uid;

            if (string.IsNullOrEmpty(accessToken))
            {
                string apiKey = GetApiKey();

                using var http = new HttpListener();
                try
                {
                    Console.WriteLine("Waiting for credentials.");
                    string state = Guid.NewGuid().ToString("N");
                    var OAuthFlow = new PKCEOAuthFlow();
                    var authorizeUri = OAuthFlow.GetAuthorizeUri(OAuthResponseType.Code, apiKey, RedirectUri.ToString(), state: state, tokenAccessType: TokenAccessType.Offline, scopeList: scopeList, includeGrantedScopes: includeGrantedScopes);
                    http.Prefixes.Add(LoopbackHost);

                    http.Start();

                    ProcessStartInfo startInfo = new ProcessStartInfo(
                        authorizeUri.ToString()){ UseShellExecute = true };

                    try
                    {
                        // open browser for authentication
                        Process.Start(startInfo);
                        Console.WriteLine("Waiting for authentication...");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("An unexpected error occured while opening the browser.");
                    }

                    // Handle OAuth redirect and send URL fragment to local server using JS.
                    await HandleOAuth2Redirect(http);

                    // Handle redirect from JS and process OAuth response.
                    Uri redirectUri = await HandleJSRedirect(http);

                    http.Stop();

                    Console.WriteLine("Exchanging code for token");
                    var tokenResult = await OAuthFlow.ProcessCodeFlowAsync(redirectUri, apiKey, RedirectUri.ToString(), state);
                    Console.WriteLine("Finished Exchanging Code for Token");
                    // Bring console window to the front.
                    SetForegroundWindow(GetConsoleWindow());
                    accessToken = tokenResult.AccessToken;
                    string refreshToken = tokenResult.RefreshToken;
                    uid = tokenResult.Uid;
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
                    Settings.Default.Reload();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                    return null;
                }
            }

            return uid;
        }

        /// <summary>
        /// Retrieve the ApiKey from the user
        /// </summary>
        /// <returns>Return the ApiKey specified by the user</returns>
        private static string GetApiKey()
        {
            string apiKey = Settings.Default.ApiKey;
            
            while (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine("Create a Dropbox App at https://www.dropbox.com/developers/apps.");
                Console.Write("Enter the API Key (or 'Quit' to exit): ");
                apiKey = Console.ReadLine();
                if (apiKey.ToLower() == "quit")
                {
                    Console.WriteLine("The API Key is required to connect to Dropbox.");
                    apiKey = null;
                    break;
                }
                else
                {
                    Settings.Default.ApiKey = apiKey;
                }
            }

            return string.IsNullOrWhiteSpace(apiKey) ? null : apiKey;
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
