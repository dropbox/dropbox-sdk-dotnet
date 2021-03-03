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
    /// <para>Set team members to see warning before sharing folders outside team.</para>
    /// </summary>
    public class SfExternalInviteWarnDetails
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<SfExternalInviteWarnDetails> Encoder = new SfExternalInviteWarnDetailsEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<SfExternalInviteWarnDetails> Decoder = new SfExternalInviteWarnDetailsDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="SfExternalInviteWarnDetails" />
        /// class.</para>
        /// </summary>
        /// <param name="targetAssetIndex">Target asset position in the Assets list.</param>
        /// <param name="originalFolderName">Original shared folder name.</param>
        /// <param name="newSharingPermission">New sharing permission.</param>
        /// <param name="previousSharingPermission">Previous sharing permission.</param>
        public SfExternalInviteWarnDetails(ulong targetAssetIndex,
                                           string originalFolderName,
                                           string newSharingPermission = null,
                                           string previousSharingPermission = null)
        {
            if (originalFolderName == null)
            {
                throw new sys.ArgumentNullException("originalFolderName");
            }

            this.TargetAssetIndex = targetAssetIndex;
            this.OriginalFolderName = originalFolderName;
            this.NewSharingPermission = newSharingPermission;
            this.PreviousSharingPermission = previousSharingPermission;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="SfExternalInviteWarnDetails" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public SfExternalInviteWarnDetails()
        {
        }

        /// <summary>
        /// <para>Target asset position in the Assets list.</para>
        /// </summary>
        public ulong TargetAssetIndex { get; protected set; }

        /// <summary>
        /// <para>Original shared folder name.</para>
        /// </summary>
        public string OriginalFolderName { get; protected set; }

        /// <summary>
        /// <para>New sharing permission.</para>
        /// </summary>
        public string NewSharingPermission { get; protected set; }

        /// <summary>
        /// <para>Previous sharing permission.</para>
        /// </summary>
        public string PreviousSharingPermission { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="SfExternalInviteWarnDetails" />.</para>
        /// </summary>
        private class SfExternalInviteWarnDetailsEncoder : enc.StructEncoder<SfExternalInviteWarnDetails>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(SfExternalInviteWarnDetails value, enc.IJsonWriter writer)
            {
                WriteProperty("target_asset_index", value.TargetAssetIndex, writer, enc.UInt64Encoder.Instance);
                WriteProperty("original_folder_name", value.OriginalFolderName, writer, enc.StringEncoder.Instance);
                if (value.NewSharingPermission != null)
                {
                    WriteProperty("new_sharing_permission", value.NewSharingPermission, writer, enc.StringEncoder.Instance);
                }
                if (value.PreviousSharingPermission != null)
                {
                    WriteProperty("previous_sharing_permission", value.PreviousSharingPermission, writer, enc.StringEncoder.Instance);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="SfExternalInviteWarnDetails" />.</para>
        /// </summary>
        private class SfExternalInviteWarnDetailsDecoder : enc.StructDecoder<SfExternalInviteWarnDetails>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="SfExternalInviteWarnDetails"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override SfExternalInviteWarnDetails Create()
            {
                return new SfExternalInviteWarnDetails();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(SfExternalInviteWarnDetails value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "target_asset_index":
                        value.TargetAssetIndex = enc.UInt64Decoder.Instance.Decode(reader);
                        break;
                    case "original_folder_name":
                        value.OriginalFolderName = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "new_sharing_permission":
                        value.NewSharingPermission = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "previous_sharing_permission":
                        value.PreviousSharingPermission = enc.StringDecoder.Instance.Decode(reader);
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
