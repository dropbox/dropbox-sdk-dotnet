//-----------------------------------------------------------------------------
// <copyright file="DropboxClient.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;

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
        /// Set the value for Dropbox-Api-Path-Root header.
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
