//-----------------------------------------------------------------------------
// <copyright file="IJsonWriter.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Stone
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The json writer interface.
    /// </summary>
    internal interface IJsonWriter
    {
        /// <summary>
        /// Write a Int32 value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteInt32(int value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Write a Int64 value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteInt64(long value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Write a UInt32 value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteUInt32(uint value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Write a UInt64 value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteUInt64(ulong value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Write a double value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteDouble(double value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Write a single value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteSingle(float value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Write a DateTime value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteDateTime(DateTime value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Write a boolean value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteBoolean(bool value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Write a byte[] value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteBytes(byte[] value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Write a string value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteString(string value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Write a null value.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteNull(CancellationToken cancellationToken = default);

        /// <summary>
        /// Write start object.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteStartObject(CancellationToken cancellationToken = default);

        /// <summary>
        /// Write end object.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteEndObject(CancellationToken cancellationToken = default);

        /// <summary>
        /// Write start array.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteStartArray(CancellationToken cancellationToken = default);

        /// <summary>
        /// Write end array.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteEndArray(CancellationToken cancellationToken = default);

        /// <summary>
        /// Write property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WritePropertyName(string name, CancellationToken cancellationToken = default);
    }
}
