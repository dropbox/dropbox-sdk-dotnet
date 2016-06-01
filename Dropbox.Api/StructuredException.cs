//-----------------------------------------------------------------------------
// <copyright file="StructuredException.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;

    using Dropbox.Api.Stone;

    /// <summary>
    /// The exception type that will be raised by an <see cref="ITransport" />
    /// implementation if there is an error processing the request which contains
    /// a json body.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    public abstract class StructuredException<TError> : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredException{TError}"/> class.
        /// </summary>
        /// <remarks>This constructor is only used when decoded from JSON.</remarks>
        protected internal StructuredException()
            : this(default(TError))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredException{TError}"/> class.
        /// </summary>
        /// <param name="errorResponse">The error response.</param>
        protected internal StructuredException(TError errorResponse)
            : this(errorResponse, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredException{TError}"/> class.
        /// </summary>
        /// <param name="errorResponse">The error response.</param>
        /// <param name="message">The message.</param>
        protected internal StructuredException(TError errorResponse, string message)
            : this(errorResponse, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredException{TError}"/> class.
        /// </summary>
        /// <param name="errorResponse">The error response.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        protected internal StructuredException(TError errorResponse, string message, Exception inner)
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
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="json">The json.</param>
        /// <param name="errorDecoder">The error json.</param>
        /// <returns>The <see cref="ApiException{TError}"/></returns>
        internal static TException Decode<TException>(string json, IDecoder<TError> errorDecoder)
            where TException : StructuredException<TError>, new()
        {
            return JsonReader.Read(json, new StructuredExceptionDecoder<TException>(errorDecoder));
        }

        /// <summary>
        /// The exception decoder.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        private class StructuredExceptionDecoder<TException> : StructDecoder<TException>
            where TException : StructuredException<TError>, new()
        {
            /// <summary>
            /// The error decoder.
            /// </summary>
            private readonly IDecoder<TError> errorDecoder;

            /// <summary>
            /// Initializes a new instance of the <see cref="StructuredExceptionDecoder{TException}"/> class.
            /// </summary>
            /// <param name="errorDecoder">The error decoder.</param>
            public StructuredExceptionDecoder(IDecoder<TError> errorDecoder)
            {
                this.errorDecoder = errorDecoder;
            }

            /// <summary>
            /// Create a struct instance.
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override TException Create()
            {
                return new TException();
            }

            /// <summary>
            /// Set given field.
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The reader.</param>
            protected override void SetField(TException value, string fieldName, IJsonReader reader)
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
