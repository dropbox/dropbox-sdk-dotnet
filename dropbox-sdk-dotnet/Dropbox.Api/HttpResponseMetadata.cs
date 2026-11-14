//-----------------------------------------------------------------------------
// <copyright file="HttpResponseMetadata.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;

    /// <summary>
    /// An immutable snapshot that does not retain the HTTP response or its header collections.
    /// </summary>
    internal sealed class HttpResponseMetadata
    {
        /// <summary>The snapshot used when no HTTP response was received.</summary>
        internal static readonly HttpResponseMetadata Empty = new HttpResponseMetadata(null, Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>());

        /// <summary>Initializes a new instance of the <see cref="HttpResponseMetadata"/> class.</summary>
        /// <param name="statusCode">The actual HTTP status code, if available.</param>
        /// <param name="headers">The headers to copy, including content headers.</param>
        internal HttpResponseMetadata(int? statusCode, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            this.StatusCode = statusCode;
            var values = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in headers)
            {
                if (!values.TryGetValue(header.Key, out var list))
                {
                    list = new List<string>();
                    values.Add(header.Key, list);
                }

                list.AddRange(header.Value);
            }

            var snapshot = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in values)
            {
                snapshot.Add(header.Key, header.Value.AsReadOnly());
            }

            this.Headers = new ReadOnlyDictionary<string, IReadOnlyList<string>>(snapshot);
        }

        /// <summary>Gets the actual HTTP status code.</summary>
        internal int? StatusCode { get; }

        /// <summary>Gets the deeply read-only headers.</summary>
        internal IReadOnlyDictionary<string, IReadOnlyList<string>> Headers { get; }

        /// <summary>Copies the response and content headers before the response is disposed.</summary>
        /// <param name="response">The HTTP response.</param>
        /// <returns>An independent snapshot.</returns>
        internal static HttpResponseMetadata FromResponse(HttpResponseMessage response)
        {
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers = response.Headers;
            if (response.Content != null)
            {
                headers = headers.Concat(response.Content.Headers);
            }

            return new HttpResponseMetadata((int)response.StatusCode, headers);
        }
    }
}
