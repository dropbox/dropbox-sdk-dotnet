//-----------------------------------------------------------------------------
// <copyright file="DropboxTeamClient.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The client which contains endpoints which perform team-level actions.
    /// </summary>
    public sealed partial class DropboxTeamClient : DropboxClientBase
    {
        /// <summary>
        /// The request handler options.
        /// </summary>
        private readonly DropboxRequestHandlerOptions options;

        /// <summary>
        /// The request handler.
        /// </summary>
        private readonly DropboxRequestHandler requestHandler;
       
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2Token">The oauth2 access token for making client requests.</param>
        public DropboxTeamClient(string oauth2Token)
            : this(oauth2Token, null, null, null, new DropboxClientConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2RefreshToken">The oauth2 access token for making client requests.</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        public DropboxTeamClient(string oauth2RefreshToken, string appKey)
            : this(null, oauth2RefreshToken, appKey, null, new DropboxClientConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2RefreshToken">The oauth2 access token for making client requests.</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxTeamClient(string oauth2RefreshToken, string appKey, DropboxClientConfig config)
            : this(null, oauth2RefreshToken, appKey, null, config)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="appSecret">The app secret to be used for refreshing tokens</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxTeamClient(string oauth2RefreshToken, string appKey, string appSecret, DropboxClientConfig config)
            : this(null, oauth2RefreshToken, appKey, appSecret, config)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class. 
        /// </summary>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="appSecret">The app secret to be used for refreshing tokens</param>
        public DropboxTeamClient(string oauth2RefreshToken, string appKey, string appSecret)
            : this(null, oauth2RefreshToken, appKey, appSecret, new DropboxClientConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxTeamClient(string oauth2AccessToken, DropboxClientConfig config)
            : this(oauth2AccessToken, null, null, null, config)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2AccessTokenExpiresAt">The time when the current access token expires, can be null if using long-lived tokens</param>
        public DropboxTeamClient(string oauth2AccessToken, DateTime oauth2AccessTokenExpiresAt)
            : this(oauth2AccessToken, null, oauth2AccessTokenExpiresAt, null, null, new DropboxClientConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2AccessTokenExpiresAt">The time when the current access token expires, can be null if using long-lived tokens</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxTeamClient(string oauth2AccessToken, DateTime oauth2AccessTokenExpiresAt, DropboxClientConfig config)
            : this(oauth2AccessToken, null, oauth2AccessTokenExpiresAt, null, null, config)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="oauth2AccessTokenExpiresAt">The time when the current access token expires, can be null if using long-lived tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="appSecret">The app secret to be used for refreshing tokens</param>
        public DropboxTeamClient(string oauth2AccessToken, string oauth2RefreshToken, DateTime oauth2AccessTokenExpiresAt, string appKey, string appSecret)
            : this(oauth2AccessToken, oauth2RefreshToken, oauth2AccessTokenExpiresAt, appKey, appSecret, new DropboxClientConfig())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="oauth2AccessTokenExpiresAt">The time when the current access token expires, can be null if using long-lived tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxTeamClient(string oauth2AccessToken, string oauth2RefreshToken, DateTime oauth2AccessTokenExpiresAt, string appKey, DropboxClientConfig config)
            : this(new DropboxRequestHandlerOptions(config, oauth2AccessToken, oauth2RefreshToken, oauth2AccessTokenExpiresAt, appKey, null))
        {
            if (oauth2AccessToken == null && oauth2RefreshToken == null)
            {
                throw new ArgumentException("Cannot pass in both null access and refresh token");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="oauth2AccessTokenExpiresAt">The time when the current access token expires, can be null if using long-lived tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        public DropboxTeamClient(string oauth2AccessToken, string oauth2RefreshToken, DateTime oauth2AccessTokenExpiresAt, string appKey)
            : this(oauth2AccessToken, oauth2RefreshToken, oauth2AccessTokenExpiresAt, appKey, null, new DropboxClientConfig())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="oauth2AccessTokenExpiresAt">The time when the current access token expires, can be null if using long-lived tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="appSecret">The app secret to be used for refreshing tokens</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxTeamClient(string oauth2AccessToken, string oauth2RefreshToken, DateTime oauth2AccessTokenExpiresAt, string appKey, string appSecret, DropboxClientConfig config)
            : this(new DropboxRequestHandlerOptions(config, oauth2AccessToken, oauth2RefreshToken, oauth2AccessTokenExpiresAt, appKey, appSecret))
        {
            if (oauth2AccessToken == null && oauth2RefreshToken == null)
            {
                throw new ArgumentException("Cannot pass in both null access and refresh token");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="appSecret">The app secret to be used for refreshing tokens</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxTeamClient(string oauth2AccessToken, string oauth2RefreshToken, string appKey, string appSecret, DropboxClientConfig config)
            : this(new DropboxRequestHandlerOptions(config, oauth2AccessToken, oauth2RefreshToken, null, appKey, appSecret))
        {
            if (oauth2AccessToken == null && oauth2RefreshToken == null)
            {
                throw new ArgumentException("Cannot pass in both null access and refresh token");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="options">The request handler options.</param>
        private DropboxTeamClient(DropboxRequestHandlerOptions options) : this(options, new DropboxRequestHandler(options))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="options">The request handler options.</param>
        /// <param name="requestHandler">The request handler.</param>
        private DropboxTeamClient(DropboxRequestHandlerOptions options, DropboxRequestHandler requestHandler) : base(requestHandler)
        {
            this.options = options;
            this.requestHandler = requestHandler;
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
        /// Convert the team client to a user client which can perform action on the given team member's Dropbox.
        /// </summary>
        /// <param name="memberId">The member id of a user who is in the team.</param>
        /// <returns>The <see cref="DropboxClient"/></returns>
        public DropboxClient AsMember(string memberId)
        {
            return new DropboxClient(this.options, selectUser: memberId);
        }

        /// <summary>
        /// Convert the team client to a user client which can perform action on team owned contents.
        /// See documentation for <a href="https://www.dropbox.com/developers/documentation/http/teams">Dropbox-API-Select-Admin</a>
        /// for detail.
        /// </summary>
        /// <param name="adminId">The member id of a team admin.</param>
        /// <returns>The <see cref="DropboxClient"/></returns>
        public DropboxClient AsAdmin(string adminId)
        {
            return new DropboxClient(this.options, selectAdmin: adminId);
        }
    }
}
