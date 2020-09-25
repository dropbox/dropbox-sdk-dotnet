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

    public class MockHttpMessageHandler : HttpClientHandler
    {
        public delegate Task<HttpResponseMessage> Sender(HttpRequestMessage message);

        /// <summary>
        /// The mock handler.
        /// </summary>
        Func<HttpRequestMessage, Sender, Task<HttpResponseMessage>> handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockHttpMessageHandler"/> class.
        /// </summary>
        /// <param name="response">The mock response.</param>
        public MockHttpMessageHandler(Func<HttpRequestMessage, Sender, Task<HttpResponseMessage>> handler)
        {
            this.handler = handler;
        }

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
