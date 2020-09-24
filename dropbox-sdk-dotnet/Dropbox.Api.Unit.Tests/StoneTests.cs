//-----------------------------------------------------------------------------
// <copyright file="StoneTests.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Unit.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Dropbox.Api.Files;
    using Dropbox.Api.Stone;

    [TestClass]
    public class StoneTests
    {
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
