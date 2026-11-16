//-----------------------------------------------------------------------------
// <copyright file="StoneTests.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Unit.Tests
{
    using System;
    using Dropbox.Api.Files;
    using Dropbox.Api.Stone;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests related to stone serialization.
    /// </summary>
    [TestClass]
    public class StoneTests
    {
        /// <summary>Verifies truncated arrays cannot keep decoding a stale token at EOF.</summary>
        /// <param name="json">The truncated JSON array.</param>
        [TestMethod]
        [DataRow("[")]
        [DataRow("[\"id\"")]
        [DataRow("[\"id\",")]
        [DataRow("[null")]
        [DataRow("[null,")]
        public void TestTruncatedArrays(string json)
        {
            Assert.ThrowsExactly<Newtonsoft.Json.JsonReaderException>(() =>
                JsonReader.Read(json, Decoder.CreateListDecoder(StringDecoder.Instance)));
        }

        /// <summary>Verifies EOF after a nested array still requires the enclosing array to close.</summary>
        /// <param name="json">The truncated nested JSON array.</param>
        [TestMethod]
        [DataRow("[[]")]
        [DataRow("[[\"id\"],")]
        public void TestTruncatedNestedArrays(string json)
        {
            Assert.ThrowsExactly<Newtonsoft.Json.JsonReaderException>(() =>
                JsonReader.Read(json, Decoder.CreateListDecoder(Decoder.CreateListDecoder(StringDecoder.Instance))));
        }

        /// <summary>Verifies valid root values, containers and nullable elements still decode at EOF.</summary>
        [TestMethod]
        public void TestValidEndOfInput()
        {
            Assert.AreEqual("value", JsonReader.Read("\"value\"", StringDecoder.Instance));
            Assert.AreEqual(42, JsonReader.Read("42", Int32Decoder.Instance));
            Assert.IsNull(JsonReader.Read("null", Metadata.Decoder));
            var nullable = Decoder.CreateListDecoder(Metadata.Decoder);
            Assert.IsNull(JsonReader.Read("[null]", nullable)[0]);
            Assert.ThrowsExactly<Newtonsoft.Json.JsonReaderException>(() => JsonReader.Read("[null,", nullable));
            var nested = Decoder.CreateListDecoder(Decoder.CreateListDecoder(StringDecoder.Instance));
            CollectionAssert.AreEqual(new[] { "id", null }, JsonReader.Read("[[\"id\",null],[]]", nested)[0]);
        }

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
    }
}
