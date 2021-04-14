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
    /// <para>Exactly one of team_member_id, email, or external_id must be provided to identify
    /// the user account.</para>
    /// </summary>
    public class MembersSetPermissions2Arg
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<MembersSetPermissions2Arg> Encoder = new MembersSetPermissions2ArgEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<MembersSetPermissions2Arg> Decoder = new MembersSetPermissions2ArgDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="MembersSetPermissions2Arg" />
        /// class.</para>
        /// </summary>
        /// <param name="user">Identity of user whose role will be set.</param>
        /// <param name="newRoles">The new roles for the member. Send empty list to make user
        /// member only. For now, only up to one role is allowed.</param>
        public MembersSetPermissions2Arg(UserSelectorArg user,
                                         col.IEnumerable<string> newRoles = null)
        {
            if (user == null)
            {
                throw new sys.ArgumentNullException("user");
            }

            var newRolesList = enc.Util.ToList(newRoles);

            if (newRoles != null)
            {
                if (newRolesList.Count > 1)
                {
                    throw new sys.ArgumentOutOfRangeException("newRoles", "List should at at most 1 items");
                }
            }

            this.User = user;
            this.NewRoles = newRolesList;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="MembersSetPermissions2Arg" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public MembersSetPermissions2Arg()
        {
        }

        /// <summary>
        /// <para>Identity of user whose role will be set.</para>
        /// </summary>
        public UserSelectorArg User { get; protected set; }

        /// <summary>
        /// <para>The new roles for the member. Send empty list to make user member only. For
        /// now, only up to one role is allowed.</para>
        /// </summary>
        public col.IList<string> NewRoles { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="MembersSetPermissions2Arg" />.</para>
        /// </summary>
        private class MembersSetPermissions2ArgEncoder : enc.StructEncoder<MembersSetPermissions2Arg>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(MembersSetPermissions2Arg value, enc.IJsonWriter writer)
            {
                WriteProperty("user", value.User, writer, global::Dropbox.Api.Team.UserSelectorArg.Encoder);
                if (value.NewRoles.Count > 0)
                {
                    WriteListProperty("new_roles", value.NewRoles, writer, enc.StringEncoder.Instance);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="MembersSetPermissions2Arg" />.</para>
        /// </summary>
        private class MembersSetPermissions2ArgDecoder : enc.StructDecoder<MembersSetPermissions2Arg>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="MembersSetPermissions2Arg"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override MembersSetPermissions2Arg Create()
            {
                return new MembersSetPermissions2Arg();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(MembersSetPermissions2Arg value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "user":
                        value.User = global::Dropbox.Api.Team.UserSelectorArg.Decoder.Decode(reader);
                        break;
                    case "new_roles":
                        value.NewRoles = ReadList<string>(reader, enc.StringDecoder.Instance);
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
