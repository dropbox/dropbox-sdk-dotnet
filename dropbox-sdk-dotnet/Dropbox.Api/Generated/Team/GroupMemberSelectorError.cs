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
    /// <para>Error that can be raised when <see cref="GroupMemberSelector" /> is used, and the
    /// user is required to be a member of the specified group.</para>
    /// </summary>
    public class GroupMemberSelectorError
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<GroupMemberSelectorError> Encoder = new GroupMemberSelectorErrorEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<GroupMemberSelectorError> Decoder = new GroupMemberSelectorErrorDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="GroupMemberSelectorError" />
        /// class.</para>
        /// </summary>
        public GroupMemberSelectorError()
        {
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is MemberNotInGroup</para>
        /// </summary>
        public bool IsMemberNotInGroup
        {
            get
            {
                return this is MemberNotInGroup;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a MemberNotInGroup, or <c>null</c>.</para>
        /// </summary>
        public MemberNotInGroup AsMemberNotInGroup
        {
            get
            {
                return this as MemberNotInGroup;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is
        /// SystemManagedGroupDisallowed</para>
        /// </summary>
        public bool IsSystemManagedGroupDisallowed
        {
            get
            {
                return this is SystemManagedGroupDisallowed;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a SystemManagedGroupDisallowed, or <c>null</c>.</para>
        /// </summary>
        public SystemManagedGroupDisallowed AsSystemManagedGroupDisallowed
        {
            get
            {
                return this as SystemManagedGroupDisallowed;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is GroupNotFound</para>
        /// </summary>
        public bool IsGroupNotFound
        {
            get
            {
                return this is GroupNotFound;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a GroupNotFound, or <c>null</c>.</para>
        /// </summary>
        public GroupNotFound AsGroupNotFound
        {
            get
            {
                return this as GroupNotFound;
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
        /// <para>Encoder for  <see cref="GroupMemberSelectorError" />.</para>
        /// </summary>
        private class GroupMemberSelectorErrorEncoder : enc.StructEncoder<GroupMemberSelectorError>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(GroupMemberSelectorError value, enc.IJsonWriter writer)
            {
                if (value is MemberNotInGroup)
                {
                    WriteProperty(".tag", "member_not_in_group", writer, enc.StringEncoder.Instance);
                    MemberNotInGroup.Encoder.EncodeFields((MemberNotInGroup)value, writer);
                    return;
                }
                if (value is SystemManagedGroupDisallowed)
                {
                    WriteProperty(".tag", "system_managed_group_disallowed", writer, enc.StringEncoder.Instance);
                    SystemManagedGroupDisallowed.Encoder.EncodeFields((SystemManagedGroupDisallowed)value, writer);
                    return;
                }
                if (value is GroupNotFound)
                {
                    WriteProperty(".tag", "group_not_found", writer, enc.StringEncoder.Instance);
                    GroupNotFound.Encoder.EncodeFields((GroupNotFound)value, writer);
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
        /// <para>Decoder for  <see cref="GroupMemberSelectorError" />.</para>
        /// </summary>
        private class GroupMemberSelectorErrorDecoder : enc.UnionDecoder<GroupMemberSelectorError>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="GroupMemberSelectorError"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override GroupMemberSelectorError Create()
            {
                return new GroupMemberSelectorError();
            }

            /// <summary>
            /// <para>Decode based on given tag.</para>
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="reader">The json reader.</param>
            /// <returns>The decoded object.</returns>
            protected override GroupMemberSelectorError Decode(string tag, enc.IJsonReader reader)
            {
                switch (tag)
                {
                    case "member_not_in_group":
                        return MemberNotInGroup.Decoder.DecodeFields(reader);
                    case "system_managed_group_disallowed":
                        return SystemManagedGroupDisallowed.Decoder.DecodeFields(reader);
                    case "group_not_found":
                        return GroupNotFound.Decoder.DecodeFields(reader);
                    case "other":
                        return Other.Decoder.DecodeFields(reader);
                    default:
                        throw new sys.InvalidOperationException();
                }
            }
        }

        #endregion

        /// <summary>
        /// <para>The specified user is not a member of this group.</para>
        /// </summary>
        public sealed class MemberNotInGroup : GroupMemberSelectorError
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<MemberNotInGroup> Encoder = new MemberNotInGroupEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<MemberNotInGroup> Decoder = new MemberNotInGroupDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="MemberNotInGroup" />
            /// class.</para>
            /// </summary>
            private MemberNotInGroup()
            {
            }

            /// <summary>
            /// <para>A singleton instance of MemberNotInGroup</para>
            /// </summary>
            public static readonly MemberNotInGroup Instance = new MemberNotInGroup();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="MemberNotInGroup" />.</para>
            /// </summary>
            private class MemberNotInGroupEncoder : enc.StructEncoder<MemberNotInGroup>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(MemberNotInGroup value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="MemberNotInGroup" />.</para>
            /// </summary>
            private class MemberNotInGroupDecoder : enc.StructDecoder<MemberNotInGroup>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="MemberNotInGroup" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override MemberNotInGroup Create()
                {
                    return MemberNotInGroup.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>This operation is not supported on system-managed groups.</para>
        /// </summary>
        public sealed class SystemManagedGroupDisallowed : GroupMemberSelectorError
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<SystemManagedGroupDisallowed> Encoder = new SystemManagedGroupDisallowedEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<SystemManagedGroupDisallowed> Decoder = new SystemManagedGroupDisallowedDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see
            /// cref="SystemManagedGroupDisallowed" /> class.</para>
            /// </summary>
            private SystemManagedGroupDisallowed()
            {
            }

            /// <summary>
            /// <para>A singleton instance of SystemManagedGroupDisallowed</para>
            /// </summary>
            public static readonly SystemManagedGroupDisallowed Instance = new SystemManagedGroupDisallowed();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="SystemManagedGroupDisallowed" />.</para>
            /// </summary>
            private class SystemManagedGroupDisallowedEncoder : enc.StructEncoder<SystemManagedGroupDisallowed>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(SystemManagedGroupDisallowed value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="SystemManagedGroupDisallowed" />.</para>
            /// </summary>
            private class SystemManagedGroupDisallowedDecoder : enc.StructDecoder<SystemManagedGroupDisallowed>
            {
                /// <summary>
                /// <para>Create a new instance of type <see
                /// cref="SystemManagedGroupDisallowed" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override SystemManagedGroupDisallowed Create()
                {
                    return SystemManagedGroupDisallowed.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>No matching group found. No groups match the specified group ID.</para>
        /// </summary>
        public sealed class GroupNotFound : GroupMemberSelectorError
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<GroupNotFound> Encoder = new GroupNotFoundEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<GroupNotFound> Decoder = new GroupNotFoundDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="GroupNotFound" />
            /// class.</para>
            /// </summary>
            private GroupNotFound()
            {
            }

            /// <summary>
            /// <para>A singleton instance of GroupNotFound</para>
            /// </summary>
            public static readonly GroupNotFound Instance = new GroupNotFound();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="GroupNotFound" />.</para>
            /// </summary>
            private class GroupNotFoundEncoder : enc.StructEncoder<GroupNotFound>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(GroupNotFound value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="GroupNotFound" />.</para>
            /// </summary>
            private class GroupNotFoundDecoder : enc.StructDecoder<GroupNotFound>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="GroupNotFound" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override GroupNotFound Create()
                {
                    return GroupNotFound.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The other object</para>
        /// </summary>
        public sealed class Other : GroupMemberSelectorError
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
