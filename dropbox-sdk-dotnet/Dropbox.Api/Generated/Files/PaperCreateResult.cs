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
    /// <para>The paper create result object</para>
    /// </summary>
    public class PaperCreateResult
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<PaperCreateResult> Encoder = new PaperCreateResultEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<PaperCreateResult> Decoder = new PaperCreateResultDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="PaperCreateResult" />
        /// class.</para>
        /// </summary>
        /// <param name="url">URL to open the Paper Doc.</param>
        /// <param name="resultPath">The fully qualified path the Paper Doc was actually
        /// created at.</param>
        /// <param name="fileId">The id to use in Dropbox APIs when referencing the Paper
        /// Doc.</param>
        /// <param name="paperRevision">The current doc revision.</param>
        public PaperCreateResult(string url,
                                 string resultPath,
                                 string fileId,
                                 long paperRevision)
        {
            if (url == null)
            {
                throw new sys.ArgumentNullException("url");
            }

            if (resultPath == null)
            {
                throw new sys.ArgumentNullException("resultPath");
            }

            if (fileId == null)
            {
                throw new sys.ArgumentNullException("fileId");
            }
            if (fileId.Length < 4)
            {
                throw new sys.ArgumentOutOfRangeException("fileId", "Length should be at least 4");
            }
            if (!re.Regex.IsMatch(fileId, @"\A(?:id:.+)\z"))
            {
                throw new sys.ArgumentOutOfRangeException("fileId", @"Value should match pattern '\A(?:id:.+)\z'");
            }

            this.Url = url;
            this.ResultPath = resultPath;
            this.FileId = fileId;
            this.PaperRevision = paperRevision;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="PaperCreateResult" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public PaperCreateResult()
        {
        }

        /// <summary>
        /// <para>URL to open the Paper Doc.</para>
        /// </summary>
        public string Url { get; protected set; }

        /// <summary>
        /// <para>The fully qualified path the Paper Doc was actually created at.</para>
        /// </summary>
        public string ResultPath { get; protected set; }

        /// <summary>
        /// <para>The id to use in Dropbox APIs when referencing the Paper Doc.</para>
        /// </summary>
        public string FileId { get; protected set; }

        /// <summary>
        /// <para>The current doc revision.</para>
        /// </summary>
        public long PaperRevision { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="PaperCreateResult" />.</para>
        /// </summary>
        private class PaperCreateResultEncoder : enc.StructEncoder<PaperCreateResult>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(PaperCreateResult value, enc.IJsonWriter writer)
            {
                WriteProperty("url", value.Url, writer, enc.StringEncoder.Instance);
                WriteProperty("result_path", value.ResultPath, writer, enc.StringEncoder.Instance);
                WriteProperty("file_id", value.FileId, writer, enc.StringEncoder.Instance);
                WriteProperty("paper_revision", value.PaperRevision, writer, enc.Int64Encoder.Instance);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="PaperCreateResult" />.</para>
        /// </summary>
        private class PaperCreateResultDecoder : enc.StructDecoder<PaperCreateResult>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="PaperCreateResult" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override PaperCreateResult Create()
            {
                return new PaperCreateResult();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(PaperCreateResult value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "url":
                        value.Url = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "result_path":
                        value.ResultPath = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "file_id":
                        value.FileId = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "paper_revision":
                        value.PaperRevision = enc.Int64Decoder.Instance.Decode(reader);
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
