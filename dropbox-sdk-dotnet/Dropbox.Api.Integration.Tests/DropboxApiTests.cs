//-----------------------------------------------------------------------------
// <copyright file="DropboxApiTests.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Dropbox.Api.Auth;
    using Dropbox.Api.Common;
    using Dropbox.Api.Files;
    using Dropbox.Api.Users;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The test class for Dropbox API.
    /// </summary>
    [TestClass]
    public class DropboxApiTests
    {
        private static readonly string TestingPath = "/Testing/Dropbox.Api.Tests";

        /// <summary>
        /// The user access token.
        /// </summary>
        private static string userAccessToken;

        /// <summary>
        /// The user refresh token.
        /// </summary>
        private static string userRefreshToken;

        /// <summary>
        /// The app key.
        /// </summary>
        private static string appKey;

        /// <summary>
        /// The app secret.
        /// </summary>
        private static string appSecret;

        /// <summary>
        /// The Dropbox client.
        /// </summary>
        private static DropboxClient client;

        /// <summary>
        /// The Dropbox team client.
        /// </summary>
        private static DropboxTeamClient teamClient;

        /// <summary>
        /// The Dropbox app client.
        /// </summary>
        private static DropboxAppClient appClient;

        /// <summary>
        /// Set up Dropbox clients.
        /// </summary>
        /// <param name="context">The VSTest context.</param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json", optional: true)
                .AddEnvironmentVariables(prefix: "DROPBOX_")
                .Build();

            appKey = config["appKey"];
            appSecret = config["appSecret"];

            userRefreshToken = config["userRefreshToken"];
            userAccessToken = config["userAccessToken"];
            client = new DropboxClient(userAccessToken);

            var teamToken = config["teamAccessToken"];
            teamClient = new DropboxTeamClient(teamToken);

            appClient = new DropboxAppClient(appKey, appSecret);
        }

        /// <summary>
        /// Ensure that the rest folder exissts and is empty before test execution.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [TestInitialize]
        public async Task Initialize()
        {
            try
            {
                var result = await client.Files.ListFolderAsync(TestingPath);
                Assert.AreEqual(0, result.Entries.Count);
            }
            catch (ApiException<ListFolderError>)
            {
                // create folder if it doesn't exist
                var result = client.Files.CreateFolderV2Async(TestingPath).Result;
                Assert.AreEqual(TestingPath, result.Metadata.PathDisplay);
            }
        }

        /// <summary>
        /// Cleans up created files after test execution.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var result = client.Files.ListFolderAsync(TestingPath).Result;

            foreach (var entry in result.Entries)
            {
                client.Files.DeleteV2Async(entry.PathLower).Wait();
            }
        }

        /// <summary>
        /// Tests creating a client with only refresh token and
        /// ensuring the client refreshed the token before making a call.
        /// </summary>
        /// <returns>The <see cref="Task" />.</returns>
        [TestMethod]
        public async Task TestRefreshClient()
        {
            var client = new DropboxClient(userRefreshToken, appKey, appSecret);
            var result = await client.Users.GetCurrentAccountAsync();
            Assert.IsNotNull(result.Email);
        }

        /// <summary>
        /// Test get metadata.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestGetMetadata()
        {
            await client.Files.UploadAsync(TestingPath + "/Foo.txt", body: GetStream("abc"));
            var metadata = await client.Files.GetMetadataAsync(TestingPath + "/Foo.txt");
            Assert.AreEqual("Foo.txt", metadata.Name);
            Assert.AreEqual(TestingPath.ToLower() + "/foo.txt", metadata.PathLower);
            Assert.AreEqual(TestingPath + "/Foo.txt", metadata.PathDisplay);
            Assert.IsTrue(metadata.IsFile);

            var file = metadata.AsFile;
            Assert.AreEqual(3, (int)file.Size);
        }

        /// <summary>
        /// Test get metadata.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestListFolder()
        {
            var files = new HashSet<string> { "a.txt", "b.txt", "c.txt" };
            foreach (var file in files)
            {
                await client.Files.UploadAsync(TestingPath + "/" + file, body: GetStream("abc"));
            }

            var response = await client.Files.ListFolderAsync(TestingPath);
            Assert.AreEqual(files.Count, response.Entries.Count);
            foreach (var entry in response.Entries)
            {
                Assert.IsTrue(files.Contains(entry.Name));
                Assert.IsTrue(entry.IsFile);
                var file = entry.AsFile;
                Assert.AreEqual(3, (int)file.Size);
            }
        }

        /// <summary>
        /// Test upload.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestUpload()
        {
            var response = await client.Files.UploadAsync(TestingPath + "/Foo.txt", body: GetStream("abc"));
            Assert.AreEqual(response.Name, "Foo.txt");
            Assert.AreEqual(response.PathLower, TestingPath.ToLower() + "/foo.txt");
            Assert.AreEqual(response.PathDisplay, TestingPath + "/Foo.txt");
            var downloadResponse = await client.Files.DownloadAsync(TestingPath + "/Foo.txt");
            var content = await downloadResponse.GetContentAsStringAsync();
            Assert.AreEqual("abc", content);
        }

        /// <summary>
        /// Test upload with retry.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestUploadRetry()
        {
            var count = 0;

            var mockHandler = new MockHttpMessageHandler((r, s) =>
            {
                if (count++ < 2)
                {
                    var error = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("Error"),
                    };

                    return Task.FromResult(error);
                }

                return s(r);
            });

            var mockClient = new HttpClient(mockHandler);
            var client = new DropboxClient(
                userAccessToken,
                new DropboxClientConfig { HttpClient = mockClient, MaxRetriesOnError = 10 });

            var response = await client.Files.UploadAsync(TestingPath + "/Foo.txt", body: GetStream("abc"));
            var downloadResponse = await DropboxApiTests.client.Files.DownloadAsync(TestingPath + "/Foo.txt");
            var content = await downloadResponse.GetContentAsStringAsync();
            Assert.AreEqual("abc", content);
        }

        /// <summary>
        /// Test upload.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestDownload()
        {
            await client.Files.UploadAsync(TestingPath + "/Foo.txt", body: GetStream("abc"));
            var downloadResponse = await client.Files.DownloadAsync(TestingPath + "/Foo.txt");
            var content = await downloadResponse.GetContentAsStringAsync();
            Assert.AreEqual("abc", content);
            var response = downloadResponse.Response;
            Assert.AreEqual(response.Name, "Foo.txt");
            Assert.AreEqual(response.PathLower, TestingPath.ToLower() + "/foo.txt");
            Assert.AreEqual(response.PathDisplay, TestingPath + "/Foo.txt");
        }

        /// <summary>
        /// Test rate limit error handling.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [TestMethod]
        public async Task TestRateLimit()
        {
            var body = "{\"error_summary\": \"too_many_requests/..\", \"error\": {\"reason\": {\".tag\": \"too_many_requests\"}, \"retry_after\": 100}}";
            var mockResponse = new HttpResponseMessage((HttpStatusCode)429)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            mockResponse.Headers.Add("X-Dropbox-Request-Id", "123");

            var mockHandler = new MockHttpMessageHandler((r, s) => Task.FromResult(mockResponse));
            var mockClient = new HttpClient(mockHandler);
            var client = new DropboxClient("dummy", new DropboxClientConfig { HttpClient = mockClient });
            try
            {
                await client.Files.GetMetadataAsync(TestingPath + "/a.txt");
            }
            catch (RateLimitException ex)
            {
                Assert.AreEqual((int)ex.ErrorResponse.RetryAfter, 100);
                Assert.AreEqual(ex.RetryAfter, 100);
                Assert.IsTrue(ex.ErrorResponse.Reason.IsTooManyRequests);
            }
        }

        /// <summary>
        /// Test request id handling.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestRequestId()
        {
            var funcs = new List<Func<Task>>
            {
                () => client.Files.GetMetadataAsync("/noob"), // 409
                () => client.Files.GetMetadataAsync("/"), // 400
            };

            foreach (var func in funcs)
            {
                try
                {
                    await func();
                }
                catch (DropboxException ex)
                {
                    Assert.IsTrue(ex.ToString().Contains("Request Id"));
                }
            }
        }

        /// <summary>
        /// Test team auth.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestTeamAuth()
        {
            var result = await teamClient.Team.GetInfoAsync();
            Assert.IsNotNull(result.TeamId);
            Assert.IsNotNull(result.Name);
        }

        /// <summary>
        /// Test team auth select user.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestTeamAuthSelectUser()
        {
            var result = await teamClient.Team.MembersListAsync();
            var memberId = result.Members[0].Profile.TeamMemberId;

            var userClient = teamClient.AsMember(memberId);
            var account = await userClient.Users.GetCurrentAccountAsync();
            Assert.AreEqual(account.TeamMemberId, memberId);
        }

        /// <summary>
        /// Test team auth select admin.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestTeamAuthSelectAdmin()
        {
            var result = await teamClient.Team.MembersListAsync();
            var adminId = result.Members.Where(m => m.Role.IsTeamAdmin).First().Profile.TeamMemberId;

            var userClient = teamClient.AsAdmin(adminId);
            var account = await userClient.Users.GetCurrentAccountAsync();
            Assert.AreEqual(account.TeamMemberId, adminId);

            // TODO: Add permission specific tests.
        }

        /// <summary>
        /// Test team auth select admin.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestPathRoot()
        {
            await client.Files.UploadAsync(TestingPath + "/Foo.txt", body: GetStream("abc"));

            var pathRootClient = client.WithPathRoot(PathRoot.Home.Instance);
            var metadata = await pathRootClient.Files.GetMetadataAsync(TestingPath + "/Foo.txt");
            Assert.AreEqual(TestingPath.ToLower() + "/foo.txt", metadata.PathLower);
            pathRootClient = client.WithPathRoot(new PathRoot.Root("123"));

            var exceptionRaised = false;

            try
            {
                await pathRootClient.Files.GetMetadataAsync(TestingPath + "/Foo.txt");
            }
            catch (PathRootException e)
            {
                exceptionRaised = true;
                var error = e.ErrorResponse;
                Assert.IsTrue(error.IsInvalidRoot);
            }

            Assert.IsTrue(exceptionRaised);
        }

        /// <summary>
        /// Test app auth.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestAppAuth()
        {
            try
            {
                var result = await appClient.Auth.TokenFromOauth1Async("foo", "bar");
            }
            catch (ApiException<TokenFromOAuth1Error>)
            {
            }
        }

        /// <summary>
        /// Test no auth.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestNoAuth()
        {
            var result = await client.Files.ListFolderAsync(string.Empty, recursive: true);
            var cursor = result.Cursor;

            var task = client.Files.ListFolderLongpollAsync(cursor);
            await client.Files.UploadAsync(TestingPath + "/foo.txt", body: GetStream("abc"));
            var response = await task;
            Assert.IsTrue(response.Changes);
        }

        /// <summary>
        /// Test APM flow.
        /// </summary>
        [TestMethod]
        public void TaskAPM()
        {
            var result = client.Users.BeginGetCurrentAccount(null);
            var account = client.Users.EndGetCurrentAccount(result);
            var accountId = account.AccountId;

            result = client.Users.BeginGetAccountBatch(new string[] { accountId }, null);
            var accounts = client.Users.EndGetAccountBatch(result);

            Assert.AreEqual(accounts.Count, 1);
            Assert.AreEqual(accounts[0].AccountId, accountId);
        }

        /// <summary>
        /// Test User-Agent header is set with default values.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestUserAgentDefault()
        {
            HttpRequestMessage lastRequest = null;
            var mockHandler = new MockHttpMessageHandler((r, s) =>
            {
                lastRequest = r;
                return s(r);
            });

            var mockClient = new HttpClient(mockHandler);
            var client = new DropboxClient(userAccessToken, new DropboxClientConfig { HttpClient = mockClient });
            await client.Users.GetCurrentAccountAsync();
            Assert.IsTrue(lastRequest.Headers.UserAgent.ToString().Contains("OfficialDropboxDotNetSDKv2"));
        }

        /// <summary>
        /// Test User-Agent header is populated with user supplied value in DropboxClientConfig.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestUserAgentUserSupplied()
        {
            HttpRequestMessage lastRequest = null;
            var mockHandler = new MockHttpMessageHandler((r, s) =>
            {
                lastRequest = r;
                return s(r);
            });

            var mockClient = new HttpClient(mockHandler);
            var userAgent = "UserAgentTest";
            var client = new DropboxClient(userAccessToken, new DropboxClientConfig { HttpClient = mockClient, UserAgent = userAgent });
            await client.Users.GetCurrentAccountAsync();
            Assert.IsTrue(lastRequest.Headers.UserAgent.ToString().Contains(userAgent));
        }

        /// <summary>
        /// Test cancel request of Dispose DropboxClient.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task TestDropboxClientDispose()
        {
            var canceled = false;
            Task<FullAccount> task;
            using (var client = new DropboxClient(userAccessToken))
            {
                task = client.Users.GetCurrentAccountAsync();
            }

            try
            {
                await task;
            }
            catch (TaskCanceledException)
            {
                canceled = true;
            }

            Assert.IsTrue(canceled);
        }

        /// <summary>
        /// Test upload with a date-time format file name.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task TestUploadWithDateName()
        {
            var fileNameWithDateFormat = DateTime.Now.ToString("s");
            var response = await client.Files.UploadAsync($"{TestingPath}/{fileNameWithDateFormat}", body: GetStream("abc"));
            Assert.AreEqual(response.Name, fileNameWithDateFormat);
            Assert.AreEqual(response.PathLower, $"{TestingPath.ToLower()}/{fileNameWithDateFormat.ToLowerInvariant()}");
            Assert.AreEqual(response.PathDisplay, $"{TestingPath}/{fileNameWithDateFormat}");
            var downloadResponse = await client.Files.DownloadAsync($"{TestingPath}/{fileNameWithDateFormat}");
            var content = await downloadResponse.GetContentAsStringAsync();
            Assert.AreEqual("abc", content);
        }

        /// <summary>
        /// Test folder creation with a date-time format folder name.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task TestCreateFolderWithDateFormat()
        {
            var folderNameWithDateFormat = DateTime.Now.ToString("s");
            var response = await client.Files.CreateFolderAsync($"{TestingPath}/{folderNameWithDateFormat}");
            Assert.AreEqual(response.Name, folderNameWithDateFormat);
            Assert.AreEqual(response.PathLower, $"{TestingPath.ToLower()}/{folderNameWithDateFormat.ToLowerInvariant()}");
            Assert.AreEqual(response.PathDisplay, $"{TestingPath}/{folderNameWithDateFormat}");
            var folders = await client.Files.ListFolderAsync($"/{TestingPath}");
            Assert.IsTrue(folders.Entries.Any(f => f.Name == folderNameWithDateFormat));
        }

        /// <summary>
        /// Converts string to a memory stream.
        /// </summary>
        /// <param name="content">The string content.</param>
        /// <returns>The memory stream.</returns>
        private static MemoryStream GetStream(string content)
        {
            var buffer = Encoding.UTF8.GetBytes(content);
            return new MemoryStream(buffer);
        }
    }
}
