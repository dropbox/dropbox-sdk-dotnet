//-----------------------------------------------------------------------------
// <copyright file="IDecoder.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Stone
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface that is used to decode objects of specific type.
    /// </summary>
    /// <typeparam name="T">The type of the decoded object.</typeparam>
    internal interface IDecoder<out T>
    {
        /// <summary>
        /// Decode into specific type.
        /// </summary>
        /// <param name="reader">The json reader.</param>
        /// <returns>The value.</returns>
        T Decode(IJsonReader reader);
    }
}
