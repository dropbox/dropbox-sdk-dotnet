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
    /// <para>The undo organize folder with tidy type object</para>
    /// </summary>
    public class UndoOrganizeFolderWithTidyType
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<UndoOrganizeFolderWithTidyType> Encoder = new UndoOrganizeFolderWithTidyTypeEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<UndoOrganizeFolderWithTidyType> Decoder = new UndoOrganizeFolderWithTidyTypeDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="UndoOrganizeFolderWithTidyType"
        /// /> class.</para>
        /// </summary>
        /// <param name="description">The description</param>
        public UndoOrganizeFolderWithTidyType(string description)
        {
            if (description == null)
            {
                throw new sys.ArgumentNullException("description");
            }

            this.Description = description;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="UndoOrganizeFolderWithTidyType"
        /// /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public UndoOrganizeFolderWithTidyType()
        {
        }

        /// <summary>
        /// <para>Gets the description of the undo organize folder with tidy type</para>
        /// </summary>
        public string Description { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="UndoOrganizeFolderWithTidyType" />.</para>
        /// </summary>
        private class UndoOrganizeFolderWithTidyTypeEncoder : enc.StructEncoder<UndoOrganizeFolderWithTidyType>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(UndoOrganizeFolderWithTidyType value, enc.IJsonWriter writer)
            {
                WriteProperty("description", value.Description, writer, enc.StringEncoder.Instance);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="UndoOrganizeFolderWithTidyType" />.</para>
        /// </summary>
        private class UndoOrganizeFolderWithTidyTypeDecoder : enc.StructDecoder<UndoOrganizeFolderWithTidyType>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="UndoOrganizeFolderWithTidyType"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override UndoOrganizeFolderWithTidyType Create()
            {
                return new UndoOrganizeFolderWithTidyType();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(UndoOrganizeFolderWithTidyType value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "description":
                        value.Description = enc.StringDecoder.Instance.Decode(reader);
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
