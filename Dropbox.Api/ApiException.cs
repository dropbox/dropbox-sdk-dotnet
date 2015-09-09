//-----------------------------------------------------------------------------
// <copyright file="ApiException.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;

    using Dropbox.Api.Babel;

    /// <summary>
    /// The exception type that will be raised by an <see cref="ITransport" />
    /// implementation if there is an error processing the request.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    public sealed class ApiException<TError> : Exception, IEncodable<ApiException<TError>>
        where TError : IEncodable<TError>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException{TError}"/> class.
        /// </summary>
        /// <remarks>This constructor is only used when deserializing from JSON.</remarks>
        public ApiException()
            : this(default(TError))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException{TError}"/> class.
        /// </summary>
        /// <param name="errorResponse">The error response.</param>
        public ApiException(TError errorResponse)
            : this(errorResponse, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException{TError}"/> class.
        /// </summary>
        /// <param name="errorResponse">The error response.</param>
        /// <param name="message">The message.</param>
        public ApiException(TError errorResponse, string message)
            : this(errorResponse, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException{TError}"/> class.
        /// </summary>
        /// <param name="errorResponse">The error response.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public ApiException(TError errorResponse, string message, Exception inner)
            : base(message, inner)
        {
            this.ErrorResponse = errorResponse;
        }

        /// <summary>
        /// Gets the error response.
        /// </summary>
        /// <value>
        /// The error response.
        /// </value>
        public TError ErrorResponse { get; private set; }

        /// <summary>
        /// Encodes the object using the supplied encoder.
        /// </summary>
        /// <param name="encoder">The encoder being used to serialize the object.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void IEncodable<ApiException<TError>>.Encode(IEncoder encoder)
        {
            throw new NotSupportedException("Exceptions cannot be encoded");
        }

        /// <summary>
        /// Decodes on object using the supplied decoder.
        /// </summary>
        /// <param name="decoder">The decoder used to deserialize the object.</param>
        /// <returns>
        /// The deserialized object. Note: this is not necessarily the current instance.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        ApiException<TError> IEncodable<ApiException<TError>>.Decode(IDecoder decoder)
        {
            using (var obj = decoder.GetObject())
            {
                this.ErrorResponse = obj.GetFieldObject<TError>("error");
            }

            return this;
        }
    }
}
