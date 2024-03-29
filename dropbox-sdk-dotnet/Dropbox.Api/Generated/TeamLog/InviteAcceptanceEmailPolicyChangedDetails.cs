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
    /// <para>Changed invite accept email policy for team.</para>
    /// </summary>
    public class InviteAcceptanceEmailPolicyChangedDetails
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<InviteAcceptanceEmailPolicyChangedDetails> Encoder = new InviteAcceptanceEmailPolicyChangedDetailsEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<InviteAcceptanceEmailPolicyChangedDetails> Decoder = new InviteAcceptanceEmailPolicyChangedDetailsDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see
        /// cref="InviteAcceptanceEmailPolicyChangedDetails" /> class.</para>
        /// </summary>
        /// <param name="newValue">To.</param>
        /// <param name="previousValue">From.</param>
        public InviteAcceptanceEmailPolicyChangedDetails(InviteAcceptanceEmailPolicy newValue,
                                                         InviteAcceptanceEmailPolicy previousValue)
        {
            if (newValue == null)
            {
                throw new sys.ArgumentNullException("newValue");
            }

            if (previousValue == null)
            {
                throw new sys.ArgumentNullException("previousValue");
            }

            this.NewValue = newValue;
            this.PreviousValue = previousValue;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see
        /// cref="InviteAcceptanceEmailPolicyChangedDetails" /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public InviteAcceptanceEmailPolicyChangedDetails()
        {
        }

        /// <summary>
        /// <para>To.</para>
        /// </summary>
        public InviteAcceptanceEmailPolicy NewValue { get; protected set; }

        /// <summary>
        /// <para>From.</para>
        /// </summary>
        public InviteAcceptanceEmailPolicy PreviousValue { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="InviteAcceptanceEmailPolicyChangedDetails" />.</para>
        /// </summary>
        private class InviteAcceptanceEmailPolicyChangedDetailsEncoder : enc.StructEncoder<InviteAcceptanceEmailPolicyChangedDetails>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(InviteAcceptanceEmailPolicyChangedDetails value, enc.IJsonWriter writer)
            {
                WriteProperty("new_value", value.NewValue, writer, global::Dropbox.Api.TeamLog.InviteAcceptanceEmailPolicy.Encoder);
                WriteProperty("previous_value", value.PreviousValue, writer, global::Dropbox.Api.TeamLog.InviteAcceptanceEmailPolicy.Encoder);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="InviteAcceptanceEmailPolicyChangedDetails" />.</para>
        /// </summary>
        private class InviteAcceptanceEmailPolicyChangedDetailsDecoder : enc.StructDecoder<InviteAcceptanceEmailPolicyChangedDetails>
        {
            /// <summary>
            /// <para>Create a new instance of type <see
            /// cref="InviteAcceptanceEmailPolicyChangedDetails" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override InviteAcceptanceEmailPolicyChangedDetails Create()
            {
                return new InviteAcceptanceEmailPolicyChangedDetails();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(InviteAcceptanceEmailPolicyChangedDetails value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "new_value":
                        value.NewValue = global::Dropbox.Api.TeamLog.InviteAcceptanceEmailPolicy.Decoder.Decode(reader);
                        break;
                    case "previous_value":
                        value.PreviousValue = global::Dropbox.Api.TeamLog.InviteAcceptanceEmailPolicy.Decoder.Decode(reader);
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
