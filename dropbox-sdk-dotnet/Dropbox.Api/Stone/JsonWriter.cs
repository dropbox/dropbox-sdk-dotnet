//-----------------------------------------------------------------------------
// <copyright file="JsonWriter.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Stone
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    /// <summary>
    /// Write json as string.
    /// </summary>
    internal sealed class JsonWriter : IJsonWriter
    {
        /// <summary>
        /// The json text writer.
        /// </summary>
        private readonly JsonTextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class.
        /// </summary>
        /// <param name="writer">The text writer.</param>
        private JsonWriter(JsonTextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Write the specified object.
        /// </summary>
        /// <typeparam name="T">The type of the object to write.</typeparam>
        /// <param name="encodable">The object to write.</param>
        /// <param name="encoder">The encoder.</param>
        /// <param name="escapeNonAscii">If escape non-ascii characters.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The encoded object as a JSON string.</returns>
        public static async Task<string> WriteAsync<T>(T encodable, IEncoder<T> encoder, bool escapeNonAscii = false, CancellationToken cancellationToken = default)
        {
            var builder = new StringBuilder();
            var textWriter = new JsonTextWriter(new StringWriter(builder)) { DateFormatString = "yyyy-MM-ddTHH:mm:ssZ" };

            if (escapeNonAscii)
            {
                textWriter.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
            }

            var writer = new JsonWriter(textWriter);
            await encoder.Encode(encodable, writer, cancellationToken);

            var json = builder.ToString();

            return !string.IsNullOrEmpty(json) ? json.Replace("\x7f", "\\u007f") : "null";
        }

        /// <summary>
        /// Write a Int32 value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteInt32(int value, CancellationToken cancellationToken)
        {
            await this.writer.WriteValueAsync(value, cancellationToken);
        }

        /// <summary>
        /// Write a Int64 value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteInt64(long value, CancellationToken cancellationToken)
        {
            await this.writer.WriteValueAsync(value, cancellationToken);
        }

        /// <summary>
        /// Write a UInt32 value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteUInt32(uint value, CancellationToken cancellationToken)
        {
            await this.writer.WriteValueAsync(value, cancellationToken);
        }

        /// <summary>
        /// Write a UInt64 value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteUInt64(ulong value, CancellationToken cancellationToken)
        {
            await this.writer.WriteValueAsync(value, cancellationToken);
        }

        /// <summary>
        /// Write a double value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteDouble(double value, CancellationToken cancellationToken)
        {
            await this.writer.WriteValueAsync(value, cancellationToken);
        }

        /// <summary>
        /// Write a single value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteSingle(float value, CancellationToken cancellationToken)
        {
            await this.writer.WriteValueAsync(value, cancellationToken);
        }

        /// <summary>
        /// Write a DateTime value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteDateTime(DateTime value, CancellationToken cancellationToken)
        {
            await this.writer.WriteValueAsync(value.ToUniversalTime(), cancellationToken);
        }

        /// <summary>
        /// Write a boolean value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteBoolean(bool value, CancellationToken cancellationToken)
        {
            await this.writer.WriteValueAsync(value, cancellationToken);
        }

        /// <summary>
        /// Write a byte[] value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteBytes(byte[] value, CancellationToken cancellationToken)
        {
            await this.writer.WriteValueAsync(value, cancellationToken);
        }

        /// <summary>
        /// Write a string value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteString(string value, CancellationToken cancellationToken)
        {
            await this.writer.WriteValueAsync(value, cancellationToken);
        }

        /// <summary>
        /// Write a null value.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteNull(CancellationToken cancellationToken)
        {
            await this.writer.WriteNullAsync(cancellationToken);
        }

        /// <summary>
        /// Write start object.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteStartObject(CancellationToken cancellationToken)
        {
            await this.writer.WriteStartObjectAsync(cancellationToken);
        }

        /// <summary>
        /// Write end object.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteEndObject(CancellationToken cancellationToken)
        {
            await this.writer.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>
        /// Write start array.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteStartArray(CancellationToken cancellationToken)
        {
            await this.writer.WriteStartArrayAsync(cancellationToken);
        }

        /// <summary>
        /// Write end array.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WriteEndArray(CancellationToken cancellationToken)
        {
            await this.writer.WriteEndArrayAsync(cancellationToken);
        }

        /// <summary>
        /// Write property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        async Task IJsonWriter.WritePropertyName(string name, CancellationToken cancellationToken)
        {
            await this.writer.WritePropertyNameAsync(name, cancellationToken);
        }
    }
}