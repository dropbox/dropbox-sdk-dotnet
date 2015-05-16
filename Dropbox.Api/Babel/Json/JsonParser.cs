//----------------------------------------------------------------------------
//  <copyright file="JsonParser.cs" company="Dropbox Inc">
//      Copyright (c) Dropbox Inc. All rights reserved.
//  </copyright>
//----------------------------------------------------------------------------

namespace Dropbox.Api.Babel.Json
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>Parses serialized JSON.</summary>
    internal sealed class JsonParser
    {
        /// <summary>Regex definition for the punctuation in serialized JSON.</summary>
        private const string Puctuation = @"(?<punctuation>[{}:,\[\]])";

        /// <summary>Regex pattern for a JSON boolean.</summary>
        private const string Boolean = @"(?<boolean>true|false)";

        /// <summary>Regex pattern for a JSON null.</summary>
        private const string Null = @"(?<null>null)";

        /// <summary>Regex pattern for a JSON number.</summary>
        private const string Number = @"(?<number>-?(0|([1-9][0-9]*))(\.[0-9]+)?([eE][\-+]?[0-9]+)?)";

        /// <summary>Regex pattern for a JSON string.</summary>
        private const string String = @"(?<string>""((\\.)|[^""])*"")";

        /// <summary>Regex pattern for JSON whitespace.</summary>
        private const string Whitespace = @"(?<whitespace>\s+)";

        /// <summary>Catch all regex pattern used to find errors in the serialized value.</summary>
        private const string Unknown = @"(?<unknown>.)";

        /// <summary>The regex used to tokenize the JSON data.</summary>
        private static readonly Regex ParseRegex = new Regex(
            Puctuation + "|" + Boolean + "|" + Null + "|" + Number + "|" + String + "|" + Whitespace + "|" + Unknown,
            RegexOptions.Singleline);

        /// <summary>The token stream of the tokenized JSON.</summary>
        private IEnumerator<object> tokens;

        /// <summary>Indicates whether <see cref="PeekToken"/> has been called 
        /// since the last call to <see cref="NextToken"/>.</summary>
        private bool isPeeking;

        /// <summary>Initializes a new instance of the <see cref="JsonParser"/> class.</summary>
        /// <param name="tokens">The token stream to parse from.</param>
        private JsonParser(IEnumerable<object> tokens)
        {
            this.tokens = tokens.GetEnumerator();
        }

        /// <summary>The punctuation types used in JSON</summary>
        private enum Punctuation
        {
            /// <summary>Undefined value.</summary>
            Unknown,

            /// <summary>The start of an object, open curly brackets.</summary>
            StartObject,

            /// <summary>The end of an object, close curly brackets.</summary>
            EndObject,

            /// <summary>The separator between a name and value, colon.</summary>
            NameValueSeparator,

            /// <summary>The separator between object and array members, comma.</summary>
            ElementSeparator,

            /// <summary>The start of an array, open square brackets.</summary>
            StartArray,

            /// <summary>The start of an array, close square brackets.</summary>
            EndArray
        }

        /// <summary>Parse a JSON value from a string.</summary>
        /// <param name="input">The input string from which to parse.</param>
        /// <returns>A parsed JSON value.</returns>
        public static object Parse(string input)
        {
            var parser = new JsonParser(Tokenize(input));

            var parsed = parser.ParseValue();
            if (!parser.AtEnd())
            {
                throw new SerializationException("Unexpected data at end of JSON");
            }

            return parsed;
        }

        /// <summary>Tokenize a string into valid JSON tokens.</summary>
        /// <param name="input">The raw JSON string.</param>
        /// <returns>A stream of tokens representing the JSON.</returns>
        private static IEnumerable<object> Tokenize(string input)
        {
            for (var match = ParseRegex.Match(input); match.Success; match = match.NextMatch())
            {
                if (match.Groups["punctuation"].Success)
                {
                    switch (match.Value)
                    {
                        case "{":
                            yield return Punctuation.StartObject;
                            break;
                        case "}":
                            yield return Punctuation.EndObject;
                            break;
                        case ":":
                            yield return Punctuation.NameValueSeparator;
                            break;
                        case ",":
                            yield return Punctuation.ElementSeparator;
                            break;
                        case "[":
                            yield return Punctuation.StartArray;
                            break;
                        case "]":
                            yield return Punctuation.EndArray;
                            break;
                    }
                }
                else if (match.Groups["boolean"].Success)
                {
                    yield return match.Value == "true";
                }
                else if (match.Groups["null"].Success)
                {
                    yield return null;
                }
                else if (match.Groups["number"].Success)
                {
                    if (match.Value.Contains(".") || match.Value.Contains("e") || match.Value.Contains("E"))
                    {
                        yield return double.Parse(match.Value, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        yield return long.Parse(match.Value, CultureInfo.InvariantCulture);
                    }
                }
                else if (match.Groups["string"].Success)
                {
                    yield return Unescape(match.Value.Substring(1, match.Value.Length - 2));
                }
                else if (match.Groups["whitespace"].Success)
                {
                    // ignore whitespace
                }
                else if (match.Groups["unknown"].Success)
                {
                    throw new SerializationException("Error parsing JSON, found unexpected character: " + match.Value);
                }
            }
        }

        /// <summary>Remove JSON escape sequences from a string.</summary>
        /// <param name="input">The string to test.</param>
        /// <returns>The unescaped string.</returns>
        private static string Unescape(string input)
        {
            if (!input.Contains("\\"))
            {
                return input;
            }

            var builder = new StringBuilder();
            var escaped = false;

            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];

                if (escaped)
                {
                    switch (c)
                    {
                        case 'b':
                            builder.Append('\b');
                            break;
                        case 'f':
                            builder.Append('\f');
                            break;
                        case 'n':
                            builder.Append('\n');
                            break;
                        case 'r':
                            builder.Append('\r');
                            break;
                        case 't':
                            builder.Append('\t');
                            break;
                        case 'u':
                            if (i + 4 >= input.Length)
                            {
                                throw new SerializationException("JSON string is too short for unicode escape sequence");
                            }
                        
                            ushort utf16;
                            if (!ushort.TryParse(input.Substring(i + 1, 4), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out utf16))
                            {
                                throw new SerializationException("JSON string has malformed unicode escape sequence");
                            }

                            builder.Append((char)utf16);
                            i += 4;
                            break;
                        default:
                            builder.Append(c);
                            break;
                    }

                    escaped = false;
                }
                else if (c == '\\')
                {
                    escaped = true;
                }
                else
                {
                    builder.Append(c);
                }
            }

            if (escaped)
            {
                throw new SerializationException("JSON string is terminated within an escape sequence");
            }

            return builder.ToString();
        }

        /// <summary>Parsed the next JSON value from the token stream.</summary>
        /// <returns>The next JSON value.</returns>
        private object ParseValue()
        {
            var token = this.NextToken();

            if (token is Punctuation)
            {
                var p = (Punctuation)token;
                if (p == Punctuation.StartObject)
                {
                    return this.ParseObject();
                }
                else if (p == Punctuation.StartArray)
                {
                    return this.ParseArray();
                }
                else
                {
                    throw new SerializationException("JSON is malformed, expecting '{' or '['");
                }
            }

            return token;
        }

        /// <summary>Parsed a <see cref="JsonObject"/> from the token stream.</summary>
        /// <returns>The parsed <see cref="JsonObject"/>.</returns>
        private JsonObject ParseObject()
        {
            var obj = new JsonObject();

            var next = this.PeekToken();
            if (next is Punctuation && (Punctuation)next == Punctuation.EndObject)
            {
                // empty object;
                this.NextToken();
                return obj;
            }

            while (true)
            {
                var name = this.NextToken() as string;
                if (name == null)
                {
                    throw new SerializationException("JSON is malformed, expecting a string");
                }

                var sep = this.NextToken();
                if (!(sep is Punctuation) || (Punctuation)sep != Punctuation.NameValueSeparator)
                {
                    throw new SerializationException("JSON is malformed, expecting ':'");
                }

                obj[name] = this.ParseValue();

                var nextSep = this.NextToken();
                if (nextSep is Punctuation)
                {
                    var p = (Punctuation)nextSep;
                    if (p == Punctuation.EndObject)
                    {
                        return obj;
                    }
                    else if (p == Punctuation.ElementSeparator)
                    {
                        continue;
                    }
                }

                throw new SerializationException("JSON is malformed, expecting ',' or '}'");
            }
        }

        /// <summary>Parse a <see cref="JsonArray"/> from the token stream.</summary>
        /// <returns>The parsed <see cref="JsonArray"/>.</returns>
        private JsonArray ParseArray()
        {
            var array = new JsonArray();

            var next = this.PeekToken();
            if (next is Punctuation && (Punctuation)next == Punctuation.EndArray)
            {
                // this is an empty array.
                this.NextToken();
                return array;
            }

            while (true)
            {
                array.Add(this.ParseValue());

                var nextSep = this.NextToken();
                if (nextSep is Punctuation)
                {
                    var p = (Punctuation)nextSep;
                    if (p == Punctuation.EndArray)
                    {
                        return array;
                    }
                    else if (p == Punctuation.ElementSeparator)
                    {
                        continue;
                    }
                }

                throw new SerializationException("JSON is malformed, expecting ',' or ']'");
            }
        }

        /// <summary>Checks if the parser is at the end of the token stream.</summary>
        /// <returns><c>true</c> if there are no more tokens; <c>false</c> otherwise.</returns>
        private bool AtEnd()
        {
            if (this.isPeeking)
            {
                return false;
            }
            else if (this.tokens.MoveNext())
            {
                this.isPeeking = true;
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>Gets the next token from the stream.</summary>
        /// <returns>The next token.</returns>
        private object NextToken()
        {
            if (this.isPeeking)
            {
                this.isPeeking = false;
                return this.tokens.Current;
            }
            else if (this.tokens.MoveNext())
            {
                return this.tokens.Current;
            }
            else
            {
                throw new SerializationException("JSON appears to be truncated");
            }
        }

        /// <summary>Gets the next token from the stream without removing it from the stream.</summary>
        /// <returns>The next token.</returns>
        private object PeekToken()
        {
            if (!this.isPeeking)
            {
                if (!this.tokens.MoveNext())
                {
                    throw new SerializationException("JSON appears to be truncated");
                }

                this.isPeeking = true;
            }

            return this.tokens.Current;
        }
    }
}
