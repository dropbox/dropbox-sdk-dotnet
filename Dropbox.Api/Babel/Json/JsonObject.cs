//----------------------------------------------------------------------------
//  <copyright file="JsonObject.cs" company="Dropbox Inc">
//      Copyright (c) Dropbox Inc. All rights reserved.
//  </copyright>
//----------------------------------------------------------------------------

namespace Dropbox.Api.Babel.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>Represents a JSON object.</summary>
    internal sealed class JsonObject : DynamicObject
    {
        /// <summary>Storage for the object members.</summary>
        private Dictionary<string, object> data = new Dictionary<string, object>();

        /// <summary>Initializes a new instance of the <see cref="JsonObject"/> class.</summary>
        public JsonObject()
        {
        }

        /// <summary>Gets the number of defined members.</summary>
        public int Count
        {
            get { return this.data.Count; }
        }

        /// <summary>Gets the names of the object members.</summary>
        public IEnumerable<string> Keys
        {
            get
            {
                return this.data.Keys;
            }
        }

        /// <summary>Gets or sets an object member by name.</summary>
        /// <param name="name">The name of the member to access.</param>
        /// <returns>The value of the member.</returns>
        public object this[string name]
        {
            get
            {
                return this.data[name];
            }

            set
            {
                this.data[name] = value;
            }
        }

        /// <summary>Parses a string as a <see cref="JsonObject"/>.</summary>
        /// <param name="input">The string to parse</param>
        /// <returns>The parsed object.</returns>
        public static JsonObject Parse(string input)
        {
            var value = JsonParser.Parse(input) as JsonObject;
            if (value == null)
            {
                throw new Exception("JSON data is not an object");
            }

            return value;
        }

        /// <summary>Parses a string as a <see cref="JsonObject"/>.</summary>
        /// <param name="input">The string to parse.</param>
        /// <param name="json">On success, the parsed object.</param>
        /// <returns><c>true</c> if <paramref name="input"/> could be parsed as a JSON object; 
        /// <c>false</c> otherwise.</returns>
        public static bool TryParse(string input, out JsonObject json)
        {
            try
            {
                json = JsonParser.Parse(input) as JsonObject;
            }
            catch (Exception)
            {
                json = null;
            }

            return json != null;
        }

        /// <summary>Gets the value of a member</summary>
        /// <param name="name">The name of the member to get.</param>
        /// <param name="value">The value of the member, if it exists.</param>
        /// <returns><c>true</c> if a member names <paramref name="name"/> exists;
        /// <c>false</c> otherwise.</returns>
        public bool TryGetValue(string name, out object value)
        {
            return this.data.TryGetValue(name, out value);
        }

        /// <summary>Gets the value of a member</summary>
        /// <typeparam name="T">The expected type of the member.</typeparam>
        /// <param name="name">The name of the member to get.</param>
        /// <param name="value">The value of the member, if it exists.</param>
        /// <returns><c>true</c> if a member names <paramref name="name"/> exists;
        /// <c>false</c> otherwise.</returns>
        public bool TryGetValue<T>(string name, out T value)
        {
            object obj;

            if (!this.data.TryGetValue(name, out obj))
            {
                value = default(T);
                return false;
            }
            else
            {
                value = (T)obj;
                return true;
            }
        }

        /// <summary>Checks if the object has a member with the specified name.</summary>
        /// <param name="name">The name of the member to check for.</param>
        /// <returns><c>true</c> if the member exists; <c>false</c> otherwise.</returns>
        public bool ContainsKey(string name)
        {
            return this.data.ContainsKey(name);
        }

        /// <summary>Clears all members.</summary>
        public void Clear()
        {
            this.data.Clear();
        }

        /// <summary>Adds a new member to the object.</summary>
        /// <param name="name">The member name.</param>
        /// <param name="value">The member value.</param>
        public void Add(string name, object value)
        {
            this.data.Add(name, value);
        }

        /// <summary>Removes a specific member from the object.</summary>
        /// <param name="name">The name of the member to remove.</param>
        /// <returns><c>true</c> if a member was removed; <c>false</c> otherwise.</returns>
        public bool Remove(string name)
        {
            return this.data.Remove(name);
        }

        /// <summary>Removes a specific member from the object.</summary>
        /// <param name="name">The name of the member to remove.</param>
        /// <param name="value">If a member was removed, the value of that member.</param>
        /// <returns><c>true</c> if a member was removed; <c>false</c> otherwise.</returns>
        public bool Remove(string name, out object value)
        {
            this.data.TryGetValue(name, out value);
            return this.data.Remove(name);
        }

        /// <summary>Create the JSON representation of the object.</summary>
        /// <returns>The JSON representation of the object.</returns>
        public string Stringify()
        {
            var builder = new StringBuilder();
            builder.Append("{");

            var index = 0;
            foreach (var pair in this.data)
            {
                builder.Append(Stringify(pair.Key)).Append(':').Append(Stringify(pair.Value));

                index++;
                if (index < this.data.Count)
                {
                    builder.Append(",");
                }
            }

            builder.Append("}");
            return builder.ToString();
        }

        /// <summary>Gets a member value</summary>
        /// <param name="binder">Information about the get member operation.</param>
        /// <param name="result">On success, the value of the member</param>
        /// <returns><c>true</c> if a member was found; <c>false</c> otherwise.</returns>
        /// <remarks>The members <c>Count</c>, <c>Length</c>, and <c>Keys</c> 
        /// return information about the object rather than the value of 
        /// member of those names. If members with those names exist, use the indexed 
        /// accessor to get or set them.</remarks>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            switch (binder.Name)
            {
                case "Count":
                case "Length":
                    result = this.data.Count;
                    return true;
                case "Keys":
                    result = this.data.Keys.ToArray();
                    return true;
                default:
                    return this.data.TryGetValue(binder.Name, out result);
            }
        }

        /// <summary>Sets a member value</summary>
        /// <param name="binder">Information about the set member operation.</param>
        /// <param name="value">The value of the member</param>
        /// <returns><c>true</c> if a member was set; <c>false</c> otherwise.</returns>
        /// <remarks>The members <c>Count</c>, <c>Length</c>, and <c>Keys</c>
        /// cannot be set directly. If members with those names exist, use the indexed 
        /// accessor to get or set them.</remarks>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            switch (binder.Name)
            {
                case "Count":
                case "Length":
                case "Keys":
                    return false;
                default:
                    this.data[binder.Name] = value;
                    return true;
            }
        }

        /// <summary>Gets an indexed value</summary>
        /// <param name="binder">Information about the operation.</param>
        /// <param name="indexes">The indices to the operation. This object only
        /// supports a single index.</param>
        /// <param name="result">The member at the index, if any.</param>
        /// <returns><c>true</c> if a member was found; <c>false</c> otherwise.</returns>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length != 1)
            {
                result = null;
                return false;
            }

            return this.data.TryGetValue(indexes[0].ToString(), out result);
        }

        /// <summary>Sets an indexed value</summary>
        /// <param name="binder">Information about the operation.</param>
        /// <param name="indexes">The indices to the operation. This object only
        /// supports a single index.</param>
        /// <param name="value">The value to set.</param>
        /// <returns><c>true</c> if a member was found; <c>false</c> otherwise.</returns>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length != 1)
            {
                return false;
            }

            this.data[indexes[0].ToString()] = value;
            return true;
        }

        /// <summary>Creates the JSON representation of an arbitrary value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The JSON representation of the value.</returns>
        internal static string Stringify(object value)
        {
            if (value == null)
            {
                return "null";
            }
            else if (value is string)
            {
                return Stringify((string)value);
            }
            else if (value is bool)
            {
                return Stringify((bool)value);
            }
            else if (value is float)
            {
                return Stringify((float)value);
            }
            else if (value is double)
            {
                return Stringify((double)value);
            }
            else if (value is JsonObject)
            {
                return ((JsonObject)value).Stringify();
            }
            else if (value is JsonArray)
            {
                return ((JsonArray)value).Stringify();
            }
            else if (value is IEnumerable)
            {
                var array = new JsonArray((IEnumerable)value);
                return array.Stringify();
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>Creates the JSON representation of a string.</summary>
        /// <param name="value">The string to serialize.</param>
        /// <returns>The JSON representation of a string.</returns>
        private static string Stringify(string value)
        {
            var builder = new StringBuilder();
            builder.Append('"');

            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];

                if (char.IsControl(c))
                {
                    switch (c)
                    {
                        case '\b':
                            builder.Append("\\b");
                            break;
                        case '\f':
                            builder.Append("\\f");
                            break;
                        case '\n':
                            builder.Append("\\n");
                            break;
                        case '\r':
                            builder.Append("\\r");
                            break;
                        case '\t':
                            builder.Append("\\t");
                            break;
                        default:
                            var utf16 = (ushort)c;
                            builder.AppendFormat("\\u{0:04X}", utf16);
                            break;
                    }
                }
                else
                {
                    if (c == '\\' || c == '"')
                    {
                        builder.Append('\\');
                    }

                    builder.Append(c);
                }
            }

            builder.Append('"');
            return builder.ToString();
        }

        /// <summary>Creates the JSON representation of a boolean.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><c>&quot;true&quot;</c> or <c>&quot;false&quot;</c>.</returns>
        private static string Stringify(bool value)
        {
            return value ? "true" : "false";
        }

        /// <summary>Creates the JSON representation of a double.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A numeric string</returns>
        private static string Stringify(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>Creates the JSON representation of a float.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A numeric string</returns>
        private static string Stringify(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
