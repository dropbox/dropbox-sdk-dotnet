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
    /// <para>Result of trying to delete a user's secondary emails. 'success' is the only value
    /// indicating that a user was successfully retrieved for deleting secondary emails. The
    /// other values explain the type of error that occurred, and include the user for which
    /// the error occurred.</para>
    /// </summary>
    public class UserDeleteResult
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<UserDeleteResult> Encoder = new UserDeleteResultEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<UserDeleteResult> Decoder = new UserDeleteResultDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="UserDeleteResult" />
        /// class.</para>
        /// </summary>
        public UserDeleteResult()
        {
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is Success</para>
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return this is Success;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a Success, or <c>null</c>.</para>
        /// </summary>
        public Success AsSuccess
        {
            get
            {
                return this as Success;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is InvalidUser</para>
        /// </summary>
        public bool IsInvalidUser
        {
            get
            {
                return this is InvalidUser;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a InvalidUser, or <c>null</c>.</para>
        /// </summary>
        public InvalidUser AsInvalidUser
        {
            get
            {
                return this as InvalidUser;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is Other</para>
        /// </summary>
        public bool IsOther
        {
            get
            {
                return this is Other;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a Other, or <c>null</c>.</para>
        /// </summary>
        public Other AsOther
        {
            get
            {
                return this as Other;
            }
        }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="UserDeleteResult" />.</para>
        /// </summary>
        private class UserDeleteResultEncoder : enc.StructEncoder<UserDeleteResult>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(UserDeleteResult value, enc.IJsonWriter writer)
            {
                if (value is Success)
                {
                    WriteProperty(".tag", "success", writer, enc.StringEncoder.Instance);
                    Success.Encoder.EncodeFields((Success)value, writer);
                    return;
                }
                if (value is InvalidUser)
                {
                    WriteProperty(".tag", "invalid_user", writer, enc.StringEncoder.Instance);
                    InvalidUser.Encoder.EncodeFields((InvalidUser)value, writer);
                    return;
                }
                if (value is Other)
                {
                    WriteProperty(".tag", "other", writer, enc.StringEncoder.Instance);
                    Other.Encoder.EncodeFields((Other)value, writer);
                    return;
                }
                throw new sys.InvalidOperationException();
            }
        }

        #endregion

        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="UserDeleteResult" />.</para>
        /// </summary>
        private class UserDeleteResultDecoder : enc.UnionDecoder<UserDeleteResult>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="UserDeleteResult" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override UserDeleteResult Create()
            {
                return new UserDeleteResult();
            }

            /// <summary>
            /// <para>Decode based on given tag.</para>
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="reader">The json reader.</param>
            /// <returns>The decoded object.</returns>
            protected override UserDeleteResult Decode(string tag, enc.IJsonReader reader)
            {
                switch (tag)
                {
                    case "success":
                        return Success.Decoder.DecodeFields(reader);
                    case "invalid_user":
                        return InvalidUser.Decoder.DecodeFields(reader);
                    default:
                        return Other.Decoder.DecodeFields(reader);
                }
            }
        }

        #endregion

        /// <summary>
        /// <para>Describes a user and the results for each attempt to delete a secondary
        /// email.</para>
        /// </summary>
        public sealed class Success : UserDeleteResult
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<Success> Encoder = new SuccessEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<Success> Decoder = new SuccessDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="Success" /> class.</para>
            /// </summary>
            /// <param name="value">The value</param>
            public Success(UserDeleteEmailsResult value)
            {
                this.Value = value;
            }
            /// <summary>
            /// <para>Initializes a new instance of the <see cref="Success" /> class.</para>
            /// </summary>
            private Success()
            {
            }

            /// <summary>
            /// <para>Gets the value of this instance.</para>
            /// </summary>
            public UserDeleteEmailsResult Value { get; private set; }

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="Success" />.</para>
            /// </summary>
            private class SuccessEncoder : enc.StructEncoder<Success>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(Success value, enc.IJsonWriter writer)
                {
                    WriteProperty("success", value.Value, writer, global::Dropbox.Api.Team.UserDeleteEmailsResult.Encoder);
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="Success" />.</para>
            /// </summary>
            private class SuccessDecoder : enc.StructDecoder<Success>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="Success" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override Success Create()
                {
                    return new Success();
                }

                /// <summary>
                /// <para>Decode fields without ensuring start and end object.</para>
                /// </summary>
                /// <param name="reader">The json reader.</param>
                /// <returns>The decoded object.</returns>
                public override Success DecodeFields(enc.IJsonReader reader)
                {
                    return new Success(global::Dropbox.Api.Team.UserDeleteEmailsResult.Decoder.DecodeFields(reader));
                }
            }

            #endregion
        }

        /// <summary>
        /// <para>Specified user is not a valid target for deleting secondary emails.</para>
        /// </summary>
        public sealed class InvalidUser : UserDeleteResult
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<InvalidUser> Encoder = new InvalidUserEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<InvalidUser> Decoder = new InvalidUserDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="InvalidUser" />
            /// class.</para>
            /// </summary>
            /// <param name="value">The value</param>
            public InvalidUser(UserSelectorArg value)
            {
                this.Value = value;
            }
            /// <summary>
            /// <para>Initializes a new instance of the <see cref="InvalidUser" />
            /// class.</para>
            /// </summary>
            private InvalidUser()
            {
            }

            /// <summary>
            /// <para>Gets the value of this instance.</para>
            /// </summary>
            public UserSelectorArg Value { get; private set; }

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="InvalidUser" />.</para>
            /// </summary>
            private class InvalidUserEncoder : enc.StructEncoder<InvalidUser>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(InvalidUser value, enc.IJsonWriter writer)
                {
                    WriteProperty("invalid_user", value.Value, writer, global::Dropbox.Api.Team.UserSelectorArg.Encoder);
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="InvalidUser" />.</para>
            /// </summary>
            private class InvalidUserDecoder : enc.StructDecoder<InvalidUser>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="InvalidUser" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override InvalidUser Create()
                {
                    return new InvalidUser();
                }

                /// <summary>
                /// <para>Set given field.</para>
                /// </summary>
                /// <param name="value">The field value.</param>
                /// <param name="fieldName">The field name.</param>
                /// <param name="reader">The json reader.</param>
                protected override void SetField(InvalidUser value, string fieldName, enc.IJsonReader reader)
                {
                    switch (fieldName)
                    {
                        case "invalid_user":
                            value.Value = global::Dropbox.Api.Team.UserSelectorArg.Decoder.Decode(reader);
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// <para>The other object</para>
        /// </summary>
        public sealed class Other : UserDeleteResult
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<Other> Encoder = new OtherEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<Other> Decoder = new OtherDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="Other" /> class.</para>
            /// </summary>
            private Other()
            {
            }

            /// <summary>
            /// <para>A singleton instance of Other</para>
            /// </summary>
            public static readonly Other Instance = new Other();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="Other" />.</para>
            /// </summary>
            private class OtherEncoder : enc.StructEncoder<Other>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(Other value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="Other" />.</para>
            /// </summary>
            private class OtherDecoder : enc.StructDecoder<Other>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="Other" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override Other Create()
                {
                    return Other.Instance;
                }

            }

            #endregion
        }
    }
}
