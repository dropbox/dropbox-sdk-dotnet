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
    /// <para>Specifies if a shared folder inherits its members from the parent folder.</para>
    /// </summary>
    public class SharedFolderMembersInheritancePolicy
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<SharedFolderMembersInheritancePolicy> Encoder = new SharedFolderMembersInheritancePolicyEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<SharedFolderMembersInheritancePolicy> Decoder = new SharedFolderMembersInheritancePolicyDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see
        /// cref="SharedFolderMembersInheritancePolicy" /> class.</para>
        /// </summary>
        public SharedFolderMembersInheritancePolicy()
        {
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is DontInheritMembers</para>
        /// </summary>
        public bool IsDontInheritMembers
        {
            get
            {
                return this is DontInheritMembers;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a DontInheritMembers, or <c>null</c>.</para>
        /// </summary>
        public DontInheritMembers AsDontInheritMembers
        {
            get
            {
                return this as DontInheritMembers;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is InheritMembers</para>
        /// </summary>
        public bool IsInheritMembers
        {
            get
            {
                return this is InheritMembers;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a InheritMembers, or <c>null</c>.</para>
        /// </summary>
        public InheritMembers AsInheritMembers
        {
            get
            {
                return this as InheritMembers;
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
        /// <para>Encoder for  <see cref="SharedFolderMembersInheritancePolicy" />.</para>
        /// </summary>
        private class SharedFolderMembersInheritancePolicyEncoder : enc.StructEncoder<SharedFolderMembersInheritancePolicy>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(SharedFolderMembersInheritancePolicy value, enc.IJsonWriter writer)
            {
                if (value is DontInheritMembers)
                {
                    WriteProperty(".tag", "dont_inherit_members", writer, enc.StringEncoder.Instance);
                    DontInheritMembers.Encoder.EncodeFields((DontInheritMembers)value, writer);
                    return;
                }
                if (value is InheritMembers)
                {
                    WriteProperty(".tag", "inherit_members", writer, enc.StringEncoder.Instance);
                    InheritMembers.Encoder.EncodeFields((InheritMembers)value, writer);
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
        /// <para>Decoder for  <see cref="SharedFolderMembersInheritancePolicy" />.</para>
        /// </summary>
        private class SharedFolderMembersInheritancePolicyDecoder : enc.UnionDecoder<SharedFolderMembersInheritancePolicy>
        {
            /// <summary>
            /// <para>Create a new instance of type <see
            /// cref="SharedFolderMembersInheritancePolicy" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override SharedFolderMembersInheritancePolicy Create()
            {
                return new SharedFolderMembersInheritancePolicy();
            }

            /// <summary>
            /// <para>Decode based on given tag.</para>
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="reader">The json reader.</param>
            /// <returns>The decoded object.</returns>
            protected override SharedFolderMembersInheritancePolicy Decode(string tag, enc.IJsonReader reader)
            {
                switch (tag)
                {
                    case "dont_inherit_members":
                        return DontInheritMembers.Decoder.DecodeFields(reader);
                    case "inherit_members":
                        return InheritMembers.Decoder.DecodeFields(reader);
                    default:
                        return Other.Decoder.DecodeFields(reader);
                }
            }
        }

        #endregion

        /// <summary>
        /// <para>The dont inherit members object</para>
        /// </summary>
        public sealed class DontInheritMembers : SharedFolderMembersInheritancePolicy
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<DontInheritMembers> Encoder = new DontInheritMembersEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<DontInheritMembers> Decoder = new DontInheritMembersDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="DontInheritMembers" />
            /// class.</para>
            /// </summary>
            private DontInheritMembers()
            {
            }

            /// <summary>
            /// <para>A singleton instance of DontInheritMembers</para>
            /// </summary>
            public static readonly DontInheritMembers Instance = new DontInheritMembers();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="DontInheritMembers" />.</para>
            /// </summary>
            private class DontInheritMembersEncoder : enc.StructEncoder<DontInheritMembers>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(DontInheritMembers value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="DontInheritMembers" />.</para>
            /// </summary>
            private class DontInheritMembersDecoder : enc.StructDecoder<DontInheritMembers>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="DontInheritMembers"
                /// />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override DontInheritMembers Create()
                {
                    return DontInheritMembers.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The inherit members object</para>
        /// </summary>
        public sealed class InheritMembers : SharedFolderMembersInheritancePolicy
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<InheritMembers> Encoder = new InheritMembersEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<InheritMembers> Decoder = new InheritMembersDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="InheritMembers" />
            /// class.</para>
            /// </summary>
            private InheritMembers()
            {
            }

            /// <summary>
            /// <para>A singleton instance of InheritMembers</para>
            /// </summary>
            public static readonly InheritMembers Instance = new InheritMembers();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="InheritMembers" />.</para>
            /// </summary>
            private class InheritMembersEncoder : enc.StructEncoder<InheritMembers>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(InheritMembers value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="InheritMembers" />.</para>
            /// </summary>
            private class InheritMembersDecoder : enc.StructDecoder<InheritMembers>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="InheritMembers" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override InheritMembers Create()
                {
                    return InheritMembers.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The other object</para>
        /// </summary>
        public sealed class Other : SharedFolderMembersInheritancePolicy
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
