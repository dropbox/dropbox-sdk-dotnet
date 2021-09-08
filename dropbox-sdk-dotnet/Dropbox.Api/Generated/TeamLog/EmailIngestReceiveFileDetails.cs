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
    /// <para>Received files via Email to my Dropbox.</para>
    /// </summary>
    public class EmailIngestReceiveFileDetails
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<EmailIngestReceiveFileDetails> Encoder = new EmailIngestReceiveFileDetailsEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<EmailIngestReceiveFileDetails> Decoder = new EmailIngestReceiveFileDetailsDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="EmailIngestReceiveFileDetails"
        /// /> class.</para>
        /// </summary>
        /// <param name="inboxName">Inbox name.</param>
        /// <param name="attachmentNames">Submitted file names.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="fromName">The name as provided by the submitter.</param>
        /// <param name="fromEmail">The email as provided by the submitter.</param>
        public EmailIngestReceiveFileDetails(string inboxName,
                                             col.IEnumerable<string> attachmentNames,
                                             string subject = null,
                                             string fromName = null,
                                             string fromEmail = null)
        {
            if (inboxName == null)
            {
                throw new sys.ArgumentNullException("inboxName");
            }

            var attachmentNamesList = enc.Util.ToList(attachmentNames);

            if (attachmentNames == null)
            {
                throw new sys.ArgumentNullException("attachmentNames");
            }

            if (fromEmail != null)
            {
                if (fromEmail.Length > 255)
                {
                    throw new sys.ArgumentOutOfRangeException("fromEmail", "Length should be at most 255");
                }
            }

            this.InboxName = inboxName;
            this.AttachmentNames = attachmentNamesList;
            this.Subject = subject;
            this.FromName = fromName;
            this.FromEmail = fromEmail;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="EmailIngestReceiveFileDetails"
        /// /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public EmailIngestReceiveFileDetails()
        {
        }

        /// <summary>
        /// <para>Inbox name.</para>
        /// </summary>
        public string InboxName { get; protected set; }

        /// <summary>
        /// <para>Submitted file names.</para>
        /// </summary>
        public col.IList<string> AttachmentNames { get; protected set; }

        /// <summary>
        /// <para>Subject of the email.</para>
        /// </summary>
        public string Subject { get; protected set; }

        /// <summary>
        /// <para>The name as provided by the submitter.</para>
        /// </summary>
        public string FromName { get; protected set; }

        /// <summary>
        /// <para>The email as provided by the submitter.</para>
        /// </summary>
        public string FromEmail { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="EmailIngestReceiveFileDetails" />.</para>
        /// </summary>
        private class EmailIngestReceiveFileDetailsEncoder : enc.StructEncoder<EmailIngestReceiveFileDetails>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(EmailIngestReceiveFileDetails value, enc.IJsonWriter writer)
            {
                WriteProperty("inbox_name", value.InboxName, writer, enc.StringEncoder.Instance);
                WriteListProperty("attachment_names", value.AttachmentNames, writer, enc.StringEncoder.Instance);
                if (value.Subject != null)
                {
                    WriteProperty("subject", value.Subject, writer, enc.StringEncoder.Instance);
                }
                if (value.FromName != null)
                {
                    WriteProperty("from_name", value.FromName, writer, enc.StringEncoder.Instance);
                }
                if (value.FromEmail != null)
                {
                    WriteProperty("from_email", value.FromEmail, writer, enc.StringEncoder.Instance);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="EmailIngestReceiveFileDetails" />.</para>
        /// </summary>
        private class EmailIngestReceiveFileDetailsDecoder : enc.StructDecoder<EmailIngestReceiveFileDetails>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="EmailIngestReceiveFileDetails"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override EmailIngestReceiveFileDetails Create()
            {
                return new EmailIngestReceiveFileDetails();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(EmailIngestReceiveFileDetails value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "inbox_name":
                        value.InboxName = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "attachment_names":
                        value.AttachmentNames = ReadList<string>(reader, enc.StringDecoder.Instance);
                        break;
                    case "subject":
                        value.Subject = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "from_name":
                        value.FromName = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "from_email":
                        value.FromEmail = enc.StringDecoder.Instance.Decode(reader);
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
