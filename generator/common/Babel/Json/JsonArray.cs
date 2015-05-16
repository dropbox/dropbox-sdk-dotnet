//----------------------------------------------------------------------------
//  <copyright file="JsonArray.cs" company="Dropbox Inc">
//      Copyright (c) Dropbox Inc. All rights reserved.
//  </copyright>
//----------------------------------------------------------------------------

namespace Dropbox.Api.Babel.Json
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>Represents an array in a JSON object.</summary>
    internal sealed class JsonArray : IList<object>
    {
        /// <summary>Storage for the array.</summary>
        private List<object> array = new List<object>();

        /// <summary>Initializes a new instance of the <see cref="JsonArray"/> class.</summary>
        public JsonArray()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonArray"/> class.</summary>
        /// <param name="source">The contents of the array</param>
        public JsonArray(params object[] source)
        {
            this.array.AddRange(source);
        }

        /// <summary>Gets the number of elements in the array.</summary>
        public int Count
        {
            get { return this.array.Count; }
        }

        /// <summary>Gets a value indicating whether the array is read-only.</summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>Gets or sets the array item at a specific index</summary>
        /// <param name="index">The array index.</param>
        /// <returns>The item at <paramref name="index"/>.</returns>
        public object this[int index]
        {
            get
            {
                return this.array[index];
            }

            set
            {
                this.array[index] = value;
            }
        }

        /// <summary>Creates a JSON string representation of the array.</summary>.
        /// <returns>The serialized JSON for the array.</returns>
        public string Stringify()
        {
            var builder = new StringBuilder();
            builder.Append("[");
            builder.Append(string.Join(",", this.array.Select(JsonObject.Stringify)));
            builder.Append("]");
            return builder.ToString();
        }

        /// <summary>Finds the index (if any) of an object in the array.</summary>
        /// <param name="item">The object to find.</param>
        /// <returns>The index of the object in the array, or -1 if it is not found.</returns>
        public int IndexOf(object item)
        {
            return this.array.IndexOf(item);
        }

        /// <summary>Insert an item at a specific position in the array.</summary>
        /// <param name="index">The index at which the item is to be inserted.</param>
        /// <param name="item">The item to insert.</param>
        public void Insert(int index, object item)
        {
            this.array.Insert(index, item);
        }

        /// <summary>Remove the item at the specified index.</summary>
        /// <param name="index">The index of the item to be removed.</param>
        public void RemoveAt(int index)
        {
            this.array.RemoveAt(index);
        }

        /// <summary>Adds an item to the end of the array.</summary>
        /// <param name="item">The item to add.</param>
        public void Add(object item)
        {
            this.array.Add(item);
        }

        /// <summary>Clears the contents of the array.</summary>
        public void Clear()
        {
            this.array.Clear();
        }

        /// <summary>Checks if the item is in the array.</summary>
        /// <param name="item">The item to search for.</param>
        /// <returns><c>true</c> if the item is in the array; <c>false</c> otherwise.</returns>
        public bool Contains(object item)
        {
            return this.array.Contains(item);
        }

        /// <summary>Copies the entire array</summary>
        /// <param name="array">The array to copy into.</param>
        /// <param name="arrayIndex">The index in <paramref name="array"/> at which the copy starts.</param>
        public void CopyTo(object[] array, int arrayIndex)
        {
            this.array.CopyTo(array, arrayIndex);
        }

        /// <summary>Removes an item from the array.</summary>
        /// <param name="item">The item to be removed.</param>
        /// <returns><c>true</c> if the item was removed; <c>false</c> otherwise.</returns>
        public bool Remove(object item)
        {
            return this.array.Remove(item);
        }

        /// <summary>Returns an enumerator that iterates through the array.</summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<object> GetEnumerator()
        {
            return this.array.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through the array.</summary>
        /// <returns>The enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}