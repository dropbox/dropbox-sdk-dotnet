//-----------------------------------------------------------------------------
// <copyright file="DropboxClientTokenTests.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Unit.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dropbox.Api.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests token validation and normalization for Dropbox clients.
    /// </summary>
    [TestClass]
    public class DropboxClientTokenTests
    {
        /// <summary>
        /// Verifies an empty access token is rejected by the user client.
        /// </summary>
        [TestMethod]
        public void TestDropboxClientRejectsEmptyAccessToken()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CreateDropboxClientWithAccessToken(string.Empty));
        }

        /// <summary>
        /// Verifies an empty refresh token is rejected by the user client.
        /// </summary>
        [TestMethod]
        public void TestDropboxClientRejectsEmptyRefreshToken()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CreateDropboxClientWithRefreshToken(string.Empty));
        }

        /// <summary>
        /// Verifies an empty access token is rejected by the team client.
        /// </summary>
        [TestMethod]
        public void TestDropboxTeamClientRejectsEmptyAccessToken()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CreateDropboxTeamClientWithAccessToken(string.Empty));
        }

        /// <summary>
        /// Verifies an empty refresh token is rejected by the team client.
        /// </summary>
        [TestMethod]
        public void TestDropboxTeamClientRejectsEmptyRefreshToken()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CreateDropboxTeamClientWithRefreshToken(string.Empty));
        }

        /// <summary>
        /// Verifies empty token strings are normalized to missing tokens.
        /// </summary>
        [TestMethod]
        public void TestRequestHandlerOptionsNormalizeEmptyTokens()
        {
            var emptyAccessTokenOptions = new DropboxRequestHandlerOptions(
                new DropboxClientConfig(),
                string.Empty,
                "refresh-token",
                null,
                "app-key",
                null);
            var emptyRefreshTokenOptions = new DropboxRequestHandlerOptions(
                new DropboxClientConfig(),
                "access-token",
                string.Empty,
                null,
                "app-key",
                null);

            Assert.IsNull(emptyAccessTokenOptions.OAuth2AccessToken);
            Assert.AreEqual("refresh-token", emptyAccessTokenOptions.OAuth2RefreshToken);
            Assert.AreEqual("access-token", emptyRefreshTokenOptions.OAuth2AccessToken);
            Assert.IsNull(emptyRefreshTokenOptions.OAuth2RefreshToken);
        }

        /// <summary>
        /// Verifies expiration conversion for every legacy DateTime kind without HTTP.
        /// </summary>
        [TestMethod]
        public void TestSnapshotExpirationKinds()
        {
            foreach (var kind in new[] { DateTimeKind.Utc, DateTimeKind.Local, DateTimeKind.Unspecified })
            {
                var expiration = new DateTime(2020, 1, 2, 3, 4, 5, kind);
                using (var http = new HttpClient(new SnapshotHandler(request => throw new AssertFailedException("Unexpected HTTP request"))))
                using (var client = new DropboxClient("cached-secret", "refresh-token", expiration, "app-key", new DropboxClientConfig { HttpClient = http }))
                {
                    var snapshot = client.GetAccessTokenSnapshot();
                    Assert.AreEqual("cached-secret", snapshot.AccessToken);
                    Assert.AreEqual(new DateTimeOffset(expiration.ToUniversalTime()), snapshot.ExpiresAt);
                    Assert.AreEqual(TimeSpan.Zero, snapshot.ExpiresAt.Value.Offset);
                    Assert.IsFalse(snapshot.ToString().Contains("cached-secret"));
                }
            }
        }

        /// <summary>
        /// Verifies missing tokens and unknown expiration on both client types.
        /// </summary>
        [TestMethod]
        public void TestSnapshotMissingTokenAndExpiration()
        {
            using (var user = new DropboxClient("refresh-token", "app-key"))
            using (var team = new DropboxTeamClient("refresh-token", "app-key"))
            using (var cachedUser = new DropboxClient("cached-token"))
            using (var cachedTeam = new DropboxTeamClient("cached-token"))
            {
                Assert.IsNull(user.GetAccessTokenSnapshot());
                Assert.IsNull(team.GetAccessTokenSnapshot());
                Assert.AreEqual("cached-token", cachedUser.GetAccessTokenSnapshot().AccessToken);
                Assert.IsNull(cachedUser.GetAccessTokenSnapshot().ExpiresAt);
                Assert.AreEqual("cached-token", cachedTeam.GetAccessTokenSnapshot().AccessToken);
                Assert.IsNull(cachedTeam.GetAccessTokenSnapshot().ExpiresAt);
            }
        }

        /// <summary>
        /// Verifies explicit refresh publishes to team, member, admin and path-root clients.
        /// Snapshots remain readable during an in-flight refresh and retain their old values.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestSnapshotDuringSharedRefresh()
        {
            var entered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var response = new TaskCompletionSource<HttpResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
            using (var http = new HttpClient(new SnapshotHandler(request =>
            {
                entered.SetResult(true);
                return response.Task;
            })))
            using (var team = new DropboxTeamClient("old-token", "refresh-token", DateTime.Now.AddHours(2), "app-key", new DropboxClientConfig { HttpClient = http }))
            using (var member = team.AsMember("member-id"))
            using (var admin = team.AsAdmin("admin-id"))
            using (var rooted = member.WithPathRoot(new PathRoot.Root("namespace-id")))
            {
                var before = team.GetAccessTokenSnapshot();
                var refresh = rooted.RefreshAccessToken(null);
                await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
                var earliest = DateTimeOffset.UtcNow;
                try
                {
                    foreach (var client in new[] { member, admin, rooted })
                    {
                        Assert.AreEqual(before.AccessToken, client.GetAccessTokenSnapshot().AccessToken);
                        Assert.AreEqual(before.ExpiresAt, client.GetAccessTokenSnapshot().ExpiresAt);
                    }
                }
                finally
                {
                    response.SetResult(TokenResponse());
                }

                Assert.IsTrue(await refresh);
                var after = team.GetAccessTokenSnapshot();
                Assert.AreEqual("new-token", after.AccessToken);
                Assert.IsTrue(after.ExpiresAt >= earliest.AddSeconds(3600));
                Assert.IsTrue(after.ExpiresAt <= DateTimeOffset.UtcNow.AddSeconds(3600));
                foreach (var client in new[] { member, admin, rooted })
                {
                    Assert.AreEqual(after.AccessToken, client.GetAccessTokenSnapshot().AccessToken);
                    Assert.AreEqual(after.ExpiresAt, client.GetAccessTokenSnapshot().ExpiresAt);
                }

                Assert.AreEqual("old-token", before.AccessToken);
                Assert.AreNotEqual(before.ExpiresAt, after.ExpiresAt);
            }
        }

        /// <summary>
        /// Verifies overlapping refreshes publish complete pairs in completion order.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestConcurrentRefreshSnapshots()
        {
            var responses = new[]
            {
                new TaskCompletionSource<HttpResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously),
                new TaskCompletionSource<HttpResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously),
            };
            int requests = 0;
            using (var http = new HttpClient(new SnapshotHandler(request => responses[Interlocked.Increment(ref requests) - 1].Task)))
            using (var client = new DropboxClient("refresh-token", "app-key", new DropboxClientConfig { HttpClient = http }))
            using (var rooted = client.WithPathRoot(new PathRoot.Root("namespace-id")))
            {
                var firstRefresh = client.RefreshAccessToken(null);
                var secondRefresh = rooted.RefreshAccessToken(null);
                Assert.AreEqual(2, requests);
                Assert.IsNull(client.GetAccessTokenSnapshot());

                responses[1].SetResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"access_token\":\"second-token\",\"expires_in\":7200}"),
                });
                Assert.IsTrue(await secondRefresh);
                var second = client.GetAccessTokenSnapshot();
                Assert.AreEqual("second-token", second.AccessToken);
                Assert.IsTrue(second.ExpiresAt > DateTimeOffset.UtcNow.AddMinutes(119));

                responses[0].SetResult(TokenResponse());
                Assert.IsTrue(await firstRefresh);
                var first = rooted.GetAccessTokenSnapshot();
                Assert.AreEqual("new-token", first.AccessToken);
                Assert.IsTrue(first.ExpiresAt < second.ExpiresAt);
                Assert.AreEqual(first.ExpiresAt, client.GetAccessTokenSnapshot().ExpiresAt);
                Assert.AreEqual("second-token", second.AccessToken);
                Assert.IsTrue(second.ExpiresAt > first.ExpiresAt);
            }
        }

        /// <summary>
        /// Verifies a snapshot waits for publication under the shared credentials lock.
        /// </summary>
        [TestMethod]
        public void TestSnapshotWaitsForSharedPublicationLock()
        {
            var oldExpiration = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Local);
            var newExpiration = oldExpiration.AddHours(1);
            var options = new DropboxRequestHandlerOptions(
                new DropboxClientConfig(), "old-token", "refresh-token", oldExpiration, "app-key", null);
            using (var client = new DropboxClient(options))
            using (var rooted = client.WithPathRoot(new PathRoot.Root("namespace-id")))
            {
                AccessTokenSnapshot snapshot = null;
                AssertWaitsForAuthStateLock(
                    options,
                    () => snapshot = rooted.GetAccessTokenSnapshot(),
                    () => options.UpdateAccessToken("new-token", newExpiration));

                Assert.AreEqual("new-token", snapshot.AccessToken);
                Assert.AreEqual(new DateTimeOffset(newExpiration.ToUniversalTime()), snapshot.ExpiresAt);
            }
        }

        /// <summary>
        /// Verifies token publication waits for the same lock used to read snapshots.
        /// </summary>
        [TestMethod]
        public void TestPublicationWaitsForSharedSnapshotLock()
        {
            var oldExpiration = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Local);
            var newExpiration = oldExpiration.AddHours(1);
            var options = new DropboxRequestHandlerOptions(
                new DropboxClientConfig(), "old-token", "refresh-token", oldExpiration, "app-key", null);
            AssertWaitsForAuthStateLock(
                options,
                () => options.UpdateAccessToken("new-token", newExpiration),
                () =>
                {
                    var snapshot = options.GetAccessTokenSnapshot();
                    Assert.AreEqual("old-token", snapshot.AccessToken);
                    Assert.AreEqual(new DateTimeOffset(oldExpiration.ToUniversalTime()), snapshot.ExpiresAt);
                });

            var after = options.GetAccessTokenSnapshot();
            Assert.AreEqual("new-token", after.AccessToken);
            Assert.AreEqual(new DateTimeOffset(newExpiration.ToUniversalTime()), after.ExpiresAt);
        }

        /// <summary>
        /// Verifies automatic refresh and reuse of its snapshot through an existing constructor.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestAutomaticRefreshSnapshotAndReuse()
        {
            int refreshCount = 0;
            int apiCount = 0;
            using (var http = new HttpClient(new SnapshotHandler(request =>
            {
                if (request.RequestUri.AbsolutePath == "/oauth2/token")
                {
                    refreshCount++;
                    return Task.FromResult(TokenResponse());
                }

                apiCount++;
                Assert.AreEqual("new-token", request.Headers.Authorization.Parameter);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"used\":0,\"allocation\":{\".tag\":\"individual\",\"allocated\":100}}"),
                });
            })))
            {
                var config = new DropboxClientConfig { HttpClient = http };
                using (var client = new DropboxClient("refresh-token", "app-key", config))
                {
                    Assert.IsNull(client.GetAccessTokenSnapshot());
                    await client.Users.GetSpaceUsageAsync();
                    var snapshot = client.GetAccessTokenSnapshot();
                    Assert.AreEqual("new-token", snapshot.AccessToken);
                    Assert.IsNotNull(snapshot.ExpiresAt);
                    using (var next = new DropboxClient(snapshot.AccessToken, "refresh-token", snapshot.ExpiresAt.Value.LocalDateTime, "app-key", config))
                    {
                        await next.Users.GetSpaceUsageAsync();
                        Assert.AreEqual(snapshot.ExpiresAt, next.GetAccessTokenSnapshot().ExpiresAt);
                    }
                }

                Assert.AreEqual(1, refreshCount);
                Assert.AreEqual(2, apiCount);
            }
        }

        /// <summary>
        /// Verifies HTTP failures and invalid successful responses preserve the whole cached pair.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestFailedRefreshPreservesSnapshot()
        {
            foreach (var status in new[] { HttpStatusCode.BadRequest, HttpStatusCode.OK })
            {
                using (var http = new HttpClient(new SnapshotHandler(request => Task.FromResult(new HttpResponseMessage(status)
                {
                    Content = new StringContent(status == HttpStatusCode.OK
                        ? "{\"access_token\":\"partial-token\",\"expires_in\":\"invalid\"}"
                        : "{\"error\":\"invalid_grant\"}"),
                }))))
                using (var client = new DropboxClient("old-token", "refresh-token", DateTime.Now.AddHours(-1), "app-key", new DropboxClientConfig { HttpClient = http }))
                {
                    var before = client.GetAccessTokenSnapshot();
                    Exception failure = null;
                    try
                    {
                        await client.RefreshAccessToken(null);
                    }
                    catch (Exception exception)
                    {
                        failure = exception;
                    }

                    Assert.IsNotNull(failure);
                    Assert.AreEqual(before.AccessToken, client.GetAccessTokenSnapshot().AccessToken);
                    Assert.AreEqual(before.ExpiresAt, client.GetAccessTokenSnapshot().ExpiresAt);
                }
            }

            using (var client = new DropboxClient("cached-token"))
            {
                Assert.IsFalse(await client.RefreshAccessToken(null));
                Assert.AreEqual("cached-token", client.GetAccessTokenSnapshot().AccessToken);
                Assert.IsNull(client.GetAccessTokenSnapshot().ExpiresAt);
            }
        }

        private static void AssertWaitsForAuthStateLock(
            DropboxRequestHandlerOptions options,
            Action operation,
            Action whileBlocked)
        {
            Exception failure = null;
            var worker = new Thread(() =>
            {
                try
                {
                    operation();
                }
                catch (Exception exception)
                {
                    failure = exception;
                }
            })
            {
                IsBackground = true,
            };

            try
            {
                lock (options.AuthStateLock)
                {
                    worker.Start();

                    // This dedicated thread has no other blocking operations. Observe it
                    // parked on the monitor or finished, rather than guessing with a delay.
                    // The timeout only bounds a stalled test; it does not select the winner.
                    Assert.IsTrue(
                        SpinWait.SpinUntil(
                            () => (worker.ThreadState & (ThreadState.WaitSleepJoin | ThreadState.Stopped)) != 0,
                            TimeSpan.FromSeconds(10)),
                        "Worker neither blocked on the credentials lock nor completed.");
                    Assert.IsTrue(
                        (worker.ThreadState & ThreadState.WaitSleepJoin) != 0,
                        "Operation completed while another thread owned the credentials lock.");
                    whileBlocked();
                }
            }
            finally
            {
                // Release the monitor before joining, including when an assertion fails.
                Assert.IsTrue(worker.Join(TimeSpan.FromSeconds(10)), "Worker did not complete after releasing the credentials lock.");
            }

            Assert.IsNull(failure, "Worker operation failed: " + failure);
        }

        private static HttpResponseMessage TokenResponse()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"new-token\",\"expires_in\":3600}"),
            };
        }

        private static void CreateDropboxClientWithAccessToken(string token)
        {
            using (var client = new DropboxClient(token))
            {
            }
        }

        private static void CreateDropboxClientWithRefreshToken(string token)
        {
            using (var client = new DropboxClient(token, "app-key"))
            {
            }
        }

        private static void CreateDropboxTeamClientWithAccessToken(string token)
        {
            using (var client = new DropboxTeamClient(token))
            {
            }
        }

        private static void CreateDropboxTeamClientWithRefreshToken(string token)
        {
            using (var client = new DropboxTeamClient(token, "app-key"))
            {
            }
        }

        private sealed class SnapshotHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> send;

            public SnapshotHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> send)
            {
                this.send = send;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return this.send(request);
            }
        }
    }
}
