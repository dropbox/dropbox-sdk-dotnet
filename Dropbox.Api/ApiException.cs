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
    public sealed class ApiException<TError> : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException{TError}"/> class.
        /// </summary>
        /// <remarks>This constructor is only used when decoded from JSON.</remarks>
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
            this.ErrorMessage = message;
        }

        /// <summary>
        /// Gets the error response.
        /// </summary>
        /// <value>
        /// The error response.
        /// </value>
        public TError ErrorResponse { get; private set; }

        /// <summary>
        /// Gets the exception message.
        /// </summary>
        public override string Message
        {
            get { return this.ErrorMessage; }
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        private string ErrorMessage { get; set; }

        /// <summary>
        /// Decode from given json using given decoder.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="errorDecoder">The error json.</param>
        /// <returns>The <see cref="ApiException{TError}"/></returns>
        internal static ApiException<TError> Decode(string json, IDecoder<TError> errorDecoder)
        {
            return JsonReader.Read(json, new ApiExceptionDecoder(errorDecoder));
        }

        /// <summary>
        /// The exception decoder.
        /// </summary>
        private class ApiExceptionDecoder : StructDecoder<ApiException<TError>>
        {
            /// <summary>
            /// The error decoder.
            /// </summary>
            private readonly IDecoder<TError> errorDecoder;

            /// <summary>
            /// Initializes a new instance of the <see cref="ApiExceptionDecoder"/> class.
            /// </summary>
            /// <param name="errorDecoder">The error decoder.</param>
            public ApiExceptionDecoder(IDecoder<TError> errorDecoder)
            {
                this.errorDecoder = errorDecoder;
            }

            /// <summary>
            /// Create a struct instance.
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override ApiException<TError> Create()
            {
                return new ApiException<TError>();
            }

            /// <summary>
            /// Set given field.
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The reader.</param>
            protected override void SetField(ApiException<TError> value, string fieldName, IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "error":
                        value.ErrorResponse = this.errorDecoder.Decode(reader);
                        break;
                    case "error_summary":
                        value.ErrorMessage = StringDecoder.Instance.Decode(reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }
    }
}
