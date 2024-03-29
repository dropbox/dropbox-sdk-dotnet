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
    /// <para>Policy for controlling if team members can share links externally</para>
    /// </summary>
    public class SharingLinkPolicy
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<SharingLinkPolicy> Encoder = new SharingLinkPolicyEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<SharingLinkPolicy> Decoder = new SharingLinkPolicyDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="SharingLinkPolicy" />
        /// class.</para>
        /// </summary>
        public SharingLinkPolicy()
        {
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is DefaultNoOne</para>
        /// </summary>
        public bool IsDefaultNoOne
        {
            get
            {
                return this is DefaultNoOne;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a DefaultNoOne, or <c>null</c>.</para>
        /// </summary>
        public DefaultNoOne AsDefaultNoOne
        {
            get
            {
                return this as DefaultNoOne;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is DefaultPrivate</para>
        /// </summary>
        public bool IsDefaultPrivate
        {
            get
            {
                return this is DefaultPrivate;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a DefaultPrivate, or <c>null</c>.</para>
        /// </summary>
        public DefaultPrivate AsDefaultPrivate
        {
            get
            {
                return this as DefaultPrivate;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is DefaultPublic</para>
        /// </summary>
        public bool IsDefaultPublic
        {
            get
            {
                return this is DefaultPublic;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a DefaultPublic, or <c>null</c>.</para>
        /// </summary>
        public DefaultPublic AsDefaultPublic
        {
            get
            {
                return this as DefaultPublic;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is OnlyPrivate</para>
        /// </summary>
        public bool IsOnlyPrivate
        {
            get
            {
                return this is OnlyPrivate;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a OnlyPrivate, or <c>null</c>.</para>
        /// </summary>
        public OnlyPrivate AsOnlyPrivate
        {
            get
            {
                return this as OnlyPrivate;
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
        /// <para>Encoder for  <see cref="SharingLinkPolicy" />.</para>
        /// </summary>
        private class SharingLinkPolicyEncoder : enc.StructEncoder<SharingLinkPolicy>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(SharingLinkPolicy value, enc.IJsonWriter writer)
            {
                if (value is DefaultNoOne)
                {
                    WriteProperty(".tag", "default_no_one", writer, enc.StringEncoder.Instance);
                    DefaultNoOne.Encoder.EncodeFields((DefaultNoOne)value, writer);
                    return;
                }
                if (value is DefaultPrivate)
                {
                    WriteProperty(".tag", "default_private", writer, enc.StringEncoder.Instance);
                    DefaultPrivate.Encoder.EncodeFields((DefaultPrivate)value, writer);
                    return;
                }
                if (value is DefaultPublic)
                {
                    WriteProperty(".tag", "default_public", writer, enc.StringEncoder.Instance);
                    DefaultPublic.Encoder.EncodeFields((DefaultPublic)value, writer);
                    return;
                }
                if (value is OnlyPrivate)
                {
                    WriteProperty(".tag", "only_private", writer, enc.StringEncoder.Instance);
                    OnlyPrivate.Encoder.EncodeFields((OnlyPrivate)value, writer);
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
        /// <para>Decoder for  <see cref="SharingLinkPolicy" />.</para>
        /// </summary>
        private class SharingLinkPolicyDecoder : enc.UnionDecoder<SharingLinkPolicy>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="SharingLinkPolicy" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override SharingLinkPolicy Create()
            {
                return new SharingLinkPolicy();
            }

            /// <summary>
            /// <para>Decode based on given tag.</para>
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="reader">The json reader.</param>
            /// <returns>The decoded object.</returns>
            protected override SharingLinkPolicy Decode(string tag, enc.IJsonReader reader)
            {
                switch (tag)
                {
                    case "default_no_one":
                        return DefaultNoOne.Decoder.DecodeFields(reader);
                    case "default_private":
                        return DefaultPrivate.Decoder.DecodeFields(reader);
                    case "default_public":
                        return DefaultPublic.Decoder.DecodeFields(reader);
                    case "only_private":
                        return OnlyPrivate.Decoder.DecodeFields(reader);
                    default:
                        return Other.Decoder.DecodeFields(reader);
                }
            }
        }

        #endregion

        /// <summary>
        /// <para>The default no one object</para>
        /// </summary>
        public sealed class DefaultNoOne : SharingLinkPolicy
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<DefaultNoOne> Encoder = new DefaultNoOneEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<DefaultNoOne> Decoder = new DefaultNoOneDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="DefaultNoOne" />
            /// class.</para>
            /// </summary>
            private DefaultNoOne()
            {
            }

            /// <summary>
            /// <para>A singleton instance of DefaultNoOne</para>
            /// </summary>
            public static readonly DefaultNoOne Instance = new DefaultNoOne();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="DefaultNoOne" />.</para>
            /// </summary>
            private class DefaultNoOneEncoder : enc.StructEncoder<DefaultNoOne>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(DefaultNoOne value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="DefaultNoOne" />.</para>
            /// </summary>
            private class DefaultNoOneDecoder : enc.StructDecoder<DefaultNoOne>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="DefaultNoOne" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override DefaultNoOne Create()
                {
                    return DefaultNoOne.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The default private object</para>
        /// </summary>
        public sealed class DefaultPrivate : SharingLinkPolicy
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<DefaultPrivate> Encoder = new DefaultPrivateEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<DefaultPrivate> Decoder = new DefaultPrivateDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="DefaultPrivate" />
            /// class.</para>
            /// </summary>
            private DefaultPrivate()
            {
            }

            /// <summary>
            /// <para>A singleton instance of DefaultPrivate</para>
            /// </summary>
            public static readonly DefaultPrivate Instance = new DefaultPrivate();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="DefaultPrivate" />.</para>
            /// </summary>
            private class DefaultPrivateEncoder : enc.StructEncoder<DefaultPrivate>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(DefaultPrivate value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="DefaultPrivate" />.</para>
            /// </summary>
            private class DefaultPrivateDecoder : enc.StructDecoder<DefaultPrivate>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="DefaultPrivate" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override DefaultPrivate Create()
                {
                    return DefaultPrivate.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The default public object</para>
        /// </summary>
        public sealed class DefaultPublic : SharingLinkPolicy
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<DefaultPublic> Encoder = new DefaultPublicEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<DefaultPublic> Decoder = new DefaultPublicDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="DefaultPublic" />
            /// class.</para>
            /// </summary>
            private DefaultPublic()
            {
            }

            /// <summary>
            /// <para>A singleton instance of DefaultPublic</para>
            /// </summary>
            public static readonly DefaultPublic Instance = new DefaultPublic();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="DefaultPublic" />.</para>
            /// </summary>
            private class DefaultPublicEncoder : enc.StructEncoder<DefaultPublic>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(DefaultPublic value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="DefaultPublic" />.</para>
            /// </summary>
            private class DefaultPublicDecoder : enc.StructDecoder<DefaultPublic>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="DefaultPublic" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override DefaultPublic Create()
                {
                    return DefaultPublic.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The only private object</para>
        /// </summary>
        public sealed class OnlyPrivate : SharingLinkPolicy
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<OnlyPrivate> Encoder = new OnlyPrivateEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<OnlyPrivate> Decoder = new OnlyPrivateDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="OnlyPrivate" />
            /// class.</para>
            /// </summary>
            private OnlyPrivate()
            {
            }

            /// <summary>
            /// <para>A singleton instance of OnlyPrivate</para>
            /// </summary>
            public static readonly OnlyPrivate Instance = new OnlyPrivate();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="OnlyPrivate" />.</para>
            /// </summary>
            private class OnlyPrivateEncoder : enc.StructEncoder<OnlyPrivate>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(OnlyPrivate value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="OnlyPrivate" />.</para>
            /// </summary>
            private class OnlyPrivateDecoder : enc.StructDecoder<OnlyPrivate>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="OnlyPrivate" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override OnlyPrivate Create()
                {
                    return OnlyPrivate.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The other object</para>
        /// </summary>
        public sealed class Other : SharingLinkPolicy
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
