//-----------------------------------------------------------------------------
// <copyright file="DropboxClient.common.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    using Dropbox.Api.Babel;
    using System.Reflection;

    /// <summary>
    /// The known route styles
    /// </summary>
    internal enum RouteStyle
    {
        /// <summary>
        /// RPC style means that the argument and result of a route are contained in the 
        /// HTTP body.
        /// </summary>
        Rpc,

        /// <summary>
        /// Download style means that the route argument goes in a <c>Dropbox-API-Args</c>
        /// header, and the result comes back in a <c>Dropbox-API-Result</c> header. The
        /// HTTP response body contains a binary payload.
        /// </summary>
        Download,

        /// <summary>
        /// Upload style means that the route argument goes in a <c>Dropbox-API-Arg</c>
        /// header. The HTTP request body contains a binary payload. The result comes
        /// back in a <c>Dropbox-API-Result</c> header.
        /// </summary>
        Upload
    }

    /// <summary>
    /// The object used to to make requests to the Dropbox API.
    /// </summary>
    public sealed partial class DropboxClient : ITransport, IDisposable
    {
        /// <summary>
        /// The API version
        /// </summary>
        private const string ApiVersion = "2-beta";

        /// <summary>
        /// The default domain
        /// </summary>
        private const string DefaultDomain = "dropbox.com";

        /// <summary>
        /// The host for RPC-style routes.
        /// </summary>
        private const string HostApi = "api";

        /// <summary>
        /// The host for upload and download style routes.
        /// </summary>
        private const string HostContent = "content";

        /// <summary>
        /// The dropbox API argument header.
        /// </summary>
        private const string DropboxApiArgHeader = "Dropbox-API-Arg";

        /// <summary>
        /// The dropbox API result header.
        /// </summary>
        private const string DropboxApiResultHeader = "Dropbox-API-Result";

        /// <summary>
        /// The base user agent, used to construct all user agent strings.
        /// </summary>
        private const string BaseUserAgent = "OfficialDropboxDotNetV2SDK";

        /// <summary>
        /// The maximum retries on a 5xx error.
        /// </summary>
        private int maxClientRetries;

        /// <summary>
        /// Maps from host types to domain names.
        /// </summary>
        private Dictionary<string, string> hostMap;

        /// <summary>
        /// The HTTP client use to send requests to the server.
        /// </summary>
        private HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxClient"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="maxRetriesOnError">The maximum retries on a 5xx error.</param>
        /// <param name="userAgent">The user agent to use when making requests.</param>
        /// <param name="apiHostname">The hostname that will process api requests;
        /// this is for internal Dropbox use only.</param>
        /// <param name="apiContentHostname">The hostname that will process api content requests;
        /// this is for internal Dropbox use only.</param>
        /// <remarks>
        /// The <paramref name="userAgent"/> helps Dropbox to identify requests coming from your application.
        /// We recommend that you use the format <c>"AppName/Version"</c>; if a value is supplied, the string
        /// <c>"/OfficialDropboxDotNetV2SDK/__version__"</c> is appended to the user agent.
        /// </remarks>
        public DropboxClient(
            string oauth2AccessToken,
            int maxRetriesOnError = 4,
            string userAgent = null,
            string apiHostname = "api." + DefaultDomain,
            string apiContentHostname = "api-content." + DefaultDomain
            )
        {
            if (string.IsNullOrEmpty(oauth2AccessToken))
            {
                throw new ArgumentNullException("oauth2AccessToken");
            }

            var fullname = new AssemblyName(typeof(DropboxClient).Assembly.FullName);
            var sdkVersion = fullname.Version.ToString();

            string clientUserAgent;

            if (string.IsNullOrEmpty(userAgent))
            {
                clientUserAgent = string.Join("/", BaseUserAgent, sdkVersion);
            }
            else
            {
                clientUserAgent = string.Join("/", userAgent, BaseUserAgent, sdkVersion);
            }

            this.hostMap = new Dictionary<string, string>
            {
                { HostApi, apiHostname },
                { HostContent, apiContentHostname }
            };

            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauth2AccessToken);
            this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", clientUserAgent);

            this.maxClientRetries = maxRetriesOnError;

            this.InitializeRoutes(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.httpClient != null)
            {
                this.httpClient.Dispose();
                this.httpClient = null;
            }
        }

        /// <summary>
        /// Sends the RPC request asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="host">The server host to send the request to.</param>
        /// <param name="route">The route name.</param>
        /// <returns>
        /// An asynchronous task for the response.
        /// </returns>
        /// <exception cref="ApiException{TError}">
        /// This exception is thrown when there is an error reported by the server.
        /// </exception>
        async Task<TResponse> ITransport.SendRpcRequestAsync<TRequest, TResponse, TError>(
            TRequest request,
            string host,
            string route)
        {
            var serializedArg = JsonEncoder.Encode(request);

            var res = await this.RequestJsonStringWithRetry(host, route, RouteStyle.Rpc, serializedArg);

            if (res.IsError)
            {
                throw JsonDecoder.Decode<ApiException<TError>>(res.ObjectResult);
            }

            return JsonDecoder.Decode<TResponse>(res.ObjectResult);
        }

        /// <summary>
        /// Sends the upload request asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="body">The document to upload.</param>
        /// <param name="host">The server host to send the request to.</param>
        /// <param name="route">The route name.</param>
        /// <returns>
        /// An asynchronous task for the response.
        /// </returns>
        /// <exception cref="ApiException{TError}">
        /// This exception is thrown when there is an error reported by the server.
        /// </exception>
        async Task<TResponse> ITransport.SendUploadRequestAsync<TRequest, TResponse, TError>(
            TRequest request,
            Stream body,
            string host,
            string route)
        {
            var serializedArg = JsonEncoder.Encode(request);
            var res = await this.RequestJsonStringWithRetry(host, route, RouteStyle.Upload, serializedArg, body);

            if (res.IsError)
            {
                throw JsonDecoder.Decode<ApiException<TError>>(res.ObjectResult);
            }

            return JsonDecoder.Decode<TResponse>(res.ObjectResult);
        }

        /// <summary>
        /// Sends the download request asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="host">The server host to send the request to.</param>
        /// <param name="route">The route name.</param>
        /// <returns>
        /// An asynchronous task for the response.
        /// </returns>
        /// <exception cref="ApiException{TError}">
        /// This exception is thrown when there is an error reported by the server.
        /// </exception>
        async Task<IDownloadResponse<TResponse>> ITransport.SendDownloadRequestAsync<TRequest, TResponse, TError>(
            TRequest request,
            string host,
            string route)
        {
            var serializedArg = JsonEncoder.Encode(request);
            var res = await this.RequestJsonStringWithRetry(host, route, RouteStyle.Download, serializedArg);

            if (res.IsError)
            {
                throw JsonDecoder.Decode<ApiException<TError>>(res.ObjectResult);
            }

            var response = JsonDecoder.Decode<TResponse>(res.ObjectResult);
            return new DownloadResponse<TResponse>(response, res.HttpResponse);
        }

        /// <summary>
        /// Requests the JSON string with retry.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeStyle">The route style.</param>
        /// <param name="requestArg">The request argument.</param>
        /// <param name="body">The body to upload if <paramref name="routeStyle"/>
        /// is <see cref="RouteStyle.Upload"/>.</param>
        /// <returns>The asynchronous task with the result.</returns>
        private async Task<Result> RequestJsonStringWithRetry(
            string host,
            string routeName,
            RouteStyle routeStyle,
            string requestArg,
            Stream body = null)
        {
            var attempt = 0;
            var maxRetries = this.maxClientRetries;
            var r = new Random();

            byte[] cachedBody = null;
            long cachedStreamStart = 0;

            if (routeStyle == RouteStyle.Upload)
            {
                // to support retry logic, the body stream must be seekable
                // if it isn't we won't retry
                if (!body.CanSeek)
                {
                    maxRetries = 0;
                }
                else if (body is MemoryStream)
                {
                    cachedStreamStart = body.Position;
                    cachedBody = ((MemoryStream)body).ToArray();
                }
                else
                {
                    cachedStreamStart = body.Position;
                    using (var mem = new MemoryStream())
                    {
                        await body.CopyToAsync(mem);
                        cachedBody = mem.ToArray();
                    }
                }
            }

            while (true)
            {
                try
                {
                    if (cachedBody == null)
                    {
                        return await this.RequestJsonString(host, routeName, routeStyle, requestArg, body);
                    }
                    else
                    {
                        using (var mem = new MemoryStream(cachedBody, writable: false))
                        {
                            mem.Position = cachedStreamStart;
                            return await this.RequestJsonString(host, routeName, routeStyle, requestArg, mem);
                        }
                    }
                }
                catch (RetryException re)
                {
                    if (!re.IsRateLimit)
                    {
                        // dropbox maps 503 - ServiceUnavailable to be a rate limiting error.
                        // do not count a rate limiting error as an attempt
                        attempt++;
                    }

                    if (attempt > maxRetries)
                    {
                        throw;
                    }
                }

                // use exponential backoff
                var backoff = TimeSpan.FromSeconds(Math.Pow(2, attempt) * r.NextDouble());
#if DOCUMENTATION_BUILD
                await Task.Delay(backoff);
#else
                await TaskEx.Delay(backoff);
#endif
            }
        }

        /// <summary>
        /// Requests the JSON string.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeStyle">The route style.</param>
        /// <param name="requestArg">The request argument.</param>
        /// <param name="body">The body to upload if <paramref name="routeStyle"/>
        /// is <see cref="RouteStyle.Upload"/>.</param>
        /// <returns>The asynchronous task with the result.</returns>
        private async Task<Result> RequestJsonString(
            string host,
            string routeName,
            RouteStyle routeStyle,
            string requestArg,
            Stream body = null)
        {
            var hostname = this.hostMap[host];
            var uri = this.GetRouteUri(hostname, routeName);

            var request = new HttpRequestMessage(HttpMethod.Post, uri);

            switch (routeStyle)
            {
                case RouteStyle.Rpc:
                    request.Content = new StringContent(requestArg, Encoding.UTF8, "application/json");
                    break;
                case RouteStyle.Download:
                    request.Headers.Add(DropboxApiArgHeader, requestArg);
                    break;
                case RouteStyle.Upload:
                    request.Headers.Add(DropboxApiArgHeader, requestArg);
                    if (body == null)
                    {
                        throw new ArgumentNullException("body");
                    }
                    request.Content = new StreamContent(body);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    break;
                default:
                    throw new InvalidOperationException(string.Format(
                        CultureInfo.InvariantCulture,
                        "Unknown route style: {0}",
                        routeStyle));
            }

            var disposeResponse = true;
            var response = await this.httpClient.SendAsync(request);

            try
            {
                if ((int)response.StatusCode >= 500)
                {
                    var text = await response.Content.ReadAsStringAsync();

                    throw new RetryException((int)response.StatusCode, message: text);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var text = await response.Content.ReadAsStringAsync();
                    throw new BadInputException(text);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var text = await response.Content.ReadAsStringAsync();

                    // TODO (paul) deal with json issues
                    throw new AuthException(text);
                }
                else if ((int)response.StatusCode == 429)
                {
                    throw new RetryException(429, isRateLimit: true);
                }
                else if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    var reason = await response.Content.ReadAsStringAsync();

                    return new Result
                    {
                        IsError = true,
                        ObjectResult = reason
                    };
                }
                else if ((int)response.StatusCode >= 200 && (int)response.StatusCode <= 299)
                {
                    if (routeStyle == RouteStyle.Download)
                    {
                        disposeResponse = false;
                        return new Result
                        {
                            IsError = false,
                            ObjectResult = response.Headers.GetValues(DropboxApiResultHeader).FirstOrDefault(),
                            HttpResponse = response
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            IsError = false,
                            ObjectResult = await response.Content.ReadAsStringAsync()
                        };
                    }
                }
                else
                {
                    var text = await response.Content.ReadAsStringAsync();
                    throw new HttpException((int)response.StatusCode, text);
                }
            }
            finally
            {
                if (disposeResponse)
                {
                    response.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets the URI for a route.
        /// </summary>
        /// <param name="hostname">The hostname for the request.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <returns>The uri for this route.</returns>
        private Uri GetRouteUri(string hostname, string routeName)
        {
            var builder = new UriBuilder("https", hostname);
            builder.Path = "/" + ApiVersion + routeName;
            return builder.Uri;
        }

        /// <summary>
        /// Used to return un-typed result information to the layer that can interpret the
        /// object types
        /// </summary>
        private class Result
        {
            /// <summary>
            /// Gets or sets a value indicating whether this instance is an error.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is an error; otherwise, <c>false</c>.
            /// </value>
            public bool IsError { get; set; }

            /// <summary>
            /// Gets or sets the un-typed object result, this will be parsed into the
            /// specific response or error type for the route.
            /// </summary>
            /// <value>
            /// The object result.
            /// </value>
            public string ObjectResult { get; set; }

            /// <summary>
            /// Gets or sets the HTTP response, this is only set if the route was a download route.
            /// </summary>
            /// <value>
            /// The HTTP response.
            /// </value>
            public HttpResponseMessage HttpResponse { get; set; }
        }

        /// <summary>
        /// An implementation of the <see cref="T:Dropbox.Api.Babel.IDownloadResponse`1"/> interface.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        private class DownloadResponse<TResponse> : IDownloadResponse<TResponse>
            where TResponse : IEncodable<TResponse>, new()
        {
            /// <summary>
            /// The HTTP response containing the body content.
            /// </summary>
            private HttpResponseMessage httpResponse;

            /// <summary>
            /// Initializes a new instance of the <see cref="DownloadResponse{TResponse}"/> class.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <param name="httpResponseMessage">The HTTP response message.</param>
            public DownloadResponse(TResponse response, HttpResponseMessage httpResponseMessage)
            {
                this.Response = response;
                this.httpResponse = httpResponseMessage;
            }

            /// <summary>Gets the response.</summary>
            /// <value>The response.</value>
            public TResponse Response { get; private set; }

            /// <summary>
            /// Asynchronously gets the content as a <see cref="Stream" />.
            /// </summary>
            /// <returns>The downloaded content as a stream.</returns>
            public Task<Stream> GetContentAsStreamAsync()
            {
                return this.httpResponse.Content.ReadAsStreamAsync();
            }

            /// <summary>
            /// Asynchronously gets the content as a <see cref="byte" /> array.
            /// </summary>
            /// <returns>The downloaded content as a byte array.</returns>
            public Task<byte[]> GetContentAsByteArrayAsync()
            {
                return this.httpResponse.Content.ReadAsByteArrayAsync();
            }

            /// <summary>
            /// Asynchronously gets the content as <see cref="String" />.
            /// </summary>
            /// <returns>The downloaded content as a string.</returns>
            public Task<string> GetContentAsStringAsync()
            {
                return this.httpResponse.Content.ReadAsStringAsync();
            }

            /// <summary>
            /// Disposes of the <see cref="HttpResponseMessage"/> in this instance.
            /// </summary>
            public void Dispose()
            {
                if (this.httpResponse != null)
                {
                    this.httpResponse.Dispose();
                    this.httpResponse = null;
                }
            }
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
        /// <param name="inner">The inner.</param>
        public HttpException(int statusCode, string message = null, Exception inner = null)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Gets the HTTP status code that prompted this exception
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public int StatusCode { get; private set; }
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
        public BadInputException(string message)
            : base(400, message)
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
        public AuthException(string message)
            : base(401, message)
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
        /// <param name="inner">The inner.</param>
        public RetryException(int statusCode, bool isRateLimit = false, string message = null, Exception inner = null)
            : base(statusCode, message, inner)
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
