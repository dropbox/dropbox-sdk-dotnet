//-----------------------------------------------------------------------------
// <copyright file="MockHttpMessageHandler.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A mock HTTP handler for use with integration tests.
    /// </summary>
    public class MockHttpMessageHandler : HttpClientHandler
    {
        /// <summary>
        /// The mock handler.
        /// </summary>
        private readonly Func<HttpRequestMessage, Sender, Task<HttpResponseMessage>> handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockHttpMessageHandler"/> class.
        /// </summary>
        /// <param name="response">The mock response.</param>
        /// <param name="handler">The mock handler.</param>
        public MockHttpMessageHandler(Func<HttpRequestMessage, Sender, Task<HttpResponseMessage>> handler)
        {
            this.handler = handler;
        }

        /// <summary>
        /// HTTP sender alias.
        /// </summary>
        /// <param name="message">The request message.</param>
        /// <returns>A <see cref="Task"/> for fetching a <see cref="HttpResponseMessage"></returns>.
        public delegate Task<HttpResponseMessage> Sender(HttpRequestMessage message);

        /// <summary>
        /// The send async override.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response.</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return this.handler(request, r => base.SendAsync(r, cancellationToken));
        }
    }
}
