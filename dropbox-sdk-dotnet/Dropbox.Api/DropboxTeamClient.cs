//-----------------------------------------------------------------------------
// <copyright file="DropboxTeamClient.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;

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
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        public DropboxTeamClient(string oauth2AccessToken)
            : this(oauth2AccessToken, new DropboxClientConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="userAgent">The user agent to use when making requests.</param>
        [Obsolete("This constructor is deprecated, please use DropboxClientConfig instead.")]
        public DropboxTeamClient(string oauth2AccessToken, string userAgent)
            : this(oauth2AccessToken, new DropboxClientConfig(userAgent))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxTeamClient(string oauth2AccessToken, DropboxClientConfig config)
            : this(new DropboxRequestHandlerOptions(config, oauth2AccessToken, null, null, null, null))
        {
            if (oauth2AccessToken == null)
            {
                throw new ArgumentNullException("oauth2AccessToken");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="options">The request handler options.</param>
        private DropboxTeamClient(DropboxRequestHandlerOptions options)
            : base(new DropboxRequestHandler(options))
        {
            this.options = options;
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
