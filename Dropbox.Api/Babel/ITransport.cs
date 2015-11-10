//-----------------------------------------------------------------------------
// <copyright file="ITransport.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Babel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Used to encapsulate both the response object and the response body from
    /// a download operation.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public interface IDownloadResponse<TResponse> : IDisposable
        where TResponse : new()
    {
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        TResponse Response { get; }

        /// <summary>
        /// Asynchronously gets the content as a <see cref="Stream"/>.
        /// </summary>
        /// <returns>The downloaded content as a stream.</returns>
        Task<Stream> GetContentAsStreamAsync();

        /// <summary>
        /// Asynchronously gets the content as a <see cref="byte"/> array.
        /// </summary>
        /// <returns>The downloaded content as a byte array.</returns>
        Task<byte[]> GetContentAsByteArrayAsync();

        /// <summary>
        /// Asynchronously gets the content as <see cref="String"/>.
        /// </summary>
        /// <returns>The downloaded content as a string.</returns>
        Task<string> GetContentAsStringAsync();
    }

    /// <summary>
    /// An interface that abstracts route transports
    /// </summary>
    internal interface ITransport
    {
        /// <summary>
        /// Sends the RPC request asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="host">The server host to send the request to.</param>
        /// <param name="route">The route name.</param>
        /// <param name="requestEncoder">The request encoder.</param>
        /// <param name="resposneDecoder">The response decoder.</param>
        /// <param name="errorDecoder">The error decoder.</param>
        /// <returns>An asynchronous task for the response.</returns>
        Task<TResponse> SendRpcRequestAsync<TRequest, TResponse, TError>(
            TRequest request,
            string host,
            string route,
            IEncoder<TRequest> requestEncoder,
            IDecoder<TResponse> resposneDecoder,
            IDecoder<TError> errorDecoder)
                where TResponse : new();

        /// <summary>
        /// Sends the upload request asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="body">The content to be uploaded.</param>
        /// <param name="host">The server host to send the request to.</param>
        /// <param name="route">The route name.</param>
        /// <param name="requestEncoder">The request encoder.</param>
        /// <param name="resposneDecoder">The response decoder.</param>
        /// <param name="errorDecoder">The error decoder.</param>
        /// <returns>An asynchronous task for the response.</returns>
        Task<TResponse> SendUploadRequestAsync<TRequest, TResponse, TError>(
            TRequest request,
            Stream body,
            string host,
            string route,
            IEncoder<TRequest> requestEncoder,
            IDecoder<TResponse> resposneDecoder,
            IDecoder<TError> errorDecoder)
                where TResponse : new();

        /// <summary>
        /// Sends the download request asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="host">The server host to send the request to.</param>
        /// <param name="route">The route name.</param>
        /// <param name="requestEncoder">The request encoder.</param>
        /// <param name="resposneDecoder">The response decoder.</param>
        /// <param name="errorDecoder">The error decoder.</param>
        /// <returns>An asynchronous task for the response.</returns>
        Task<IDownloadResponse<TResponse>> SendDownloadRequestAsync<TRequest, TResponse, TError>(
            TRequest request,
            string host,
            string route,
            IEncoder<TRequest> requestEncoder,
            IDecoder<TResponse> resposneDecoder,
            IDecoder<TError> errorDecoder)
                where TResponse : new();
    }
}
