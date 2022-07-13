// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Openid
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>The auth error object</para>
    /// </summary>
    public class AuthError
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<AuthError> Encoder = new AuthErrorEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<AuthError> Decoder = new AuthErrorDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="AuthError" /> class.</para>
        /// </summary>
        public AuthError()
        {
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is InvalidToken</para>
        /// </summary>
        public bool IsInvalidToken
        {
            get
            {
                return this is InvalidToken;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a InvalidToken, or <c>null</c>.</para>
        /// </summary>
        public InvalidToken AsInvalidToken
        {
            get
            {
                return this as InvalidToken;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is NoOpenidAuth</para>
        /// </summary>
        public bool IsNoOpenidAuth
        {
            get
            {
                return this is NoOpenidAuth;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a NoOpenidAuth, or <c>null</c>.</para>
        /// </summary>
        public NoOpenidAuth AsNoOpenidAuth
        {
            get
            {
                return this as NoOpenidAuth;
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
        /// <para>Encoder for  <see cref="AuthError" />.</para>
        /// </summary>
        private class AuthErrorEncoder : enc.StructEncoder<AuthError>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(AuthError value, enc.IJsonWriter writer)
            {
                if (value is InvalidToken)
                {
                    WriteProperty(".tag", "invalid_token", writer, enc.StringEncoder.Instance);
                    InvalidToken.Encoder.EncodeFields((InvalidToken)value, writer);
                    return;
                }
                if (value is NoOpenidAuth)
                {
                    WriteProperty(".tag", "no_openid_auth", writer, enc.StringEncoder.Instance);
                    NoOpenidAuth.Encoder.EncodeFields((NoOpenidAuth)value, writer);
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
        /// <para>Decoder for  <see cref="AuthError" />.</para>
        /// </summary>
        private class AuthErrorDecoder : enc.UnionDecoder<AuthError>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="AuthError" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override AuthError Create()
            {
                return new AuthError();
            }

            /// <summary>
            /// <para>Decode based on given tag.</para>
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="reader">The json reader.</param>
            /// <returns>The decoded object.</returns>
            protected override AuthError Decode(string tag, enc.IJsonReader reader)
            {
                switch (tag)
                {
                    case "invalid_token":
                        return InvalidToken.Decoder.DecodeFields(reader);
                    case "no_openid_auth":
                        return NoOpenidAuth.Decoder.DecodeFields(reader);
                    default:
                        return Other.Decoder.DecodeFields(reader);
                }
            }
        }

        #endregion

        /// <summary>
        /// <para>The invalid token object</para>
        /// </summary>
        public sealed class InvalidToken : AuthError
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<InvalidToken> Encoder = new InvalidTokenEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<InvalidToken> Decoder = new InvalidTokenDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="InvalidToken" />
            /// class.</para>
            /// </summary>
            private InvalidToken()
            {
            }

            /// <summary>
            /// <para>A singleton instance of InvalidToken</para>
            /// </summary>
            public static readonly InvalidToken Instance = new InvalidToken();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="InvalidToken" />.</para>
            /// </summary>
            private class InvalidTokenEncoder : enc.StructEncoder<InvalidToken>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(InvalidToken value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="InvalidToken" />.</para>
            /// </summary>
            private class InvalidTokenDecoder : enc.StructDecoder<InvalidToken>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="InvalidToken" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override InvalidToken Create()
                {
                    return InvalidToken.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The no openid auth object</para>
        /// </summary>
        public sealed class NoOpenidAuth : AuthError
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<NoOpenidAuth> Encoder = new NoOpenidAuthEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<NoOpenidAuth> Decoder = new NoOpenidAuthDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="NoOpenidAuth" />
            /// class.</para>
            /// </summary>
            private NoOpenidAuth()
            {
            }

            /// <summary>
            /// <para>A singleton instance of NoOpenidAuth</para>
            /// </summary>
            public static readonly NoOpenidAuth Instance = new NoOpenidAuth();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="NoOpenidAuth" />.</para>
            /// </summary>
            private class NoOpenidAuthEncoder : enc.StructEncoder<NoOpenidAuth>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(NoOpenidAuth value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="NoOpenidAuth" />.</para>
            /// </summary>
            private class NoOpenidAuthDecoder : enc.StructDecoder<NoOpenidAuth>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="NoOpenidAuth" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override NoOpenidAuth Create()
                {
                    return NoOpenidAuth.Instance;
                }

            }

            #endregion
        }

        /// <summary>
        /// <para>The other object</para>
        /// </summary>
        public sealed class Other : AuthError
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
