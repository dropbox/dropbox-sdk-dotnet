//-----------------------------------------------------------------------------
// <copyright file="ContentHasherTests.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Unit.Tests
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Dropbox.Api.Files;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for Dropbox content hash computation.
    /// </summary>
    [TestClass]
    public class ContentHasherTests
    {
        /// <summary>
        /// Verifies known Dropbox content hash vectors.
        /// </summary>
        [TestMethod]
        public void TestComputeHashKnownVectors()
        {
            Assert.AreEqual(
                "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
                ContentHasher.ComputeHash(new MemoryStream()));

            Assert.AreEqual(
                "bf5d3affb73efd2ec6c36ad3112dd933efed63c4e1cbffcfa88e2759c144f2d8",
                ContentHasher.ComputeHash(new MemoryStream(Encoding.UTF8.GetBytes("a"))));

            Assert.AreEqual(
                "4f8b42c22dd3729b519ba6f68d2da7cc5b2d606d05daed5ad5128cc03e6c6358",
                ContentHasher.ComputeHash(new MemoryStream(Encoding.UTF8.GetBytes("abc"))));
        }

        /// <summary>
        /// Verifies block boundary handling.
        /// </summary>
        [TestMethod]
        public void TestComputeHashBlockBoundary()
        {
            var content = new byte[ContentHasher.BlockSize + 1];
            for (var i = 0; i < content.Length; i++)
            {
                content[i] = (byte)'a';
            }

            Assert.AreEqual(
                "5f858b62ccd88447586305aec6fd53c96747cfebf527cbba129a6dfed47d9624",
                ContentHasher.ComputeHash(new MemoryStream(content)));
        }

        /// <summary>
        /// Verifies asynchronous hash computation.
        /// </summary>
        /// <returns>The asynchronous test task.</returns>
        [TestMethod]
        public async Task TestComputeHashAsync()
        {
            var content = new MemoryStream(Encoding.UTF8.GetBytes("abc"));
            var contentHash = await ContentHasher.ComputeHashAsync(content).ConfigureAwait(false);

            Assert.AreEqual("4f8b42c22dd3729b519ba6f68d2da7cc5b2d606d05daed5ad5128cc03e6c6358", contentHash);
        }
    }
}
