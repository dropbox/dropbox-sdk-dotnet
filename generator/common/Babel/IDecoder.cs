//-----------------------------------------------------------------------------
// <copyright file="IDecoder.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Babel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface that is used to decode objects that implement the
    /// <see cref="T:Dropbox.Api.Babel.IEncodable`1"/> interface.
    /// </summary>
    public interface IDecoder
    {
        /// <summary>
        /// Used to get the name of the current union.
        /// </summary>
        /// <returns>The union name</returns>
        string GetUnionName();

        /// <summary>
        /// Gets a context for the current object, the context object is used to access
        /// individual fields.
        /// </summary>
        /// <returns>The decoder context for the current object.</returns>
        IDecoderContext GetObject();

        /// <summary>
        /// Sets the current source for this instance.
        /// </summary>
        /// <param name="source">The source as a string.</param>
        void SetSource(string source);

        /// <summary>
        /// Sets the current source for this instance.
        /// </summary>
        /// <param name="source">The source as a byte array.</param>
        void SetSource(byte[] source);
    }

    /// <summary>
    /// A context used to get decoded field values from an encoded object.
    /// </summary>
    public interface IDecoderContext : IDisposable
    {
        /// <summary>
        /// Indicates whether the specified field is present in the object.
        /// </summary>
        /// <param name="name">The name of the field to check.</param>
        /// <returns><c>true</c> if the field is present; <c>false</c> otherwise.</returns>
        bool HasField(string name);

        /// <summary>
        /// Gets the value of a field.
        /// </summary>
        /// <typeparam name="T">The expected type of the field.</typeparam>
        /// <param name="name">The name of the field to get.</param>
        /// <returns>The decoded value of the field.</returns>
        T GetField<T>(string name);

        /// <summary>
        /// Gets the value of a field which is an object.
        /// </summary>
        /// <typeparam name="T">The expected type of the object. This must implement the
        /// <see cref="T:Dropbox.Api.Babel.IEncodable`1"/> interface.</typeparam>
        /// <param name="name">The name of the field to get.</param>
        /// <returns>The decoded value of the field.</returns>
        T GetFieldObject<T>(string name) where T : IEncodable<T>, new();

        /// <summary>
        /// Gets the value of a field which is a list.
        /// </summary>
        /// <typeparam name="T">The expected type of the list elements.</typeparam>
        /// <param name="name">The name of the field.</param>
        /// <returns>The decoded list of items.</returns>
        IEnumerable<T> GetFieldList<T>(string name);

        /// <summary>
        /// Gets the value of a field which is a list of objects that implement
        /// <see cref="T:Dropbox.Api.Babel.IEncodable`1"/>.
        /// </summary>
        /// <typeparam name="T">The expected type of the list elements.</typeparam>
        /// <param name="name">The name of the fields.</param>
        /// <returns>The decoded list of items.</returns>
        IEnumerable<T> GetFieldObjectList<T>(string name) where T : IEncodable<T>, new();
    }
}
