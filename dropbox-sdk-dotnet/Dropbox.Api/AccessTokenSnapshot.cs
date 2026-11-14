//-----------------------------------------------------------------------------
// <copyright file="AccessTokenSnapshot.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;

    /// <summary>
    /// An immutable snapshot of a cached access token and its expiration.
    /// </summary>
    /// <remarks>
    /// The token is sensitive and may be expired or revoked. A snapshot does not validate
    /// the token and does not change when the client refreshes its cached credentials.
    /// </remarks>
    public sealed class AccessTokenSnapshot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenSnapshot"/> class.
        /// </summary>
        /// <param name="accessToken">The cached access token.</param>
        /// <param name="expiresAt">The UTC expiration, or null if unknown.</param>
        internal AccessTokenSnapshot(string accessToken, DateTimeOffset? expiresAt)
        {
            this.AccessToken = accessToken;
            this.ExpiresAt = expiresAt;
        }

        /// <summary>
        /// Gets the sensitive access token. Do not log or disclose this value.
        /// </summary>
        public string AccessToken { get; }

        /// <summary>
        /// Gets the absolute expiration in UTC, or null if unknown.
        /// Unknown expiration does not imply an unlimited lifetime.
        /// </summary>
        public DateTimeOffset? ExpiresAt { get; }
    }
}
