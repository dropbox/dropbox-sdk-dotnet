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
    /// <para>The members get info v2 arg object</para>
    /// </summary>
    public class MembersGetInfoV2Arg
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<MembersGetInfoV2Arg> Encoder = new MembersGetInfoV2ArgEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<MembersGetInfoV2Arg> Decoder = new MembersGetInfoV2ArgDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="MembersGetInfoV2Arg" />
        /// class.</para>
        /// </summary>
        /// <param name="members">List of team members.</param>
        public MembersGetInfoV2Arg(col.IEnumerable<UserSelectorArg> members)
        {
            var membersList = enc.Util.ToList(members);

            if (members == null)
            {
                throw new sys.ArgumentNullException("members");
            }

            this.Members = membersList;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="MembersGetInfoV2Arg" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public MembersGetInfoV2Arg()
        {
        }

        /// <summary>
        /// <para>List of team members.</para>
        /// </summary>
        public col.IList<UserSelectorArg> Members { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="MembersGetInfoV2Arg" />.</para>
        /// </summary>
        private class MembersGetInfoV2ArgEncoder : enc.StructEncoder<MembersGetInfoV2Arg>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(MembersGetInfoV2Arg value, enc.IJsonWriter writer)
            {
                WriteListProperty("members", value.Members, writer, global::Dropbox.Api.Team.UserSelectorArg.Encoder);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="MembersGetInfoV2Arg" />.</para>
        /// </summary>
        private class MembersGetInfoV2ArgDecoder : enc.StructDecoder<MembersGetInfoV2Arg>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="MembersGetInfoV2Arg" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override MembersGetInfoV2Arg Create()
            {
                return new MembersGetInfoV2Arg();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(MembersGetInfoV2Arg value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "members":
                        value.Members = ReadList<UserSelectorArg>(reader, global::Dropbox.Api.Team.UserSelectorArg.Decoder);
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
