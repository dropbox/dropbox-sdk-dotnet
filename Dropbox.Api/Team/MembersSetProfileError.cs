// <auto-generated>
// Auto-generated by BabelAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Team
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Babel;

    /// <summary>
    /// <para>The members set profile error object</para>
    /// </summary>
    public class MembersSetProfileError
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<MembersSetProfileError> Encoder = new MembersSetProfileErrorEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<MembersSetProfileError> Decoder = new MembersSetProfileErrorDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="MembersSetProfileError" />
        /// class.</para>
        /// </summary>
        public MembersSetProfileError()
        {
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is
        /// ExternalIdAndNewExternalIdUnsafe</para>
        /// </summary>
        public bool IsExternalIdAndNewExternalIdUnsafe
        {
            get
            {
                return this is ExternalIdAndNewExternalIdUnsafe;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a ExternalIdAndNewExternalIdUnsafe, or
        /// <c>null</c>.</para>
        /// </summary>
        public ExternalIdAndNewExternalIdUnsafe AsExternalIdAndNewExternalIdUnsafe
        {
            get
            {
                return this as ExternalIdAndNewExternalIdUnsafe;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is NoNewDataSpecified</para>
        /// </summary>
        public bool IsNoNewDataSpecified
        {
            get
            {
                return this is NoNewDataSpecified;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a NoNewDataSpecified, or <c>null</c>.</para>
        /// </summary>
        public NoNewDataSpecified AsNoNewDataSpecified
        {
            get
            {
                return this as NoNewDataSpecified;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is
        /// EmailReservedForOtherUser</para>
        /// </summary>
        public bool IsEmailReservedForOtherUser
        {
            get
            {
                return this is EmailReservedForOtherUser;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a EmailReservedForOtherUser, or <c>null</c>.</para>
        /// </summary>
        public EmailReservedForOtherUser AsEmailReservedForOtherUser
        {
            get
            {
                return this as EmailReservedForOtherUser;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is
        /// ExternalIdUsedByOtherUser</para>
        /// </summary>
        public bool IsExternalIdUsedByOtherUser
        {
            get
            {
                return this is ExternalIdUsedByOtherUser;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a ExternalIdUsedByOtherUser, or <c>null</c>.</para>
        /// </summary>
        public ExternalIdUsedByOtherUser AsExternalIdUsedByOtherUser
        {
            get
            {
                return this as ExternalIdUsedByOtherUser;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is SetProfileDisallowed</para>
        /// </summary>
        public bool IsSetProfileDisallowed
        {
            get
            {
                return this is SetProfileDisallowed;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a SetProfileDisallowed, or <c>null</c>.</para>
        /// </summary>
        public SetProfileDisallowed AsSetProfileDisallowed
        {
            get
            {
                return this as SetProfileDisallowed;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is ParamCannotBeEmpty</para>
        /// </summary>
        public bool IsParamCannotBeEmpty
        {
            get
            {
                return this is ParamCannotBeEmpty;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a ParamCannotBeEmpty, or <c>null</c>.</para>
        /// </summary>
        public ParamCannotBeEmpty AsParamCannotBeEmpty
        {
            get
            {
                return this as ParamCannotBeEmpty;
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

        /// <summary>
        /// <para>Gets a value indicating whether this instance is UserNotInTeam</para>
        /// </summary>
        public bool IsUserNotInTeam
        {
            get
            {
                return this is UserNotInTeam;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a UserNotInTeam, or <c>null</c>.</para>
        /// </summary>
        public UserNotInTeam AsUserNotInTeam
        {
            get
            {
                return this as UserNotInTeam;
            }
        }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="MembersSetProfileError" />.</para>
        /// </summary>
        private class MembersSetProfileErrorEncoder : enc.StructEncoder<MembersSetProfileError>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(MembersSetProfileError value, enc.IJsonWriter writer)
            {
                if (value is ExternalIdAndNewExternalIdUnsafe)
                {
                    WriteProperty(".tag", "external_id_and_new_external_id_unsafe", writer, enc.StringEncoder.Instance);
                    ExternalIdAndNewExternalIdUnsafe.Encoder.EncodeFields((ExternalIdAndNewExternalIdUnsafe)value, writer);
                    return;
                }
                if (value is NoNewDataSpecified)
                {
                    WriteProperty(".tag", "no_new_data_specified", writer, enc.StringEncoder.Instance);
                    NoNewDataSpecified.Encoder.EncodeFields((NoNewDataSpecified)value, writer);
                    return;
                }
                if (value is EmailReservedForOtherUser)
                {
                    WriteProperty(".tag", "email_reserved_for_other_user", writer, enc.StringEncoder.Instance);
                    EmailReservedForOtherUser.Encoder.EncodeFields((EmailReservedForOtherUser)value, writer);
                    return;
                }
                if (value is ExternalIdUsedByOtherUser)
                {
                    WriteProperty(".tag", "external_id_used_by_other_user", writer, enc.StringEncoder.Instance);
                    ExternalIdUsedByOtherUser.Encoder.EncodeFields((ExternalIdUsedByOtherUser)value, writer);
                    return;
                }
                if (value is SetProfileDisallowed)
                {
                    WriteProperty(".tag", "set_profile_disallowed", writer, enc.StringEncoder.Instance);
                    SetProfileDisallowed.Encoder.EncodeFields((SetProfileDisallowed)value, writer);
                    return;
                }
                if (value is ParamCannotBeEmpty)
                {
                    WriteProperty(".tag", "param_cannot_be_empty", writer, enc.StringEncoder.Instance);
                    ParamCannotBeEmpty.Encoder.EncodeFields((ParamCannotBeEmpty)value, writer);
                    return;
                }
                if (value is Other)
                {
                    WriteProperty(".tag", "other", writer, enc.StringEncoder.Instance);
                    Other.Encoder.EncodeFields((Other)value, writer);
                    return;
                }
                if (value is UserNotInTeam)
                {
                    WriteProperty(".tag", "user_not_in_team", writer, enc.StringEncoder.Instance);
                    UserNotInTeam.Encoder.EncodeFields((UserNotInTeam)value, writer);
                    return;
                }
                throw new sys.InvalidOperationException();
            }
        }

        #endregion

        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="MembersSetProfileError" />.</para>
        /// </summary>
        private class MembersSetProfileErrorDecoder : enc.UnionDecoder<MembersSetProfileError>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="MembersSetProfileError"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override MembersSetProfileError Create()
            {
                return new MembersSetProfileError();
            }

            /// <summary>
            /// <para>Decode based on given tag.</para>
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="reader">The json reader.</param>
            /// <returns>The decoded object.</returns>
            protected override MembersSetProfileError Decode(string tag, enc.IJsonReader reader)
            {
                switch (tag)
                {
                    case "external_id_and_new_external_id_unsafe":
                        return ExternalIdAndNewExternalIdUnsafe.Decoder.DecodeFields(reader);
                    case "no_new_data_specified":
                        return NoNewDataSpecified.Decoder.DecodeFields(reader);
                    case "email_reserved_for_other_user":
                        return EmailReservedForOtherUser.Decoder.DecodeFields(reader);
                    case "external_id_used_by_other_user":
                        return ExternalIdUsedByOtherUser.Decoder.DecodeFields(reader);
                    case "set_profile_disallowed":
                        return SetProfileDisallowed.Decoder.DecodeFields(reader);
                    case "param_cannot_be_empty":
                        return ParamCannotBeEmpty.Decoder.DecodeFields(reader);
                    default:
                        return Other.Decoder.DecodeFields(reader);
                    case "user_not_in_team":
                        return UserNotInTeam.Decoder.DecodeFields(reader);
                }
            }
        }

        #endregion

        /// <summary>
        /// <para>It is unsafe to use both external_id and new_external_id</para>
        /// </summary>
        public sealed class ExternalIdAndNewExternalIdUnsafe : MembersSetProfileError
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<ExternalIdAndNewExternalIdUnsafe> Encoder = new ExternalIdAndNewExternalIdUnsafeEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<ExternalIdAndNewExternalIdUnsafe> Decoder = new ExternalIdAndNewExternalIdUnsafeDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see
            /// cref="ExternalIdAndNewExternalIdUnsafe" /> class.</para>
            /// </summary>
            private ExternalIdAndNewExternalIdUnsafe()
            {
            }

            /// <summary>
            /// <para>A singleton instance of ExternalIdAndNewExternalIdUnsafe</para>
            /// </summary>
            public static readonly ExternalIdAndNewExternalIdUnsafe Instance = new ExternalIdAndNewExternalIdUnsafe();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="ExternalIdAndNewExternalIdUnsafe" />.</para>
            /// </summary>
            private class ExternalIdAndNewExternalIdUnsafeEncoder : enc.StructEncoder<ExternalIdAndNewExternalIdUnsafe>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(ExternalIdAndNewExternalIdUnsafe value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="ExternalIdAndNewExternalIdUnsafe" />.</para>
            /// </summary>
            private class ExternalIdAndNewExternalIdUnsafeDecoder : enc.StructDecoder<ExternalIdAndNewExternalIdUnsafe>
            {
                /// <summary>
                /// <para>Create a new instance of type <see
                /// cref="ExternalIdAndNewExternalIdUnsafe" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override ExternalIdAndNewExternalIdUnsafe Create()
                {
                    return new ExternalIdAndNewExternalIdUnsafe();
                }

                /// <summary>
                /// <para>Decode fields without ensuring start and end object.</para>
                /// </summary>
                /// <param name="reader">The json reader.</param>
                /// <returns>The decoded object.</returns>
                public override ExternalIdAndNewExternalIdUnsafe DecodeFields(enc.IJsonReader reader)
                {
                    return ExternalIdAndNewExternalIdUnsafe.Instance;
                }
            }

            #endregion
        }

        /// <summary>
        /// <para>None of new_email, new_given_name, new_surname, or new_external_id are
        /// specified</para>
        /// </summary>
        public sealed class NoNewDataSpecified : MembersSetProfileError
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<NoNewDataSpecified> Encoder = new NoNewDataSpecifiedEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<NoNewDataSpecified> Decoder = new NoNewDataSpecifiedDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="NoNewDataSpecified" />
            /// class.</para>
            /// </summary>
            private NoNewDataSpecified()
            {
            }

            /// <summary>
            /// <para>A singleton instance of NoNewDataSpecified</para>
            /// </summary>
            public static readonly NoNewDataSpecified Instance = new NoNewDataSpecified();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="NoNewDataSpecified" />.</para>
            /// </summary>
            private class NoNewDataSpecifiedEncoder : enc.StructEncoder<NoNewDataSpecified>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(NoNewDataSpecified value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="NoNewDataSpecified" />.</para>
            /// </summary>
            private class NoNewDataSpecifiedDecoder : enc.StructDecoder<NoNewDataSpecified>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="NoNewDataSpecified"
                /// />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override NoNewDataSpecified Create()
                {
                    return new NoNewDataSpecified();
                }

                /// <summary>
                /// <para>Decode fields without ensuring start and end object.</para>
                /// </summary>
                /// <param name="reader">The json reader.</param>
                /// <returns>The decoded object.</returns>
                public override NoNewDataSpecified DecodeFields(enc.IJsonReader reader)
                {
                    return NoNewDataSpecified.Instance;
                }
            }

            #endregion
        }

        /// <summary>
        /// <para>Email is already reserved for another user.</para>
        /// </summary>
        public sealed class EmailReservedForOtherUser : MembersSetProfileError
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<EmailReservedForOtherUser> Encoder = new EmailReservedForOtherUserEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<EmailReservedForOtherUser> Decoder = new EmailReservedForOtherUserDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="EmailReservedForOtherUser"
            /// /> class.</para>
            /// </summary>
            private EmailReservedForOtherUser()
            {
            }

            /// <summary>
            /// <para>A singleton instance of EmailReservedForOtherUser</para>
            /// </summary>
            public static readonly EmailReservedForOtherUser Instance = new EmailReservedForOtherUser();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="EmailReservedForOtherUser" />.</para>
            /// </summary>
            private class EmailReservedForOtherUserEncoder : enc.StructEncoder<EmailReservedForOtherUser>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(EmailReservedForOtherUser value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="EmailReservedForOtherUser" />.</para>
            /// </summary>
            private class EmailReservedForOtherUserDecoder : enc.StructDecoder<EmailReservedForOtherUser>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="EmailReservedForOtherUser"
                /// />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override EmailReservedForOtherUser Create()
                {
                    return new EmailReservedForOtherUser();
                }

                /// <summary>
                /// <para>Decode fields without ensuring start and end object.</para>
                /// </summary>
                /// <param name="reader">The json reader.</param>
                /// <returns>The decoded object.</returns>
                public override EmailReservedForOtherUser DecodeFields(enc.IJsonReader reader)
                {
                    return EmailReservedForOtherUser.Instance;
                }
            }

            #endregion
        }

        /// <summary>
        /// <para>The external ID is already in use by another team member.</para>
        /// </summary>
        public sealed class ExternalIdUsedByOtherUser : MembersSetProfileError
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<ExternalIdUsedByOtherUser> Encoder = new ExternalIdUsedByOtherUserEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<ExternalIdUsedByOtherUser> Decoder = new ExternalIdUsedByOtherUserDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="ExternalIdUsedByOtherUser"
            /// /> class.</para>
            /// </summary>
            private ExternalIdUsedByOtherUser()
            {
            }

            /// <summary>
            /// <para>A singleton instance of ExternalIdUsedByOtherUser</para>
            /// </summary>
            public static readonly ExternalIdUsedByOtherUser Instance = new ExternalIdUsedByOtherUser();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="ExternalIdUsedByOtherUser" />.</para>
            /// </summary>
            private class ExternalIdUsedByOtherUserEncoder : enc.StructEncoder<ExternalIdUsedByOtherUser>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(ExternalIdUsedByOtherUser value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="ExternalIdUsedByOtherUser" />.</para>
            /// </summary>
            private class ExternalIdUsedByOtherUserDecoder : enc.StructDecoder<ExternalIdUsedByOtherUser>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="ExternalIdUsedByOtherUser"
                /// />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override ExternalIdUsedByOtherUser Create()
                {
                    return new ExternalIdUsedByOtherUser();
                }

                /// <summary>
                /// <para>Decode fields without ensuring start and end object.</para>
                /// </summary>
                /// <param name="reader">The json reader.</param>
                /// <returns>The decoded object.</returns>
                public override ExternalIdUsedByOtherUser DecodeFields(enc.IJsonReader reader)
                {
                    return ExternalIdUsedByOtherUser.Instance;
                }
            }

            #endregion
        }

        /// <summary>
        /// <para>Setting profile disallowed</para>
        /// </summary>
        public sealed class SetProfileDisallowed : MembersSetProfileError
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<SetProfileDisallowed> Encoder = new SetProfileDisallowedEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<SetProfileDisallowed> Decoder = new SetProfileDisallowedDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="SetProfileDisallowed" />
            /// class.</para>
            /// </summary>
            private SetProfileDisallowed()
            {
            }

            /// <summary>
            /// <para>A singleton instance of SetProfileDisallowed</para>
            /// </summary>
            public static readonly SetProfileDisallowed Instance = new SetProfileDisallowed();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="SetProfileDisallowed" />.</para>
            /// </summary>
            private class SetProfileDisallowedEncoder : enc.StructEncoder<SetProfileDisallowed>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(SetProfileDisallowed value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="SetProfileDisallowed" />.</para>
            /// </summary>
            private class SetProfileDisallowedDecoder : enc.StructDecoder<SetProfileDisallowed>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="SetProfileDisallowed"
                /// />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override SetProfileDisallowed Create()
                {
                    return new SetProfileDisallowed();
                }

                /// <summary>
                /// <para>Decode fields without ensuring start and end object.</para>
                /// </summary>
                /// <param name="reader">The json reader.</param>
                /// <returns>The decoded object.</returns>
                public override SetProfileDisallowed DecodeFields(enc.IJsonReader reader)
                {
                    return SetProfileDisallowed.Instance;
                }
            }

            #endregion
        }

        /// <summary>
        /// <para>New  new_email, new_given_name or new_surname value cannot be empty.</para>
        /// </summary>
        public sealed class ParamCannotBeEmpty : MembersSetProfileError
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<ParamCannotBeEmpty> Encoder = new ParamCannotBeEmptyEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<ParamCannotBeEmpty> Decoder = new ParamCannotBeEmptyDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="ParamCannotBeEmpty" />
            /// class.</para>
            /// </summary>
            private ParamCannotBeEmpty()
            {
            }

            /// <summary>
            /// <para>A singleton instance of ParamCannotBeEmpty</para>
            /// </summary>
            public static readonly ParamCannotBeEmpty Instance = new ParamCannotBeEmpty();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="ParamCannotBeEmpty" />.</para>
            /// </summary>
            private class ParamCannotBeEmptyEncoder : enc.StructEncoder<ParamCannotBeEmpty>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(ParamCannotBeEmpty value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="ParamCannotBeEmpty" />.</para>
            /// </summary>
            private class ParamCannotBeEmptyDecoder : enc.StructDecoder<ParamCannotBeEmpty>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="ParamCannotBeEmpty"
                /// />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override ParamCannotBeEmpty Create()
                {
                    return new ParamCannotBeEmpty();
                }

                /// <summary>
                /// <para>Decode fields without ensuring start and end object.</para>
                /// </summary>
                /// <param name="reader">The json reader.</param>
                /// <returns>The decoded object.</returns>
                public override ParamCannotBeEmpty DecodeFields(enc.IJsonReader reader)
                {
                    return ParamCannotBeEmpty.Instance;
                }
            }

            #endregion
        }

        /// <summary>
        /// <para>An unspecified error.</para>
        /// </summary>
        public sealed class Other : MembersSetProfileError
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
                    return new Other();
                }

                /// <summary>
                /// <para>Decode fields without ensuring start and end object.</para>
                /// </summary>
                /// <param name="reader">The json reader.</param>
                /// <returns>The decoded object.</returns>
                public override Other DecodeFields(enc.IJsonReader reader)
                {
                    return Other.Instance;
                }
            }

            #endregion
        }

        /// <summary>
        /// <para>The user is not a member of the team.</para>
        /// </summary>
        public sealed class UserNotInTeam : MembersSetProfileError
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<UserNotInTeam> Encoder = new UserNotInTeamEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<UserNotInTeam> Decoder = new UserNotInTeamDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="UserNotInTeam" />
            /// class.</para>
            /// </summary>
            private UserNotInTeam()
            {
            }

            /// <summary>
            /// <para>A singleton instance of UserNotInTeam</para>
            /// </summary>
            public static readonly UserNotInTeam Instance = new UserNotInTeam();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="UserNotInTeam" />.</para>
            /// </summary>
            private class UserNotInTeamEncoder : enc.StructEncoder<UserNotInTeam>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(UserNotInTeam value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="UserNotInTeam" />.</para>
            /// </summary>
            private class UserNotInTeamDecoder : enc.StructDecoder<UserNotInTeam>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="UserNotInTeam" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override UserNotInTeam Create()
                {
                    return new UserNotInTeam();
                }

                /// <summary>
                /// <para>Decode fields without ensuring start and end object.</para>
                /// </summary>
                /// <param name="reader">The json reader.</param>
                /// <returns>The decoded object.</returns>
                public override UserNotInTeam DecodeFields(enc.IJsonReader reader)
                {
                    return UserNotInTeam.Instance;
                }
            }

            #endregion
        }
    }
}
