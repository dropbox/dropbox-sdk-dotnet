//-----------------------------------------------------------------------------
// <copyright file="ApiException.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;

    using Dropbox.Api.Stone;

    /// <summary>
    /// The exception type that will be raised by an <see cref="ITransport" />
    /// implementation if there is an error processing the request which is caused by
    /// failure in API route.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    public sealed class ApiException<TError> : StructuredException<TError>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException{TError}"/> class.
        /// </summary>
        /// <param name="requestId">The Dropbox request id.</param>
        /// <remarks>This constructor is only used when decoded from JSON.</remarks>
        internal ApiException(string requestId)
            : base(requestId)
        {
        }
    }
}
