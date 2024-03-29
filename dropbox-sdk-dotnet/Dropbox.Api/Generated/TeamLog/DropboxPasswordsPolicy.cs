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
    /// <para>Policy for deciding whether team users can use Dropbox Passwords</para>
    /// </summary>
    public class DropboxPasswordsPolicy
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<DropboxPasswordsPolicy> Encoder = new DropboxPasswordsPolicyEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<DropboxPasswordsPolicy> Decoder = new DropboxPasswordsPolicyDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="DropboxPasswordsPolicy" />
        /// class.</para>
        /// </summary>
        public DropboxPasswordsPolicy()
        {
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is Default</para>
        /// </summary>
        public bool IsDefault
        {
            get
            {
                return this is Default;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a Default, or <c>null</c>.</para>
        /// </summary>
        public Default AsDefault
        {
            get
            {
                return this as Default;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is Disabled</para>
        /// </summary>
        public bool IsDisabled
        {
            get
            {
                return this is Disabled;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a Disabled, or <c>null</c>.</para>
        /// </summary>
        public Disabled AsDisabled
        {
            get
            {
                return this as Disabled;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is Enabled</para>
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return this is Enabled;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a Enabled, or <c>null</c>.</para>
        /// </summary>
        public Enabled AsEnabled
        {
            get
            {
                return this as Enabled;
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
        /// <para>Encoder for  <see cref="DropboxPasswordsPolicy" />.</para>
        /// </summary>
        private class DropboxPasswordsPolicyEncoder : enc.StructEncoder<DropboxPasswordsPolicy>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(DropboxPasswordsPolicy value, enc.IJsonWriter writer)
            {
                if (value is Default)
                {
                    WriteProperty(".tag", "default", writer, enc.StringEncoder.Instance);
                    Default.Encoder.EncodeFields((Default)value, writer);
                    return;
                }
                if (value is Disabled)
                {
                    WriteProperty(".tag", "disabled", writer, enc.StringEncoder.Instance);
                    Disabled.Encoder.EncodeFields((Disabled)value, writer);
                    return;
                }
                if (value is Enabled)
                {
                    WriteProperty(".tag", "enabled", writer, enc.StringEncoder.Instance);
                    Enabled.Encoder.EncodeFields((Enabled)value, writer);
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
        /// <para>Decoder for  <see cref="DropboxPasswordsPolicy" />.</para>
        /// </summary>
        private class DropboxPasswordsPolicyDecoder : enc.UnionDecoder<DropboxPasswordsPolicy>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="DropboxPasswordsPolicy"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override DropboxPasswordsPolicy Create()
            {
                return new DropboxPasswordsPolicy();
            }

            /// <summary>
            /// <para>Decode based on given tag.</para>
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="reader">The json reader.</param>
            /// <returns>The decoded object.</returns>
            protected override DropboxPasswordsPolicy Decode(string tag, enc.IJsonReader reader)
            {
                switch (tag)
                {
                    case "default":
                        return Default.Decoder.DecodeFields(reader);
                    case "disabled":
                        return Disabled.Decoder.DecodeFields(reader);
                    case "enabled":
                        return Enabled.Decoder.DecodeFields(reader);
                    default:
                        return Other.Decoder.DecodeFields(reader);
                }
            }
        }

        #endregion

        /// <summary>
        /// <para>The default object</para>
        /// </summary>
        public sealed class Default : DropboxPasswordsPolicy
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<Default> Encoder = new DefaultEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<Default> Decoder = new DefaultDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="Default" /> class.</para>
            /// </summary>
            private Default()
            {
            }

            /// <summary>
            /// <para>A singleton instance of Default</para>
            /// </summary>
            public static readonly Default Instance = new Default();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="Default" />.</para>
            /// </summary>
            private class DefaultEncoder : enc.StructEncoder<Default>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(Default value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="Default" />.</para>
            /// </summary>
            private class DefaultDecoder : enc.StructDecoder<Default>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="Default" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override Default Create()
                {
                    return Default.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The disabled object</para>
        /// </summary>
        public sealed class Disabled : DropboxPasswordsPolicy
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<Disabled> Encoder = new DisabledEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<Disabled> Decoder = new DisabledDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="Disabled" /> class.</para>
            /// </summary>
            private Disabled()
            {
            }

            /// <summary>
            /// <para>A singleton instance of Disabled</para>
            /// </summary>
            public static readonly Disabled Instance = new Disabled();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="Disabled" />.</para>
            /// </summary>
            private class DisabledEncoder : enc.StructEncoder<Disabled>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(Disabled value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="Disabled" />.</para>
            /// </summary>
            private class DisabledDecoder : enc.StructDecoder<Disabled>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="Disabled" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override Disabled Create()
                {
                    return Disabled.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The enabled object</para>
        /// </summary>
        public sealed class Enabled : DropboxPasswordsPolicy
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<Enabled> Encoder = new EnabledEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<Enabled> Decoder = new EnabledDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="Enabled" /> class.</para>
            /// </summary>
            private Enabled()
            {
            }

            /// <summary>
            /// <para>A singleton instance of Enabled</para>
            /// </summary>
            public static readonly Enabled Instance = new Enabled();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="Enabled" />.</para>
            /// </summary>
            private class EnabledEncoder : enc.StructEncoder<Enabled>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(Enabled value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="Enabled" />.</para>
            /// </summary>
            private class EnabledDecoder : enc.StructDecoder<Enabled>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="Enabled" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override Enabled Create()
                {
                    return Enabled.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The other object</para>
        /// </summary>
        public sealed class Other : DropboxPasswordsPolicy
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
