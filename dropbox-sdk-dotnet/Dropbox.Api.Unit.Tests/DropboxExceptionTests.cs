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
    using Dropbox.Api.Stone;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests HTTP response metadata exposed on Dropbox exceptions.
    /// </summary>
    [TestClass]
    public class DropboxExceptionTests
    {
        private const string Error = "{\"error_summary\":\"test-error\",\"error\":{\".tag\":\"other\"}}";
        private const string PathError = "{\"error_summary\":\"path/not_found/..\",\"error\":{\".tag\":\"path\",\"path\":{\".tag\":\"not_found\"}}}";
        private const string RateError = "{\"error_summary\":\"too_many_requests/..\",\"error\":{\"reason\":{\".tag\":\"too_many_requests\"},\"retry_after\":100}}";

        /// <summary>Verifies actual statuses, concrete types, and metadata on the shared HTTP error path.</summary>
        /// <param name="status">The response status.</param>
        /// <param name="body">The response body.</param>
        /// <param name="exceptionType">The expected exception type.</param>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        [DataRow(400, "bad input", typeof(BadInputException))]
        [DataRow(401, Error, typeof(AuthException))]
        [DataRow(403, Error, typeof(AccessException))]
        [DataRow(404, PathError, typeof(ApiException<GetMetadataError>))]
        [DataRow(409, PathError, typeof(ApiException<GetMetadataError>))]
        [DataRow(418, "teapot", typeof(HttpException))]
        [DataRow(422, Error, typeof(PathRootException))]
        [DataRow(429, RateError, typeof(RateLimitException))]
        [DataRow(500, "server error", typeof(RetryException))]
        [DataRow(503, "unavailable", typeof(RetryException))]
        public async Task TestHttpErrors(int status, string body, Type exceptionType)
        {
            var handler = new ResponseHandler(() => CreateResponse((HttpStatusCode)status, body));
            using (var client = CreateClient(handler))
            {
                var exception = await Assert.ThrowsAsync<DropboxException>(() => client.Files.GetMetadataAsync("/test.txt")).ConfigureAwait(false);
                Assert.AreEqual(exceptionType, exception.GetType());
                AssertMetadata(exception, status);
                Assert.IsTrue(((TrackedResponse)handler.LastResponse).Disposed);
                Assert.AreEqual(1, handler.RequestCount);
                Assert.IsFalse(exception.Message.Contains("private-timing"));
                Assert.IsFalse(exception.ToString().Contains("X-Server-Response-Time"));
                Assert.IsFalse(exception.ToString().Contains("private-timing"));
                if (exception is HttpException http)
                {
                    Assert.AreEqual(status, http.StatusCode);
                    Assert.AreEqual(body, http.Message);
                    Assert.IsNotNull(http.RequestUri);
                }

                if (exception is ApiException<GetMetadataError> api)
                {
                    Assert.IsTrue(api.ErrorResponse.IsPath);
                    Assert.AreEqual("path/not_found/..", api.Message);
                }
            }
        }

        /// <summary>Verifies upload and download errors carry metadata through the internal result.</summary>
        /// <param name="style">The route style.</param>
        /// <param name="exceptionType">The expected structured exception type.</param>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        [DataRow("upload", typeof(ApiException<UploadError>))]
        [DataRow("download", typeof(ApiException<DownloadError>))]
        public async Task TestContentRouteErrors(string style, Type exceptionType)
        {
            var body = style == "upload" ? "{\"error\":{\".tag\":\"payload_too_large\"}}" : PathError;
            var handler = new ResponseHandler(() => CreateResponse(HttpStatusCode.Conflict, body));
            using (var client = CreateClient(handler))
            {
                var exception = await Assert.ThrowsAsync<DropboxException>(() => InvokeRoute(client, style)).ConfigureAwait(false);
                Assert.AreEqual(exceptionType, exception.GetType());
                AssertMetadata(exception, 409);
                Assert.IsTrue(((TrackedResponse)handler.LastResponse).Disposed);
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
                var exception = await Assert.ThrowsAsync<BadInputException>(() => client.Files.DownloadAsync("/test.txt")).ConfigureAwait(false);
                Assert.AreEqual<int?>(400, exception.ResponseStatusCode);
                Assert.IsNull(exception.RequestId);
                Assert.AreEqual(0, exception.ResponseHeaders.Count);
                AssertReadOnly(exception.ResponseHeaders);
            }
        }

        /// <summary>Verifies the public snapshot is independent of response headers and input lists.</summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestSnapshotIsDeeplyReadOnly()
        {
            var values = new List<string> { "first", "second" };
            var response = CreateResponse(HttpStatusCode.BadRequest, "bad input");
            response.Headers.Add("X-Test", values);
            response.Content.Headers.Add("x-test", "third");
            var handler = new ResponseHandler(() => response);
            using (var client = CreateClient(handler))
            {
                var exception = await Assert.ThrowsAsync<BadInputException>(() => client.Files.GetMetadataAsync("/test.txt")).ConfigureAwait(false);
                values[0] = "changed";
                response.Headers.Remove("X-Test");
                response.Content.Headers.ContentType = null;
                CollectionAssert.AreEqual(new[] { "first", "second", "third" }, exception.ResponseHeaders["x-test"].ToArray());
                AssertMetadata(exception, 400);
                Assert.ThrowsExactly<NotSupportedException>(() => ((IList<string>)exception.ResponseHeaders["X-Test"])[0] = "changed");
                Assert.ThrowsExactly<NotSupportedException>(() => ((IList<string>)exception.ResponseHeaders["X-Test"]).Add("third"));
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
                var response = (TrackedResponse)CreateResponse((HttpStatusCode)status, Error);
                response.Headers.Remove("X-Dropbox-Request-Id");
                response.Headers.Add("X-Dropbox-Request-Id", "attempt-" + (++attempts));
                responses.Add(response);
                return response;
            });
            using (var client = CreateClient(handler, maxRetries: 1))
            {
                var exception = await Assert.ThrowsAsync<DropboxException>(() => client.Files.GetMetadataAsync("/test.txt")).ConfigureAwait(false);
                Assert.AreEqual(status == 401 ? typeof(AuthException) : typeof(RetryException), exception.GetType());
                Assert.AreEqual(2, handler.RequestCount);
                Assert.AreEqual("attempt-2", exception.RequestId);
                Assert.AreEqual<int?>(status, exception.ResponseStatusCode);
                Assert.AreEqual("attempt-2", exception.ResponseHeaders["X-Dropbox-Request-Id"][0]);
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
                var response = CreateResponse((HttpStatusCode)429, RateError);
                response.Headers.Add("Retry-After", "999");
                return response;
            });
            using (var client = CreateClient(handler, maxRetries: 2))
            {
                var exception = await Assert.ThrowsAsync<RateLimitException>(() => client.Files.GetMetadataAsync("/test.txt")).ConfigureAwait(false);
                AssertMetadata(exception, 429);
                Assert.AreEqual(100, exception.RetryAfter);
                Assert.AreEqual("999", exception.ResponseHeaders["retry-after"][0]);
                Assert.AreEqual(1, handler.RequestCount);
            }
        }

        /// <summary>Verifies malformed structured responses report decoding failures without wrapping them.</summary>
        /// <param name="body">The invalid error body.</param>
        /// <param name="exceptionType">The original decoder exception type.</param>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        [DataRow("", typeof(InvalidOperationException))]
        [DataRow("null", typeof(Newtonsoft.Json.JsonReaderException))]
        [DataRow("{", typeof(Newtonsoft.Json.JsonReaderException))]
        [DataRow("not-json", typeof(Newtonsoft.Json.JsonReaderException))]
        [DataRow("{\"error_summary\":]}", typeof(Newtonsoft.Json.JsonReaderException))]
        [DataRow("{\"error_summary\":", typeof(Newtonsoft.Json.JsonReaderException))]
        [DataRow("{\"unknown\":", typeof(Newtonsoft.Json.JsonReaderException))]
        [DataRow("{\"unknown\":[[1],", typeof(Newtonsoft.Json.JsonReaderException))]
        [DataRow("{\"error\":{\".tag\":", typeof(Newtonsoft.Json.JsonReaderException))]
        [DataRow("{\"error_summary\":42}", typeof(InvalidCastException))]
        public async Task TestMalformedStructuredResponses(string body, Type exceptionType)
        {
            foreach (var status in new[] { 401, 409 })
            {
                foreach (var style in new[] { "rpc", "upload", "download" })
                {
                    var handler = new ResponseHandler(() => CreateResponse((HttpStatusCode)status, body));
                    using (var client = CreateClient(handler))
                    {
                        var exception = await Assert.ThrowsAsync<Exception>(() => InvokeRoute(client, style)).ConfigureAwait(false);
                        Assert.AreEqual(exceptionType, exception.GetType());
                        Assert.IsNull(exception.InnerException);
                        Assert.IsTrue(((TrackedResponse)handler.LastResponse).Disposed);
                        Assert.AreEqual(1, handler.RequestCount);
                    }
                }
            }
        }

        /// <summary>Verifies a truncated array in a successful response fails decoding without hanging.</summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestTruncatedArrayInSuccessResponse()
        {
            var handler = new ResponseHandler(() => CreateResponse(HttpStatusCode.OK, "{\"template_ids\":[\"id\","));
            using (var client = CreateClient(handler))
            {
                await Assert.ThrowsExactlyAsync<Newtonsoft.Json.JsonReaderException>(
                    () => client.FileProperties.TemplatesListForUserAsync()).ConfigureAwait(false);
                Assert.IsTrue(((TrackedResponse)handler.LastResponse).Disposed);
                Assert.AreEqual(1, handler.RequestCount);
            }
        }

        /// <summary>Verifies decoder and factory failures propagate without changing the exception instance.</summary>
        /// <param name="exceptionType">The failure type to inject.</param>
        [TestMethod]
        [DataRow(typeof(InvalidOperationException))]
        [DataRow(typeof(InvalidCastException))]
        [DataRow(typeof(OverflowException))]
        [DataRow(typeof(NullReferenceException))]
        [DataRow(typeof(IOException))]
        public void TestUnrelatedDecodingFailuresAreNotWrapped(Type exceptionType)
        {
            var failure = (Exception)Activator.CreateInstance(exceptionType, "unrelated failure");
            var decoder = new FailingDecoder(failure);
            var decoded = Assert.Throws<Exception>(() =>
                StructuredException<GetMetadataError>.Decode("{\"error\":{}}", decoder, () => new ApiException<GetMetadataError>(null)));
            Assert.AreSame(failure, decoded);
            var constructed = Assert.Throws<Exception>(() =>
                StructuredException<GetMetadataError>.Decode<ApiException<GetMetadataError>>("{}", GetMetadataError.Decoder, () => throw failure));
            Assert.AreSame(failure, constructed);
        }

        /// <summary>Verifies the original decoder still decides whether to construct an exception.</summary>
        [TestMethod]
        public void TestOriginalExceptionConstruction()
        {
            var calls = 0;
            Func<ApiException<GetMetadataError>> factory = () =>
            {
                calls++;
                return new ApiException<GetMetadataError>("request-123");
            };
            Assert.ThrowsExactly<Newtonsoft.Json.JsonReaderException>(() =>
                StructuredException<GetMetadataError>.Decode("null", GetMetadataError.Decoder, factory));
            Assert.AreEqual(0, calls);
            Assert.ThrowsExactly<Newtonsoft.Json.JsonReaderException>(() =>
                StructuredException<GetMetadataError>.Decode("not-json", GetMetadataError.Decoder, factory));
            Assert.AreEqual(0, calls);
            var incomplete = StructuredException<GetMetadataError>.Decode("{}", GetMetadataError.Decoder, factory);
            Assert.AreEqual(1, calls);
            Assert.IsNull(incomplete.ErrorResponse);
            Assert.IsNull(incomplete.Message);
        }

        /// <summary>Verifies OAuth invalid_grant retains actual 400/401 status and response metadata.</summary>
        /// <param name="status">The OAuth response status.</param>
        /// <param name="error">The OAuth error code.</param>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        [DataRow(400, "invalid_grant")]
        [DataRow(401, "invalid_grant")]
        [DataRow(400, "invalid_client")]
        public async Task TestOAuthResponseMetadata(int status, string error)
        {
            var body = "{\"error\":\"" + error + "\",\"error_description\":\"Refresh token is invalid.\"}";
            var handler = new ResponseHandler(() => CreateResponse((HttpStatusCode)status, body));
            var config = new DropboxClientConfig { HttpClient = new HttpClient(handler) };
            using (var client = new DropboxClient("refresh-token", "app-key", "app-secret", config))
            {
                if (error == "invalid_grant")
                {
                    var exception = await Assert.ThrowsExactlyAsync<AuthException>(() => client.RefreshAccessToken(null)).ConfigureAwait(false);
                    AssertMetadata(exception, status);
                    Assert.AreEqual("Refresh token is invalid.", exception.Message);
                }
                else
                {
                    await Assert.ThrowsExactlyAsync<HttpRequestException>(() => client.RefreshAccessToken(null)).ConfigureAwait(false);
                }

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

        private sealed class FailingDecoder : IDecoder<GetMetadataError>
        {
            private readonly Exception failure;

            public FailingDecoder(Exception failure)
            {
                this.failure = failure;
            }

            public GetMetadataError Decode(IJsonReader reader)
            {
                throw this.failure;
            }
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
