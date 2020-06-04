//-----------------------------------------------------------------------------
// <copyright file="DropboxClient.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;
    using System.Threading.Tasks;
    using Dropbox.Api.Common;

    /// <summary>
    /// The client which contains endpoints which perform user-level actions.
    /// </summary>
    public sealed partial class DropboxClient : DropboxClientBase
    {
        /// <summary>
        /// The request handler.
        /// </summary>
        private readonly DropboxRequestHandler requestHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2Token">The oauth2 access token for making client requests.</param>
        public DropboxClient(string oauth2Token)
            : this(oauth2Token, null, null, null, new DropboxClientConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2RefreshToken">The oauth2 access token for making client requests.</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        public DropboxClient(string oauth2RefreshToken, string appKey)
            : this(null, oauth2RefreshToken, appKey, null, new DropboxClientConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2RefreshToken">The oauth2 access token for making client requests.</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxClient(string oauth2RefreshToken, string appKey, DropboxClientConfig config)
            : this(null, oauth2RefreshToken, appKey, null, config)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="appSecret">The app secret to be used for refreshing tokens</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxClient(string oauth2RefreshToken, string appKey, string appSecret, DropboxClientConfig config)
            : this(null, oauth2RefreshToken, appKey, appSecret, config)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class. 
        /// </summary>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="appSecret">The app secret to be used for refreshing tokens</param>
        public DropboxClient(string oauth2RefreshToken, string appKey, string appSecret)
            : this(null, oauth2RefreshToken, appKey, appSecret, new DropboxClientConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxClient(string oauth2AccessToken, DropboxClientConfig config)
            : this(oauth2AccessToken, null, null, null, config)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2AccessTokenExpiresAt">The time when the current access token expires, can be null if using long-lived tokens</param>
        public DropboxClient(string oauth2AccessToken, DateTime oauth2AccessTokenExpiresAt)
            : this(oauth2AccessToken, null, oauth2AccessTokenExpiresAt, null, null, new DropboxClientConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2AccessTokenExpiresAt">The time when the current access token expires, can be null if using long-lived tokens</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxClient(string oauth2AccessToken, DateTime oauth2AccessTokenExpiresAt, DropboxClientConfig config)
            : this(oauth2AccessToken, null, oauth2AccessTokenExpiresAt, null, null, config)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="oauth2AccessTokenExpiresAt">The time when the current access token expires, can be null if using long-lived tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="appSecret">The app secret to be used for refreshing tokens</param>
        public DropboxClient(string oauth2AccessToken, string oauth2RefreshToken, DateTime oauth2AccessTokenExpiresAt, string appKey, string appSecret)
            : this(oauth2AccessToken, oauth2RefreshToken, oauth2AccessTokenExpiresAt, appKey, appSecret, new DropboxClientConfig())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="oauth2AccessTokenExpiresAt">The time when the current access token expires, can be null if using long-lived tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxClient(string oauth2AccessToken, string oauth2RefreshToken, DateTime oauth2AccessTokenExpiresAt, string appKey, DropboxClientConfig config)
            : this(new DropboxRequestHandlerOptions(config, oauth2AccessToken, oauth2RefreshToken, oauth2AccessTokenExpiresAt, appKey, null))
        {
            if (oauth2AccessToken == null && oauth2RefreshToken == null)
            {
                throw new ArgumentException("Cannot pass in both null access and refresh token");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="oauth2AccessTokenExpiresAt">The time when the current access token expires, can be null if using long-lived tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        public DropboxClient(string oauth2AccessToken, string oauth2RefreshToken, DateTime oauth2AccessTokenExpiresAt, string appKey)
            : this(oauth2AccessToken, oauth2RefreshToken, oauth2AccessTokenExpiresAt, appKey, null, new DropboxClientConfig())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="oauth2AccessTokenExpiresAt">The time when the current access token expires, can be null if using long-lived tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="appSecret">The app secret to be used for refreshing tokens</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxClient(string oauth2AccessToken, string oauth2RefreshToken, DateTime oauth2AccessTokenExpiresAt, string appKey, string appSecret, DropboxClientConfig config)
            : this(new DropboxRequestHandlerOptions(config, oauth2AccessToken, oauth2RefreshToken, oauth2AccessTokenExpiresAt, appKey, appSecret))
        {
            if (oauth2AccessToken == null && oauth2RefreshToken == null)
            {
                throw new ArgumentException("Cannot pass in both null access and refresh token");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="appSecret">The app secret to be used for refreshing tokens</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxClient(string oauth2AccessToken, string oauth2RefreshToken, string appKey, string appSecret, DropboxClientConfig config)
            : this(new DropboxRequestHandlerOptions(config, oauth2AccessToken, oauth2RefreshToken, null, appKey, appSecret))
        {
            if (oauth2AccessToken == null && oauth2RefreshToken == null)
            {
                throw new ArgumentException("Cannot pass in both null access and refresh token");
            }
        }

        /// <summary>
        /// Set the value for Dropbox-Api-Path-Root header. This allows accessing content outside of user's
        /// home namespace. Below is sample code of accessing content inside team space. See
        /// <a href="https://www.dropbox.com/developers/reference/namespace-guide">Namespace Guide</a> for details
        /// about user space vs team space.
        /// <code>
        /// // Fetch root namespace info from user's account info.
        /// var account = await client.Users.GetCurrentAccountAsync();
        ///
        /// if (!account.RootInfo.IsTeam)
        /// {
        ///     Console.WriteLine("This user doesn't belong to a team with shared space.");
        /// }
        /// else
        /// {
        ///     try
        ///     {
        ///         // Point path root to namespace id of team space.
        ///         client = client.WithPathRoot(new PathRoot.Root(account.RootInfo.RootNamespaceId));
        ///         await client.Files.ListFolderAsync(path);
        ///     }
        ///     catch (PathRootException ex)
        ///     {
        ///         // Handle race condition when user switched team.
        ///         Console.WriteLine(
        ///             "The user's root namespace ID has changed to {0}",
        ///             ex.ErrorResponse.AsInvalidRoot.Value);
        ///     }
        /// }
        /// </code>
        /// </summary>
        /// <param name="pathRoot">The path root object.</param>
        /// <returns>A <see cref="DropboxClient"/> instance with Dropbox-Api-Path-Root header set.</returns>
        public DropboxClient WithPathRoot(PathRoot pathRoot)
        {
            if (pathRoot == null)
            {
                throw new ArgumentNullException("pathRoot");
            }

            return new DropboxClient(this.requestHandler.WithPathRoot(pathRoot));
        }

        /// <summary>
        /// Refreshes access token regardless of if existing token is expired
        /// </summary>
        /// <param name="scopeList">subset of scopes to refresh token with, or null to refresh with all scopes</param>
        /// <returns>true if token is successfully refreshed, false otherwise</returns>
        public Task<bool> RefreshAccessToken(string[] scopeList)
        {
            return this.requestHandler.RefreshAccessToken(scopeList);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="options">The request handler options.</param>
        /// <param name="selectUser">The member id of the selected user. If provided together with
        /// a team access token, actions will be performed on this this user's Dropbox.</param>
        /// <param name="selectAdmin">The member id of the selected admin. If provided together with
        /// a team access token, access is allowed for all team owned contents.</param>
        /// <param name="pathRoot">The path root value used as Dropbox-Api-Path-Root header.</param>
        internal DropboxClient(
            DropboxRequestHandlerOptions options,
            string selectUser = null,
            string selectAdmin = null,
            PathRoot pathRoot = null)
            : this(new DropboxRequestHandler(options, selectUser: selectUser, selectAdmin: selectAdmin, pathRoot: pathRoot))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="requestHandler">The request handler.</param>
        private DropboxClient(DropboxRequestHandler requestHandler): base(requestHandler)
        {
            this.requestHandler = requestHandler;
        }
    }
}
