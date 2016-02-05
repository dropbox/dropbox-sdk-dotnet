// <auto-generated>
// Auto-generated by BabelAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Files
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Babel;

    /// <summary>
    /// <para>Sharing info for a folder which is contained in a shared folder or is a shared
    /// folder mount point.</para>
    /// </summary>
    /// <seealso cref="Dropbox.Api.Files.SharingInfo" />
    public class FolderSharingInfo : SharingInfo
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<FolderSharingInfo> Encoder = new FolderSharingInfoEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<FolderSharingInfo> Decoder = new FolderSharingInfoDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="FolderSharingInfo" />
        /// class.</para>
        /// </summary>
        /// <param name="readOnly">True if the file or folder is inside a read-only shared
        /// folder.</param>
        /// <param name="parentSharedFolderId">Set if the folder is contained by a shared
        /// folder.</param>
        /// <param name="sharedFolderId">If this folder is a shared folder mount point, the ID
        /// of the shared folder mounted at this location.</param>
        public FolderSharingInfo(bool readOnly,
                                 string parentSharedFolderId = null,
                                 string sharedFolderId = null)
            : base(readOnly)
        {
            if (parentSharedFolderId != null && (!re.Regex.IsMatch(parentSharedFolderId, @"\A(?:[-_0-9a-zA-Z:]+)\z")))
            {
                throw new sys.ArgumentOutOfRangeException("parentSharedFolderId");
            }

            if (sharedFolderId != null && (!re.Regex.IsMatch(sharedFolderId, @"\A(?:[-_0-9a-zA-Z:]+)\z")))
            {
                throw new sys.ArgumentOutOfRangeException("sharedFolderId");
            }

            this.ParentSharedFolderId = parentSharedFolderId;
            this.SharedFolderId = sharedFolderId;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="FolderSharingInfo" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        public FolderSharingInfo()
        {
        }

        /// <summary>
        /// <para>Set if the folder is contained by a shared folder.</para>
        /// </summary>
        public string ParentSharedFolderId { get; protected set; }

        /// <summary>
        /// <para>If this folder is a shared folder mount point, the ID of the shared folder
        /// mounted at this location.</para>
        /// </summary>
        public string SharedFolderId { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="FolderSharingInfo" />.</para>
        /// </summary>
        private class FolderSharingInfoEncoder : enc.StructEncoder<FolderSharingInfo>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(FolderSharingInfo value, enc.IJsonWriter writer)
            {
                WriteProperty("read_only", value.ReadOnly, writer, enc.BooleanEncoder.Instance);
                if (value.ParentSharedFolderId != null)
                {
                    WriteProperty("parent_shared_folder_id", value.ParentSharedFolderId, writer, enc.StringEncoder.Instance);
                }
                if (value.SharedFolderId != null)
                {
                    WriteProperty("shared_folder_id", value.SharedFolderId, writer, enc.StringEncoder.Instance);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="FolderSharingInfo" />.</para>
        /// </summary>
        private class FolderSharingInfoDecoder : enc.StructDecoder<FolderSharingInfo>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="FolderSharingInfo" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override FolderSharingInfo Create()
            {
                return new FolderSharingInfo();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(FolderSharingInfo value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "read_only":
                        value.ReadOnly = enc.BooleanDecoder.Instance.Decode(reader);
                        break;
                    case "parent_shared_folder_id":
                        value.ParentSharedFolderId = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "shared_folder_id":
                        value.SharedFolderId = enc.StringDecoder.Instance.Decode(reader);
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
