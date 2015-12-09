//-----------------------------------------------------------------------------
// <copyright file="JsonWriter.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Babel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

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
        /// <returns>The encoded object as a JSON string.</returns>
        public static string Write<T>(T encodable, IEncoder<T> encoder, bool escapeNonAscii = false)
        {
            var builder = new StringBuilder();
            var textWriter = new JsonTextWriter(new StringWriter(builder));

            if (escapeNonAscii)
            {
                textWriter.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
            }

            var writer = new JsonWriter(textWriter);
            encoder.Encode(encodable, writer);

            var json = builder.ToString();

            return !string.IsNullOrEmpty(json) ? json : "null";
        }

        /// <summary>
        /// Write a Int32 value.
        /// </summary>
        /// <param name="value">The value.</param>
        void IJsonWriter.WriteInt32(int value)
        {
            this.writer.WriteValue(value);
        }

        /// <summary>
        /// Write a Int64 value.
        /// </summary>
        /// <param name="value">The value.</param>
        void IJsonWriter.WriteInt64(long value)
        {
            this.writer.WriteValue(value);
        }

        /// <summary>
        /// Write a UInt32 value.
        /// </summary>
        /// <param name="value">The value.</param>
        void IJsonWriter.WriteUInt32(uint value)
        {
            this.writer.WriteValue(value);
        }

        /// <summary>
        /// Write a UInt64 value.
        /// </summary>
        /// <param name="value">The value.</param>
        void IJsonWriter.WriteUInt64(ulong value)
        {
            this.writer.WriteValue(value);
        }

        /// <summary>
        /// Write a double value.
        /// </summary>
        /// <param name="value">The value.</param>
        void IJsonWriter.WriteDouble(double value)
        {
            this.writer.WriteValue(value);
        }

        /// <summary>
        /// Write a single value.
        /// </summary>
        /// <param name="value">The value.</param>
        void IJsonWriter.WriteSingle(float value)
        {
            this.writer.WriteValue(value);
        }

        /// <summary>
        /// Write a DateTime value.
        /// </summary>
        /// <param name="value">The value.</param>
        void IJsonWriter.WriteDateTime(DateTime value)
        {
            this.writer.WriteValue(value);
        }

        /// <summary>
        /// Write a boolean value.
        /// </summary>
        /// <param name="value">The value.</param>
        void IJsonWriter.WriteBoolean(bool value)
        {
            this.writer.WriteValue(value);
        }

        /// <summary>
        /// Write a byte[] value.
        /// </summary>
        /// <param name="value">The value.</param>
        void IJsonWriter.WriteBytes(byte[] value)
        {
            this.writer.WriteValue(value);
        }

        /// <summary>
        /// Write a string value.
        /// </summary>
        /// <param name="value">The value.</param>
        void IJsonWriter.WriteString(string value)
        {
            this.writer.WriteValue(value);
        }

        /// <summary>
        /// Write start object.
        /// </summary>
        void IJsonWriter.WriteStartObject()
        {
            this.writer.WriteStartObject();
        }

        /// <summary>
        /// Write end object.
        /// </summary>
        void IJsonWriter.WriteEndObject()
        {
            this.writer.WriteEndObject();
        }

        /// <summary>
        /// Write start array.
        /// </summary>
        void IJsonWriter.WriteStartArray()
        {
            this.writer.WriteStartArray();
        }

        /// <summary>
        /// Write end array.
        /// </summary>
        void IJsonWriter.WriteEndArray()
        {
            this.writer.WriteEndArray();
        }

        /// <summary>
        /// Write property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        void IJsonWriter.WritePropertyName(string name)
        {
            this.writer.WritePropertyName(name);
        }
    }
}
