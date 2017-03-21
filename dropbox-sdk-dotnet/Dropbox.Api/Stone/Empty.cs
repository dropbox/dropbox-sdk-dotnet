//-----------------------------------------------------------------------------
// <copyright file="Empty.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Stone
{
    /// <summary>
    /// An empty object used when a route doesn't have one or more of the
    /// request, response, or error types specified.
    /// </summary>
    internal sealed class Empty
    {
        /// <summary>
        /// A static instance of the <see cref="Empty"/> class.
        /// </summary>
        public static readonly Empty Instance = new Empty();
    }
}
