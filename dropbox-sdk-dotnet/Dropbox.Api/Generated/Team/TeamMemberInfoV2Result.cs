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
    /// <para>Information about a team member, after the change, like at <see
    /// cref="Dropbox.Api.Team.Routes.TeamTeamRoutes.MembersSetProfileV2Async" />.</para>
    /// </summary>
    public class TeamMemberInfoV2Result
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<TeamMemberInfoV2Result> Encoder = new TeamMemberInfoV2ResultEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<TeamMemberInfoV2Result> Decoder = new TeamMemberInfoV2ResultDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="TeamMemberInfoV2Result" />
        /// class.</para>
        /// </summary>
        /// <param name="memberInfo">Member info, after the change.</param>
        public TeamMemberInfoV2Result(TeamMemberInfoV2 memberInfo)
        {
            if (memberInfo == null)
            {
                throw new sys.ArgumentNullException("memberInfo");
            }

            this.MemberInfo = memberInfo;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="TeamMemberInfoV2Result" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public TeamMemberInfoV2Result()
        {
        }

        /// <summary>
        /// <para>Member info, after the change.</para>
        /// </summary>
        public TeamMemberInfoV2 MemberInfo { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="TeamMemberInfoV2Result" />.</para>
        /// </summary>
        private class TeamMemberInfoV2ResultEncoder : enc.StructEncoder<TeamMemberInfoV2Result>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(TeamMemberInfoV2Result value, enc.IJsonWriter writer)
            {
                WriteProperty("member_info", value.MemberInfo, writer, global::Dropbox.Api.Team.TeamMemberInfoV2.Encoder);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="TeamMemberInfoV2Result" />.</para>
        /// </summary>
        private class TeamMemberInfoV2ResultDecoder : enc.StructDecoder<TeamMemberInfoV2Result>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="TeamMemberInfoV2Result"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override TeamMemberInfoV2Result Create()
            {
                return new TeamMemberInfoV2Result();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(TeamMemberInfoV2Result value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "member_info":
                        value.MemberInfo = global::Dropbox.Api.Team.TeamMemberInfoV2.Decoder.Decode(reader);
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
