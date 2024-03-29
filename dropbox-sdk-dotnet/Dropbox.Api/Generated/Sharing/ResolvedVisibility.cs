// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Sharing
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>The actual access permissions values of shared links after taking into account
    /// user preferences and the team and shared folder settings. Check the <see
    /// cref="RequestedVisibility" /> for more info on the possible visibility values that can
    /// be set by the shared link's owner.</para>
    /// </summary>
    public class ResolvedVisibility
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<ResolvedVisibility> Encoder = new ResolvedVisibilityEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<ResolvedVisibility> Decoder = new ResolvedVisibilityDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ResolvedVisibility" />
        /// class.</para>
        /// </summary>
        public ResolvedVisibility()
        {
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is TeamAndPassword</para>
        /// </summary>
        public bool IsTeamAndPassword
        {
            get
            {
                return this is TeamAndPassword;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a TeamAndPassword, or <c>null</c>.</para>
        /// </summary>
        public TeamAndPassword AsTeamAndPassword
        {
            get
            {
                return this as TeamAndPassword;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is SharedFolderOnly</para>
        /// </summary>
        public bool IsSharedFolderOnly
        {
            get
            {
                return this is SharedFolderOnly;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a SharedFolderOnly, or <c>null</c>.</para>
        /// </summary>
        public SharedFolderOnly AsSharedFolderOnly
        {
            get
            {
                return this as SharedFolderOnly;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is NoOne</para>
        /// </summary>
        public bool IsNoOne
        {
            get
            {
                return this is NoOne;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a NoOne, or <c>null</c>.</para>
        /// </summary>
        public NoOne AsNoOne
        {
            get
            {
                return this as NoOne;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is OnlyYou</para>
        /// </summary>
        public bool IsOnlyYou
        {
            get
            {
                return this is OnlyYou;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a OnlyYou, or <c>null</c>.</para>
        /// </summary>
        public OnlyYou AsOnlyYou
        {
            get
            {
                return this as OnlyYou;
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
        /// <para>Gets a value indicating whether this instance is Public</para>
        /// </summary>
        public bool IsPublic
        {
            get
            {
                return this is Public;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a Public, or <c>null</c>.</para>
        /// </summary>
        public Public AsPublic
        {
            get
            {
                return this as Public;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is TeamOnly</para>
        /// </summary>
        public bool IsTeamOnly
        {
            get
            {
                return this is TeamOnly;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a TeamOnly, or <c>null</c>.</para>
        /// </summary>
        public TeamOnly AsTeamOnly
        {
            get
            {
                return this as TeamOnly;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is Password</para>
        /// </summary>
        public bool IsPassword
        {
            get
            {
                return this is Password;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a Password, or <c>null</c>.</para>
        /// </summary>
        public Password AsPassword
        {
            get
            {
                return this as Password;
            }
        }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="ResolvedVisibility" />.</para>
        /// </summary>
        private class ResolvedVisibilityEncoder : enc.StructEncoder<ResolvedVisibility>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(ResolvedVisibility value, enc.IJsonWriter writer)
            {
                if (value is TeamAndPassword)
                {
                    WriteProperty(".tag", "team_and_password", writer, enc.StringEncoder.Instance);
                    TeamAndPassword.Encoder.EncodeFields((TeamAndPassword)value, writer);
                    return;
                }
                if (value is SharedFolderOnly)
                {
                    WriteProperty(".tag", "shared_folder_only", writer, enc.StringEncoder.Instance);
                    SharedFolderOnly.Encoder.EncodeFields((SharedFolderOnly)value, writer);
                    return;
                }
                if (value is NoOne)
                {
                    WriteProperty(".tag", "no_one", writer, enc.StringEncoder.Instance);
                    NoOne.Encoder.EncodeFields((NoOne)value, writer);
                    return;
                }
                if (value is OnlyYou)
                {
                    WriteProperty(".tag", "only_you", writer, enc.StringEncoder.Instance);
                    OnlyYou.Encoder.EncodeFields((OnlyYou)value, writer);
                    return;
                }
                if (value is Other)
                {
                    WriteProperty(".tag", "other", writer, enc.StringEncoder.Instance);
                    Other.Encoder.EncodeFields((Other)value, writer);
                    return;
                }
                if (value is Public)
                {
                    WriteProperty(".tag", "public", writer, enc.StringEncoder.Instance);
                    Public.Encoder.EncodeFields((Public)value, writer);
                    return;
                }
                if (value is TeamOnly)
                {
                    WriteProperty(".tag", "team_only", writer, enc.StringEncoder.Instance);
                    TeamOnly.Encoder.EncodeFields((TeamOnly)value, writer);
                    return;
                }
                if (value is Password)
                {
                    WriteProperty(".tag", "password", writer, enc.StringEncoder.Instance);
                    Password.Encoder.EncodeFields((Password)value, writer);
                    return;
                }
                throw new sys.InvalidOperationException();
            }
        }

        #endregion

        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="ResolvedVisibility" />.</para>
        /// </summary>
        private class ResolvedVisibilityDecoder : enc.UnionDecoder<ResolvedVisibility>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="ResolvedVisibility" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override ResolvedVisibility Create()
            {
                return new ResolvedVisibility();
            }

            /// <summary>
            /// <para>Decode based on given tag.</para>
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="reader">The json reader.</param>
            /// <returns>The decoded object.</returns>
            protected override ResolvedVisibility Decode(string tag, enc.IJsonReader reader)
            {
                switch (tag)
                {
                    case "team_and_password":
                        return TeamAndPassword.Decoder.DecodeFields(reader);
                    case "shared_folder_only":
                        return SharedFolderOnly.Decoder.DecodeFields(reader);
                    case "no_one":
                        return NoOne.Decoder.DecodeFields(reader);
                    case "only_you":
                        return OnlyYou.Decoder.DecodeFields(reader);
                    default:
                        return Other.Decoder.DecodeFields(reader);
                    case "public":
                        return Public.Decoder.DecodeFields(reader);
                    case "team_only":
                        return TeamOnly.Decoder.DecodeFields(reader);
                    case "password":
                        return Password.Decoder.DecodeFields(reader);
                }
            }
        }

        #endregion

        /// <summary>
        /// <para>Only members of the same team who have the link-specific password can access
        /// the link. Login is required.</para>
        /// </summary>
        public sealed class TeamAndPassword : ResolvedVisibility
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<TeamAndPassword> Encoder = new TeamAndPasswordEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<TeamAndPassword> Decoder = new TeamAndPasswordDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="TeamAndPassword" />
            /// class.</para>
            /// </summary>
            private TeamAndPassword()
            {
            }

            /// <summary>
            /// <para>A singleton instance of TeamAndPassword</para>
            /// </summary>
            public static readonly TeamAndPassword Instance = new TeamAndPassword();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="TeamAndPassword" />.</para>
            /// </summary>
            private class TeamAndPasswordEncoder : enc.StructEncoder<TeamAndPassword>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(TeamAndPassword value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="TeamAndPassword" />.</para>
            /// </summary>
            private class TeamAndPasswordDecoder : enc.StructDecoder<TeamAndPassword>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="TeamAndPassword" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override TeamAndPassword Create()
                {
                    return TeamAndPassword.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>Only members of the shared folder containing the linked file can access the
        /// link. Login is required.</para>
        /// </summary>
        public sealed class SharedFolderOnly : ResolvedVisibility
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<SharedFolderOnly> Encoder = new SharedFolderOnlyEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<SharedFolderOnly> Decoder = new SharedFolderOnlyDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="SharedFolderOnly" />
            /// class.</para>
            /// </summary>
            private SharedFolderOnly()
            {
            }

            /// <summary>
            /// <para>A singleton instance of SharedFolderOnly</para>
            /// </summary>
            public static readonly SharedFolderOnly Instance = new SharedFolderOnly();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="SharedFolderOnly" />.</para>
            /// </summary>
            private class SharedFolderOnlyEncoder : enc.StructEncoder<SharedFolderOnly>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(SharedFolderOnly value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="SharedFolderOnly" />.</para>
            /// </summary>
            private class SharedFolderOnlyDecoder : enc.StructDecoder<SharedFolderOnly>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="SharedFolderOnly" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override SharedFolderOnly Create()
                {
                    return SharedFolderOnly.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The link merely points the user to the content, and does not grant any
        /// additional rights. Existing members of the content who use this link can only
        /// access the content with their pre-existing access rights. Either on the file
        /// directly, or inherited from a parent folder.</para>
        /// </summary>
        public sealed class NoOne : ResolvedVisibility
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<NoOne> Encoder = new NoOneEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<NoOne> Decoder = new NoOneDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="NoOne" /> class.</para>
            /// </summary>
            private NoOne()
            {
            }

            /// <summary>
            /// <para>A singleton instance of NoOne</para>
            /// </summary>
            public static readonly NoOne Instance = new NoOne();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="NoOne" />.</para>
            /// </summary>
            private class NoOneEncoder : enc.StructEncoder<NoOne>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(NoOne value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="NoOne" />.</para>
            /// </summary>
            private class NoOneDecoder : enc.StructDecoder<NoOne>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="NoOne" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override NoOne Create()
                {
                    return NoOne.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>Only the current user can view this link.</para>
        /// </summary>
        public sealed class OnlyYou : ResolvedVisibility
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<OnlyYou> Encoder = new OnlyYouEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<OnlyYou> Decoder = new OnlyYouDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="OnlyYou" /> class.</para>
            /// </summary>
            private OnlyYou()
            {
            }

            /// <summary>
            /// <para>A singleton instance of OnlyYou</para>
            /// </summary>
            public static readonly OnlyYou Instance = new OnlyYou();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="OnlyYou" />.</para>
            /// </summary>
            private class OnlyYouEncoder : enc.StructEncoder<OnlyYou>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(OnlyYou value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="OnlyYou" />.</para>
            /// </summary>
            private class OnlyYouDecoder : enc.StructDecoder<OnlyYou>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="OnlyYou" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override OnlyYou Create()
                {
                    return OnlyYou.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The other object</para>
        /// </summary>
        public sealed class Other : ResolvedVisibility
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

        /// <summary>
        /// <para>Anyone who has received the link can access it. No login required.</para>
        /// </summary>
        public sealed class Public : ResolvedVisibility
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<Public> Encoder = new PublicEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<Public> Decoder = new PublicDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="Public" /> class.</para>
            /// </summary>
            private Public()
            {
            }

            /// <summary>
            /// <para>A singleton instance of Public</para>
            /// </summary>
            public static readonly Public Instance = new Public();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="Public" />.</para>
            /// </summary>
            private class PublicEncoder : enc.StructEncoder<Public>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(Public value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="Public" />.</para>
            /// </summary>
            private class PublicDecoder : enc.StructDecoder<Public>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="Public" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override Public Create()
                {
                    return Public.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>Only members of the same team can access the link. Login is required.</para>
        /// </summary>
        public sealed class TeamOnly : ResolvedVisibility
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<TeamOnly> Encoder = new TeamOnlyEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<TeamOnly> Decoder = new TeamOnlyDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="TeamOnly" /> class.</para>
            /// </summary>
            private TeamOnly()
            {
            }

            /// <summary>
            /// <para>A singleton instance of TeamOnly</para>
            /// </summary>
            public static readonly TeamOnly Instance = new TeamOnly();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="TeamOnly" />.</para>
            /// </summary>
            private class TeamOnlyEncoder : enc.StructEncoder<TeamOnly>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(TeamOnly value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="TeamOnly" />.</para>
            /// </summary>
            private class TeamOnlyDecoder : enc.StructDecoder<TeamOnly>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="TeamOnly" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override TeamOnly Create()
                {
                    return TeamOnly.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>A link-specific password is required to access the link. Login is not
        /// required.</para>
        /// </summary>
        public sealed class Password : ResolvedVisibility
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<Password> Encoder = new PasswordEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<Password> Decoder = new PasswordDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="Password" /> class.</para>
            /// </summary>
            private Password()
            {
            }

            /// <summary>
            /// <para>A singleton instance of Password</para>
            /// </summary>
            public static readonly Password Instance = new Password();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="Password" />.</para>
            /// </summary>
            private class PasswordEncoder : enc.StructEncoder<Password>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(Password value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="Password" />.</para>
            /// </summary>
            private class PasswordDecoder : enc.StructDecoder<Password>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="Password" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override Password Create()
                {
                    return Password.Instance;
                }

            }

            #endregion
        }
    }
}
