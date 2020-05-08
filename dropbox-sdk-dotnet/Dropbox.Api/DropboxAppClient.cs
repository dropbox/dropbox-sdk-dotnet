//-----------------------------------------------------------------------------
// <copyright file="DropboxAppClient.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;
    using System.Text;

    /// <summary>
    /// The client which contains endpoints which perform app-auth actions.
    /// </summary>
    public sealed partial class DropboxAppClient : DropboxClientBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxAppClient"/> class.
        /// </summary>
        /// <param name="appKey">The Dropbox app key (e.g. consumer key in OAuth).</param>
        /// <param name="appSecret">The Dropbox app secret (e.g. consumer secret in OAuth).</param>
        public DropboxAppClient(string appKey, string appSecret)
            : this(appKey, appSecret, new DropboxClientConfig())
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxAppClient"/> class.
        /// </summary>
        /// <param name="appKey">The Dropbox app key (e.g. consumer key in OAuth).</param>
        /// <param name="appSecret">The Dropbox app secret (e.g. consumer secret in OAuth).</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxAppClient(string appKey, string appSecret, DropboxClientConfig config)
            : this(new DropboxRequestHandlerOptions(config, GetBasicAuthHeader(appKey, appSecret), null, null, null, null))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxAppClient"/> class.
        /// </summary>
        /// <param name="options">The request handler options.</param>
        private DropboxAppClient(DropboxRequestHandlerOptions options)
            : base(new DropboxRequestHandler(options))
        {
        }

        /// <summary>
        /// Gets the basic auth header from app key and app secret.
        /// </summary>
        /// <param name="appKey">The app key.</param>
        /// <param name="appSecret">The app secret.</param>
        /// <returns>The basic auth header.</returns>
        private static string GetBasicAuthHeader(string appKey, string appSecret)
        {
            if (appKey == null)
            {
                throw new ArgumentNullException("appKey");
            }

            if (appSecret == null)
            {
                throw new ArgumentNullException("appSecret");
            }

            var rawValue = string.Format("{0}:{1}", appKey, appSecret);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(rawValue));
        }
    }
}
