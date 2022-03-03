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
    /// <para>The upload arg object</para>
    /// </summary>
    /// <seealso cref="Global::Dropbox.Api.Files.CommitInfo" />
    public class UploadArg : CommitInfo
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<UploadArg> Encoder = new UploadArgEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<UploadArg> Decoder = new UploadArgDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="UploadArg" /> class.</para>
        /// </summary>
        /// <param name="path">Path in the user's Dropbox to save the file.</param>
        /// <param name="mode">Selects what to do if the file already exists.</param>
        /// <param name="autorename">If there's a conflict, as determined by <paramref
        /// name="mode" />, have the Dropbox server try to autorename the file to avoid
        /// conflict.</param>
        /// <param name="clientModified">The value to store as the <paramref
        /// name="clientModified" /> timestamp. Dropbox automatically records the time at which
        /// the file was written to the Dropbox servers. It can also record an additional
        /// timestamp, provided by Dropbox desktop clients, mobile clients, and API apps of
        /// when the file was actually created or modified.</param>
        /// <param name="mute">Normally, users are made aware of any file modifications in
        /// their Dropbox account via notifications in the client software. If <c>true</c>,
        /// this tells the clients that this modification shouldn't result in a user
        /// notification.</param>
        /// <param name="propertyGroups">List of custom properties to add to file.</param>
        /// <param name="strictConflict">Be more strict about how each <see cref="WriteMode" />
        /// detects conflict. For example, always return a conflict error when <paramref
        /// name="mode" /> = <see cref="Dropbox.Api.Files.WriteMode.Update" /> and the given
        /// "rev" doesn't match the existing file's "rev", even if the existing file has been
        /// deleted. This also forces a conflict even when the target path refers to a file
        /// with identical contents.</param>
        /// <param name="contentHash">A hash of the file content uploaded in this call. If
        /// provided and the uploaded content does not match this hash, an error will be
        /// returned. For more information see our <a
        /// href="https://www.dropbox.com/developers/reference/content-hash">Content hash</a>
        /// page.</param>
        public UploadArg(string path,
                         WriteMode mode = null,
                         bool autorename = false,
                         sys.DateTime? clientModified = null,
                         bool mute = false,
                         col.IEnumerable<global::Dropbox.Api.FileProperties.PropertyGroup> propertyGroups = null,
                         bool strictConflict = false,
                         string contentHash = null)
            : base(path, mode, autorename, clientModified, mute, propertyGroups, strictConflict)
        {
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

            this.ContentHash = contentHash;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="UploadArg" /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public UploadArg()
        {
        }

        /// <summary>
        /// <para>A hash of the file content uploaded in this call. If provided and the
        /// uploaded content does not match this hash, an error will be returned. For more
        /// information see our <a
        /// href="https://www.dropbox.com/developers/reference/content-hash">Content hash</a>
        /// page.</para>
        /// </summary>
        public string ContentHash { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="UploadArg" />.</para>
        /// </summary>
        private class UploadArgEncoder : enc.StructEncoder<UploadArg>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(UploadArg value, enc.IJsonWriter writer)
            {
                WriteProperty("path", value.Path, writer, enc.StringEncoder.Instance);
                WriteProperty("mode", value.Mode, writer, global::Dropbox.Api.Files.WriteMode.Encoder);
                WriteProperty("autorename", value.Autorename, writer, enc.BooleanEncoder.Instance);
                if (value.ClientModified != null)
                {
                    WriteProperty("client_modified", value.ClientModified.Value, writer, enc.DateTimeEncoder.Instance);
                }
                WriteProperty("mute", value.Mute, writer, enc.BooleanEncoder.Instance);
                if (value.PropertyGroups.Count > 0)
                {
                    WriteListProperty("property_groups", value.PropertyGroups, writer, global::Dropbox.Api.FileProperties.PropertyGroup.Encoder);
                }
                WriteProperty("strict_conflict", value.StrictConflict, writer, enc.BooleanEncoder.Instance);
                if (value.ContentHash != null)
                {
                    WriteProperty("content_hash", value.ContentHash, writer, enc.StringEncoder.Instance);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="UploadArg" />.</para>
        /// </summary>
        private class UploadArgDecoder : enc.StructDecoder<UploadArg>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="UploadArg" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override UploadArg Create()
            {
                return new UploadArg();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(UploadArg value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "path":
                        value.Path = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "mode":
                        value.Mode = global::Dropbox.Api.Files.WriteMode.Decoder.Decode(reader);
                        break;
                    case "autorename":
                        value.Autorename = enc.BooleanDecoder.Instance.Decode(reader);
                        break;
                    case "client_modified":
                        value.ClientModified = enc.DateTimeDecoder.Instance.Decode(reader);
                        break;
                    case "mute":
                        value.Mute = enc.BooleanDecoder.Instance.Decode(reader);
                        break;
                    case "property_groups":
                        value.PropertyGroups = ReadList<global::Dropbox.Api.FileProperties.PropertyGroup>(reader, global::Dropbox.Api.FileProperties.PropertyGroup.Decoder);
                        break;
                    case "strict_conflict":
                        value.StrictConflict = enc.BooleanDecoder.Instance.Decode(reader);
                        break;
                    case "content_hash":
                        value.ContentHash = enc.StringDecoder.Instance.Decode(reader);
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
