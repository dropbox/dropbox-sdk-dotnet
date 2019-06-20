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
    /// <para>The get thumbnail batch result data object</para>
    /// </summary>
    public class GetThumbnailBatchResultData
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<GetThumbnailBatchResultData> Encoder = new GetThumbnailBatchResultDataEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<GetThumbnailBatchResultData> Decoder = new GetThumbnailBatchResultDataDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="GetThumbnailBatchResultData" />
        /// class.</para>
        /// </summary>
        /// <param name="metadata">The metadata</param>
        /// <param name="thumbnail">A string containing the base64-encoded thumbnail data for
        /// this file.</param>
        public GetThumbnailBatchResultData(FileMetadata metadata,
                                           string thumbnail)
        {
            if (metadata == null)
            {
                throw new sys.ArgumentNullException("metadata");
            }

            if (thumbnail == null)
            {
                throw new sys.ArgumentNullException("thumbnail");
            }

            this.Metadata = metadata;
            this.Thumbnail = thumbnail;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="GetThumbnailBatchResultData" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public GetThumbnailBatchResultData()
        {
        }

        /// <summary>
        /// <para>Gets the metadata of the get thumbnail batch result data</para>
        /// </summary>
        public FileMetadata Metadata { get; protected set; }

        /// <summary>
        /// <para>A string containing the base64-encoded thumbnail data for this file.</para>
        /// </summary>
        public string Thumbnail { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="GetThumbnailBatchResultData" />.</para>
        /// </summary>
        private class GetThumbnailBatchResultDataEncoder : enc.StructEncoder<GetThumbnailBatchResultData>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(GetThumbnailBatchResultData value, enc.IJsonWriter writer)
            {
                WriteProperty("metadata", value.Metadata, writer, global::Dropbox.Api.Files.FileMetadata.Encoder);
                WriteProperty("thumbnail", value.Thumbnail, writer, enc.StringEncoder.Instance);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="GetThumbnailBatchResultData" />.</para>
        /// </summary>
        private class GetThumbnailBatchResultDataDecoder : enc.StructDecoder<GetThumbnailBatchResultData>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="GetThumbnailBatchResultData"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override GetThumbnailBatchResultData Create()
            {
                return new GetThumbnailBatchResultData();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(GetThumbnailBatchResultData value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "metadata":
                        value.Metadata = global::Dropbox.Api.Files.FileMetadata.Decoder.Decode(reader);
                        break;
                    case "thumbnail":
                        value.Thumbnail = enc.StringDecoder.Instance.Decode(reader);
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