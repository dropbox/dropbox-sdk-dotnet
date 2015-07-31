namespace Dropbox.Api.Babel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// An <see cref="IDecoder"/> implementation that decodes
    /// <see cref="T:Dropbox.Api.Babel.IEncodable`1"/> objects from a
    /// JSON string.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "The context object doesn't represent a resource that can leak, and when it is disposed it cleans up the internal reference")]
    public class JsonDecoder : IDecoder
    {
        /// <summary>
        /// The current object decode context.
        /// </summary>
        private Context context;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDecoder"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public JsonDecoder(string source)
        {
            this.SetSource(source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDecoder"/> class.
        /// </summary>
        /// <param name="json">The JSON object to decoder.</param>
        /// <param name="str">The string representation of the object.</param>
        private JsonDecoder(Json.JsonObject json, string str)
        {
            this.JsonObject = json;
            this.JsonString = str;
        }

        /// <summary>
        /// Gets or sets the JSON object.
        /// </summary>
        /// <value>The JSON object.</value>
        private Json.JsonObject JsonObject { get; set; }

        /// <summary>
        /// Gets or sets the JSON string.
        /// </summary>
        /// <value>The JSON string.</value>
        private string JsonString { get; set; }

        /// <summary>
        /// Decodes the specified source.
        /// </summary>
        /// <typeparam name="T">The type of the object to decode.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The decoded object.</returns>
        public static T Decode<T>(string source)
            where T : IEncodable<T>, new()
        {
            var encodable = new T();
            var decoder = new JsonDecoder(source);

            return encodable.Decode(decoder);
        }

        /// <summary>
        /// Used to get the name of the current union.
        /// </summary>
        /// <returns>The union name</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Unable to parse union</exception>
        public string GetUnionName()
        {
            if (this.JsonObject != null)
            {
                if (this.JsonObject.ContainsKey(".tag"))
                {
                    return this.JsonObject[".tag"].ToString();
                }
            }

            throw new InvalidOperationException(
                "Unable to parse union");
        }

        /// <summary>
        /// Gets a context for the current object, the context object is used to access
        /// individual fields.
        /// </summary>
        /// <returns>The decoder context for the current object.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Nesting GetObject calls is not permitted</exception>
        public IDecoderContext GetObject()
        {
            if (this.context != null)
            {
                throw new InvalidOperationException(
                    "Nesting GetObject calls is not permitted");
            }

            this.context = new Context(this);
            return this.context;
        }

        /// <summary>
        /// Sets the current source for this instance.
        /// </summary>
        /// <param name="source">The source as a string.</param>
        public void SetSource(string source)
        {
            this.context = null;

            var parsed = Json.JsonParser.Parse(source);
            var obj = parsed as Json.JsonObject;
            this.JsonObject = obj;

            var str = parsed as string;
            this.JsonString = str;
        }

        /// <summary>
        /// Sets the current source for this instance.
        /// </summary>
        /// <param name="source">The source as a byte array.</param>
        public void SetSource(byte[] source)
        {
            this.SetSource(Encoding.UTF8.GetString(source, 0, source.Length));
        }

        /// <summary>
        /// Closes the context.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// There is no current context.</exception>
        private void CloseContext()
        {
            if (this.context == null)
            {
                throw new InvalidOperationException("There is no current context.");
            }

            this.context = null;
        }

        /// <summary>
        /// The object encoding context returned by <see cref="JsonDecoder.GetObject"/>.
        /// </summary>
        private class Context : IDecoderContext
        {
            /// <summary>
            /// The parent decoder context.
            /// </summary>
            private JsonDecoder parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="Context"/> class.
            /// </summary>
            /// <param name="parentDecoder">The parent decoder.</param>
            public Context(JsonDecoder parentDecoder)
            {
                this.parent = parentDecoder;
            }

            /// <summary>
            /// Gets the JSON object from the parent decoder.
            /// </summary>
            /// <value>The JSON object.</value>
            public Json.JsonObject JsonObject
            {
                get
                {
                    return this.parent.JsonObject;
                }
            }

            /// <summary>
            /// Indicates whether the specified field is present in the object.
            /// </summary>
            /// <param name="name">The name of the field to check.</param>
            /// <returns>
            ///   <c>true</c> if the field is present; <c>false</c> otherwise.
            /// </returns>
            public bool HasField(string name)
            {
                return this.JsonObject.ContainsKey(name);
            }

            /// <summary>
            /// Gets the value of a field.
            /// </summary>
            /// <typeparam name="T">The expected type of the field.</typeparam>
            /// <param name="name">The name of the field to get.</param>
            /// <returns>
            /// The decoded value of the field.
            /// </returns>
            public T GetField<T>(string name)
            {
                return this.ObjectAsField<T>(this.JsonObject[name], name);
            }

            /// <summary>
            /// Gets the value of a field which is an object.
            /// </summary>
            /// <typeparam name="T">The expected type of the object. This must implement the
            /// <see cref="T:Dropbox.Api.Babel.IEncodable`1" /> interface.</typeparam>
            /// <param name="name">The name of the field to get.</param>
            /// <returns>
            /// The decoded value of the field.
            /// </returns>
            public T GetFieldObject<T>(string name)
                where T : IEncodable<T>, new()
            {
                var value = this.JsonObject[name];
                return this.ObjectAsEncodable<T>(value, name);
            }

            /// <summary>
            /// Gets the value of a field which is a list.
            /// </summary>
            /// <typeparam name="T">The expected type of the list elements.</typeparam>
            /// <param name="name">The name of the field.</param>
            /// <returns>
            /// The decoded list of items.
            /// </returns>
            /// <exception cref="System.InvalidCastException">
            /// The field is not a list.</exception>
            public IEnumerable<T> GetFieldList<T>(string name)
            {
                var value = this.JsonObject[name];
                if (value == null)
                {
                    yield break;
                }

                var list = this.JsonObject[name] as Json.JsonArray;
                if (list == null)
                {
                    throw new InvalidCastException(string.Format(
                        CultureInfo.InvariantCulture,
                        "Field '{0}' is not a list",
                        name));
                }

                foreach (var item in list)
                {
                    yield return this.ObjectAsField<T>(item, name + "[]");
                }
            }

            /// <summary>
            /// Gets the value of a field which is a list of objects that implement
            /// <see cref="T:Dropbox.Api.Babel.IEncodable`1" />.
            /// </summary>
            /// <typeparam name="T">The expected type of the list elements.</typeparam>
            /// <param name="name">The name of the fields.</param>
            /// <returns>
            /// The decoded list of items.
            /// </returns>
            /// <exception cref="System.InvalidCastException">
            /// The field is not a list.</exception>
            public IEnumerable<T> GetFieldObjectList<T>(string name)
                where T : IEncodable<T>, new()
            {
                var value = this.JsonObject[name];
                if (value == null)
                {
                    yield break;
                }

                var list = value as Json.JsonArray;
                if (list == null)
                {
                    throw new InvalidCastException(string.Format(
                        CultureInfo.InvariantCulture,
                        "Field '{0}' is not a list",
                        name));
                }

                foreach (var item in list)
                {
                    yield return this.ObjectAsEncodable<T>(item, name + "[]");
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing,
            /// or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.parent.CloseContext();
            }

            /// <summary>
            /// Decodes a JSON object or a string into an 
            /// <see cref="T:Dropbox.Api.Babel.IEncodable`1"/> object.
            /// </summary>
            /// <typeparam name="T">The type of the object to decode.</typeparam>
            /// <param name="item">The JSON item to decode.</param>
            /// <param name="name">The name of the object.</param>
            /// <returns>The decoded object.</returns>
            /// <exception cref="System.InvalidCastException">
            /// The <paramref name="item"/> is not a JSON object or a string.
            /// </exception>
            private T ObjectAsEncodable<T>(object item, string name)
                where T : IEncodable<T>, new()
            {
                if (item == null)
                {
                    return default(T);
                }

                var obj = item as Json.JsonObject;
                var str = item as string;

                if (obj == null && string.IsNullOrEmpty(str))
                {
                    throw new InvalidCastException(string.Format(
                        CultureInfo.InvariantCulture,
                        "Field '{0}' is not an object or string",
                        name));
                }

                var encodable = new T();
                return encodable.Decode(new JsonDecoder(obj, str));
            }

            /// <summary>
            /// Decodes an object as a field.
            /// </summary>
            /// <typeparam name="T">The type of the field to decode.</typeparam>
            /// <param name="item">The item to decode.</param>
            /// <param name="name">The field name.</param>
            /// <returns>
            /// The decoded object.
            /// </returns>
            /// <exception cref="System.NotSupportedException">The type is not a decodable type.</exception>
            private T ObjectAsField<T>(object item, string name)
            {
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.DateTime:
                        return (T)(object)DateTime.Parse(
                            item.ToString(),
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                    case TypeCode.Boolean:
                        return (T)item;
                    case TypeCode.Double:
                        return (T)(object)Convert.ToDouble(item);
                    case TypeCode.Int32:
                        return (T)(object)Convert.ToInt32(item);
                    case TypeCode.Int64:
                        return (T)(object)Convert.ToInt64(item);
                    case TypeCode.Single:
                        return (T)(object)Convert.ToSingle(item);
                    case TypeCode.UInt32:
                        return (T)(object)Convert.ToUInt32(item);
                    case TypeCode.UInt64:
                        return (T)(object)Convert.ToUInt64(item);
                    case TypeCode.String:
                        return (T)item;
                    default:
                        if (typeof(T).IsAssignableFrom(typeof(byte[])))
                        {
                            return (T)(object)Convert.FromBase64String(item.ToString());
                        }

                        throw new NotSupportedException(string.Format(
                            CultureInfo.InvariantCulture,
                            "Type '{0}' is not supported",
                            typeof(T)));
                }
            }
        }
    }
}
