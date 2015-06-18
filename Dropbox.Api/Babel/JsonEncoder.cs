namespace Dropbox.Api.Babel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// An <see cref="IEncoder"/> implementation that encodes
    /// <see cref="T:Dropbox.Api.Babel.IEncodable`1"/> objects to a
    /// JSON string.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "The context object doesn't represent a resource that can leak, and when it is disposed it cleans up the internal reference")]
    public sealed class JsonEncoder : IEncoder
    {
        /// <summary>
        /// The object encoding context, if any.
        /// </summary>
        private Context context;

        /// <summary>
        /// Gets or sets the JSON object.
        /// </summary>
        /// <value>The JSON object.</value>
        private object JsonObject { get; set; }

        /// <summary>
        /// Gets the JSON string.
        /// </summary>
        /// <value>The JSON string.</value>
        private string JsonString
        {
            get
            {
                return Json.JsonObject.Stringify(this.JsonObject);
            }
        }

        /// <summary>
        /// Encodes the specified object.
        /// </summary>
        /// <typeparam name="T">The type of the object to encode.</typeparam>
        /// <param name="encodable">The object to encode.</param>
        /// <returns>The encoded object as a JSON string.</returns>
        public static string Encode<T>(T encodable)
            where T : IEncodable<T>
        {
            var encoder = new JsonEncoder();
            encodable.Encode(encoder);
            return encoder.JsonString;
        }

        /// <summary>
        /// Adds an object, returning an encoder context that can be used to
        /// add fields to the object.
        /// </summary>
        /// <returns>The encoder context.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Cannot open a context while one is currently open</exception>
        public IEncoderContext AddObject()
        {
            if (this.context != null)
            {
                throw new InvalidOperationException("Cannot open a context while one is currently open");
            }

            this.context = new Context(this);

            return this.context;
        }

        /// <summary>
        /// Gets the encoded object as a string.
        /// </summary>
        /// <returns>The string encoding of the object.</returns>
        public string GetEncodedString()
        {
            return this.JsonString;
        }

        /// <summary>
        /// Gets the encoded object as a byte array.
        /// </summary>
        /// <returns>The byte array encoding of the object.</returns>
        public byte[] GetEncodedBytes()
        {
            return Encoding.UTF8.GetBytes(this.JsonString);
        }

        /// <summary>
        /// Closes the context.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <exception cref="System.InvalidOperationException">
        /// There is no current context.</exception>
        private void CloseContext(Json.JsonObject obj)
        {
            if (this.context == null)
            {
                throw new InvalidOperationException("There is no current context.");
            }

            this.context = null;
            this.JsonObject = obj;
        }

        /// <summary>
        /// The object encoding context.
        /// </summary>
        private class Context : IEncoderContext
        {
            /// <summary>
            /// The parent encoder.
            /// </summary>
            private JsonEncoder parent;

            /// <summary>
            /// The object being encoded.
            /// </summary>
            private Json.JsonObject obj;

            /// <summary>
            /// Initializes a new instance of the <see cref="Context"/> class.
            /// </summary>
            /// <param name="parentEncoder">The parent.</param>
            public Context(JsonEncoder parentEncoder)
            {
                this.parent = parentEncoder;
                this.obj = new Json.JsonObject();
            }

            /// <summary>
            /// Adds a field to the current encoding context.
            /// </summary>
            /// <typeparam name="T">The type of the field that is being added.</typeparam>
            /// <param name="name">The field name.</param>
            /// <param name="value">The value of the field.</param>
            public void AddField<T>(string name, T value)
            {
                this.obj[name] = this.EncodeField<T>(value);
            }

            /// <summary>
            /// Adds a field that implements the
            /// <see cref="T:Dropbox.Api.Babel.IEncodable`1" /> interface to the current
            /// encoding context.
            /// </summary>
            /// <typeparam name="T">The type of the field that is being added.</typeparam>
            /// <param name="name">The field name.</param>
            /// <param name="value">The value of the field.</param>
            public void AddFieldObject<T>(string name, T value)
                where T : IEncodable<T>, new()
            {
                var encoder = new JsonEncoder();
                value.Encode(encoder);
                this.obj[name] = encoder.JsonObject;
            }

            /// <summary>
            /// Adds a field that is a list of objects that implement the
            /// <see cref="T:Dropbox.Api.Babel.IEncodable`1" /> interface.
            /// </summary>
            /// <typeparam name="T">The element type of each list item.</typeparam>
            /// <param name="name">The field name.</param>
            /// <param name="value">The value of the field.</param>
            public void AddFieldObjectList<T>(string name, IEnumerable<T> value)
                where T : IEncodable<T>, new()
            {
                var array = new Json.JsonArray();

                foreach (var item in value)
                {
                    var encoder = new JsonEncoder();
                    item.Encode(encoder);
                    array.Add(encoder.JsonObject);
                }

                this.obj[name] = array;
            }

            /// <summary>
            /// Adds a field that is a list of items.
            /// </summary>
            /// <typeparam name="T">The element type of each list item.</typeparam>
            /// <param name="name">The field name.</param>
            /// <param name="value">The value of the field.</param>
            public void AddFieldList<T>(string name, IEnumerable<T> value)
            {
                this.obj[name] = new Json.JsonArray(value.Select(this.EncodeField<T>).ToArray());
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing,
            /// or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.parent.CloseContext(this.obj);
            }

            /// <summary>
            /// Encodes a field value
            /// </summary>
            /// <typeparam name="T">The type of the field to encode.</typeparam>
            /// <param name="value">The value of the field.</param>
            /// <returns>The encoded field.</returns>
            /// <exception cref="System.NotSupportedException">
            /// The field type is not supported.
            /// </exception>
            private object EncodeField<T>(T value)
            {
                if (value == null)
                {
                    return null;
                }

                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.DateTime:
                        return ((DateTime)(object)value).ToString("O");
                    case TypeCode.Boolean:
                    case TypeCode.Double:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Single:
                    case TypeCode.String:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return value;
                    default:
                        var array = value as byte[];
                        if (array != null)
                        {
                            return Convert.ToBase64String(array);
                        }

                        throw new NotSupportedException(string.Format(
                            CultureInfo.InvariantCulture,
                            "The type {0} is not supported",
                            typeof(T)));
                }
            }
        }
    }
}
