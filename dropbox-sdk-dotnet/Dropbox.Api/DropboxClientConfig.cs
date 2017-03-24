//-----------------------------------------------------------------------------
// <copyright file="DropboxClientConfig.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System.Net.Http;

    /// <summary>
    /// The class which contains all configurations for Dropbox client.
    /// </summary>
    public sealed class DropboxClientConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropboxClientConfig"/> class.
        /// </summary>
        public DropboxClientConfig()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropboxClientConfig"/> class.
        /// </summary>
        /// <param name="userAgent">The user agent to use when making requests.</param>
        public DropboxClientConfig(string userAgent)
            : this(userAgent, 4)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropboxClientConfig"/> class.
        /// </summary>
        /// <param name="userAgent">The user agent to use when making requests.</param>
        /// <param name="maxRetriesOnError">The max number retries on error.</param>
        public DropboxClientConfig(string userAgent, int maxRetriesOnError)
        {
            this.UserAgent = userAgent;
            this.MaxRetriesOnError = maxRetriesOnError;
        }

        /// <summary>
        /// Gets or sets the max number retries on error. Default value is 4.
        /// </summary>
        public int MaxRetriesOnError { get; set; }

        /// <summary>
        /// Gets or sets the user agent to use when making requests.
        /// </summary>
        /// <remarks>
        /// This value helps Dropbox to identify requests coming from your application.
        /// We recommend that you use the format <c>"AppName/Version"</c>; if a value is supplied, the string
        /// <c>"/OfficialDropboxDotNetV2SDK/__version__"</c> is appended to the user agent.
        /// </remarks>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the custom http client. If not set, a default http client will be created.
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// Gets or sets the custom http client for long poll request. If not set, a default
        /// http client with a longer timeout (480 seconds) will be created.
        /// </summary>
        public HttpClient LongPollHttpClient { get; set; }
    }
}
