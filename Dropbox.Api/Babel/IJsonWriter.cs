//-----------------------------------------------------------------------------
// <copyright file="IJsonWriter.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Babel
{
    using System;

    /// <summary>
    /// The json writer interface.
    /// </summary>
    internal interface IJsonWriter
    {
        /// <summary>
        /// Write a Int32 value.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteInt32(int value);

        /// <summary>
        /// Write a Int64 value.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteInt64(long value);

        /// <summary>
        /// Write a UInt32 value.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteUInt32(uint value);

        /// <summary>
        /// Write a UInt64 value.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteUInt64(ulong value);

        /// <summary>
        /// Write a double value.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteDouble(double value);

        /// <summary>
        /// Write a single value.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteSingle(float value);

        /// <summary>
        /// Write a DateTime value.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteDateTime(DateTime value);

        /// <summary>
        /// Write a boolean value.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteBoolean(bool value);

        /// <summary>
        /// Write a byte[] value.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteBytes(byte[] value);

        /// <summary>
        /// Write a string value.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteString(string value);

        /// <summary>
        /// Write start object.
        /// </summary>
        void WriteStartObject();

        /// <summary>
        /// Write end object.
        /// </summary>
        void WriteEndObject();

        /// <summary>
        /// Write start array.
        /// </summary>
        void WriteStartArray();

        /// <summary>
        /// Write end array.
        /// </summary>
        void WriteEndArray();

        /// <summary>
        /// Write property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        void WritePropertyName(string name);
    }
}
