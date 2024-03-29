// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Files
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>The list folder longpoll result object</para>
    /// </summary>
    public class ListFolderLongpollResult
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<ListFolderLongpollResult> Encoder = new ListFolderLongpollResultEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<ListFolderLongpollResult> Decoder = new ListFolderLongpollResultDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ListFolderLongpollResult" />
        /// class.</para>
        /// </summary>
        /// <param name="changes">Indicates whether new changes are available. If true, call
        /// <see cref="Dropbox.Api.Files.Routes.FilesAppRoutes.ListFolderContinueAsync" /> <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderContinueAsync" /> to
        /// retrieve the changes.</param>
        /// <param name="backoff">If present, backoff for at least this many seconds before
        /// calling <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderLongpollAsync" />
        /// again.</param>
        public ListFolderLongpollResult(bool changes,
                                        ulong? backoff = null)
        {
            this.Changes = changes;
            this.Backoff = backoff;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ListFolderLongpollResult" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public ListFolderLongpollResult()
        {
        }

        /// <summary>
        /// <para>Indicates whether new changes are available. If true, call <see
        /// cref="Dropbox.Api.Files.Routes.FilesAppRoutes.ListFolderContinueAsync" /> <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderContinueAsync" /> to
        /// retrieve the changes.</para>
        /// </summary>
        public bool Changes { get; protected set; }

        /// <summary>
        /// <para>If present, backoff for at least this many seconds before calling <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderLongpollAsync" />
        /// again.</para>
        /// </summary>
        public ulong? Backoff { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="ListFolderLongpollResult" />.</para>
        /// </summary>
        private class ListFolderLongpollResultEncoder : enc.StructEncoder<ListFolderLongpollResult>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(ListFolderLongpollResult value, enc.IJsonWriter writer)
            {
                WriteProperty("changes", value.Changes, writer, enc.BooleanEncoder.Instance);
                if (value.Backoff != null)
                {
                    WriteProperty("backoff", value.Backoff.Value, writer, enc.UInt64Encoder.Instance);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="ListFolderLongpollResult" />.</para>
        /// </summary>
        private class ListFolderLongpollResultDecoder : enc.StructDecoder<ListFolderLongpollResult>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="ListFolderLongpollResult"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override ListFolderLongpollResult Create()
            {
                return new ListFolderLongpollResult();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(ListFolderLongpollResult value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "changes":
                        value.Changes = enc.BooleanDecoder.Instance.Decode(reader);
                        break;
                    case "backoff":
                        value.Backoff = enc.UInt64Decoder.Instance.Decode(reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        #endregion
    }
}
