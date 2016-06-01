//-----------------------------------------------------------------------------
// <copyright file="IEncoder.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Stone
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The encoder interface.
    /// </summary>
    /// <typeparam name="T">The type to encode.</typeparam>
    internal interface IEncoder<T>
    {
        /// <summary>
        /// Encode given data using provided writer.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        void Encode(T value, IJsonWriter writer);
    }
}
