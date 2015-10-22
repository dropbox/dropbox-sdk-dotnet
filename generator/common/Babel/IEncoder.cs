//-----------------------------------------------------------------------------
// <copyright file="IEncoder.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Babel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface that is used to encode objects that implement the
    /// <see cref="T:Dropbox.Api.Babel.IEncodable`1"/> interface.
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        /// Adds an object, returning an encoder context that can be used to
        /// add fields to the object.
        /// </summary>
        /// <returns>The encoder context.</returns>
        IEncoderContext AddObject();

        /// <summary>
        /// Gets the encoded object as a string.
        /// </summary>
        /// <returns>The string encoding of the object.</returns>
        string GetEncodedString();

        /// <summary>
        /// Gets the encoded object as a byte array.
        /// </summary>
        /// <returns>The byte array encoding of the object.</returns>
        byte[] GetEncodedBytes();
    }

    /// <summary>
    /// A context used to set field values for an encoded object.
    /// </summary>
    public interface IEncoderContext : IDisposable
    {
        /// <summary>
        /// Adds a field to the current encoding context.
        /// </summary>
        /// <typeparam name="T">The type of the field that is being added.</typeparam>
        /// <param name="name">The field name.</param>
        /// <param name="value">The value of the field.</param>
        void AddField<T>(string name, T value);

        /// <summary>
        /// Adds a field that implements the
        /// <see cref="T:Dropbox.Api.Babel.IEncodable`1"/> interface to the current
        /// encoding context.
        /// </summary>
        /// <typeparam name="T">The type of the field that is being added.</typeparam>
        /// <param name="name">The field name.</param>
        /// <param name="value">The value of the field.</param>
        void AddFieldObject<T>(string name, T value)
            where T : IEncodable<T>, new();

        /// <summary>
        /// Adds a field that is a list of items.
        /// </summary>
        /// <typeparam name="T">The element type of each list item.</typeparam>
        /// <param name="name">The field name.</param>
        /// <param name="value">The value of the field.</param>
        void AddFieldList<T>(string name, IEnumerable<T> value);

        /// <summary>
        /// Adds a field that is a list of objects that implement the
        /// <see cref="T:Dropbox.Api.Babel.IEncodable`1"/> interface.
        /// </summary>
        /// <typeparam name="T">The element type of each list item.</typeparam>
        /// <param name="name">The field name.</param>
        /// <param name="value">The value of the field.</param>
        void AddFieldObjectList<T>(string name, IEnumerable<T> value)
            where T : IEncodable<T>, new();
    }
}
