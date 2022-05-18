// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Sharing.Routes
{
    using sys = System;
    using io = System.IO;
    using col = System.Collections.Generic;
    using t = System.Threading.Tasks;
    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>The routes for the <see cref="N:Dropbox.Api.Sharing"/> namespace</para>
    /// </summary>
    public class SharingAppRoutes
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="SharingAppRoutes" />
        /// class.</para>
        /// </summary>
        /// <param name="transport">The transport to use</param>
        internal SharingAppRoutes(enc.ITransport transport)
        {
            this.Transport = transport;
        }

        /// <summary>
        /// <para>Gets the transport used for these routes</para>
        /// </summary>
        internal enc.ITransport Transport { get; private set; }

        /// <summary>
        /// <para>Get the shared link's metadata.</para>
        /// </summary>
        /// <param name="getSharedLinkMetadataArg">The request parameters</param>
        /// <returns>The task that represents the asynchronous send operation. The TResult
        /// parameter contains the response from the server.</returns>
        /// <exception cref="Dropbox.Api.ApiException{TError}">Thrown if there is an error
        /// processing the request; This will contain a <see
        /// cref="SharedLinkError"/>.</exception>
        public t.Task<SharedLinkMetadata> GetSharedLinkMetadataAsync(GetSharedLinkMetadataArg getSharedLinkMetadataArg)
        {
            return this.Transport.SendRpcRequestAsync<GetSharedLinkMetadataArg, SharedLinkMetadata, SharedLinkError>(getSharedLinkMetadataArg, "api", "/sharing/get_shared_link_metadata", "app", global::Dropbox.Api.Sharing.GetSharedLinkMetadataArg.Encoder, global::Dropbox.Api.Sharing.SharedLinkMetadata.Decoder, global::Dropbox.Api.Sharing.SharedLinkError.Decoder);
        }

        /// <summary>
        /// <para>Begins an asynchronous send to the get shared link metadata route.</para>
        /// </summary>
        /// <param name="getSharedLinkMetadataArg">The request parameters.</param>
        /// <param name="callback">The method to be called when the asynchronous send is
        /// completed.</param>
        /// <param name="state">A user provided object that distinguished this send from other
        /// send requests.</param>
        /// <returns>An object that represents the asynchronous send request.</returns>
        public sys.IAsyncResult BeginGetSharedLinkMetadata(GetSharedLinkMetadataArg getSharedLinkMetadataArg, sys.AsyncCallback callback, object state = null)
        {
            var task = this.GetSharedLinkMetadataAsync(getSharedLinkMetadataArg);

            return enc.Util.ToApm(task, callback, state);
        }

        /// <summary>
        /// <para>Get the shared link's metadata.</para>
        /// </summary>
        /// <param name="url">URL of the shared link.</param>
        /// <param name="path">If the shared link is to a folder, this parameter can be used to
        /// retrieve the metadata for a specific file or sub-folder in this folder. A relative
        /// path should be used.</param>
        /// <param name="linkPassword">If the shared link has a password, this parameter can be
        /// used.</param>
        /// <returns>The task that represents the asynchronous send operation. The TResult
        /// parameter contains the response from the server.</returns>
        /// <exception cref="Dropbox.Api.ApiException{TError}">Thrown if there is an error
        /// processing the request; This will contain a <see
        /// cref="SharedLinkError"/>.</exception>
        public t.Task<SharedLinkMetadata> GetSharedLinkMetadataAsync(string url,
                                                                     string path = null,
                                                                     string linkPassword = null)
        {
            var getSharedLinkMetadataArg = new GetSharedLinkMetadataArg(url,
                                                                        path,
                                                                        linkPassword);

            return this.GetSharedLinkMetadataAsync(getSharedLinkMetadataArg);
        }

        /// <summary>
        /// <para>Begins an asynchronous send to the get shared link metadata route.</para>
        /// </summary>
        /// <param name="url">URL of the shared link.</param>
        /// <param name="path">If the shared link is to a folder, this parameter can be used to
        /// retrieve the metadata for a specific file or sub-folder in this folder. A relative
        /// path should be used.</param>
        /// <param name="linkPassword">If the shared link has a password, this parameter can be
        /// used.</param>
        /// <param name="callback">The method to be called when the asynchronous send is
        /// completed.</param>
        /// <param name="callbackState">A user provided object that distinguished this send
        /// from other send requests.</param>
        /// <returns>An object that represents the asynchronous send request.</returns>
        public sys.IAsyncResult BeginGetSharedLinkMetadata(string url,
                                                           string path = null,
                                                           string linkPassword = null,
                                                           sys.AsyncCallback callback = null,
                                                           object callbackState = null)
        {
            var getSharedLinkMetadataArg = new GetSharedLinkMetadataArg(url,
                                                                        path,
                                                                        linkPassword);

            return this.BeginGetSharedLinkMetadata(getSharedLinkMetadataArg, callback, callbackState);
        }

        /// <summary>
        /// <para>Waits for the pending asynchronous send to the get shared link metadata route
        /// to complete</para>
        /// </summary>
        /// <param name="asyncResult">The reference to the pending asynchronous send
        /// request</param>
        /// <returns>The response to the send request</returns>
        /// <exception cref="Dropbox.Api.ApiException{TError}">Thrown if there is an error
        /// processing the request; This will contain a <see
        /// cref="SharedLinkError"/>.</exception>
        public SharedLinkMetadata EndGetSharedLinkMetadata(sys.IAsyncResult asyncResult)
        {
            var task = asyncResult as t.Task<SharedLinkMetadata>;
            if (task == null)
            {
                throw new sys.InvalidOperationException();
            }

            return task.Result;
        }
    }
}
