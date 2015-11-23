//-----------------------------------------------------------------------------
// <copyright file="IJsonReader.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Babel
{
    using System;

    /// <summary>
    /// The json reader interface.
    /// </summary>
    internal interface IJsonReader
    {
        /// <summary>
        /// Gets a value indicating whether current token is start object.
        /// </summary>
        bool IsStartObject { get; }

        /// <summary>
        /// Gets a value indicating whether current token is end object.
        /// </summary>
        bool IsEndObject { get; }

        /// <summary>
        /// Gets a value indicating whether current token is start array.
        /// </summary>
        bool IsStartArray { get; }

        /// <summary>
        /// Gets a value indicating whether current token is end array.
        /// </summary>
        bool IsEndArray { get; }

        /// <summary>
        /// Gets a value indicating whether current token is property name.
        /// </summary>
        bool IsPropertyName { get; }

        /// <summary>
        /// Read one token.
        /// </summary>
        /// <returns>If read succeeded.</returns>
        bool Read();

        /// <summary>
        /// Skip current token.
        /// </summary>
        void Skip();

        /// <summary>
        /// Read value as Int32
        /// </summary>
        /// <returns>The value.</returns>
        int ReadInt32();

        /// <summary>
        /// Read value as Int64
        /// </summary>
        /// <returns>The value</returns>
        long ReadInt64();

        /// <summary>
        /// Read value as UInt32
        /// </summary>
        /// <returns>The value.</returns>
        uint ReadUInt32();

        /// <summary>
        /// Read value as UInt64
        /// </summary>
        /// <returns>The value</returns>
        ulong ReadUInt64();

        /// <summary>
        /// Read value as double
        /// </summary>
        /// <returns>The value.</returns>
        double ReadDouble();

        /// <summary>
        /// Read value as float
        /// </summary>
        /// <returns>The value</returns>
        float ReadSingle();

        /// <summary>
        /// Read value as DateTime
        /// </summary>
        /// <returns>The value</returns>
        DateTime ReadDateTime();

        /// <summary>
        /// Read value as boolean.
        /// </summary>
        /// <returns>The value</returns>
        bool ReadBoolean();

        /// <summary>
        /// Read value as bytes
        /// </summary>
        /// <returns>The value</returns>
        byte[] ReadBytes();

        /// <summary>
        /// Read value as string.
        /// </summary>
        /// <returns>The value.</returns>
        string ReadString();
    }
}
