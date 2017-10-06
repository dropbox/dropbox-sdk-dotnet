// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.TeamLog
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>Previewed the shared file or folder.</para>
    /// </summary>
    public class SharedContentViewDetails
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<SharedContentViewDetails> Encoder = new SharedContentViewDetailsEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<SharedContentViewDetails> Decoder = new SharedContentViewDetailsDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="SharedContentViewDetails" />
        /// class.</para>
        /// </summary>
        /// <param name="sharedContentLink">Shared content link.</param>
        /// <param name="targetAssetIndex">Target asset position in the Assets list.</param>
        /// <param name="sharingPermission">Sharing permission. Might be missing due to
        /// historical data gap.</param>
        public SharedContentViewDetails(string sharedContentLink,
                                        ulong targetAssetIndex,
                                        string sharingPermission = null)
        {
            if (sharedContentLink == null)
            {
                throw new sys.ArgumentNullException("sharedContentLink");
            }

            this.SharedContentLink = sharedContentLink;
            this.TargetAssetIndex = targetAssetIndex;
            this.SharingPermission = sharingPermission;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="SharedContentViewDetails" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public SharedContentViewDetails()
        {
        }

        /// <summary>
        /// <para>Shared content link.</para>
        /// </summary>
        public string SharedContentLink { get; protected set; }

        /// <summary>
        /// <para>Target asset position in the Assets list.</para>
        /// </summary>
        public ulong TargetAssetIndex { get; protected set; }

        /// <summary>
        /// <para>Sharing permission. Might be missing due to historical data gap.</para>
        /// </summary>
        public string SharingPermission { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="SharedContentViewDetails" />.</para>
        /// </summary>
        private class SharedContentViewDetailsEncoder : enc.StructEncoder<SharedContentViewDetails>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(SharedContentViewDetails value, enc.IJsonWriter writer)
            {
                WriteProperty("shared_content_link", value.SharedContentLink, writer, enc.StringEncoder.Instance);
                WriteProperty("target_asset_index", value.TargetAssetIndex, writer, enc.UInt64Encoder.Instance);
                if (value.SharingPermission != null)
                {
                    WriteProperty("sharing_permission", value.SharingPermission, writer, enc.StringEncoder.Instance);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="SharedContentViewDetails" />.</para>
        /// </summary>
        private class SharedContentViewDetailsDecoder : enc.StructDecoder<SharedContentViewDetails>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="SharedContentViewDetails"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override SharedContentViewDetails Create()
            {
                return new SharedContentViewDetails();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(SharedContentViewDetails value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "shared_content_link":
                        value.SharedContentLink = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "target_asset_index":
                        value.TargetAssetIndex = enc.UInt64Decoder.Instance.Decode(reader);
                        break;
                    case "sharing_permission":
                        value.SharingPermission = enc.StringDecoder.Instance.Decode(reader);
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