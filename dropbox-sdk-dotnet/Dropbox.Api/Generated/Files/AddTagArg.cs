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
    /// <para>The add tag arg object</para>
    /// </summary>
    public class AddTagArg
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<AddTagArg> Encoder = new AddTagArgEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<AddTagArg> Decoder = new AddTagArgDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="AddTagArg" /> class.</para>
        /// </summary>
        /// <param name="path">Path to the item to be tagged.</param>
        /// <param name="tagText">The value of the tag to add. Will be automatically converted
        /// to lowercase letters.</param>
        public AddTagArg(string path,
                         string tagText)
        {
            if (path == null)
            {
                throw new sys.ArgumentNullException("path");
            }
            if (!re.Regex.IsMatch(path, @"\A(?:/(.|[\r\n])*)\z"))
            {
                throw new sys.ArgumentOutOfRangeException("path", @"Value should match pattern '\A(?:/(.|[\r\n])*)\z'");
            }

            if (tagText == null)
            {
                throw new sys.ArgumentNullException("tagText");
            }
            if (tagText.Length < 1)
            {
                throw new sys.ArgumentOutOfRangeException("tagText", "Length should be at least 1");
            }
            if (tagText.Length > 32)
            {
                throw new sys.ArgumentOutOfRangeException("tagText", "Length should be at most 32");
            }
            if (!re.Regex.IsMatch(tagText, @"\A(?:[A-Za-z0-9_]+)\z"))
            {
                throw new sys.ArgumentOutOfRangeException("tagText", @"Value should match pattern '\A(?:[A-Za-z0-9_]+)\z'");
            }

            this.Path = path;
            this.TagText = tagText;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="AddTagArg" /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public AddTagArg()
        {
        }

        /// <summary>
        /// <para>Path to the item to be tagged.</para>
        /// </summary>
        public string Path { get; protected set; }

        /// <summary>
        /// <para>The value of the tag to add. Will be automatically converted to lowercase
        /// letters.</para>
        /// </summary>
        public string TagText { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="AddTagArg" />.</para>
        /// </summary>
        private class AddTagArgEncoder : enc.StructEncoder<AddTagArg>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(AddTagArg value, enc.IJsonWriter writer)
            {
                WriteProperty("path", value.Path, writer, enc.StringEncoder.Instance);
                WriteProperty("tag_text", value.TagText, writer, enc.StringEncoder.Instance);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="AddTagArg" />.</para>
        /// </summary>
        private class AddTagArgDecoder : enc.StructDecoder<AddTagArg>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="AddTagArg" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override AddTagArg Create()
            {
                return new AddTagArg();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(AddTagArg value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "path":
                        value.Path = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "tag_text":
                        value.TagText = enc.StringDecoder.Instance.Decode(reader);
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
