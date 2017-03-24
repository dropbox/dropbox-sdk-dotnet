//-----------------------------------------------------------------------------
// <copyright file="DropboxClient.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;

    /// <summary>
    /// The client which contains endpoints which perform user-level actions.
    /// </summary>
    public sealed partial class DropboxClient : DropboxClientBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        public DropboxClient(string oauth2AccessToken)
            : this(oauth2AccessToken, new DropboxClientConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="userAgent">The user agent to use when making requests.</param>
        [Obsolete("This constructor is deprecated, please use DropboxClientConfig instead.")]
        public DropboxClient(string oauth2AccessToken, string userAgent)
            : this(oauth2AccessToken, new DropboxClientConfig(userAgent))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxClient(string oauth2AccessToken, DropboxClientConfig config)
            : this(new DropboxRequestHandlerOptions(config, oauth2AccessToken))
        {
            if (oauth2AccessToken == null)
            {
                throw new ArgumentNullException("oauth2AccessToken");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="options">The request handler options.</param>
        /// <param name="selectUser">The member id of the selected user. If provided together with
        /// a team access token, actions will be performed on this this user's Dropbox.</param>
        /// <param name="selectAdmin">The member id of the selected admin. If provided together with
        /// a team access token, access is allowed for all team owned contents.</param>
        internal DropboxClient(DropboxRequestHandlerOptions options, string selectUser = null, string selectAdmin = null)
            : base(new DropboxRequestHandler(options, selectUser: selectUser, selectAdmin: selectAdmin))
        {
        }
    }
}
