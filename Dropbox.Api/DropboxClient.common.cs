//-----------------------------------------------------------------------------
// <copyright file="DropboxClient.common.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using Dropbox.Api.Stone;
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Text;

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
            : this(userAgent, maxRetriesOnError, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropboxClientConfig"/> class.
        /// </summary>
        /// <param name="userAgent">The user agent to use when making requests.</param>
        /// <param name="maxRetriesOnError">The max number retries on error.</param>
        /// <param name="httpClient">The custom http client.</param>
        internal DropboxClientConfig(string userAgent, int maxRetriesOnError, HttpClient httpClient)
        {
            this.UserAgent = userAgent;
            this.MaxRetriesOnError = maxRetriesOnError;
            this.HttpClient = httpClient;
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
    }

    /// <summary>
    /// The client which contains endpoints which perform user-level actions.
    /// </summary>
    public sealed partial class DropboxClient : IDisposable
    {
        /// <summary>
        /// Transport
        /// </summary>
        private ITransport transport;

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
            : this(new DropboxRequestHandlerOptions(oauth2AccessToken, config.MaxRetriesOnError, config.UserAgent, httpClient: config.HttpClient))
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
        internal DropboxClient(DropboxRequestHandlerOptions options, string selectUser = null)
        {
            this.transport = new DropboxRequestHandler(options, selectUser);
            this.InitializeRoutes(transport);
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            transport.Dispose();
        }
    }

    /// <summary>
    /// The client which contains endpoints which perform app-auth actions.
    /// </summary>
    public sealed partial class DropboxAppClient : IDisposable
    {
        /// <summary>
        /// Transport
        /// </summary>
        private ITransport transport;

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
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="appKey">The Dropbox app key (e.g. consumer key in OAuth).</param>
        /// <param name="appSecret">The Dropbox app secret (e.g. consumer secret in OAuth).</param>
        /// <param name="config">The <see cref="DropboxClientConfig"/>.</param>
        public DropboxAppClient(string appKey, string appSecret, DropboxClientConfig config)
        {
            if (appKey == null)
            {
                throw new ArgumentNullException("appKey");
            }

            if (appSecret == null)
            {
                throw new ArgumentNullException("appSecret");
            }

            var options = new DropboxRequestHandlerOptions(
                GetBasicAuthHeader(appKey, appSecret), 
                config.MaxRetriesOnError, 
                config.UserAgent, 
                httpClient: config.HttpClient);

            this.transport = new DropboxRequestHandler(options);
            this.InitializeRoutes(this.transport);
        }

        /// <summary>
        /// Gets the basic auth header from app key and app secret.
        /// </summary>
        /// <param name="appKey">The app key.</param>
        /// <param name="appSecret">The app secret.</param>
        /// <returns>The basic auth header.</returns>
        private static string GetBasicAuthHeader(string appKey, string appSecret) 
        {
            var rawValue = string.Format("{0}:{1}", appKey, appSecret);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(rawValue));
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            transport.Dispose();
        }
    }

    /// <summary>
    /// The client which contains endpoints which perform team-level actions.
    /// </summary>
    public sealed partial class DropboxTeamClient : IDisposable
    {
        /// <summary>
        /// Transport
        /// </summary>
        private ITransport transport;

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
        {
            if (oauth2AccessToken == null)
            {
                throw new ArgumentNullException("oauth2AccessToken");
            }

            this.options = new DropboxRequestHandlerOptions(oauth2AccessToken, config.MaxRetriesOnError, config.UserAgent, httpClient: config.HttpClient);
            this.transport = new DropboxRequestHandler(options);
            this.InitializeRoutes(this.transport);
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

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            transport.Dispose();
        }
    }

    /// <summary>
    /// Base class for all exceptions from Dropbox service.
    /// </summary>
    public class DropboxException : Exception
    {
        /// <summary>
        /// The request id.
        /// </summary>
        private string requestId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropboxException"/> class.
        /// </summary>
        /// <param name="requestId">The Dropbox request id.</param>
        /// <param name="message">The error message.</param>
        /// <param name="inner">The inner.</param>
        internal DropboxException(string requestId, string message = null, Exception inner = null)
            : base(message, inner)
        {
            this.requestId = requestId;
        }

        /// <summary>
        ///  The ToString().
        /// </summary>
        /// <returns>A string that represents the current <see cref="DropboxException"/>.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());
            if (this.requestId != null) 
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "; Request Id: {0}", this.requestId);
            }

            return builder.ToString();
        }
    }

    /// <summary>
    /// General HTTP exception
    /// </summary>
    public class HttpException : DropboxException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpException"/> class.
        /// </summary>
        /// <param name="requestId">The Dropbox request id.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="message">The message.</param>
        /// <param name="uri">The request uri.</param>
        /// <param name="inner">The inner.</param>
        internal HttpException(string requestId, int statusCode, string message = null, Uri uri = null, Exception inner = null)
            : base(requestId, message, inner)
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
        /// <param name="requestId">The Dropbox request id.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="uri">The request URI.</param>
        internal BadInputException(string requestId, string message, Uri uri = null)
            : base(requestId, 400, message, uri)
        {
        }
    }

    /// <summary>
    /// An exception that is caused by the server reporting an authentication problem.
    /// </summary>
    public partial class AuthException
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="AuthException"/> class.</para>
        /// </summary>
        internal AuthException(string requestId)
            : base(requestId)
        {
            this.StatusCode = 401;
            this.RequestUri = null;
        }

        /// <summary>
        /// Gets the HTTP status code that prompted this exception
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        [Obsolete("This field will be removed soon.")]
        public int StatusCode { get; private set; }

        /// <summary>
        /// Gets the URI for the request that prompted this exception.
        /// </summary>
        /// <value>
        /// The request URI.
        /// </value>
        [Obsolete("This field will be removed soon.")]
        public Uri RequestUri { get; private set; }
    }

    /// <summary>
    /// An exception which is caused by account not having access to this endpoint.
    /// </summary>
    public partial class AccessException
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="AccessException"/> class.</para>
        /// </summary>
        internal AccessException(string requestId)
            : base(requestId)
        {
        }
    }
    
    /// <summary>
    /// An HTTP Exception that will cause a retry due to transient failure. The SDK will perform
    /// a certain number of retries which is configurable in <see cref="DropboxClient"/>. If the client
    /// still gets this exception, it's up to the client to decide whether to continue retrying or not.
    /// </summary>
    public class RetryException : HttpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetryException"/> class.
        /// </summary>
        /// <param name="requestId">The Dropbox request id.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="message">The message.</param>
        /// <param name="uri">The request URI.</param>
        /// <param name="inner">The inner.</param>
        internal RetryException(string requestId, int statusCode, string message = null, Uri uri = null, Exception inner = null)
            : base(requestId, statusCode, message, uri, inner)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this error represents a rate limiting response from the server.
        /// </summary>
        /// <value>
        /// <c>true</c> if this response is a rate limit; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("This field will be removed soon. Please catch RateLimitException separately.")]
        public virtual bool IsRateLimit
        {
            get { return false; }
        }
    }

    /// <summary>
    /// An exception that will cause a retry due to rate limiting. The SDK will not do auto-retry for
    /// this type of exception. The client should do proper backoff based on the value of
    /// <see cref="RateLimitException.RetryAfter"/> field.
    /// </summary>
    public partial class RateLimitException
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="requestId"/> class.</para>
        /// </summary>
        internal RateLimitException(string requestId)
            : base(requestId)
        {
            this.StatusCode = 429;
            this.RequestUri = null;
        }

        /// <summary>
        /// Gets the HTTP status code that prompted this exception
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        [Obsolete("This field will be removed soon.")]
        public int StatusCode { get; private set; }

        /// <summary>
        /// Gets the URI for the request that prompted this exception.
        /// </summary>
        /// <value>
        /// The request URI.
        /// </value>
        [Obsolete("This field will be removed soon.")]
        public Uri RequestUri { get; private set; }

        /// <summary>
        /// Gets the value in second which the client should backoff and retry after.
        /// </summary>
        public int RetryAfter 
        {
            get { return (int)this.ErrorResponse.RetryAfter; }
        }

        /// <summary>
        /// Gets a value indicating whether this error represents a rate limiting response from the server.
        /// </summary>
        /// <value>
        /// <c>true</c> if this response is a rate limit; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("This field will be removed soon.")]
        public bool IsRateLimit
        {
            get { return true; }
        }
    }
}
