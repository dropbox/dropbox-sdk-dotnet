//-----------------------------------------------------------------------------
// <copyright file="DropboxClientTokenTests.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Unit.Tests
{
    using System;
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
    }
}
