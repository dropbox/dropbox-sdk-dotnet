//-----------------------------------------------------------------------------
// <copyright file="DropboxExceptionTests.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Unit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Dropbox.Api.Files;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests HTTP response metadata exposed on Dropbox exceptions.
    /// </summary>
    [TestClass]
    public class DropboxExceptionTests
    {
        /// <summary>
        /// Verifies direct HTTP exceptions expose their Dropbox request ID.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestBadInputExceptionExposesRequestId()
        {
            var handler = new ResponseHandler(() => CreateResponse(HttpStatusCode.BadRequest, "bad input"));
            using (var client = CreateClient(handler))
            {
                var exception = await CaptureExceptionAsync<BadInputException>(
                    () => client.Files.GetMetadataAsync("/test.txt")).ConfigureAwait(false);

                Assert.AreEqual("request-123", exception.RequestId);
            }
        }

        /// <summary>
        /// Verifies structured API exceptions expose their Dropbox request ID.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestApiExceptionExposesRequestId()
        {
            const string Body = "{\"error_summary\":\"path/not_found/..\",\"error\":{\".tag\":\"path\",\"path\":{\".tag\":\"not_found\"}}}";
            var handler = new ResponseHandler(() => CreateResponse(HttpStatusCode.Conflict, Body));
            using (var client = CreateClient(handler))
            {
                var exception = await CaptureExceptionAsync<ApiException<GetMetadataError>>(
                    () => client.Files.GetMetadataAsync("/missing.txt")).ConfigureAwait(false);

                Assert.AreEqual("request-123", exception.RequestId);
                Assert.IsTrue(exception.ErrorResponse.IsPath);
            }
        }

        /// <summary>
        /// Verifies decoded rate-limit exceptions expose their Dropbox request ID.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestRateLimitExceptionExposesRequestId()
        {
            const string Body = "{\"error_summary\":\"too_many_requests/..\",\"error\":{\"reason\":{\".tag\":\"too_many_requests\"},\"retry_after\":100}}";
            var handler = new ResponseHandler(() => CreateResponse((HttpStatusCode)429, Body));
            using (var client = CreateClient(handler))
            {
                var exception = await CaptureExceptionAsync<RateLimitException>(
                    () => client.Files.GetMetadataAsync("/test.txt")).ConfigureAwait(false);

                Assert.AreEqual("request-123", exception.RequestId);
                Assert.AreEqual(100, exception.RetryAfter);
            }
        }

        /// <summary>
        /// Verifies OAuth refresh exceptions expose their Dropbox request ID.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestRefreshAuthExceptionExposesRequestId()
        {
            const string Body = "{\"error\":\"invalid_grant\",\"error_description\":\"Refresh token is invalid.\"}";
            var handler = new ResponseHandler(() => CreateResponse(HttpStatusCode.BadRequest, Body));
            var config = new DropboxClientConfig { HttpClient = new HttpClient(handler) };
            using (var client = new DropboxClient("refresh-token", "app-key", "app-secret", config))
            {
                var exception = await CaptureExceptionAsync<AuthException>(
                    () => client.RefreshAccessToken(null)).ConfigureAwait(false);

                Assert.AreEqual("request-123", exception.RequestId);
            }
        }

        /// <summary>Verifies actual statuses and concrete types across all route styles.</summary>
        /// <param name="status">The response status.</param>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        [DataRow(400)]
        [DataRow(401)]
        [DataRow(403)]
        [DataRow(404)]
        [DataRow(409)]
        [DataRow(422)]
        [DataRow(429)]
        [DataRow(500)]
        [DataRow(503)]
        [DataRow(418)]
        public async Task TestMetadataAcrossRouteStyles(int status)
        {
            foreach (var style in new[] { "rpc", "upload", "download" })
            {
                var handler = new ResponseHandler(() => CreateResponse((HttpStatusCode)status, ErrorBody(status, style)));
                using (var client = CreateClient(handler))
                {
                    var exception = await CaptureExceptionAsync<DropboxException>(() => InvokeRoute(client, style)).ConfigureAwait(false);
                    Assert.AreEqual(ExpectedType(status, style), exception.GetType());
                    AssertMetadata(exception, status);
                    Assert.IsTrue(((TrackedResponse)handler.LastResponse).Disposed);
                    Assert.AreEqual(1, handler.RequestCount);
                    Assert.IsFalse(exception.Message.Contains("private-timing"));
                    Assert.IsFalse(exception.ToString().Contains("private-timing"));
                    Assert.IsFalse(exception.ToString().Contains("X-Server-Response-Time"));
                    if (exception is HttpException http)
                    {
                        Assert.AreEqual(status, http.StatusCode);
                        Assert.IsNotNull(http.RequestUri);
                    }

                    if (exception is RateLimitException rateLimit)
                    {
                        Assert.AreEqual(100, rateLimit.RetryAfter);
                    }
                }
            }
        }

        /// <summary>Verifies absent responses and absent headers have distinct status behavior.</summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestAbsentResponseAndHeaders()
        {
            var local = new DropboxException(null, "local failure");
            Assert.IsNull(local.ResponseStatusCode);
            Assert.AreEqual(0, local.ResponseHeaders.Count);
            AssertReadOnly(local.ResponseHeaders);

            var handler = new ResponseHandler(() =>
            {
                var response = CreateResponse(HttpStatusCode.BadRequest, "bad input");
                response.Headers.Clear();
                response.Content.Headers.Clear();
                return response;
            });
            using (var client = CreateClient(handler))
            {
                var exception = await CaptureExceptionAsync<BadInputException>(() => client.Files.DownloadAsync("/test.txt")).ConfigureAwait(false);
                Assert.AreEqual<int?>(400, exception.ResponseStatusCode);
                Assert.IsNull(exception.RequestId);
                Assert.AreEqual(0, exception.ResponseHeaders.Count);
                AssertReadOnly(exception.ResponseHeaders);
            }
        }

        /// <summary>Verifies neither input collections nor public collection casts can mutate the snapshot.</summary>
        [TestMethod]
        public void TestSnapshotIsDeeplyReadOnly()
        {
            var values = new List<string> { "first", "second" };
            var source = new Dictionary<string, IEnumerable<string>> { { "X-Test", values } };
            var metadata = new HttpResponseMetadata(503, source);
            values[0] = "changed";
            values.Add("third");
            source.Clear();
            CollectionAssert.AreEqual(new[] { "first", "second" }, metadata.Headers["x-test"].ToArray());
            AssertReadOnly(metadata.Headers);
            Assert.ThrowsExactly<NotSupportedException>(() => ((IList<string>)metadata.Headers["X-Test"])[0] = "changed");
            Assert.ThrowsExactly<NotSupportedException>(() => ((IList<string>)metadata.Headers["X-Test"]).Add("third"));

            using (var response = CreateResponse(HttpStatusCode.BadRequest, "bad input"))
            {
                var snapshot = HttpResponseMetadata.FromResponse(response);
                response.Headers.Remove("X-Server-Response-Time");
                response.Content.Headers.ContentType = null;
                Assert.AreEqual(2, snapshot.Headers["X-Server-Response-Time"].Count);
                Assert.AreEqual("application/json; charset=utf-8", snapshot.Headers["Content-Type"][0]);
            }
        }

        /// <summary>Verifies final response metadata and the existing retry count for transient/auth errors.</summary>
        /// <param name="status">The retryable response status.</param>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        [DataRow(401)]
        [DataRow(500)]
        [DataRow(503)]
        public async Task TestRetryExhaustion(int status)
        {
            var attempts = 0;
            var responses = new List<TrackedResponse>();
            var handler = new ResponseHandler(() =>
            {
                var response = (TrackedResponse)CreateResponse((HttpStatusCode)status, ErrorBody(status, "rpc"));
                response.Headers.Remove("X-Dropbox-Request-Id");
                response.Headers.Add("X-Dropbox-Request-Id", "attempt-" + (++attempts));
                response.Headers.Add("X-Attempt", attempts.ToString(System.Globalization.CultureInfo.InvariantCulture));
                responses.Add(response);
                return response;
            });
            using (var client = CreateClient(handler, maxRetries: 1))
            {
                var exception = await CaptureExceptionAsync<DropboxException>(() => client.Files.GetMetadataAsync("/test.txt")).ConfigureAwait(false);
                Assert.AreEqual(ExpectedType(status, "rpc"), exception.GetType());
                Assert.AreEqual(2, handler.RequestCount);
                Assert.AreEqual("attempt-2", exception.RequestId);
                Assert.AreEqual<int?>(status, exception.ResponseStatusCode);
                CollectionAssert.AreEqual(new[] { "2" }, exception.ResponseHeaders["X-Attempt"].ToArray());
                Assert.IsTrue(responses.All(response => response.Disposed));
            }
        }

        /// <summary>Verifies RetryAfter comes from the body and rate limits never auto-retry.</summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestRateLimitDoesNotRetryOrUseHeaderForBackoff()
        {
            var handler = new ResponseHandler(() =>
            {
                var response = CreateResponse((HttpStatusCode)429, ErrorBody(429, "rpc"));
                response.Headers.Add("Retry-After", "999");
                return response;
            });
            using (var client = CreateClient(handler, maxRetries: 2))
            {
                var exception = await CaptureExceptionAsync<RateLimitException>(() => client.Files.GetMetadataAsync("/test.txt")).ConfigureAwait(false);
                Assert.AreEqual(100, exception.RetryAfter);
                Assert.AreEqual("999", exception.ResponseHeaders["retry-after"][0]);
                Assert.AreEqual(1, handler.RequestCount);
            }
        }

        /// <summary>Verifies malformed structured responses retain context and the decoder cause.</summary>
        /// <param name="body">The invalid error body.</param>
        /// <param name="causeType">The original decoder exception type.</param>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        [DataRow("", typeof(InvalidOperationException))]
        [DataRow("null", typeof(InvalidOperationException))]
        [DataRow("{", typeof(InvalidOperationException))]
        [DataRow("not-json", typeof(Newtonsoft.Json.JsonReaderException))]
        [DataRow("{\"error_summary\":42}", typeof(InvalidCastException))]
        public async Task TestMalformedStructuredResponses(string body, Type causeType)
        {
            foreach (var status in new[] { 401, 403, 404, 409, 422, 429 })
            {
                foreach (var style in new[] { "rpc", "upload", "download" })
                {
                    var handler = new ResponseHandler(() => CreateResponse((HttpStatusCode)status, body));
                    using (var client = CreateClient(handler))
                    {
                        var exception = await CaptureExceptionAsync<DropboxException>(() => InvokeRoute(client, style)).ConfigureAwait(false);
                        Assert.AreEqual(typeof(DropboxException), exception.GetType());
                        Assert.AreEqual(causeType, exception.InnerException.GetType());
                        AssertMetadata(exception, status);
                        Assert.IsTrue(((TrackedResponse)handler.LastResponse).Disposed);
                        Assert.AreEqual(1, handler.RequestCount);
                    }
                }
            }
        }

        /// <summary>Preserves the existing permissive decoding of objects with missing error fields.</summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestMissingStructuredFieldsPreservesExceptionType()
        {
            foreach (var status in new[] { 401, 403, 409, 422, 429 })
            {
                var handler = new ResponseHandler(() => CreateResponse((HttpStatusCode)status, "{}"));
                using (var client = CreateClient(handler))
                {
                    var exception = await CaptureExceptionAsync<DropboxException>(() => client.Files.GetMetadataAsync("/test.txt")).ConfigureAwait(false);
                    Assert.AreEqual(ExpectedType(status, "rpc"), exception.GetType());
                    Assert.IsNull(exception.InnerException);
                    Assert.IsNull(exception.Message);
                    AssertMetadata(exception, status);
                }
            }
        }

        /// <summary>Verifies OAuth invalid_grant retains actual 400/401 status and response metadata.</summary>
        /// <param name="status">The OAuth response status.</param>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        [DataRow(400)]
        [DataRow(401)]
        public async Task TestOAuthResponseMetadata(int status)
        {
            const string Body = "{\"error\":\"invalid_grant\",\"error_description\":\"Refresh token is invalid.\"}";
            var handler = new ResponseHandler(() => CreateResponse((HttpStatusCode)status, Body));
            var config = new DropboxClientConfig { HttpClient = new HttpClient(handler) };
            using (var client = new DropboxClient("refresh-token", "app-key", "app-secret", config))
            {
                var exception = await CaptureExceptionAsync<AuthException>(() => client.RefreshAccessToken(null)).ConfigureAwait(false);
                AssertMetadata(exception, status);
                Assert.AreEqual("Refresh token is invalid.", exception.Message);
                Assert.IsTrue(((TrackedResponse)handler.LastResponse).Disposed);
                Assert.AreEqual(1, handler.RequestCount);
            }
        }

        private static void AssertMetadata(DropboxException exception, int status)
        {
            Assert.AreEqual<int?>(status, exception.ResponseStatusCode);
            Assert.AreEqual("request-123", exception.RequestId);
            Assert.AreEqual("request-123", exception.ResponseHeaders["x-dropbox-request-id"][0]);
            CollectionAssert.AreEqual(new[] { "private-timing-one", "private-timing-two" }, exception.ResponseHeaders["x-SERVER-response-TIME"].ToArray());
            Assert.AreEqual("application/json; charset=utf-8", exception.ResponseHeaders["content-type"][0]);
            AssertReadOnly(exception.ResponseHeaders);
        }

        private static void AssertReadOnly(IReadOnlyDictionary<string, IReadOnlyList<string>> headers)
        {
            Assert.ThrowsExactly<NotSupportedException>(() => ((IDictionary<string, IReadOnlyList<string>>)headers).Add("new", new[] { "value" }));
        }

        private static Task InvokeRoute(DropboxClient client, string style)
        {
            switch (style)
            {
                case "upload":
                    return client.Files.UploadAsync(new UploadArg("/test.txt"), new MemoryStream(new byte[] { 1 }));
                case "download":
                    return client.Files.DownloadAsync("/test.txt");
                default:
                    return client.Files.GetMetadataAsync("/test.txt");
            }
        }

        private static string ErrorBody(int status, string style)
        {
            if (status == 429)
            {
                return "{\"error_summary\":\"too_many_requests/..\",\"error\":{\"reason\":{\".tag\":\"too_many_requests\"},\"retry_after\":100}}";
            }

            if (status == 409 || status == 404)
            {
                return style == "upload"
                    ? "{\"error_summary\":\"payload_too_large/..\",\"error\":{\".tag\":\"payload_too_large\"}}"
                    : "{\"error_summary\":\"path/not_found/..\",\"error\":{\".tag\":\"path\",\"path\":{\".tag\":\"not_found\"}}}";
            }

            return "{\"error_summary\":\"test-error\",\"error\":{\".tag\":\"other\"}}";
        }

        private static Type ExpectedType(int status, string style)
        {
            switch (status)
            {
                case 400: return typeof(BadInputException);
                case 401: return typeof(AuthException);
                case 403: return typeof(AccessException);
                case 404:
                case 409:
                    if (style == "upload")
                    {
                        return typeof(ApiException<UploadError>);
                    }

                    return style == "download" ? typeof(ApiException<DownloadError>) : typeof(ApiException<GetMetadataError>);
                case 422: return typeof(PathRootException);
                case 429: return typeof(RateLimitException);
                case 500:
                case 503: return typeof(RetryException);
                default: return typeof(HttpException);
            }
        }

        private static DropboxClient CreateClient(ResponseHandler handler, int maxRetries = 0)
        {
            return new DropboxClient(
                "access-token",
                new DropboxClientConfig
                {
                    HttpClient = new HttpClient(handler),
                    MaxRetriesOnError = maxRetries,
                });
        }

        private static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string body)
        {
            var response = new TrackedResponse(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };
            response.Headers.Add("X-Dropbox-Request-Id", "request-123");
            response.Headers.Add("X-Server-Response-Time", new[] { "private-timing-one", "private-timing-two" });
            return response;
        }

        private static async Task<TException> CaptureExceptionAsync<TException>(Func<Task> action)
            where TException : Exception
        {
            try
            {
                await action().ConfigureAwait(false);
            }
            catch (TException exception)
            {
                return exception;
            }

            Assert.Fail(string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "Expected an exception of type {0}.",
                typeof(TException).FullName));
            return null;
        }

        private sealed class TrackedResponse : HttpResponseMessage
        {
            public TrackedResponse(HttpStatusCode status)
                : base(status)
            {
            }

            public bool Disposed { get; private set; }

            protected override void Dispose(bool disposing)
            {
                this.Disposed = true;
                base.Dispose(disposing);
            }
        }

        private sealed class ResponseHandler : HttpMessageHandler
        {
            private readonly Func<HttpResponseMessage> responseFactory;

            public ResponseHandler(Func<HttpResponseMessage> responseFactory)
            {
                this.responseFactory = responseFactory;
            }

            public int RequestCount { get; private set; }

            public HttpResponseMessage LastResponse { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                this.RequestCount++;
                this.LastResponse = this.responseFactory();
                return Task.FromResult(this.LastResponse);
            }
        }
    }
}
