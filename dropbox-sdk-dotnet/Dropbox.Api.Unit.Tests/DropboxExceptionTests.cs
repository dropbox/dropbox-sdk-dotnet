//-----------------------------------------------------------------------------
// <copyright file="DropboxExceptionTests.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Unit.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Dropbox.Api.Files;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the request ID exposed on Dropbox exceptions.
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

        private static DropboxClient CreateClient(ResponseHandler handler)
        {
            return new DropboxClient(
                "access-token",
                new DropboxClientConfig
                {
                    HttpClient = new HttpClient(handler),
                });
        }

        private static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string body)
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };
            response.Headers.Add("X-Dropbox-Request-Id", "request-123");
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

        private sealed class ResponseHandler : HttpMessageHandler
        {
            private readonly Func<HttpResponseMessage> responseFactory;

            public ResponseHandler(Func<HttpResponseMessage> responseFactory)
            {
                this.responseFactory = responseFactory;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(this.responseFactory());
            }
        }
    }
}
