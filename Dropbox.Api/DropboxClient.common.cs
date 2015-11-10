//-----------------------------------------------------------------------------
// <copyright file="DropboxClient.common.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;
    using System.Net.Http;

    /// <summary>
    /// The client which contains endpoints which perform user-level actions.
    /// </summary>
    public sealed partial class DropboxClient : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="maxRetriesOnError">The maximum retries on a 5xx error.</param>
        /// <param name="userAgent">The user agent to use when making requests.</param>
        /// <param name="httpClient">The custom http client. If not provided, a default 
        /// http client will be created.</param>
        /// <remarks>
        /// The <paramref name="userAgent"/> helps Dropbox to identify requests coming from your application.
        /// We recommend that you use the format <c>"AppName/Version"</c>; if a value is supplied, the string
        /// <c>"/OfficialDropboxDotNetV2SDK/__version__"</c> is appended to the user agent.
        /// </remarks>
        public DropboxClient(
            string oauth2AccessToken,
            int maxRetriesOnError = 4,
            string userAgent = null,
            HttpClient httpClient = null)
            : this(new DrpoboxRequestHandlerOptions(oauth2AccessToken, maxRetriesOnError, userAgent, httpClient: httpClient))
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
        internal DropboxClient(DrpoboxRequestHandlerOptions options, string selectUser = null)
        {
            this.InitializeRoutes(new DropboxRequestHandler(options, selectUser));
        }

        /// <summary>
        /// Dummy dispose method.
        /// </summary>
        public void Dispose()
        {
        }
    }

    /// <summary>
    /// The client which contains endpoints which perform team-level actions.
    /// </summary>
    public sealed partial class DropboxTeamClient
    {
        /// <summary>
        /// The request handler options.
        /// </summary>
        private readonly DrpoboxRequestHandlerOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxTeamClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The team oauth2 access token for making client requests.</param>
        /// <param name="maxRetriesOnError">The maximum retries on a 5xx error.</param>
        /// <param name="userAgent">The user agent to use when making requests.</param>
        /// <param name="httpClient">The custom http client. If not provided, a default 
        /// http client will be created.</param>
        /// <remarks>
        /// The <paramref name="userAgent"/> helps Dropbox to identify requests coming from your application.
        /// We recommend that you use the format <c>"AppName/Version"</c>; if a value is supplied, the string
        /// <c>"/OfficialDropboxDotNetV2SDK/__version__"</c> is appended to the user agent.
        /// </remarks>
        public DropboxTeamClient(
            string oauth2AccessToken,
            int maxRetriesOnError = 4,
            string userAgent = null,
            HttpClient httpClient = null)
        {
            if (oauth2AccessToken == null)
            {
                throw new ArgumentNullException("oauth2AccessToken");
            }

            this.options = new DrpoboxRequestHandlerOptions(oauth2AccessToken, maxRetriesOnError, userAgent, httpClient: httpClient);
            this.InitializeRoutes(new DropboxRequestHandler(this.options));
        }

        /// <summary>
        /// Convert the team client to a user client which can perform action on the given team member's Dropbox.
        /// </summary>
        /// <param name="memberId">The member id of a user who is in the team.</param>
        /// <returns>The <see cref="DropboxClient"/></returns>
        public DropboxClient AsMember(string memberId)
        {
            return new DropboxClient(this.options, memberId);
        }
    }

    /// <summary>
    /// General HTTP exception
    /// </summary>
    public class HttpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpException"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="message">The message.</param>
        /// <param name="uri">The request uri.</param>
        /// <param name="inner">The inner.</param>
        public HttpException(int statusCode, string message = null, Uri uri = null, Exception inner = null)
            : base(message, inner)
        {
            this.StatusCode = statusCode;
            this.RequestUri = uri;
        }

        /// <summary>
        /// Gets the HTTP status code that prompted this exception
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public int StatusCode { get; private set; }

        /// <summary>
        /// Gets the URI for the request that prompted this exception.
        /// </summary>
        /// <value>
        /// The request URI.
        /// </value>
        public Uri RequestUri { get; private set; }
    }

    /// <summary>
    /// An HTTP exception that is caused by the server reporting a bad request.
    /// </summary>
    public class BadInputException : HttpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadInputException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="uri">The request URI.</param>
        public BadInputException(string message, Uri uri = null)
            : base(400, message, uri)
        {
        }
    }

    /// <summary>
    /// An HTTP exception that is caused by the server reporting an authentication problem.
    /// </summary>
    public class AuthException : HttpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="uri">The request URI</param>
        public AuthException(string message, Uri uri = null)
            : base(401, message, uri)
        {
        }
    }

    /// <summary>
    /// An HTTP Exception that will cause a retry
    /// </summary>
    internal class RetryException : HttpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetryException"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="isRateLimit">if set to <c>true</c> the server responded with
        /// an error indicating rate limiting.</param>
        /// <param name="message">The message.</param>
        /// <param name="uri">The request URI.</param>
        /// <param name="inner">The inner.</param>
        public RetryException(int statusCode, bool isRateLimit = false, string message = null, Uri uri = null, Exception inner = null)
            : base(statusCode, message, uri, inner)
        {
            this.IsRateLimit = isRateLimit;
        }

        /// <summary>
        /// Gets a value indicating whether this error represents a rate limiting response from the server.
        /// </summary>
        /// <value>
        /// <c>true</c> if this response is a rate limit; otherwise, <c>false</c>.
        /// </value>
        public bool IsRateLimit { get; private set; }
    }
}
