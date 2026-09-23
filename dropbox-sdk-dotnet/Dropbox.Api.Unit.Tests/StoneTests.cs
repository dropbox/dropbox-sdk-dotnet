//-----------------------------------------------------------------------------
// <copyright file="StoneTests.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Unit.Tests
{
    using System;
    using Dropbox.Api.Files;
    using Dropbox.Api.Sharing;
    using Dropbox.Api.Stone;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests related to stone serialization.
    /// </summary>
    [TestClass]
    public class StoneTests
    {
        /// <summary>
        /// Smoke test for nested unions.
        /// </summary>
        [TestMethod]
        public void TestNestedUnion()
        {
            var result = JsonWriter.Write(
                new GetMetadataError.Path(LookupError.NotFound.Instance),
                GetMetadataError.Encoder);

            var obj = JsonReader.Read(result, GetMetadataError.Decoder);

            Assert.IsTrue(obj.IsPath);
            Assert.IsTrue(obj.AsPath.Value.IsNotFound);
        }

        /// <summary>
        /// Null optional lists are omitted when encoding a struct.
        /// </summary>
        [TestMethod]
        public void TestListFoldersArgsWithNullActions()
        {
            var result = JsonWriter.Write(new ListFoldersArgs(), ListFoldersArgs.Encoder);

            Assert.AreEqual("{\"limit\":1000}", result);
        }
    }
}
