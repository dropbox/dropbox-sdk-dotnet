//-----------------------------------------------------------------------------
// <copyright file="ApiException.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The test class for Dropbox API.
    /// </summary>
    [TestClass]
    public class DropboxApiTests
    {
        /// <summary>
        /// The Dropbox client.
        /// </summary>
        public static DropboxClient Client;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var token = context.Properties["accessToken"].ToString();
            Client = new DropboxClient(token);
        }


        [TestCleanup]
        public void Cleanup()
        {
            var result = Client.Files.ListFolderAsync("").Result;

            foreach (var entry in result.Entries) {
                Client.Files.DeleteAsync(entry.PathLower).Wait();
            }
        }

        /// <summary>
        /// Test get metadata.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestGetMetadata()
        {
            await Client.Files.UploadAsync("/Foo.txt", body: GetStream("abc"));
            var metadata = await Client.Files.GetMetadataAsync("/Foo.txt");
            Assert.AreEqual("Foo.txt", metadata.Name);
            Assert.AreEqual("/foo.txt", metadata.PathLower);
            Assert.AreEqual("/Foo.txt", metadata.PathDisplay);
            Assert.IsTrue(metadata.IsFile);

            var file = metadata.AsFile;
            Assert.AreEqual(3, (int)file.Size);
        }

        /// <summary>
        /// Test get metadata.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestListFolder()
        {
            var files = new HashSet<string> { "/a.txt", "/b.txt", "/c.txt" };
            foreach (var file in files)
            {
                await Client.Files.UploadAsync(file, body: GetStream("abc"));
            }

            var response = await Client.Files.ListFolderAsync("");
            Assert.AreEqual(files.Count, response.Entries.Count);
            foreach (var entry in response.Entries)
            {
                Assert.IsTrue(files.Contains(entry.PathLower));
                Assert.IsTrue(entry.IsFile);
                var file = entry.AsFile;
                Assert.AreEqual(3, (int)file.Size);
            }
        }

        /// <summary>
        /// Test upload.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestUpload()
        {
            var response = await Client.Files.UploadAsync("/Foo.txt", body: GetStream("abc"));
            Assert.AreEqual(response.Name, "Foo.txt");
            Assert.AreEqual(response.PathLower, "/foo.txt");
            Assert.AreEqual(response.PathDisplay, "/Foo.txt");
            var downloadResponse = await Client.Files.DownloadAsync("/Foo.txt");
            var content = await downloadResponse.GetContentAsStringAsync();
            Assert.AreEqual("abc", content);
        }

        /// <summary>
        /// Test upload.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestDownload()
        {
            await Client.Files.UploadAsync("/Foo.txt", body: GetStream("abc"));
            var downloadResponse = await Client.Files.DownloadAsync("/Foo.txt");
            var content = await downloadResponse.GetContentAsStringAsync();
            Assert.AreEqual("abc", content);
            var response = downloadResponse.Response;
            Assert.AreEqual(response.Name, "Foo.txt");
            Assert.AreEqual(response.PathLower, "/foo.txt");
            Assert.AreEqual(response.PathDisplay, "/Foo.txt");
        }

        /// <summary>
        /// Converts string to a memory stream.
        /// </summary>
        /// <param name="content">The string content.</param>
        /// <returns>The memory stream.</returns>
        private static MemoryStream GetStream(string content) 
        {
            var buffer = Encoding.UTF8.GetBytes(content);
            return new MemoryStream(buffer);
        }
    }
}
