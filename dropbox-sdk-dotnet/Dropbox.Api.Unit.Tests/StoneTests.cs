//-----------------------------------------------------------------------------
// <copyright file="StoneTests.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Unit.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Dropbox.Api.Files;
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
        public async Task TestNestedUnion()
        {
            var result = await JsonWriter.WriteAsync(
                new GetMetadataError.Path(LookupError.NotFound.Instance),
                GetMetadataError.Encoder);

            var obj = JsonReader.Read(result, GetMetadataError.Decoder);

            Assert.IsTrue(obj.IsPath);
            Assert.IsTrue(obj.AsPath.Value.IsNotFound);
        }

        /// <summary>
        /// Smoke test for JsonWriter with cancellation token.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [TestMethod]
        public async Task TestNestedUnionWithCancellation()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () =>
            {
               await JsonWriter.WriteAsync(
                    new GetMetadataError.Path(LookupError.NotFound.Instance),
                    GetMetadataError.Encoder,
                    cancellationToken: cts.Token);
            });
        }
    }
}