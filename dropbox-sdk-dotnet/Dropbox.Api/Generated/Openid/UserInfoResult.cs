// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Openid
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>The user info result object</para>
    /// </summary>
    public class UserInfoResult
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<UserInfoResult> Encoder = new UserInfoResultEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<UserInfoResult> Decoder = new UserInfoResultDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="UserInfoResult" /> class.</para>
        /// </summary>
        /// <param name="familyName">Last name of user.</param>
        /// <param name="givenName">First name of user.</param>
        /// <param name="email">Email address of user.</param>
        /// <param name="emailVerified">If user is email verified.</param>
        /// <param name="iss">Issuer of token (in this case Dropbox).</param>
        /// <param name="sub">An identifier for the user. This is the Dropbox account_id, a
        /// string value such as dbid:AAH4f99T0taONIb-OurWxbNQ6ywGRopQngc.</param>
        public UserInfoResult(string familyName = null,
                              string givenName = null,
                              string email = null,
                              bool? emailVerified = null,
                              string iss = "",
                              string sub = "")
        {
            if (iss == null)
            {
                throw new sys.ArgumentNullException("iss");
            }

            if (sub == null)
            {
                throw new sys.ArgumentNullException("sub");
            }

            this.FamilyName = familyName;
            this.GivenName = givenName;
            this.Email = email;
            this.EmailVerified = emailVerified;
            this.Iss = iss;
            this.Sub = sub;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="UserInfoResult" /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public UserInfoResult()
        {
            this.Iss = "";
            this.Sub = "";
        }

        /// <summary>
        /// <para>Last name of user.</para>
        /// </summary>
        public string FamilyName { get; protected set; }

        /// <summary>
        /// <para>First name of user.</para>
        /// </summary>
        public string GivenName { get; protected set; }

        /// <summary>
        /// <para>Email address of user.</para>
        /// </summary>
        public string Email { get; protected set; }

        /// <summary>
        /// <para>If user is email verified.</para>
        /// </summary>
        public bool? EmailVerified { get; protected set; }

        /// <summary>
        /// <para>Issuer of token (in this case Dropbox).</para>
        /// </summary>
        public string Iss { get; protected set; }

        /// <summary>
        /// <para>An identifier for the user. This is the Dropbox account_id, a string value
        /// such as dbid:AAH4f99T0taONIb-OurWxbNQ6ywGRopQngc.</para>
        /// </summary>
        public string Sub { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="UserInfoResult" />.</para>
        /// </summary>
        private class UserInfoResultEncoder : enc.StructEncoder<UserInfoResult>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(UserInfoResult value, enc.IJsonWriter writer)
            {
                if (value.FamilyName != null)
                {
                    WriteProperty("family_name", value.FamilyName, writer, enc.StringEncoder.Instance);
                }
                if (value.GivenName != null)
                {
                    WriteProperty("given_name", value.GivenName, writer, enc.StringEncoder.Instance);
                }
                if (value.Email != null)
                {
                    WriteProperty("email", value.Email, writer, enc.StringEncoder.Instance);
                }
                if (value.EmailVerified != null)
                {
                    WriteProperty("email_verified", value.EmailVerified.Value, writer, enc.BooleanEncoder.Instance);
                }
                WriteProperty("iss", value.Iss, writer, enc.StringEncoder.Instance);
                WriteProperty("sub", value.Sub, writer, enc.StringEncoder.Instance);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="UserInfoResult" />.</para>
        /// </summary>
        private class UserInfoResultDecoder : enc.StructDecoder<UserInfoResult>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="UserInfoResult" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override UserInfoResult Create()
            {
                return new UserInfoResult();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(UserInfoResult value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "family_name":
                        value.FamilyName = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "given_name":
                        value.GivenName = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "email":
                        value.Email = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "email_verified":
                        value.EmailVerified = enc.BooleanDecoder.Instance.Decode(reader);
                        break;
                    case "iss":
                        value.Iss = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "sub":
                        value.Sub = enc.StringDecoder.Instance.Decode(reader);
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
