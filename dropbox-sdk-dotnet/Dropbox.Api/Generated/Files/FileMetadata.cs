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
    /// <para>The file metadata object</para>
    /// </summary>
    /// <seealso cref="ExportResult" />
    /// <seealso cref="GetTemporaryLinkResult" />
    /// <seealso cref="GetThumbnailBatchResultData" />
    /// <seealso cref="Global::Dropbox.Api.Files.Metadata" />
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
        /// <param name="id">A unique identifier for the file.</param>
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
        /// <param name="pathLower">The lowercased full path in the user's Dropbox. This always
        /// starts with a slash. This field will be null if the file or folder is not
        /// mounted.</param>
        /// <param name="pathDisplay">The cased path to be used for display purposes only. In
        /// rare instances the casing will not correctly match the user's filesystem, but this
        /// behavior will match the path provided in the Core API v1, and at least the last
        /// path component will have the correct casing. Changes to only the casing of paths
        /// won't be returned by <see
        /// cref="Dropbox.Api.Files.Routes.FilesAppRoutes.ListFolderContinueAsync" /> <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderContinueAsync" />. This
        /// field will be null if the file or folder is not mounted.</param>
        /// <param name="parentSharedFolderId">Please use <see
        /// cref="Dropbox.Api.Files.FileSharingInfo.ParentSharedFolderId" /> or <see
        /// cref="Dropbox.Api.Files.FolderSharingInfo.ParentSharedFolderId" /> instead.</param>
        /// <param name="previewUrl">The preview URL of the file.</param>
        /// <param name="mediaInfo">Additional information if the file is a photo or video.
        /// This field will not be set on entries returned by <see
        /// cref="Dropbox.Api.Files.Routes.FilesAppRoutes.ListFolderAsync" /> <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderAsync" />, <see
        /// cref="Dropbox.Api.Files.Routes.FilesAppRoutes.ListFolderContinueAsync" /> <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderContinueAsync" />, or <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.GetThumbnailBatchAsync" />, starting
        /// December 2, 2019.</param>
        /// <param name="symlinkInfo">Set if this file is a symlink.</param>
        /// <param name="sharingInfo">Set if this file is contained in a shared folder.</param>
        /// <param name="isDownloadable">If true, file can be downloaded directly; else the
        /// file must be exported.</param>
        /// <param name="exportInfo">Information about format this file can be exported to.
        /// This filed must be set if <paramref name="isDownloadable" /> is set to
        /// false.</param>
        /// <param name="propertyGroups">Additional information if the file has custom
        /// properties with the property template specified.</param>
        /// <param name="hasExplicitSharedMembers">This flag will only be present if
        /// include_has_explicit_shared_members  is true in <see
        /// cref="Dropbox.Api.Files.Routes.FilesAppRoutes.ListFolderAsync" /> <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderAsync" /> or <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.GetMetadataAsync" />. If this  flag
        /// is present, it will be true if this file has any explicit shared  members. This is
        /// different from sharing_info in that this could be true  in the case where a file
        /// has explicit members but is not contained within  a shared folder.</param>
        /// <param name="contentHash">A hash of the file content. This field can be used to
        /// verify data integrity. For more information see our <a
        /// href="https://www.dropbox.com/developers/reference/content-hash">Content hash</a>
        /// page.</param>
        /// <param name="fileLockInfo">If present, the metadata associated with the file's
        /// current lock.</param>
        public FileMetadata(string name,
                            string id,
                            sys.DateTime clientModified,
                            sys.DateTime serverModified,
                            string rev,
                            ulong size,
                            string pathLower = null,
                            string pathDisplay = null,
                            string parentSharedFolderId = null,
                            string previewUrl = null,
                            MediaInfo mediaInfo = null,
                            SymlinkInfo symlinkInfo = null,
                            FileSharingInfo sharingInfo = null,
                            bool isDownloadable = true,
                            ExportInfo exportInfo = null,
                            col.IEnumerable<global::Dropbox.Api.FileProperties.PropertyGroup> propertyGroups = null,
                            bool? hasExplicitSharedMembers = null,
                            string contentHash = null,
                            FileLockMetadata fileLockInfo = null)
            : base(name, pathLower, pathDisplay, parentSharedFolderId, previewUrl)
        {
            if (id == null)
            {
                throw new sys.ArgumentNullException("id");
            }
            if (id.Length < 1)
            {
                throw new sys.ArgumentOutOfRangeException("id", "Length should be at least 1");
            }

            if (rev == null)
            {
                throw new sys.ArgumentNullException("rev");
            }
            if (rev.Length < 9)
            {
                throw new sys.ArgumentOutOfRangeException("rev", "Length should be at least 9");
            }
            if (!re.Regex.IsMatch(rev, @"\A(?:[0-9a-f]+)\z"))
            {
                throw new sys.ArgumentOutOfRangeException("rev", @"Value should match pattern '\A(?:[0-9a-f]+)\z'");
            }

            var propertyGroupsList = enc.Util.ToList(propertyGroups);

            if (contentHash != null)
            {
                if (contentHash.Length < 64)
                {
                    throw new sys.ArgumentOutOfRangeException("contentHash", "Length should be at least 64");
                }
                if (contentHash.Length > 64)
                {
                    throw new sys.ArgumentOutOfRangeException("contentHash", "Length should be at most 64");
                }
            }

            this.Id = id;
            this.ClientModified = clientModified;
            this.ServerModified = serverModified;
            this.Rev = rev;
            this.Size = size;
            this.MediaInfo = mediaInfo;
            this.SymlinkInfo = symlinkInfo;
            this.SharingInfo = sharingInfo;
            this.IsDownloadable = isDownloadable;
            this.ExportInfo = exportInfo;
            this.PropertyGroups = propertyGroupsList;
            this.HasExplicitSharedMembers = hasExplicitSharedMembers;
            this.ContentHash = contentHash;
            this.FileLockInfo = fileLockInfo;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="FileMetadata" /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public FileMetadata()
        {
            this.IsDownloadable = true;
        }

        /// <summary>
        /// <para>A unique identifier for the file.</para>
        /// </summary>
        public string Id { get; protected set; }

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
        /// <para>Additional information if the file is a photo or video. This field will not
        /// be set on entries returned by <see
        /// cref="Dropbox.Api.Files.Routes.FilesAppRoutes.ListFolderAsync" /> <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderAsync" />, <see
        /// cref="Dropbox.Api.Files.Routes.FilesAppRoutes.ListFolderContinueAsync" /> <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderContinueAsync" />, or <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.GetThumbnailBatchAsync" />, starting
        /// December 2, 2019.</para>
        /// </summary>
        public MediaInfo MediaInfo { get; protected set; }

        /// <summary>
        /// <para>Set if this file is a symlink.</para>
        /// </summary>
        public SymlinkInfo SymlinkInfo { get; protected set; }

        /// <summary>
        /// <para>Set if this file is contained in a shared folder.</para>
        /// </summary>
        public FileSharingInfo SharingInfo { get; protected set; }

        /// <summary>
        /// <para>If true, file can be downloaded directly; else the file must be
        /// exported.</para>
        /// </summary>
        public bool IsDownloadable { get; protected set; }

        /// <summary>
        /// <para>Information about format this file can be exported to. This filed must be set
        /// if <see cref="IsDownloadable" /> is set to false.</para>
        /// </summary>
        public ExportInfo ExportInfo { get; protected set; }

        /// <summary>
        /// <para>Additional information if the file has custom properties with the property
        /// template specified.</para>
        /// </summary>
        public col.IList<global::Dropbox.Api.FileProperties.PropertyGroup> PropertyGroups { get; protected set; }

        /// <summary>
        /// <para>This flag will only be present if include_has_explicit_shared_members  is
        /// true in <see cref="Dropbox.Api.Files.Routes.FilesAppRoutes.ListFolderAsync" /> <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderAsync" /> or <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.GetMetadataAsync" />. If this  flag
        /// is present, it will be true if this file has any explicit shared  members. This is
        /// different from sharing_info in that this could be true  in the case where a file
        /// has explicit members but is not contained within  a shared folder.</para>
        /// </summary>
        public bool? HasExplicitSharedMembers { get; protected set; }

        /// <summary>
        /// <para>A hash of the file content. This field can be used to verify data integrity.
        /// For more information see our <a
        /// href="https://www.dropbox.com/developers/reference/content-hash">Content hash</a>
        /// page.</para>
        /// </summary>
        public string ContentHash { get; protected set; }

        /// <summary>
        /// <para>If present, the metadata associated with the file's current lock.</para>
        /// </summary>
        public FileLockMetadata FileLockInfo { get; protected set; }

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
                WriteProperty("id", value.Id, writer, enc.StringEncoder.Instance);
                WriteProperty("client_modified", value.ClientModified, writer, enc.DateTimeEncoder.Instance);
                WriteProperty("server_modified", value.ServerModified, writer, enc.DateTimeEncoder.Instance);
                WriteProperty("rev", value.Rev, writer, enc.StringEncoder.Instance);
                WriteProperty("size", value.Size, writer, enc.UInt64Encoder.Instance);
                if (value.PathLower != null)
                {
                    WriteProperty("path_lower", value.PathLower, writer, enc.StringEncoder.Instance);
                }
                if (value.PathDisplay != null)
                {
                    WriteProperty("path_display", value.PathDisplay, writer, enc.StringEncoder.Instance);
                }
                if (value.ParentSharedFolderId != null)
                {
                    WriteProperty("parent_shared_folder_id", value.ParentSharedFolderId, writer, enc.StringEncoder.Instance);
                }
                if (value.PreviewUrl != null)
                {
                    WriteProperty("preview_url", value.PreviewUrl, writer, enc.StringEncoder.Instance);
                }
                if (value.MediaInfo != null)
                {
                    WriteProperty("media_info", value.MediaInfo, writer, global::Dropbox.Api.Files.MediaInfo.Encoder);
                }
                if (value.SymlinkInfo != null)
                {
                    WriteProperty("symlink_info", value.SymlinkInfo, writer, global::Dropbox.Api.Files.SymlinkInfo.Encoder);
                }
                if (value.SharingInfo != null)
                {
                    WriteProperty("sharing_info", value.SharingInfo, writer, global::Dropbox.Api.Files.FileSharingInfo.Encoder);
                }
                WriteProperty("is_downloadable", value.IsDownloadable, writer, enc.BooleanEncoder.Instance);
                if (value.ExportInfo != null)
                {
                    WriteProperty("export_info", value.ExportInfo, writer, global::Dropbox.Api.Files.ExportInfo.Encoder);
                }
                if (value.PropertyGroups.Count > 0)
                {
                    WriteListProperty("property_groups", value.PropertyGroups, writer, global::Dropbox.Api.FileProperties.PropertyGroup.Encoder);
                }
                if (value.HasExplicitSharedMembers != null)
                {
                    WriteProperty("has_explicit_shared_members", value.HasExplicitSharedMembers.Value, writer, enc.BooleanEncoder.Instance);
                }
                if (value.ContentHash != null)
                {
                    WriteProperty("content_hash", value.ContentHash, writer, enc.StringEncoder.Instance);
                }
                if (value.FileLockInfo != null)
                {
                    WriteProperty("file_lock_info", value.FileLockInfo, writer, global::Dropbox.Api.Files.FileLockMetadata.Encoder);
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
                    case "id":
                        value.Id = enc.StringDecoder.Instance.Decode(reader);
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
                    case "path_lower":
                        value.PathLower = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "path_display":
                        value.PathDisplay = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "parent_shared_folder_id":
                        value.ParentSharedFolderId = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "preview_url":
                        value.PreviewUrl = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "media_info":
                        value.MediaInfo = global::Dropbox.Api.Files.MediaInfo.Decoder.Decode(reader);
                        break;
                    case "symlink_info":
                        value.SymlinkInfo = global::Dropbox.Api.Files.SymlinkInfo.Decoder.Decode(reader);
                        break;
                    case "sharing_info":
                        value.SharingInfo = global::Dropbox.Api.Files.FileSharingInfo.Decoder.Decode(reader);
                        break;
                    case "is_downloadable":
                        value.IsDownloadable = enc.BooleanDecoder.Instance.Decode(reader);
                        break;
                    case "export_info":
                        value.ExportInfo = global::Dropbox.Api.Files.ExportInfo.Decoder.Decode(reader);
                        break;
                    case "property_groups":
                        value.PropertyGroups = ReadList<global::Dropbox.Api.FileProperties.PropertyGroup>(reader, global::Dropbox.Api.FileProperties.PropertyGroup.Decoder);
                        break;
                    case "has_explicit_shared_members":
                        value.HasExplicitSharedMembers = enc.BooleanDecoder.Instance.Decode(reader);
                        break;
                    case "content_hash":
                        value.ContentHash = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "file_lock_info":
                        value.FileLockInfo = global::Dropbox.Api.Files.FileLockMetadata.Decoder.Decode(reader);
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
