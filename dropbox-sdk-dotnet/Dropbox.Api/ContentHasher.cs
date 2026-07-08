//-----------------------------------------------------------------------------
// <copyright file="ContentHasher.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Files
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    /// <summary>
    /// Computes Dropbox API content hashes for file contents.
    /// </summary>
    /// <remarks>
    /// Dropbox content hashes are computed by splitting content into 4 MiB blocks, hashing
    /// each block with SHA-256, concatenating those block hashes, and SHA-256 hashing the
    /// concatenation.
    /// </remarks>
    public static class ContentHasher
    {
        /// <summary>
        /// The number of bytes in each Dropbox content hash block.
        /// </summary>
        public const int BlockSize = 4 * 1024 * 1024;

        private static readonly byte[] EmptyBytes = new byte[0];

        /// <summary>
        /// Computes the Dropbox API content hash for the stream from its current position to
        /// the end of the stream.
        /// </summary>
        /// <param name="content">The content stream to hash.</param>
        /// <returns>The lowercase hexadecimal Dropbox API content hash.</returns>
        public static string ComputeHash(Stream content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            var buffer = new byte[BlockSize];
            using (var overallHasher = SHA256.Create())
            {
                while (true)
                {
                    var count = ReadBlock(content, buffer);
                    if (count == 0)
                    {
                        break;
                    }

                    using (var blockHasher = SHA256.Create())
                    {
                        var blockHash = blockHasher.ComputeHash(buffer, 0, count);
                        overallHasher.TransformBlock(blockHash, 0, blockHash.Length, null, 0);
                    }
                }

                overallHasher.TransformFinalBlock(EmptyBytes, 0, 0);
                return ToHex(overallHasher.Hash);
            }
        }

        /// <summary>
        /// Computes the Dropbox API content hash for the stream from its current position to
        /// the end of the stream.
        /// </summary>
        /// <param name="content">The content stream to hash.</param>
        /// <returns>The task that represents the asynchronous hash operation. The TResult
        /// parameter contains the lowercase hexadecimal Dropbox API content hash.</returns>
        public static async Task<string> ComputeHashAsync(Stream content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            var buffer = new byte[BlockSize];
            using (var overallHasher = SHA256.Create())
            {
                while (true)
                {
                    var count = await ReadBlockAsync(content, buffer).ConfigureAwait(false);
                    if (count == 0)
                    {
                        break;
                    }

                    using (var blockHasher = SHA256.Create())
                    {
                        var blockHash = blockHasher.ComputeHash(buffer, 0, count);
                        overallHasher.TransformBlock(blockHash, 0, blockHash.Length, null, 0);
                    }
                }

                overallHasher.TransformFinalBlock(EmptyBytes, 0, 0);
                return ToHex(overallHasher.Hash);
            }
        }

        /// <summary>
        /// Returns a stream wrapper that disables automatic SDK content hash population for
        /// upload requests.
        /// </summary>
        /// <param name="content">The upload content stream.</param>
        /// <returns>A stream wrapper that disables automatic content hash population.</returns>
        /// <remarks>
        /// A manually supplied ContentHash value on the upload argument is still sent. This
        /// wrapper delegates reads, seeking, and disposal to the wrapped stream.
        /// </remarks>
        public static Stream WithoutAutoContentHash(Stream content)
        {
            if (content == null)
            {
                return null;
            }

            return new AutoContentHashDisabledStream(content);
        }

        /// <summary>
        /// Returns whether automatic SDK content hash population is disabled for the stream.
        /// </summary>
        /// <param name="content">The upload content stream.</param>
        /// <returns><c>true</c> if automatic content hash population is disabled.</returns>
        internal static bool IsAutoContentHashDisabled(Stream content)
        {
            return content is AutoContentHashDisabledStream;
        }

        private static int ReadBlock(Stream content, byte[] buffer)
        {
            var offset = 0;
            while (offset < buffer.Length)
            {
                var count = content.Read(buffer, offset, buffer.Length - offset);
                if (count == 0)
                {
                    break;
                }

                offset += count;
            }

            return offset;
        }

        private static async Task<int> ReadBlockAsync(Stream content, byte[] buffer)
        {
            var offset = 0;
            while (offset < buffer.Length)
            {
                var count = await content.ReadAsync(buffer, offset, buffer.Length - offset).ConfigureAwait(false);
                if (count == 0)
                {
                    break;
                }

                offset += count;
            }

            return offset;
        }

        private static string ToHex(byte[] bytes)
        {
            var chars = new char[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var b = bytes[i];
                chars[i * 2] = GetHexValue(b / 16);
                chars[(i * 2) + 1] = GetHexValue(b % 16);
            }

            return new string(chars);
        }

        private static char GetHexValue(int value)
        {
            return value.ToString("x", CultureInfo.InvariantCulture)[0];
        }

        private sealed class AutoContentHashDisabledStream : Stream
        {
            private readonly Stream inner;

            public AutoContentHashDisabledStream(Stream inner)
            {
                this.inner = inner;
            }

            public override bool CanRead => this.inner.CanRead;

            public override bool CanSeek => this.inner.CanSeek;

            public override bool CanWrite => this.inner.CanWrite;

            public override long Length => this.inner.Length;

            public override long Position
            {
                get => this.inner.Position;
                set => this.inner.Position = value;
            }

            public override void Flush()
            {
                this.inner.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return this.inner.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return this.inner.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                this.inner.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                this.inner.Write(buffer, offset, count);
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
            {
                return this.inner.ReadAsync(buffer, offset, count, cancellationToken);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.inner.Dispose();
                }

                base.Dispose(disposing);
            }
        }
    }
}
