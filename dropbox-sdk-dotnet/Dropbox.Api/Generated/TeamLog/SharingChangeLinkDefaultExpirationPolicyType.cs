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
    /// <para>The sharing change link default expiration policy type object</para>
    /// </summary>
    public class SharingChangeLinkDefaultExpirationPolicyType
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<SharingChangeLinkDefaultExpirationPolicyType> Encoder = new SharingChangeLinkDefaultExpirationPolicyTypeEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<SharingChangeLinkDefaultExpirationPolicyType> Decoder = new SharingChangeLinkDefaultExpirationPolicyTypeDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see
        /// cref="SharingChangeLinkDefaultExpirationPolicyType" /> class.</para>
        /// </summary>
        /// <param name="description">The description</param>
        public SharingChangeLinkDefaultExpirationPolicyType(string description)
        {
            if (description == null)
            {
                throw new sys.ArgumentNullException("description");
            }

            this.Description = description;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see
        /// cref="SharingChangeLinkDefaultExpirationPolicyType" /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public SharingChangeLinkDefaultExpirationPolicyType()
        {
        }

        /// <summary>
        /// <para>Gets the description of the sharing change link default expiration policy
        /// type</para>
        /// </summary>
        public string Description { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="SharingChangeLinkDefaultExpirationPolicyType"
        /// />.</para>
        /// </summary>
        private class SharingChangeLinkDefaultExpirationPolicyTypeEncoder : enc.StructEncoder<SharingChangeLinkDefaultExpirationPolicyType>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(SharingChangeLinkDefaultExpirationPolicyType value, enc.IJsonWriter writer)
            {
                WriteProperty("description", value.Description, writer, enc.StringEncoder.Instance);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="SharingChangeLinkDefaultExpirationPolicyType"
        /// />.</para>
        /// </summary>
        private class SharingChangeLinkDefaultExpirationPolicyTypeDecoder : enc.StructDecoder<SharingChangeLinkDefaultExpirationPolicyType>
        {
            /// <summary>
            /// <para>Create a new instance of type <see
            /// cref="SharingChangeLinkDefaultExpirationPolicyType" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override SharingChangeLinkDefaultExpirationPolicyType Create()
            {
                return new SharingChangeLinkDefaultExpirationPolicyType();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(SharingChangeLinkDefaultExpirationPolicyType value, string fieldName, enc.IJsonReader reader)
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
