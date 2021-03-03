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
    /// <para>Deleted group.</para>
    /// </summary>
    public class GroupDeleteDetails
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<GroupDeleteDetails> Encoder = new GroupDeleteDetailsEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<GroupDeleteDetails> Decoder = new GroupDeleteDetailsDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="GroupDeleteDetails" />
        /// class.</para>
        /// </summary>
        /// <param name="isCompanyManaged">Is company managed group.</param>
        public GroupDeleteDetails(bool? isCompanyManaged = null)
        {
            this.IsCompanyManaged = isCompanyManaged;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="GroupDeleteDetails" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public GroupDeleteDetails()
        {
        }

        /// <summary>
        /// <para>Is company managed group.</para>
        /// </summary>
        public bool? IsCompanyManaged { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="GroupDeleteDetails" />.</para>
        /// </summary>
        private class GroupDeleteDetailsEncoder : enc.StructEncoder<GroupDeleteDetails>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(GroupDeleteDetails value, enc.IJsonWriter writer)
            {
                if (value.IsCompanyManaged != null)
                {
                    WriteProperty("is_company_managed", value.IsCompanyManaged.Value, writer, enc.BooleanEncoder.Instance);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="GroupDeleteDetails" />.</para>
        /// </summary>
        private class GroupDeleteDetailsDecoder : enc.StructDecoder<GroupDeleteDetails>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="GroupDeleteDetails" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override GroupDeleteDetails Create()
            {
                return new GroupDeleteDetails();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(GroupDeleteDetails value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "is_company_managed":
                        value.IsCompanyManaged = enc.BooleanDecoder.Instance.Decode(reader);
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
