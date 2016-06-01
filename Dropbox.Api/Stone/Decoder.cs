//-----------------------------------------------------------------------------
// <copyright file="Decoder.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Stone
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// The factory class for decoders.
    /// </summary>
    internal static class Decoder
    {
        /// <summary>
        /// Create a instance of the <see cref="ListDecoder{T}"/> class.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="itemDecoder">The item decoder.</param>
        /// <returns>The list decoder.</returns>
        public static IDecoder<List<T>> CreateListDecoder<T>(IDecoder<T> itemDecoder)
        {
            return new ListDecoder<T>(itemDecoder);
        }
    }

    /// <summary>
    /// Decoder for nullable struct.
    /// </summary>
    /// <typeparam name="T">Type of the struct.</typeparam>
    internal sealed class NullableDecoder<T> : IDecoder<T?> where T : struct
    {
        /// <summary>
        /// The decoder.
        /// </summary>
        private readonly IDecoder<T> decoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableDecoder{T}"/> class.
        /// </summary>
        /// <param name="decoder">The decoder.</param>
        public NullableDecoder(IDecoder<T> decoder)
        {
            this.decoder = decoder;
        }

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The value.</returns>
        public T? Decode(IJsonReader reader)
        {
            if (reader.IsNull)
            {
                reader.Read();
                return null;
            }

            return this.decoder.Decode(reader);
        }
    }

    /// <summary>
    /// Decoder for Int32.
    /// </summary>
    internal sealed class Int32Decoder : IDecoder<int>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IDecoder<int> Instance = new Int32Decoder();

        /// <summary>
        /// The instance for nullable.
        /// </summary>
        public static readonly IDecoder<int?> NullableInstance = new NullableDecoder<int>(Instance);

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The value.</returns>
        public int Decode(IJsonReader reader)
        {
            return reader.ReadInt32();
        }
    }

    /// <summary>
    /// Decoder for Int64.
    /// </summary>
    internal sealed class Int64Decoder : IDecoder<long>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IDecoder<long> Instance = new Int64Decoder();

        /// <summary>
        /// The instance for nullable.
        /// </summary>
        public static readonly IDecoder<long?> NullableInstance = new NullableDecoder<long>(Instance);

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The value.</returns>
        public long Decode(IJsonReader reader)
        {
            return reader.ReadInt64();
        }
    }

    /// <summary>
    /// Decoder for UInt32.
    /// </summary>
    internal sealed class UInt32Decoder : IDecoder<uint>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IDecoder<uint> Instance = new UInt32Decoder();

        /// <summary>
        /// The instance for nullable.
        /// </summary>
        public static readonly IDecoder<uint?> NullableInstance = new NullableDecoder<uint>(Instance);

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The value.</returns>
        public uint Decode(IJsonReader reader)
        {
            return reader.ReadUInt32();
        }
    }

    /// <summary>
    /// Decoder for UInt64.
    /// </summary>
    internal sealed class UInt64Decoder : IDecoder<ulong>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IDecoder<ulong> Instance = new UInt64Decoder();

        /// <summary>
        /// The instance for nullable.
        /// </summary>
        public static readonly IDecoder<ulong?> NullableInstance = new NullableDecoder<ulong>(Instance);

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The value.</returns>
        public ulong Decode(IJsonReader reader)
        {
            return reader.ReadUInt64();
        }
    }

    /// <summary>
    /// Decoder for Float.
    /// </summary>
    internal sealed class SingleDecoder : IDecoder<float>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IDecoder<float> Instance = new SingleDecoder();

        /// <summary>
        /// The instance for nullable.
        /// </summary>
        public static readonly IDecoder<float?> NullableInstance = new NullableDecoder<float>(Instance);

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The value.</returns>
        public float Decode(IJsonReader reader)
        {
            return reader.ReadSingle();
        }
    }

    /// <summary>
    /// Decoder for double.
    /// </summary>
    internal sealed class DoubleDecoder : IDecoder<double>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IDecoder<double> Instance = new DoubleDecoder();

        /// <summary>
        /// The instance for nullable.
        /// </summary>
        public static readonly IDecoder<double?> NullableInstance = new NullableDecoder<double>(Instance);

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The value.</returns>
        public double Decode(IJsonReader reader)
        {
            return reader.ReadDouble();
        }
    }

    /// <summary>
    /// Decoder for boolean.
    /// </summary>
    internal sealed class BooleanDecoder : IDecoder<bool>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IDecoder<bool> Instance = new BooleanDecoder();

        /// <summary>
        /// The instance for nullable.
        /// </summary>
        public static readonly IDecoder<bool?> NullableInstance = new NullableDecoder<bool>(Instance);

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The value.</returns>
        public bool Decode(IJsonReader reader)
        {
            return reader.ReadBoolean();
        }
    }

    /// <summary>
    /// Decoder for DateTime.
    /// </summary>
    internal sealed class DateTimeDecoder : IDecoder<DateTime>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IDecoder<DateTime> Instance = new DateTimeDecoder();

        /// <summary>
        /// The instance for nullable.
        /// </summary>
        public static readonly IDecoder<DateTime?> NullableInstance = new NullableDecoder<DateTime>(Instance);

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The value.</returns>
        public DateTime Decode(IJsonReader reader)
        {
            return reader.ReadDateTime();
        }
    }

    /// <summary>
    /// Decoder for bytes.
    /// </summary>
    internal sealed class BytesDecoder : IDecoder<byte[]>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IDecoder<byte[]> Instance = new BytesDecoder();

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The value.</returns>
        public byte[] Decode(IJsonReader reader)
        {
            return reader.ReadBytes();
        }
    }

    /// <summary>
    /// Decoder for string.
    /// </summary>
    internal sealed class StringDecoder : IDecoder<string>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IDecoder<string> Instance = new StringDecoder();

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The value.</returns>
        public string Decode(IJsonReader reader)
        {
            return reader.ReadString();
        }
    }

    /// <summary>
    /// Decoder for struct type.
    /// </summary>
    /// <typeparam name="T">The struct type.</typeparam>
    internal abstract class StructDecoder<T> : IDecoder<T> where T : class
    {
        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The value.</returns>
        public T Decode(IJsonReader reader)
        {
            if (reader.IsNull)
            {
                reader.Read();
                return null;
            }

            EnsureStartObject(reader);

            var obj = this.DecodeFields(reader);

            EnsureEndObject(reader);

            return obj;
        }

        /// <summary>
        /// Decode fields without ensuring start and end object.
        /// </summary>
        /// <param name="reader">The json reader.</param>
        /// <returns>The decoded object.</returns>
        public virtual T DecodeFields(IJsonReader reader)
        {
            var obj = this.Create();

            string fieldName;

            while (TryReadPropertyName(reader, out fieldName))
            {
                this.SetField(obj, fieldName, reader);
            }

            return obj;
        }

        /// <summary>
        /// Try read next token as property name.
        /// </summary>
        /// <param name="reader">The json reader.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>If succeeded.</returns>
        protected static bool TryReadPropertyName(IJsonReader reader, out string propertyName)
        {
            if (reader.IsPropertyName)
            {
                propertyName = reader.ReadString();
                return true;
            }

            propertyName = null;
            return false;
        }

        /// <summary>
        /// Read list of specific type.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="reader">The json reader.</param>
        /// <param name="itemDecoder">The item decoder.</param>
        /// <returns>The decoded list.</returns>
        protected static List<TItem> ReadList<TItem>(IJsonReader reader, IDecoder<TItem> itemDecoder)
        {
            return ListDecoder<TItem>.Decode(reader, itemDecoder);
        }

        /// <summary>
        /// Create a struct instance.
        /// </summary>
        /// <returns>The struct instance.</returns>
        protected abstract T Create();

        /// <summary>
        /// Set given field.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="reader">The json reader.</param>
        protected virtual void SetField(T value, string fieldName, IJsonReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ensure current token is start object.
        /// </summary>
        /// <param name="reader">The json reader.</param>
        private static void EnsureStartObject(IJsonReader reader)
        {
            if (!reader.IsStartObject)
            {
                throw new InvalidOperationException("Invalid json token. Expect start object");
            }

            reader.Read();
        }

        /// <summary>
        /// Ensure next token is end object.
        /// </summary>
        /// <param name="reader">The json reader.</param>
        private static void EnsureEndObject(IJsonReader reader)
        {
            if (!reader.IsEndObject)
            {
                throw new InvalidOperationException("Invalid json token. Expect end object");
            }

            reader.Read();
        }
    }

    /// <summary>
    /// Decoder for union type.
    /// </summary>
    /// <typeparam name="T">The union type.</typeparam>
    internal abstract class UnionDecoder<T> : StructDecoder<T> where T : class, new()
    {
        /// <summary>
        /// Decode fields without ensuring start and end object.
        /// </summary>
        /// <param name="reader">The json reader.</param>
        /// <returns>The decoded object.</returns>
        public override T DecodeFields(IJsonReader reader)
        {
            string fieldName;

            if (!StructDecoder<T>.TryReadPropertyName(reader, out fieldName))
            {
                throw new InvalidOperationException("Not property found.");
            }

            if (fieldName != ".tag")
            {
                throw new InvalidOperationException(
                    string.Format(
                    CultureInfo.InvariantCulture,
                    "Expect '.tag' field, got {0}",
                    fieldName));
            }

            return this.Decode(StringDecoder.Instance.Decode(reader), reader);
        }

        /// <summary>
        /// Decode based on given tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="reader">The reader.</param>
        /// <returns>The decoded object.</returns>
        protected abstract T Decode(string tag, IJsonReader reader);
    }

    /// <summary>
    /// The decoder for Empty Type.
    /// </summary>
    internal sealed class EmptyDecoder : IDecoder<Empty>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IDecoder<Empty> Instance = new EmptyDecoder();

        /// <summary>
        /// Decoder for struct type.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The empty instance.</returns>
        public Empty Decode(IJsonReader reader)
        {
            reader.Skip();
            return Empty.Instance;
        }
    }

    /// <summary>
    /// Decoder for generic list.
    /// </summary>
    /// <typeparam name="T">The list item type.</typeparam>
    internal sealed class ListDecoder<T> : IDecoder<List<T>>
    {
        /// <summary>
        /// Decoder for list item.
        /// </summary>
        private readonly IDecoder<T> itemDecoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListDecoder{T}"/> class.
        /// </summary>
        /// <param name="itemDecoder">The item decoder.</param>
        public ListDecoder(IDecoder<T> itemDecoder)
        {
            this.itemDecoder = itemDecoder;
        }

        /// <summary>
        /// Decode into list of specific type.
        /// </summary>
        /// <param name="reader">The json reader.</param>
        /// <param name="itemDecoder">The item decoder.</param>
        /// <returns>The list.</returns>
        public static List<T> Decode(IJsonReader reader, IDecoder<T> itemDecoder)
        {
            var list = new List<T>();

            EnsureStartArray(reader);

            T item;

            while (TryReadArrayItem(reader, itemDecoder, out item))
            {
                list.Add(item);
            }

            EnsureEndArray(reader);

            return list;
        }

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The value.</returns>
        public List<T> Decode(IJsonReader reader)
        {
            return Decode(reader, this.itemDecoder);
        }

        /// <summary>
        /// Ensure current token is start array.
        /// </summary>
        /// <param name="reader">The json reader.</param>
        private static void EnsureStartArray(IJsonReader reader)
        {
            if (!reader.IsStartArray)
            {
                throw new InvalidOperationException("Invalid json token. Expect start array");
            }

            reader.Read();
        }

        /// <summary>
        /// Ensure next token is end array.
        /// </summary>
        /// <param name="reader">The json reader.</param>
        private static void EnsureEndArray(IJsonReader reader)
        {
            if (!reader.IsEndArray)
            {
                throw new InvalidOperationException("Invalid json token. Expect end array");
            }

            reader.Read();
        }

        /// <summary>
        /// Try read next array item..
        /// </summary>
        /// <param name="reader">The json reader.</param>
        /// <param name="decoder">The decoder.</param>
        /// <param name="value">The value of the array item.</param>
        /// <returns>If succeeded.</returns>
        private static bool TryReadArrayItem(IJsonReader reader, IDecoder<T> decoder, out T value)
        {
            value = default(T);

            while (!reader.IsEndArray)
            {
                value = decoder.Decode(reader);
                return true;
            }

            return false;
        }
    }
}
