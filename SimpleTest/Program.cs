namespace SimpleTest
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows;

    using Dropbox.Api;
    using Dropbox.Api.Files;

    partial class Program
    {
        // Add an ApiKey (from https://www.dropbox.com/developers/apps) here
        // private const string ApiKey = "XXXXXXXXXXXXXXX";

        private DropboxClient client;

        [STAThread]
        static int Main(string[] args)
        {
            var instance = new Program();

            var task = Task.Run((Func<Task<int>>)instance.Run);

            task.Wait();

            return task.Result;
        }

        private async Task<int> Run()
        {
            InitializeCertPinning();

            var access_token = this.GetAccessToken();
            if (string.IsNullOrEmpty(access_token))
            {
                return 1;
            }

            this.client = new DropboxClient(access_token, userAgent: "SimpleTestApp");

            await GetCurrentAccount();

            var path = "/DotNetApi/Help";
            var list = await ListFolder(path);

            var firstFile = list.Entries.FirstOrDefault(i => i.Metadata.IsFile);
            if (firstFile != null)
            {
                await Download(path, firstFile);
            }

            return 0;
        }

        /// <summary>
        /// Initializes ssl certificate pinning.
        /// </summary>
        private void InitializeCertPinning()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                var root = chain.ChainElements[chain.ChainElements.Count - 1];
                var publicKey = root.Certificate.GetPublicKeyString();

                return DropboxCertHelper.IsKnownRootCertPublicKey(publicKey);
            };
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
        private string GetAccessToken()
        {
            Console.Write("Reset settings (Y/N) ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Settings.Default.Reset();
            }
            Console.WriteLine();

            var access_token = Settings.Default.AccessToken;

            if (string.IsNullOrEmpty(access_token))
            {
                Console.WriteLine("Waiting for credentials.");
                var app = new Application();
                var login = new LoginForm(ApiKey);
                app.Run(login);
                Console.WriteLine("and back...");

                if (!login.Result)
                {
                    Console.WriteLine("Unable to authenticate with Dropbox");
                    return null;
                }

                access_token = login.AccessToken;
                Console.WriteLine("Uid: {0}", login.Uid);

                Settings.Default.AccessToken = access_token;
                Settings.Default.Uid = login.Uid;

                Settings.Default.Save();
            }

            return access_token;
        }

        /// <summary>
        /// Gets information about the currently authorized account.
        /// <para>
        /// This demonstrates calling a simple rpc style api from the Users namespace.
        /// </para>
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        private async Task GetCurrentAccount()
        {
            var full = await this.client.Users.GetCurrentAccountAsync();

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
        /// Lists the items within a folder.
        /// </summary>
        /// <remarks>This is a demonstrates calling an rpc style api in the Files namespace.</remarks>
        /// <param name="path">The path to list.</param>
        /// <returns>The result from the ListFolderAsync call.</returns>
        private async Task<ListFolderResult> ListFolder(string path)
        {
            Console.WriteLine("--- Files ---");
            var list = await this.client.Files.ListFolderAsync(path);

            // show folders then files
            foreach (var item in list.Entries.Where(i => i.Metadata.IsFolder))
            {
                Console.WriteLine("D  {0}/", item.Name);
            }

            foreach (var item in list.Entries.Where(i => i.Metadata.IsFile))
            {
                var file = item.Metadata.AsFile;

                Console.WriteLine("F{0,8} {1}",
                    file.Size,
                    item.Name);
            }

            if (list.Footer.HasMore)
            {
                Console.WriteLine("   ...");
            }
            return list;
        }

        /// <summary>
        /// Downloads a file.
        /// </summary>
        /// <remarks>This demonstrates calling a download style api in the Files namespace.</remarks>
        /// <param name="folder">The folder path in which the file should be found.</param>
        /// <param name="file">The file to download within <paramref name="folder"/>.</param>
        /// <returns></returns>
        private async Task Download(string folder, ChangeEntry file)
        {
            Console.WriteLine("Download file...");

            using (var response = await this.client.Files.DownloadAsync(folder + "/" + file.Name))
            {
                Console.WriteLine("Downloaded {0} Rev {1}", response.Response.Name, response.Response.Metadata.Rev);
                Console.WriteLine("------------------------------");
                Console.WriteLine(await response.GetContentAsStringAsync());
                Console.WriteLine("------------------------------");
            }
        }
    }
}
