//-----------------------------------------------------------------------------
// <copyright file="DropboxClient.common.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;
    using System.Globalization;
    using System.Text;

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
