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
    /// <para>The file metadata object</para>
    /// </summary>
    /// <seealso cref="Dropbox.Api.Files.Metadata" />
    public class FileMetadata : Metadata
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<FileMetadata> Encoder = new FileMetadataEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<FileMetadata> Decoder = new FileMetadataDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="FileMetadata" /> class.</para>
        /// </summary>
        /// <param name="name">The last component of the path (including extension). This never
        /// contains a slash.</param>
        /// <param name="pathLower">The lowercased full path in the user's Dropbox. This always
        /// starts with a slash.</param>
        /// <param name="clientModified">For files, this is the modification time set by the
        /// desktop client when the file was added to Dropbox. Since this time is not verified
        /// (the Dropbox server stores whatever the desktop client sends up), this should only
        /// be used for display purposes (such as sorting) and not, for example, to determine
        /// if a file has changed or not.</param>
        /// <param name="serverModified">The last time the file was modified on
        /// Dropbox.</param>
        /// <param name="rev">A unique identifier for the current revision of a file. This
        /// field is the same rev as elsewhere in the API and can be used to detect changes and
        /// avoid conflicts.</param>
        /// <param name="size">The file size in bytes.</param>
        /// <param name="parentSharedFolderId">Deprecated. Please use
        /// :field:'FileSharingInfo.parent_shared_folder_id' or
        /// :field:'FolderSharingInfo.parent_shared_folder_id' instead.</param>
        /// <param name="id">A unique identifier for the file.</param>
        /// <param name="mediaInfo">Additional information if the file is a photo or
        /// video.</param>
        /// <param name="sharingInfo">Set if this file is contained in a shared folder.</param>
        public FileMetadata(string name,
                            string pathLower,
                            sys.DateTime clientModified,
                            sys.DateTime serverModified,
                            string rev,
                            ulong size,
                            string parentSharedFolderId = null,
                            string id = null,
                            MediaInfo mediaInfo = null,
                            FileSharingInfo sharingInfo = null)
            : base(name, pathLower, parentSharedFolderId)
        {
            if (rev == null)
            {
                throw new sys.ArgumentNullException("rev");
            }
            else if (rev.Length < 9 || !re.Regex.IsMatch(rev, @"\A(?:[0-9a-f]+)\z"))
            {
                throw new sys.ArgumentOutOfRangeException("rev");
            }

            if (id != null && (id.Length < 1))
            {
                throw new sys.ArgumentOutOfRangeException("id");
            }

            this.ClientModified = clientModified;
            this.ServerModified = serverModified;
            this.Rev = rev;
            this.Size = size;
            this.Id = id;
            this.MediaInfo = mediaInfo;
            this.SharingInfo = sharingInfo;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="FileMetadata" /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        public FileMetadata()
        {
        }

        /// <summary>
        /// <para>For files, this is the modification time set by the desktop client when the
        /// file was added to Dropbox. Since this time is not verified (the Dropbox server
        /// stores whatever the desktop client sends up), this should only be used for display
        /// purposes (such as sorting) and not, for example, to determine if a file has changed
        /// or not.</para>
        /// </summary>
        public sys.DateTime ClientModified { get; protected set; }

        /// <summary>
        /// <para>The last time the file was modified on Dropbox.</para>
        /// </summary>
        public sys.DateTime ServerModified { get; protected set; }

        /// <summary>
        /// <para>A unique identifier for the current revision of a file. This field is the
        /// same rev as elsewhere in the API and can be used to detect changes and avoid
        /// conflicts.</para>
        /// </summary>
        public string Rev { get; protected set; }

        /// <summary>
        /// <para>The file size in bytes.</para>
        /// </summary>
        public ulong Size { get; protected set; }

        /// <summary>
        /// <para>A unique identifier for the file.</para>
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// <para>Additional information if the file is a photo or video.</para>
        /// </summary>
        public MediaInfo MediaInfo { get; protected set; }

        /// <summary>
        /// <para>Set if this file is contained in a shared folder.</para>
        /// </summary>
        public FileSharingInfo SharingInfo { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="FileMetadata" />.</para>
        /// </summary>
        private class FileMetadataEncoder : enc.StructEncoder<FileMetadata>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(FileMetadata value, enc.IJsonWriter writer)
            {
                WriteProperty("name", value.Name, writer, enc.StringEncoder.Instance);
                WriteProperty("path_lower", value.PathLower, writer, enc.StringEncoder.Instance);
                WriteProperty("client_modified", value.ClientModified, writer, enc.DateTimeEncoder.Instance);
                WriteProperty("server_modified", value.ServerModified, writer, enc.DateTimeEncoder.Instance);
                WriteProperty("rev", value.Rev, writer, enc.StringEncoder.Instance);
                WriteProperty("size", value.Size, writer, enc.UInt64Encoder.Instance);
                if (value.ParentSharedFolderId != null)
                {
                    WriteProperty("parent_shared_folder_id", value.ParentSharedFolderId, writer, enc.StringEncoder.Instance);
                }
                if (value.Id != null)
                {
                    WriteProperty("id", value.Id, writer, enc.StringEncoder.Instance);
                }
                if (value.MediaInfo != null)
                {
                    WriteProperty("media_info", value.MediaInfo, writer, Dropbox.Api.Files.MediaInfo.Encoder);
                }
                if (value.SharingInfo != null)
                {
                    WriteProperty("sharing_info", value.SharingInfo, writer, Dropbox.Api.Files.FileSharingInfo.Encoder);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="FileMetadata" />.</para>
        /// </summary>
        private class FileMetadataDecoder : enc.StructDecoder<FileMetadata>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="FileMetadata" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override FileMetadata Create()
            {
                return new FileMetadata();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(FileMetadata value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "name":
                        value.Name = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "path_lower":
                        value.PathLower = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "client_modified":
                        value.ClientModified = enc.DateTimeDecoder.Instance.Decode(reader);
                        break;
                    case "server_modified":
                        value.ServerModified = enc.DateTimeDecoder.Instance.Decode(reader);
                        break;
                    case "rev":
                        value.Rev = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "size":
                        value.Size = enc.UInt64Decoder.Instance.Decode(reader);
                        break;
                    case "parent_shared_folder_id":
                        value.ParentSharedFolderId = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "id":
                        value.Id = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "media_info":
                        value.MediaInfo = Dropbox.Api.Files.MediaInfo.Decoder.Decode(reader);
                        break;
                    case "sharing_info":
                        value.SharingInfo = Dropbox.Api.Files.FileSharingInfo.Decoder.Decode(reader);
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
