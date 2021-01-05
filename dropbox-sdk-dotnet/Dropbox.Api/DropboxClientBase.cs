//-----------------------------------------------------------------------------
// <copyright file="DropboxClientBase.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;
    using Dropbox.Api.Stone;

    /// <summary>
    /// The base class for all Dropbox clients.
    /// </summary>
    public abstract class DropboxClientBase : IDisposable
    {
        /// <summary>
        /// The transport.
        /// </summary>
        private readonly ITransport transport;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropboxClientBase"/> class.
        /// </summary>
        /// <param name="transport">The transport.</param>
        internal DropboxClientBase(ITransport transport)
        {
            this.transport = transport;
            this.InitializeRoutes(this.transport);
        }

        /// <summary>
        /// The public dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes routes.
        /// </summary>
        /// <param name="transport">The transport.</param>
        internal abstract void InitializeRoutes(ITransport transport);

        /// <summary>
        /// The actual disposing logic.
        /// </summary>
        /// <param name="disposing">If is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // ITransport is safe for multiple disposal.
                this.transport.Dispose();
            }
        }
    }
}
