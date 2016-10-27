//-----------------------------------------------------------------------------
// <copyright file="StructuredException.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace <Namespace>
{
    using System;

    using <Namespace>.Stone;

    /// <summary>
    /// The exception type that will be raised by an <see cref="ITransport" />
    /// implementation if there is an error processing the request which contains
    /// a json body.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    public abstract class StructuredException<TError> : DropboxException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredException{TError}"/> class.
        /// </summary>
        /// <param name="requestId">The Dropbox request id.</param>
        /// <remarks>This constructor is only used when decoded from JSON.</remarks>
        protected internal StructuredException(string requestId)
            : base(requestId, null)
        {
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
        /// <param name="exceptionFunc">The function to create exception.</param>
        /// <returns>The structured exception.</returns>
        internal static TException Decode<TException>(string json, IDecoder<TError> errorDecoder, Func<TException> exceptionFunc)
            where TException : StructuredException<TError>
        {
            return JsonReader.Read(json, new StructuredExceptionDecoder<TException>(errorDecoder, exceptionFunc));
        }

        /// <summary>
        /// The exception decoder.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        private class StructuredExceptionDecoder<TException> : StructDecoder<TException>
            where TException : StructuredException<TError>
        {
            /// <summary>
            /// The error decoder.
            /// </summary>
            private readonly IDecoder<TError> errorDecoder;

            /// <summary>
            /// The function which creates the exception.
            /// </summary>
            private readonly Func<TException> execptionFunc;


            /// <summary>
            /// Initializes a new instance of the <see cref="StructuredExceptionDecoder{TException}"/> class.
            /// </summary>
            /// <param name="errorDecoder">The error decoder.</param>
            /// <param name="execptionFunc">The function which creates the exception.</param>
            public StructuredExceptionDecoder(IDecoder<TError> errorDecoder, Func<TException> execptionFunc)
            {
                this.errorDecoder = errorDecoder;
                this.execptionFunc = execptionFunc;
            }

            /// <summary>
            /// Create a struct instance.
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override TException Create()
            {
                return this.execptionFunc();
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
