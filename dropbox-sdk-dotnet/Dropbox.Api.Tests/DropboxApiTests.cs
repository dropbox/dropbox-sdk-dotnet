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

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Dropbox.Api.Auth;
    using Dropbox.Api.Common;
    using Dropbox.Api.Users;
    using Dropbox.Api.Files;

    /// <summary>
    /// The test class for Dropbox API.
    /// </summary>
    [TestClass]
    public class DropboxApiTests
    {
        /// <summary>
        /// The user access token.
        /// </summary>
        public static string UserAccessToken;

        /// <summary>
        /// The user refresh token.
        /// </summary>
        public static string UserRefreshToken;

        /// <summary>
        /// The app key
        /// </summary>
        public static string AppKey;

        /// <summary>
        /// The app secret
        /// </summary>
        public static string AppSecret;

        /// <summary>
        /// The Dropbox client.
        /// </summary>
        public static DropboxClient Client;

        /// <summary>
        /// The Dropbox team client.
        /// </summary>
        public static DropboxTeamClient TeamClient;

        /// <summary>
        /// The Dropbox app client.
        /// </summary>
        public static DropboxAppClient AppClient;

        private readonly static string TestingPath = "/Testing/Dropbox.Api.Tests";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            
            AppKey = context.Properties["appKey"].ToString();
            AppSecret = context.Properties["appSecret"].ToString();
            
            UserRefreshToken = context.Properties["userRefreshToken"].ToString();
            UserAccessToken = context.Properties["userAccessToken"].ToString();
            Client = new DropboxClient(UserAccessToken);

            var teamToken = context.Properties["teamAccessToken"].ToString();
            TeamClient = new DropboxTeamClient(teamToken);
            
            AppClient = new DropboxAppClient(AppKey, AppSecret);
        }

        [TestInitialize]
        public async void Initialize()
        {
            try
            {
                var result = await Client.Files.ListFolderAsync(TestingPath);
                Assert.AreEqual(0, result.Entries.Count);
            } catch (ApiException<ListFolderError>)
            {
                // create folder if it doesn't exist
                var result = Client.Files.CreateFolderV2Async(TestingPath).Result;
                Assert.AreEqual(TestingPath, result.Metadata.PathDisplay);
            }
        }


        [TestCleanup]
        public void Cleanup()
        {
            var result = Client.Files.ListFolderAsync(TestingPath).Result;

            foreach (var entry in result.Entries) {
                Client.Files.DeleteV2Async(entry.PathLower).Wait();
            }
        }

        /// <summary>
        /// Tests creating a client with only refresh token and
        /// ensuring the client refreshed the token before making a call
        /// </summary>
        /// <returns>The <see cref="Task" /></returns>
        [TestMethod]
        public async Task TestRefreshClient()
        {
            var client = new DropboxClient(UserRefreshToken, AppKey, AppSecret);
            var result = await client.Users.GetCurrentAccountAsync();
            Assert.IsNotNull(result.Email);
        }

        /// <summary>
        /// Test get metadata.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestGetMetadata()
        {
            await Client.Files.UploadAsync(TestingPath + "/Foo.txt", body: GetStream("abc"));
            var metadata = await Client.Files.GetMetadataAsync(TestingPath + "/Foo.txt");
            Assert.AreEqual("Foo.txt", metadata.Name);
            Assert.AreEqual(TestingPath.ToLower() + "/foo.txt", metadata.PathLower);
            Assert.AreEqual(TestingPath  + "/Foo.txt", metadata.PathDisplay);
            Assert.IsTrue(metadata.IsFile);

            var file = metadata.AsFile;
            Assert.AreEqual(3, (int)file.Size);
        }

        /// <summary>
        /// Test get metadata.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestListFolder()
        {
            var files = new HashSet<string> { "a.txt", "b.txt", "c.txt" };
            foreach (var file in files)
            {
                await Client.Files.UploadAsync(TestingPath + "/" + file, body: GetStream("abc"));
            }

            var response = await Client.Files.ListFolderAsync(TestingPath);
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
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestUpload()
        {
            var response = await Client.Files.UploadAsync(TestingPath + "/Foo.txt", body: GetStream("abc"));
            Assert.AreEqual(response.Name, "Foo.txt");
            Assert.AreEqual(response.PathLower, TestingPath.ToLower() + "/foo.txt");
            Assert.AreEqual(response.PathDisplay, TestingPath + "/Foo.txt");
            var downloadResponse = await Client.Files.DownloadAsync(TestingPath + "/Foo.txt");
            var content = await downloadResponse.GetContentAsStringAsync();
            Assert.AreEqual("abc", content);
        }

        /// <summary>
        /// Test upload with retry.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
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
                        Content = new StringContent("Error")
                    };

                    return Task.FromResult(error);
                }

                return s(r);
            });

            var mockClient = new HttpClient(mockHandler);
            var client = new DropboxClient(
                UserAccessToken,
                new DropboxClientConfig { HttpClient = mockClient, MaxRetriesOnError = 10 });

            var response = await client.Files.UploadAsync(TestingPath + "/Foo.txt", body: GetStream("abc"));
            var downloadResponse = await Client.Files.DownloadAsync(TestingPath + "/Foo.txt");
            var content = await downloadResponse.GetContentAsStringAsync();
            Assert.AreEqual("abc", content);
        }


        /// <summary>
        /// Test upload.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestDownload()
        {
            await Client.Files.UploadAsync(TestingPath + "/Foo.txt", body: GetStream("abc"));
            var downloadResponse = await Client.Files.DownloadAsync(TestingPath + "/Foo.txt");
            var content = await downloadResponse.GetContentAsStringAsync();
            Assert.AreEqual("abc", content);
            var response = downloadResponse.Response;
            Assert.AreEqual(response.Name, "Foo.txt");
            Assert.AreEqual(response.PathLower, TestingPath.ToLower() + "/foo.txt");
            Assert.AreEqual(response.PathDisplay, TestingPath + "/Foo.txt");
        }

        /// Test rate limit error handling.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestRateLimit()
        {
            var body = "{\"error_summary\": \"too_many_requests/..\", \"error\": {\"reason\": {\".tag\": \"too_many_requests\"}, \"retry_after\": 100}}";
            var mockResponse = new HttpResponseMessage((HttpStatusCode)429)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
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

        /// Test request id handling.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestRequestId()
        {
            var funcs = new List<Func<Task>>
            {
                () => Client.Files.GetMetadataAsync("/noob"), // 409
                () => Client.Files.GetMetadataAsync("/"), // 400
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

        /// Test team auth.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestTeamAuth()
        {
            var result = await TeamClient.Team.GetInfoAsync();
            Assert.IsNotNull(result.TeamId);
            Assert.IsNotNull(result.Name);
        }

        /// Test team auth select user.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestTeamAuthSelectUser()
        {
            var result = await TeamClient.Team.MembersListAsync();
            var memberId = result.Members[0].Profile.TeamMemberId;

            var userClient = TeamClient.AsMember(memberId);
            var account = await userClient.Users.GetCurrentAccountAsync();
            Assert.AreEqual(account.TeamMemberId, memberId);
        }


        /// Test team auth select admin.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestTeamAuthSelectAdmin()
        {
            var result = await TeamClient.Team.MembersListAsync();
            var adminId = result.Members.Where(m => m.Role.IsTeamAdmin).First().Profile.TeamMemberId;

            var userClient = TeamClient.AsAdmin(adminId);
            var account = await userClient.Users.GetCurrentAccountAsync();
            Assert.AreEqual(account.TeamMemberId, adminId);

            // TODO: Add permission specific tests.
        }

        /// Test team auth select admin.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestPathRoot()
        {
            await Client.Files.UploadAsync(TestingPath + "/Foo.txt", body: GetStream("abc"));

            var pathRootClient = Client.WithPathRoot(PathRoot.Home.Instance);
            var metadata = await pathRootClient.Files.GetMetadataAsync(TestingPath + "/Foo.txt");
            Assert.AreEqual(TestingPath.ToLower() + "/foo.txt", metadata.PathLower);
            pathRootClient = Client.WithPathRoot(new PathRoot.Root("123"));

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

        /// Test app auth.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestAppAuth()
        {
            try
            {
                var result = await AppClient.Auth.TokenFromOauth1Async("foo", "bar");
            }
            catch (ApiException<TokenFromOAuth1Error>)
            {
            }
        }

        /// Test no auth.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestNoAuth()
        {
            var result = await Client.Files.ListFolderAsync("", recursive: true);
            var cursor = result.Cursor;

            var task = Client.Files.ListFolderLongpollAsync(cursor);
            await Client.Files.UploadAsync(TestingPath + "/foo.txt", body: GetStream("abc"));
            var response = await task;
            Assert.IsTrue(response.Changes);
        }

        /// Test APM flow.
        /// </summary>
        [TestMethod]
        public void TaskAPM()
        {
            var result = Client.Users.BeginGetCurrentAccount(null);
            var account = Client.Users.EndGetCurrentAccount(result);
            var accountId = account.AccountId;

            result = Client.Users.BeginGetAccountBatch(new string[] { accountId }, null);
            var accounts = Client.Users.EndGetAccountBatch(result);

            Assert.AreEqual(accounts.Count, 1);
            Assert.AreEqual(accounts[0].AccountId, accountId);
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
        
        /// Test User-Agent header is set with default values.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
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
            var client = new DropboxClient(UserAccessToken, new DropboxClientConfig { HttpClient = mockClient });
            await client.Users.GetCurrentAccountAsync();
            Assert.IsTrue(lastRequest.Headers.UserAgent.ToString().Contains("OfficialDropboxDotNetSDKv2"));
        }

        /// Test User-Agent header is populated with user supplied value in DropboxClientConfig.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
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
            var client = new DropboxClient(UserAccessToken, new DropboxClientConfig { HttpClient = mockClient, UserAgent = userAgent });
            await client.Users.GetCurrentAccountAsync();
            Assert.IsTrue(lastRequest.Headers.UserAgent.ToString().Contains(userAgent));
        }

        /// <summary>
        /// Test cancel request of Dispose DropboxClient
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDropboxClientDispose()
        {
            var canceled = false;
            Task<FullAccount> task;
            using (var client = new DropboxClient(UserAccessToken))
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
    }
}
