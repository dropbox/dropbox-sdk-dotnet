//-----------------------------------------------------------------------------
// <copyright file="JsonReader.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Babel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    using Newtonsoft.Json;

    /// <summary>
    /// Parse and read from json string.
    /// </summary>
    internal sealed class JsonReader : IJsonReader
    {
        /// <summary>
        /// The json text reader.
        /// </summary>
        private readonly JsonTextReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> class.
        /// </summary>
        /// <param name="reader">The json text reader.</param>
        private JsonReader(JsonTextReader reader)
        {
            this.reader = reader;
            this.reader.Read();
        }

        /// <summary>
        /// Gets a value indicating whether current token is start object.
        /// </summary>
        bool IJsonReader.IsStartObject
        {
            get { return this.reader.TokenType == JsonToken.StartObject; }
        }

        /// <summary>
        /// Gets a value indicating whether current token is end object.
        /// </summary>
        bool IJsonReader.IsEndObject
        {
            get { return this.reader.TokenType == JsonToken.EndObject; }
        }

        /// <summary>
        /// Gets a value indicating whether current token is start array.
        /// </summary>
        bool IJsonReader.IsStartArray
        {
            get { return this.reader.TokenType == JsonToken.StartArray; }
        }

        /// <summary>
        /// Gets a value indicating whether current token is end array.
        /// </summary>
        bool IJsonReader.IsEndArray
        {
            get { return this.reader.TokenType == JsonToken.EndArray; }
        }

        /// <summary>
        /// Gets a value indicating whether current token is property name.
        /// </summary>
        bool IJsonReader.IsPropertyName
        {
            get { return this.reader.TokenType == JsonToken.PropertyName; }
        }

        /// <summary>
        /// Read specific type form given json.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="json">The json.</param>
        /// <param name="decoder">The decoder.</param>
        /// <returns>The decoded object.</returns>
        public static T Read<T>(string json, IDecoder<T> decoder)
        {
            var reader = new JsonReader(new JsonTextReader(new StringReader(json)));
            return decoder.Decode(reader);
        }

        /// <summary>
        /// Read one token.
        /// </summary>
        /// <returns>If read succeeded.</returns>
        bool IJsonReader.Read()
        {
            return this.reader.Read();
        }
        
        /// <summary>
        /// Skip current token.
        /// </summary>
        void IJsonReader.Skip()
        {
            this.reader.Skip();
            this.reader.Read();
        }

        /// <summary>
        /// Read value as Int32
        /// </summary>
        /// <returns>The value.</returns>
        int IJsonReader.ReadInt32()
        {
            return Convert.ToInt32(this.ReadValue<long>());
        }

        /// <summary>
        /// Read value as Int64
        /// </summary>
        /// <returns>The value</returns>
        long IJsonReader.ReadInt64()
        {
            return this.ReadValue<long>();
        }

        /// <summary>
        /// Read value as UInt32
        /// </summary>
        /// <returns>The value.</returns>
        uint IJsonReader.ReadUInt32()
        {
            return Convert.ToUInt32(this.ReadValue<long>());
        }

        /// <summary>
        /// Read value as UInt64
        /// </summary>
        /// <returns>The value</returns>
        ulong IJsonReader.ReadUInt64()
        {
            return Convert.ToUInt64(this.ReadValue<long>());
        }

        /// <summary>
        /// Read value as double
        /// </summary>
        /// <returns>The value.</returns>
        double IJsonReader.ReadDouble()
        {
            return this.ReadValue<double>();
        }

        /// <summary>
        /// Read value as float
        /// </summary>
        /// <returns>The value</returns>
        float IJsonReader.ReadSingle()
        {
            return Convert.ToSingle(this.ReadValue<double>());
        }

        /// <summary>
        /// Read value as DateTime
        /// </summary>
        /// <returns>The value</returns>
        DateTime IJsonReader.ReadDateTime()
        {
            return this.ReadValue<DateTime>();
        }

        /// <summary>
        /// Read value as boolean
        /// </summary>
        /// <returns>The value</returns>
        bool IJsonReader.ReadBoolean()
        {
            return this.ReadValue<bool>();
        }

        /// <summary>
        /// Read value as bytes
        /// </summary>
        /// <returns>The value</returns>
        byte[] IJsonReader.ReadBytes()
        {
            return this.ReadValue<byte[]>();
        }

        /// <summary>
        /// Read value as string.
        /// </summary>
        /// <returns>The value.</returns>
        string IJsonReader.ReadString()
        {
            return this.ReadValue<string>();
        }

        /// <summary>
        /// Read next token as specific value type.
        /// </summary>
        /// <typeparam name="T">The type of the value. The type will be used as explicit cast.</typeparam>
        /// <returns>The value.</returns>
        private T ReadValue<T>()
        {
            try
            {
                var value = (T)this.reader.Value;
                this.reader.Read();
                return value;
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Value '{0}' is not valid {1} type",
                    this.reader.Value,
                    typeof(T)));
            }
        }
    }
}
