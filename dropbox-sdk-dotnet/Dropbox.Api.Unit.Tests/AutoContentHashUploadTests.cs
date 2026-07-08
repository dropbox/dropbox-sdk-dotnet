//-----------------------------------------------------------------------------
// <copyright file="AutoContentHashUploadTests.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Unit.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Dropbox.Api.Files;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Tests for automatic upload content hash injection.
    /// </summary>
    [TestClass]
    public class AutoContentHashUploadTests
    {
        /// <summary>
        /// Verifies seekable upload streams get a computed content hash.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestAutoContentHashUpload()
        {
            var handler = new CaptureUploadHandler();
            using (var client = CreateClient(handler))
            {
                var uploadArg = new UploadArg("/auto.txt");
                var content = new MemoryStream(Encoding.UTF8.GetBytes("payload"));

                await client.Files.UploadAsync(uploadArg, content).ConfigureAwait(false);

                var request = handler.Requests[0];
                var arg = JObject.Parse(request.Arg);
                Assert.AreEqual(ContentHasher.ComputeHash(new MemoryStream(Encoding.UTF8.GetBytes("payload"))), (string)arg["content_hash"]);
                Assert.AreEqual("payload", request.Body);
                Assert.IsNull(uploadArg.ContentHash);
                Assert.AreEqual(Encoding.UTF8.GetByteCount("payload"), content.Position);
            }
        }

        /// <summary>
        /// Verifies manual content hash values are preserved.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestManualContentHashWins()
        {
            const string ManualHash = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            var handler = new CaptureUploadHandler();
            using (var client = CreateClient(handler))
            {
                await client.Files.UploadAsync(
                    new UploadArg("/manual.txt", contentHash: ManualHash),
                    new MemoryStream(Encoding.UTF8.GetBytes("payload"))).ConfigureAwait(false);

                var arg = JObject.Parse(handler.Requests[0].Arg);
                Assert.AreEqual(ManualHash, (string)arg["content_hash"]);
            }
        }

        /// <summary>
        /// Verifies the per-stream opt-out wrapper suppresses auto hash population.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestWithoutAutoContentHash()
        {
            var handler = new CaptureUploadHandler();
            using (var client = CreateClient(handler))
            {
                await client.Files.UploadAsync(
                    new UploadArg("/opt-out.txt"),
                    ContentHasher.WithoutAutoContentHash(new MemoryStream(Encoding.UTF8.GetBytes("payload")))).ConfigureAwait(false);

                var arg = JObject.Parse(handler.Requests[0].Arg);
                Assert.IsNull(arg["content_hash"]);
                Assert.AreEqual("payload", handler.Requests[0].Body);
            }
        }

        /// <summary>
        /// Verifies non-seekable streams are uploaded without automatic content hashes.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestNonSeekableStreamSkipsAutoContentHash()
        {
            var handler = new CaptureUploadHandler();
            using (var client = CreateClient(handler))
            {
                await client.Files.UploadAsync(
                    new UploadArg("/non-seekable.txt"),
                    new NonSeekableStream(Encoding.UTF8.GetBytes("payload"))).ConfigureAwait(false);

                var arg = JObject.Parse(handler.Requests[0].Arg);
                Assert.IsNull(arg["content_hash"]);
                Assert.AreEqual("payload", handler.Requests[0].Body);
            }
        }

        /// <summary>
        /// Verifies auto hashing uses the stream's current position as the upload start.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestAutoContentHashPreservesCurrentPosition()
        {
            var handler = new CaptureUploadHandler();
            using (var client = CreateClient(handler))
            {
                var content = new MemoryStream(Encoding.UTF8.GetBytes("prefix:payload"));
                content.Position = Encoding.UTF8.GetByteCount("prefix:");

                await client.Files.UploadAsync(new UploadArg("/offset.txt"), content).ConfigureAwait(false);

                var arg = JObject.Parse(handler.Requests[0].Arg);
                Assert.AreEqual(ContentHasher.ComputeHash(new MemoryStream(Encoding.UTF8.GetBytes("payload"))), (string)arg["content_hash"]);
                Assert.AreEqual("payload", handler.Requests[0].Body);
            }
        }

        /// <summary>
        /// Verifies the client-level opt-out suppresses auto hash population.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestConfigDisablesAutoContentHash()
        {
            var handler = new CaptureUploadHandler();
            using (var client = CreateClient(handler, autoContentHash: false))
            {
                await client.Files.UploadAsync(
                    new UploadArg("/disabled.txt"),
                    new MemoryStream(Encoding.UTF8.GetBytes("payload"))).ConfigureAwait(false);

                var arg = JObject.Parse(handler.Requests[0].Arg);
                Assert.IsNull(arg["content_hash"]);
            }
        }

        private static DropboxClient CreateClient(CaptureUploadHandler handler, bool autoContentHash = true)
        {
            return new DropboxClient(
                "access-token",
                new DropboxClientConfig
                {
                    HttpClient = new HttpClient(handler),
                    DisposeUploadStream = false,
                    AutoContentHash = autoContentHash,
                });
        }

        private sealed class CaptureUploadHandler : HttpMessageHandler
        {
            public IList<CapturedUploadRequest> Requests { get; } = new List<CapturedUploadRequest>();

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var body = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                this.Requests.Add(new CapturedUploadRequest
                {
                    Arg = string.Join(string.Empty, request.Headers.GetValues("Dropbox-API-Arg")),
                    Body = body,
                });

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        "{\"name\":\"file.txt\",\"id\":\"id:test\",\"client_modified\":\"2020-01-02T03:04:05Z\",\"server_modified\":\"2020-01-02T03:04:06Z\",\"rev\":\"abcdef123\",\"size\":1,\"is_downloadable\":true}",
                        Encoding.UTF8,
                        "application/json"),
                };
            }
        }

        private sealed class CapturedUploadRequest
        {
            public string Arg { get; set; }

            public string Body { get; set; }
        }

        private sealed class NonSeekableStream : MemoryStream
        {
            public NonSeekableStream(byte[] buffer)
                : base(buffer)
            {
            }

            public override bool CanSeek => false;
        }
    }
}
