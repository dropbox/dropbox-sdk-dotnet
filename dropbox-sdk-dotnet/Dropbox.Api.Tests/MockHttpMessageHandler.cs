//-----------------------------------------------------------------------------
// <copyright file="MockHttpMessageHandler.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Tests
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        /// <summary>
        /// The request as it would be sent to the server.
        /// </summary>
        public HttpRequestMessage lastRequest { get; set; }

        /// <summary>
        /// The fake response.
        /// </summary>
        private readonly HttpResponseMessage response;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MockHttpMessageHandler"/> class.
        /// </summary>
        /// <param name="response">The mock response.</param>
        public MockHttpMessageHandler(HttpResponseMessage response)
        {
            this.response = response;
        }

        /// <summary>
        /// The send async override.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response.</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.lastRequest = request;
            return Task.FromResult(this.response);
        }
    }
}
