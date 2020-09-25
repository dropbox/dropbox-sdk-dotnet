//-----------------------------------------------------------------------------
// <copyright file="DropboxRequestHandler.cs" company="Dropbox Inc">
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
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using Dropbox.Api.Stone;
    using Dropbox.Api.Common;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The object used to to make requests to the Dropbox API.
    /// </summary>
    internal sealed class DropboxRequestHandler : ITransport
    {
        private const int TokenExpirationBuffer = 300;
        /// <summary>
        /// The API version
        /// </summary>
        private const string ApiVersion = "2";

        /// <summary>
        /// The dropbox API argument header.
        /// </summary>
        private const string DropboxApiArgHeader = "Dropbox-API-Arg";

        /// <summary>
        /// The dropbox API result header.
        /// </summary>
        private const string DropboxApiResultHeader = "Dropbox-API-Result";

        /// <summary>
        /// The member id of the selected user.
        /// </summary>
        private readonly string selectUser;

        /// <summary>
        /// The member id of the selected admin.
        /// </summary>
        private readonly string selectAdmin;

        /// <summary>
        /// The path root value used to make API call.
        /// </summary>
        private readonly PathRoot pathRoot;

        /// <summary>
        /// The configuration options for dropbox client.
        /// </summary>
        private readonly DropboxRequestHandlerOptions options;

        /// <summary>
        /// The default http client instance.
        /// </summary>
        private readonly HttpClient defaultHttpClient = new HttpClient();

        /// <summary>
        /// The default long poll http client instance.
        /// </summary>
        private readonly HttpClient defaultLongPollHttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(480) };

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dropbox.Api.DropboxRequestHandler"/> class.
        /// </summary>
        /// <param name="options">The configuration options for dropbox client.</param>
        /// <param name="selectUser">The member id of the selected user.</param>
        /// <param name="selectAdmin">The member id of the selected admin.</param>
        /// <param name="pathRoot">The path root to make requests from.</param>
        public DropboxRequestHandler(
            DropboxRequestHandlerOptions options,
            string selectUser = null,
            string selectAdmin = null,
            PathRoot pathRoot = null)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.options = options;
            this.selectUser = selectUser;
            this.selectAdmin = selectAdmin;
            this.pathRoot = pathRoot;
        }

        /// <summary>
        /// Set the value for Dropbox-Api-Path-Root header.
        /// </summary>
        /// <param name="pathRoot">The path root object.</param>
        /// <returns>A <see cref="DropboxClient"/> instance with Dropbox-Api-Path-Root header set.</returns>
        internal DropboxRequestHandler WithPathRoot(PathRoot pathRoot)
        {
            if (pathRoot == null)
            {
                throw new ArgumentNullException("pathRoot");
            }

            return new DropboxRequestHandler(
                this.options,
                selectUser: this.selectUser,
                selectAdmin: this.selectAdmin,
                pathRoot: pathRoot);
        }

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
        /// Sends the upload request asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="host">The server host to send the request to.</param>
        /// <param name="route">The route name.</param>
        /// <param name="auth">The auth type of the route.</param>
        /// <param name="requestEncoder">The request encoder.</param>
        /// <param name="resposneDecoder">The response decoder.</param>
        /// <param name="errorDecoder">The error decoder.</param>
        /// <returns>An asynchronous task for the response.</returns>
        /// <exception cref="ApiException{TError}">
        /// This exception is thrown when there is an error reported by the server.
        /// </exception>
        async Task<TResponse> ITransport.SendRpcRequestAsync<TRequest, TResponse, TError>(
            TRequest request,
            string host,
            string route,
            string auth,
            IEncoder<TRequest> requestEncoder,
            IDecoder<TResponse> resposneDecoder,
            IDecoder<TError> errorDecoder)
        {
            var serializedArg = JsonWriter.Write(request, requestEncoder);
            var res = await this.RequestJsonStringWithRetry(host, route, auth, RouteStyle.Rpc, serializedArg)
                .ConfigureAwait(false);

            if (res.IsError)
            {
                throw StructuredException<TError>.Decode<ApiException<TError>>(
                    res.ObjectResult, errorDecoder, () => new ApiException<TError>(res.RequestId));
            }

            return JsonReader.Read(res.ObjectResult, resposneDecoder);
        }

        /// <summary>
        /// Sends the upload request asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="body">The content to be uploaded.</param>
        /// <param name="host">The server host to send the request to.</param>
        /// <param name="route">The route name.</param>
        /// <param name="auth">The auth type of the route.</param>
        /// <param name="requestEncoder">The request encoder.</param>
        /// <param name="resposneDecoder">The response decoder.</param>
        /// <param name="errorDecoder">The error decoder.</param>
        /// <returns>An asynchronous task for the response.</returns>
        /// <exception cref="ApiException{TError}">
        /// This exception is thrown when there is an error reported by the server.
        /// </exception>
        async Task<TResponse> ITransport.SendUploadRequestAsync<TRequest, TResponse, TError>(
            TRequest request,
            Stream body,
            string host,
            string route,
            string auth,
            IEncoder<TRequest> requestEncoder,
            IDecoder<TResponse> resposneDecoder,
            IDecoder<TError> errorDecoder)
        {
            var serializedArg = JsonWriter.Write(request, requestEncoder, true);
            var res = await this.RequestJsonStringWithRetry(host, route, auth, RouteStyle.Upload, serializedArg, body)
                .ConfigureAwait(false);

            if (res.IsError)
            {
                throw StructuredException<TError>.Decode<ApiException<TError>>(
                    res.ObjectResult, errorDecoder, () => new ApiException<TError>(res.RequestId));
            }

            return JsonReader.Read(res.ObjectResult, resposneDecoder);
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
        /// <param name="auth">The auth type of the route.</param>
        /// <param name="requestEncoder">The request encoder.</param>
        /// <param name="resposneDecoder">The response decoder.</param>
        /// <param name="errorDecoder">The error decoder.</param>
        /// <returns>An asynchronous task for the response.</returns>
        /// <exception cref="ApiException{TError}">
        /// This exception is thrown when there is an error reported by the server.
        /// </exception>
        async Task<IDownloadResponse<TResponse>> ITransport.SendDownloadRequestAsync<TRequest, TResponse, TError>(
            TRequest request,
            string host,
            string route,
            string auth,
            IEncoder<TRequest> requestEncoder,
            IDecoder<TResponse> resposneDecoder,
            IDecoder<TError> errorDecoder)
        {
            var serializedArg = JsonWriter.Write(request, requestEncoder, true);
            var res = await this.RequestJsonStringWithRetry(host, route, auth, RouteStyle.Download, serializedArg)
                .ConfigureAwait(false);

            if (res.IsError)
            {
                throw StructuredException<TError>.Decode<ApiException<TError>>(
                    res.ObjectResult, errorDecoder, () => new ApiException<TError>(res.RequestId));
            }

            var response = JsonReader.Read(res.ObjectResult, resposneDecoder);
            return new DownloadResponse<TResponse>(response, res.HttpResponse);
        }

        /// <summary>
        /// Requests the JSON string with retry.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="auth">The auth type of the route.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeStyle">The route style.</param>
        /// <param name="requestArg">The request argument.</param>
        /// <param name="body">The body to upload if <paramref name="routeStyle"/>
        /// is <see cref="RouteStyle.Upload"/>.</param>
        /// <returns>The asynchronous task with the result.</returns>
        private async Task<Result> RequestJsonStringWithRetry(
            string host,
            string routeName,
            string auth,
            RouteStyle routeStyle,
            string requestArg,
            Stream body = null)
        {
            var attempt = 0;
            var hasRefreshed = false;
            var maxRetries = this.options.MaxClientRetries;
            var r = new Random();

            if (routeStyle == RouteStyle.Upload)
            {
                if (body == null)
                {
                    throw new ArgumentNullException("body");
                }

                // to support retry logic, the body stream must be seekable
                // if it isn't we won't retry
                if (!body.CanSeek)
                {
                    maxRetries = 0;
                }
            }
            await CheckAndRefreshAccessToken();
            try
            {
                while (true)
                {
                    try
                    {
                        return await this.RequestJsonString(host, routeName, auth, routeStyle, requestArg, body)
                            .ConfigureAwait(false);
                    }
                    catch (AuthException e)
                    {
                        if (e.Message == "expired_access_token")
                        {
                            if (hasRefreshed)
                            {
                                throw;
                            }
                            else 
                            {
                                await RefreshAccessToken();
                                hasRefreshed = true;
                            }

                        }
                        else 
                        {
                            // dropbox maps 503 - ServiceUnavailable to be a rate limiting error.
                            // do not count a rate limiting error as an attempt
                            if (++attempt > maxRetries)
                            {
                                throw;
                            }
                        }
                        
                    }
                    catch (RateLimitException)
                    {
                        throw;
                    }
                    catch (RetryException)
                    {
                        // dropbox maps 503 - ServiceUnavailable to be a rate limiting error.
                        // do not count a rate limiting error as an attempt
                        if (++attempt > maxRetries)
                        {
                            throw;
                        }
                    }

                    // use exponential backoff
                    var backoff = TimeSpan.FromSeconds(Math.Pow(2, attempt) * r.NextDouble());
#if PORTABLE40
                    await TaskEx.Delay(backoff);
#else
                    await Task.Delay(backoff).ConfigureAwait(false);
#endif
                    if (body != null)
                    {
                        body.Position = 0;
                    }
                }
            }
            finally
            {
                if (body != null)
                {
                    body.Dispose();
                }
            }
        }

        /// <summary>
        /// Attempts to extract the value of a field named <c>error</c> from <paramref name="text"/>
        /// if it is a valid JSON object.
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <returns>The contents of the error field if present, otherwise <paramref name="text" />.</returns>
        private string CheckForError(string text)
        {
            try
            {
                var obj = JObject.Parse(text);
                JToken error;
                if (obj.TryGetValue("error", out error))
                {
                    return error.ToString();
                }

                return text;
            }
            catch (Exception)
            {
                return text;
            }
        }

        /// <summary>
        /// Requests the JSON string.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="auth">The auth type of the route.</param>
        /// <param name="routeStyle">The route style.</param>
        /// <param name="requestArg">The request argument.</param>
        /// <param name="body">The body to upload if <paramref name="routeStyle"/>
        /// is <see cref="RouteStyle.Upload"/>.</param>
        /// <returns>The asynchronous task with the result.</returns>
        private async Task<Result> RequestJsonString(
            string host,
            string routeName,
            string auth,
            RouteStyle routeStyle,
            string requestArg,
            Stream body = null)
        {
            var hostname = this.options.HostMap[host];
            var uri = this.GetRouteUri(hostname, routeName);

            var request = new HttpRequestMessage(HttpMethod.Post, uri);

            if (auth == AuthType.User || auth == AuthType.Team)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.options.OAuth2AccessToken);
            }
            else if (auth == AuthType.App)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", this.options.OAuth2AccessToken);
            }
            else if (auth == AuthType.NoAuth)
            {
            }
            else
            {
                throw new ArgumentException("Invalid auth type", auth);
            }

            request.Headers.TryAddWithoutValidation("User-Agent", this.options.UserAgent);

            if (this.selectUser != null)
            {
                request.Headers.TryAddWithoutValidation("Dropbox-Api-Select-User", this.selectUser);
            }

            if (this.selectAdmin != null)
            {
                request.Headers.TryAddWithoutValidation("Dropbox-Api-Select-Admin", this.selectAdmin);
            }

            if (this.pathRoot != null)
            {
                request.Headers.TryAddWithoutValidation(
                    "Dropbox-Api-Path-Root",
                    JsonWriter.Write(this.pathRoot, PathRoot.Encoder));
            }

            var completionOption = HttpCompletionOption.ResponseContentRead;

            switch (routeStyle)
            {
                case RouteStyle.Rpc:
                    request.Content = new StringContent(requestArg, Encoding.UTF8, "application/json");
                    break;
                case RouteStyle.Download:
                    request.Headers.Add(DropboxApiArgHeader, requestArg);

                    // This is required to force libcurl remove default content type header.
                    request.Content = new StringContent("");
                    request.Content.Headers.ContentType = null;

                    completionOption = HttpCompletionOption.ResponseHeadersRead;
                    break;
                case RouteStyle.Upload:
                    request.Headers.Add(DropboxApiArgHeader, requestArg);
                    if (body == null)
                    {
                        throw new ArgumentNullException("body");
                    }

                    request.Content = new CustomStreamContent(body);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    break;
                default:
                    throw new InvalidOperationException(string.Format(
                        CultureInfo.InvariantCulture,
                        "Unknown route style: {0}",
                        routeStyle));
            }

            var disposeResponse = true;
            var response = await this.getHttpClient(host).SendAsync(request, completionOption).ConfigureAwait(false);

            var requestId = GetRequestId(response);
            try
            {
                if ((int)response.StatusCode >= 500)
                {
                    var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    text = this.CheckForError(text);
                    throw new RetryException(requestId, (int)response.StatusCode, message: text, uri: uri);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    text = this.CheckForError(text);
                    throw new BadInputException(requestId, text, uri);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var reason = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw AuthException.Decode(reason, () => new AuthException(GetRequestId(response)));
                }
                else if ((int)response.StatusCode == 429)
                {
                    var reason = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw RateLimitException.Decode(reason, () => new RateLimitException(GetRequestId(response)));
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    var reason = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw AccessException.Decode(reason, () => new AccessException(GetRequestId(response)));
                }
                else if ((int)response.StatusCode == 422)
                {
                    var reason = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw PathRootException.Decode(reason, () => new PathRootException(GetRequestId(response)));
                }
                else if (response.StatusCode == HttpStatusCode.Conflict ||
                    response.StatusCode == HttpStatusCode.NotFound)
                {
                    var reason = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    return new Result
                    {
                        IsError = true,
                        ObjectResult = reason,
                        RequestId = GetRequestId(response)
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
                            ObjectResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false)
                        };
                    }
                }
                else
                {
                    var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    text = this.CheckForError(text);
                    throw new HttpException(requestId, (int)response.StatusCode, text, uri);
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

        private async Task<bool> CheckAndRefreshAccessToken()
        {
            bool canRefresh = this.options.OAuth2RefreshToken != null && this.options.AppKey != null;
            bool needsRefresh = this.options.OAuth2AccessTokenExpiresAt.HasValue && DateTime.Now.AddSeconds(TokenExpirationBuffer) >= this.options.OAuth2AccessTokenExpiresAt.Value;
            bool needsToken = this.options.OAuth2AccessToken == null;
            if ((needsRefresh || needsToken) && canRefresh)
            {
                return await RefreshAccessToken();
            }
            return false;
        }
        public async Task<bool> RefreshAccessToken(string[] scopeList = null) 
        {
            if (this.options.OAuth2RefreshToken == null || this.options.AppKey == null)
            {
                // Cannot refresh token if you do not have at a minimum refresh token and app key
                return false;
            }

            var url = "https://api.dropbox.com/oauth2/token";

            var parameters = new Dictionary<string, string>
                {
                    { "refresh_token", this.options.OAuth2RefreshToken} ,
                    { "grant_type", "refresh_token" },
                    { "client_id", this.options.AppKey }
                };

            if (!string.IsNullOrEmpty(this.options.AppSecret))
            {
                parameters["client_secret"] = this.options.AppSecret;
            }

            if (scopeList != null)
            {
                parameters["scope"] =  String.Join(" ", scopeList);
            }

            var bodyContent = new FormUrlEncodedContent(parameters);

            var response = await this.defaultHttpClient.PostAsync(url, bodyContent).ConfigureAwait(false);
            //if response is an invalid grant, we want to throw this exception rather than the one thrown in 
            // response.EnsureSuccessStatusCode();
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var reason = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (reason == "invalid_grant")
                {
                    throw AuthException.Decode(reason, () => new AuthException(GetRequestId(response)));
                }
            }
            
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                string accessToken = json["access_token"].ToString();
                DateTime tokenExpiration = DateTime.Now.AddSeconds(json["expires_in"].ToObject<int>());
                this.options.OAuth2AccessToken = accessToken;
                this.options.OAuth2AccessTokenExpiresAt = tokenExpiration;
                return true;
            }
            return false;
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
        /// Gets the Dropbox request id.
        /// </summary>
        /// <param name="response">Response.</param>
        /// <returns>The request id.</returns>
        private string GetRequestId(HttpResponseMessage response)
        {
            IEnumerable<string> requestId;

            if (response.Headers.TryGetValues("X-Dropbox-Request-Id", out requestId))
            {
                return requestId.FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// Get http client for given host.
        /// </summary>
        /// <param name="host">The host type.</param>
        /// <returns>The <see cref="HttpClient"/>.</returns>
        private HttpClient getHttpClient(string host)
        {
            if (host == HostType.ApiNotify)
            {
                return this.options.LongPollHttpClient ?? this.defaultLongPollHttpClient;
            }
            else
            {
                return this.options.HttpClient ?? this.defaultHttpClient;
            }
        }

        /// <summary>
        /// The actual disposing logic.
        /// </summary>
        /// <param name="disposing">If is disposing.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // HttpClient is safe for multiple disposal.
                this.defaultHttpClient.Dispose();
                this.defaultLongPollHttpClient.Dispose();
            }
        }

        /// <summary>
        /// The public dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
            /// Gets or sets the Dropbox request id.
            /// </summary>
            /// <value>The request id.</value>
            public string RequestId { get; set; }

            /// <summary>
            /// Gets or sets the HTTP response, this is only set if the route was a download route.
            /// </summary>
            /// <value>
            /// The HTTP response.
            /// </value>
            public HttpResponseMessage HttpResponse { get; set; }
        }

        /// <summary>
        /// An implementation of the <see cref="T:Dropbox.Api.Stone.IDownloadResponse`1"/> interface.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        private class DownloadResponse<TResponse> : IDownloadResponse<TResponse>
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
    /// The stream content which doesn't dispose the underlying stream. This
    /// is useful for retry.
    /// </summary>
    internal class CustomStreamContent : StreamContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomStreamContent"/> class.
        /// </summary>
        /// <param name="content">The stream content.</param>
        public CustomStreamContent(Stream content)
            : base(content)
        {
        }

        protected override void Dispose(bool disposing)
        {
            // Do not dispose the stream.
        }
    }

    /// <summary>
    /// The type of api hosts.
    /// </summary>
    internal class HostType
    {
        /// <summary>
        /// Host type for api.
        /// </summary>
        public const string Api = "api";

        /// <summary>
        /// Host type for api content.
        /// </summary>
        public const string ApiContent = "content";

        /// <summary>
        /// Host type for api notify.
        /// </summary>
        public const string ApiNotify = "notify";
    }

    /// <summary>
    /// The type of api auth.
    /// </summary>
    internal class AuthType
    {
        /// <summary>
        /// Auth type for user auth.
        /// </summary>
        public const string User = "user";

        /// <summary>
        /// Auth type for team auth.
        /// </summary>
        public const string Team = "team";

        /// <summary>
        /// Host type for app auth.
        /// </summary>
        public const string App = "app";

        /// <summary>
        /// Host type for no auth.
        /// </summary>
        public const string NoAuth = "noauth";
    }

    /// <summary>
    /// The class which contains configurations for the request handler.
    /// </summary>
    internal sealed class DropboxRequestHandlerOptions
    {
        /// <summary>
        /// The default api domain
        /// </summary>
        private const string DefaultApiDomain = "api.dropboxapi.com";

        /// <summary>
        /// The default api content domain
        /// </summary>
        private const string DefaultApiContentDomain = "content.dropboxapi.com";

        /// <summary>
        /// The default api notify domain
        /// </summary>
        private const string DefaultApiNotifyDomain = "notify.dropboxapi.com";

        /// <summary>
        /// The base user agent, used to construct all user agent strings.
        /// </summary>
        private const string BaseUserAgent = "OfficialDropboxDotNetSDKv2";

        public DropboxRequestHandlerOptions(DropboxClientConfig config, string oauth2AccessToken, string oauth2RefreshToken, Nullable<DateTime> oauth2AccessTokenExpiresAt, string appKey, string appSecret)
            : this(
            oauth2AccessToken,
            oauth2RefreshToken,
            oauth2AccessTokenExpiresAt,
            appKey,
            appSecret,
            config.MaxRetriesOnError,
            config.UserAgent,
            DefaultApiDomain,
            DefaultApiContentDomain,
            DefaultApiNotifyDomain,
            config.HttpClient,
            config.LongPollHttpClient)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DropboxRequestHandlerOptions"/> class.
        /// </summary>
        /// <param name="oauth2AccessToken">The oauth2 access token for making client requests.</param>
        /// <param name="oauth2RefreshToken">The oauth2 refresh token for refreshing access tokens</param>
        /// <param name="oauth2AccessTokenExpiresAt">The time when the current access token expires, can be null if using long-lived tokens</param>
        /// <param name="appKey">The app key to be used for refreshing tokens</param>
        /// <param name="appSecret">The app secret to be used for refreshing tokens</param>
        /// <param name="maxRetriesOnError">The maximum retries on a 5xx error.</param>
        /// <param name="userAgent">The user agent to use when making requests.</param>
        /// <param name="apiHostname">The hostname that will process api requests;
        /// this is for internal Dropbox use only.</param>
        /// <param name="apiContentHostname">The hostname that will process api content requests;
        /// this is for internal Dropbox use only.</param>
        /// <param name="apiNotifyHostname">The hostname that will process api notify requests;
        /// this is for internal Dropbox use only.</param>
        /// <param name="httpClient">The custom http client. If not provided, a default 
        /// http client will be created.</param>
        /// <param name="longPollHttpClient">The custom http client for long poll. If not provided, a default 
        /// http client with longer timeout will be created.</param>
        public DropboxRequestHandlerOptions(
            string oauth2AccessToken,
            string oauth2RefreshToken,
            Nullable<DateTime> oauth2AccessTokenExpiresAt,
            string appKey,
            string appSecret,
            int maxRetriesOnError,
            string userAgent,
            string apiHostname,
            string apiContentHostname,
            string apiNotifyHostname,
            HttpClient httpClient,
            HttpClient longPollHttpClient)
        {
            var type = typeof(DropboxRequestHandlerOptions);
#if PORTABLE40
            var assembly = type.Assembly;
#else
            var assembly = type.GetTypeInfo().Assembly;
#endif
            var name = new AssemblyName(assembly.FullName);
            var sdkVersion = name.Version.ToString();

            this.UserAgent = userAgent == null
                ? string.Join("/", BaseUserAgent, sdkVersion)
                : string.Join("/", userAgent, BaseUserAgent, sdkVersion);

            this.HttpClient = httpClient;
            this.LongPollHttpClient = longPollHttpClient;
            this.OAuth2AccessToken = oauth2AccessToken;
            this.OAuth2RefreshToken = oauth2RefreshToken;
            this.OAuth2AccessTokenExpiresAt = oauth2AccessTokenExpiresAt;
            this.AppKey = appKey;
            this.AppSecret = appSecret;
            this.MaxClientRetries = maxRetriesOnError;
            this.HostMap = new Dictionary<string, string>
            {
                { HostType.Api, apiHostname },
                { HostType.ApiContent, apiContentHostname },
                { HostType.ApiNotify, apiNotifyHostname }
            };
        }

        /// <summary>
        /// Gets the maximum retries on a 5xx error.
        /// </summary>
        public int MaxClientRetries { get; private set; }

        /// <summary>
        /// Gets the OAuth2 token.
        /// </summary>
        public string OAuth2AccessToken { get; set; }

        /// <summary>
        /// Get the OAuth2 refresh token
        /// </summary>
        public string OAuth2RefreshToken { get; private set; }

        /// <summary>
        /// Gets the time the access token expires at
        /// </summary>
        public Nullable<DateTime> OAuth2AccessTokenExpiresAt { get; set; }

        /// <summary>
        /// Gets the app key to use when refreshing tokens
        /// </summary>
        public string AppKey { get; private set; }

        /// <summary>
        /// Gets the app secret to use when refreshing tokens
        /// </summary>
        public string AppSecret { get; private set; }

        /// <summary>
        /// Gets the HTTP client use to send requests to the server.
        /// </summary>
        public HttpClient HttpClient { get; private set; }

        /// <summary>
        /// Gets the HTTP client use to send long poll requests to the server.
        /// </summary>
        public HttpClient LongPollHttpClient { get; private set; }

        /// <summary>
        /// Gets the user agent string.
        /// </summary>
        public string UserAgent { get; private set; }

        /// <summary>
        /// Gets the maps from host types to domain names.
        /// </summary>
        public IDictionary<string, string> HostMap { get; private set; }
    }
}
