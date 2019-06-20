// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Paper
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>The paper doc export result object</para>
    /// </summary>
    public class PaperDocExportResult
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<PaperDocExportResult> Encoder = new PaperDocExportResultEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<PaperDocExportResult> Decoder = new PaperDocExportResultDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="PaperDocExportResult" />
        /// class.</para>
        /// </summary>
        /// <param name="owner">The Paper doc owner's email address.</param>
        /// <param name="title">The Paper doc title.</param>
        /// <param name="revision">The Paper doc revision. Simply an ever increasing
        /// number.</param>
        /// <param name="mimeType">MIME type of the export. This corresponds to <see
        /// cref="ExportFormat" /> specified in the request.</param>
        public PaperDocExportResult(string owner,
                                    string title,
                                    long revision,
                                    string mimeType)
        {
            if (owner == null)
            {
                throw new sys.ArgumentNullException("owner");
            }

            if (title == null)
            {
                throw new sys.ArgumentNullException("title");
            }

            if (mimeType == null)
            {
                throw new sys.ArgumentNullException("mimeType");
            }

            this.Owner = owner;
            this.Title = title;
            this.Revision = revision;
            this.MimeType = mimeType;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="PaperDocExportResult" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public PaperDocExportResult()
        {
        }

        /// <summary>
        /// <para>The Paper doc owner's email address.</para>
        /// </summary>
        public string Owner { get; protected set; }

        /// <summary>
        /// <para>The Paper doc title.</para>
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// <para>The Paper doc revision. Simply an ever increasing number.</para>
        /// </summary>
        public long Revision { get; protected set; }

        /// <summary>
        /// <para>MIME type of the export. This corresponds to <see cref="ExportFormat" />
        /// specified in the request.</para>
        /// </summary>
        public string MimeType { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="PaperDocExportResult" />.</para>
        /// </summary>
        private class PaperDocExportResultEncoder : enc.StructEncoder<PaperDocExportResult>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(PaperDocExportResult value, enc.IJsonWriter writer)
            {
                WriteProperty("owner", value.Owner, writer, enc.StringEncoder.Instance);
                WriteProperty("title", value.Title, writer, enc.StringEncoder.Instance);
                WriteProperty("revision", value.Revision, writer, enc.Int64Encoder.Instance);
                WriteProperty("mime_type", value.MimeType, writer, enc.StringEncoder.Instance);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="PaperDocExportResult" />.</para>
        /// </summary>
        private class PaperDocExportResultDecoder : enc.StructDecoder<PaperDocExportResult>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="PaperDocExportResult" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override PaperDocExportResult Create()
            {
                return new PaperDocExportResult();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(PaperDocExportResult value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "owner":
                        value.Owner = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "title":
                        value.Title = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "revision":
                        value.Revision = enc.Int64Decoder.Instance.Decode(reader);
                        break;
                    case "mime_type":
                        value.MimeType = enc.StringDecoder.Instance.Decode(reader);
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