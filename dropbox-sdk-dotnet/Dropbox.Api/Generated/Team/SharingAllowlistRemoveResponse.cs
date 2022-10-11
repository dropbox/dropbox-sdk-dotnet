// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Team
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>This struct is empty. The comment here is intentionally emitted to avoid
    /// indentation issues with Stone.</para>
    /// </summary>
    public class SharingAllowlistRemoveResponse
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<SharingAllowlistRemoveResponse> Encoder = new SharingAllowlistRemoveResponseEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<SharingAllowlistRemoveResponse> Decoder = new SharingAllowlistRemoveResponseDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="SharingAllowlistRemoveResponse"
        /// /> class.</para>
        /// </summary>
        public SharingAllowlistRemoveResponse()
        {
        }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="SharingAllowlistRemoveResponse" />.</para>
        /// </summary>
        private class SharingAllowlistRemoveResponseEncoder : enc.StructEncoder<SharingAllowlistRemoveResponse>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(SharingAllowlistRemoveResponse value, enc.IJsonWriter writer)
            {
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="SharingAllowlistRemoveResponse" />.</para>
        /// </summary>
        private class SharingAllowlistRemoveResponseDecoder : enc.StructDecoder<SharingAllowlistRemoveResponse>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="SharingAllowlistRemoveResponse"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override SharingAllowlistRemoveResponse Create()
            {
                return new SharingAllowlistRemoveResponse();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(SharingAllowlistRemoveResponse value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        #endregion
    }
}
