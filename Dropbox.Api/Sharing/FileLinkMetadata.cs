// <auto-generated>
// Auto-generated by BabelAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Sharing
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Babel;

    /// <summary>
    /// <para>The metadata of a file shared link</para>
    /// </summary>
    /// <seealso cref="Dropbox.Api.Sharing.SharedLinkMetadata" />
    public class FileLinkMetadata : SharedLinkMetadata
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<FileLinkMetadata> Encoder = new FileLinkMetadataEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<FileLinkMetadata> Decoder = new FileLinkMetadataDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="FileLinkMetadata" />
        /// class.</para>
        /// </summary>
        /// <param name="url">URL of the shared link.</param>
        /// <param name="name">The linked file name (including extension). This never contains
        /// a slash.</param>
        /// <param name="linkPermissions">The link's access permissions.</param>
        /// <param name="clientModified">The modification time set by the desktop client when
        /// the file was added to Dropbox. Since this time is not verified (the Dropbox server
        /// stores whatever the desktop client sends up), this should only be used for display
        /// purposes (such as sorting) and not, for example, to determine if a file has changed
        /// or not.</param>
        /// <param name="serverModified">The last time the file was modified on
        /// Dropbox.</param>
        /// <param name="rev">A unique identifier for the current revision of a file. This
        /// field is the same rev as elsewhere in the API and can be used to detect changes and
        /// avoid conflicts.</param>
        /// <param name="size">The file size in bytes.</param>
        /// <param name="id">A unique identifier for the linked file.</param>
        /// <param name="expires">Expiration time, if set. By default the link won't
        /// expire.</param>
        /// <param name="pathLower">The lowercased full path in the user's Dropbox. This always
        /// starts with a slash. This field will only be present only if the linked file is in
        /// the authenticated user's  dropbox.</param>
        /// <param name="teamMemberInfo">The team membership information of the link's owner.
        /// This field will only be present  if the link's owner is a team member.</param>
        /// <param name="contentOwnerTeamInfo">The team information of the content's owner.
        /// This field will only be present if the content's owner is a team member and the
        /// content's owner team is different from the link's owner team.</param>
        public FileLinkMetadata(string url,
                                string name,
                                LinkPermissions linkPermissions,
                                sys.DateTime clientModified,
                                sys.DateTime serverModified,
                                string rev,
                                ulong size,
                                string id = null,
                                sys.DateTime? expires = null,
                                string pathLower = null,
                                TeamMemberInfo teamMemberInfo = null,
                                Dropbox.Api.Users.Team contentOwnerTeamInfo = null)
            : base(url, name, linkPermissions, id, expires, pathLower, teamMemberInfo, contentOwnerTeamInfo)
        {
            if (rev == null)
            {
                throw new sys.ArgumentNullException("rev");
            }
            else if (rev.Length < 9 || !re.Regex.IsMatch(rev, @"\A(?:[0-9a-f]+)\z"))
            {
                throw new sys.ArgumentOutOfRangeException("rev");
            }

            this.ClientModified = clientModified;
            this.ServerModified = serverModified;
            this.Rev = rev;
            this.Size = size;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="FileLinkMetadata" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        public FileLinkMetadata()
        {
        }

        /// <summary>
        /// <para>The modification time set by the desktop client when the file was added to
        /// Dropbox. Since this time is not verified (the Dropbox server stores whatever the
        /// desktop client sends up), this should only be used for display purposes (such as
        /// sorting) and not, for example, to determine if a file has changed or not.</para>
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

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="FileLinkMetadata" />.</para>
        /// </summary>
        private class FileLinkMetadataEncoder : enc.StructEncoder<FileLinkMetadata>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(FileLinkMetadata value, enc.IJsonWriter writer)
            {
                WriteProperty("url", value.Url, writer, enc.StringEncoder.Instance);
                WriteProperty("name", value.Name, writer, enc.StringEncoder.Instance);
                WriteProperty("link_permissions", value.LinkPermissions, writer, Dropbox.Api.Sharing.LinkPermissions.Encoder);
                WriteProperty("client_modified", value.ClientModified, writer, enc.DateTimeEncoder.Instance);
                WriteProperty("server_modified", value.ServerModified, writer, enc.DateTimeEncoder.Instance);
                WriteProperty("rev", value.Rev, writer, enc.StringEncoder.Instance);
                WriteProperty("size", value.Size, writer, enc.UInt64Encoder.Instance);
                if (value.Id != null)
                {
                    WriteProperty("id", value.Id, writer, enc.StringEncoder.Instance);
                }
                if (value.Expires != null)
                {
                    WriteProperty("expires", value.Expires.Value, writer, enc.DateTimeEncoder.Instance);
                }
                if (value.PathLower != null)
                {
                    WriteProperty("path_lower", value.PathLower, writer, enc.StringEncoder.Instance);
                }
                if (value.TeamMemberInfo != null)
                {
                    WriteProperty("team_member_info", value.TeamMemberInfo, writer, Dropbox.Api.Sharing.TeamMemberInfo.Encoder);
                }
                if (value.ContentOwnerTeamInfo != null)
                {
                    WriteProperty("content_owner_team_info", value.ContentOwnerTeamInfo, writer, Dropbox.Api.Users.Team.Encoder);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="FileLinkMetadata" />.</para>
        /// </summary>
        private class FileLinkMetadataDecoder : enc.StructDecoder<FileLinkMetadata>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="FileLinkMetadata" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override FileLinkMetadata Create()
            {
                return new FileLinkMetadata();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(FileLinkMetadata value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "url":
                        value.Url = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "name":
                        value.Name = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "link_permissions":
                        value.LinkPermissions = Dropbox.Api.Sharing.LinkPermissions.Decoder.Decode(reader);
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
                    case "id":
                        value.Id = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "expires":
                        value.Expires = enc.DateTimeDecoder.Instance.Decode(reader);
                        break;
                    case "path_lower":
                        value.PathLower = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "team_member_info":
                        value.TeamMemberInfo = Dropbox.Api.Sharing.TeamMemberInfo.Decoder.Decode(reader);
                        break;
                    case "content_owner_team_info":
                        value.ContentOwnerTeamInfo = Dropbox.Api.Users.Team.Decoder.Decode(reader);
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
