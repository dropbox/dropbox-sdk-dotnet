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
    /// <para>Argument for selecting a group and a list of users.</para>
    /// </summary>
    public class GroupMembersSelector
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<GroupMembersSelector> Encoder = new GroupMembersSelectorEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<GroupMembersSelector> Decoder = new GroupMembersSelectorDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="GroupMembersSelector" />
        /// class.</para>
        /// </summary>
        /// <param name="group">Specify a group.</param>
        /// <param name="users">A list of users that are members of <paramref name="group"
        /// />.</param>
        public GroupMembersSelector(GroupSelector @group,
                                    UsersSelectorArg users)
        {
            if (@group == null)
            {
                throw new sys.ArgumentNullException("@group");
            }

            if (users == null)
            {
                throw new sys.ArgumentNullException("users");
            }

            this.Group = @group;
            this.Users = users;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="GroupMembersSelector" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        public GroupMembersSelector()
        {
        }

        /// <summary>
        /// <para>Specify a group.</para>
        /// </summary>
        public GroupSelector Group { get; protected set; }

        /// <summary>
        /// <para>A list of users that are members of <see cref="Group" />.</para>
        /// </summary>
        public UsersSelectorArg Users { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="GroupMembersSelector" />.</para>
        /// </summary>
        private class GroupMembersSelectorEncoder : enc.StructEncoder<GroupMembersSelector>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(GroupMembersSelector value, enc.IJsonWriter writer)
            {
                WriteProperty("group", value.Group, writer, Dropbox.Api.Team.GroupSelector.Encoder);
                WriteProperty("users", value.Users, writer, Dropbox.Api.Team.UsersSelectorArg.Encoder);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="GroupMembersSelector" />.</para>
        /// </summary>
        private class GroupMembersSelectorDecoder : enc.StructDecoder<GroupMembersSelector>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="GroupMembersSelector" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override GroupMembersSelector Create()
            {
                return new GroupMembersSelector();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(GroupMembersSelector value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "group":
                        value.Group = Dropbox.Api.Team.GroupSelector.Decoder.Decode(reader);
                        break;
                    case "users":
                        value.Users = Dropbox.Api.Team.UsersSelectorArg.Decoder.Decode(reader);
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
