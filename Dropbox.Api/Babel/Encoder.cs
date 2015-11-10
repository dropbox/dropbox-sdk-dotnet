//-----------------------------------------------------------------------------
// <copyright file="Encoder.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Babel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The factory class for encoders.
    /// </summary>
    internal static class Encoder
    {
        /// <summary>
        /// Create a list encoder instance.
        /// </summary>
        /// <typeparam name="T">The list item type.</typeparam>
        /// <param name="itemEncoder">The item encoder.</param>
        /// <returns>The list encoder.</returns>
        public static IEncoder<IList<T>> CreateListEncoder<T>(IEncoder<T> itemEncoder)
        {
            return new ListEncoder<T>(itemEncoder);
        }
    }

    /// <summary>
    /// Encoder for Int32.
    /// </summary>
    internal sealed class Int32Encoder : IEncoder<int>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IEncoder<int> Instance = new Int32Encoder();

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        public void Encode(int value, IJsonWriter writer)
        {
            writer.WriteInt32(value);
        }
    }

    /// <summary>
    /// Encoder for Int64.
    /// </summary>
    internal sealed class Int64Encoder : IEncoder<long>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IEncoder<long> Instance = new Int64Encoder();

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        public void Encode(long value, IJsonWriter writer)
        {
            writer.WriteInt64(value);
        }
    }

    /// <summary>
    /// Encoder for UInt32.
    /// </summary>
    internal sealed class UInt32Encoder : IEncoder<uint>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IEncoder<uint> Instance = new UInt32Encoder();

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        public void Encode(uint value, IJsonWriter writer)
        {
            writer.WriteUInt32(value);
        }
    }

    /// <summary>
    /// Encoder for UInt64.
    /// </summary>
    internal sealed class UInt64Encoder : IEncoder<ulong>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IEncoder<ulong> Instance = new UInt64Encoder();

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        public void Encode(ulong value, IJsonWriter writer)
        {
            writer.WriteUInt64(value);
        }
    }

    /// <summary>
    /// Encoder for Float.
    /// </summary>
    internal sealed class SingleEncoder : IEncoder<float>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IEncoder<float> Instance = new SingleEncoder();

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        public void Encode(float value, IJsonWriter writer)
        {
            writer.WriteSingle(value);
        }
    }

    /// <summary>
    /// Encoder for double.
    /// </summary>
    internal sealed class DoubleEncoder : IEncoder<double>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IEncoder<double> Instance = new DoubleEncoder();

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        public void Encode(double value, IJsonWriter writer)
        {
            writer.WriteDouble(value);
        }
    }

    /// <summary>
    /// Encoder for boolean.
    /// </summary>
    internal sealed class BooleanEncoder : IEncoder<bool>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IEncoder<bool> Instance = new BooleanEncoder();

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        public void Encode(bool value, IJsonWriter writer)
        {
            writer.WriteBoolean(value);
        }
    }

    /// <summary>
    /// Encoder for DateTime.
    /// </summary>
    internal sealed class DateTimeEncoder : IEncoder<DateTime>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IEncoder<DateTime> Instance = new DateTimeEncoder();

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        public void Encode(DateTime value, IJsonWriter writer)
        {
            writer.WriteDateTime(value);
        }
    }

    /// <summary>
    /// Encoder for bytes.
    /// </summary>
    internal sealed class BytesEncoder : IEncoder<byte[]>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IEncoder<byte[]> Instance = new BytesEncoder();

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        public void Encode(byte[] value, IJsonWriter writer)
        {
            writer.WriteBytes(value);
        }
    }

    /// <summary>
    /// Encoder for string.
    /// </summary>
    internal sealed class StringEncoder : IEncoder<string>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IEncoder<string> Instance = new StringEncoder();

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param> 
        public void Encode(string value, IJsonWriter writer)
        {
            writer.WriteString(value);
        }
    }

    /// <summary>
    /// Encoder for empty.
    /// </summary>
    internal sealed class EmptyEncoder : IEncoder<Empty>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IEncoder<Empty> Instance = new EmptyEncoder();

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param> 
        public void Encode(Empty value, IJsonWriter writer)
        {
        }
    }

    /// <summary>
    /// Encoder for struct type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    internal abstract class StructEncoder<T> : IEncoder<T>
    {
        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param> 
        public void Encode(T value, IJsonWriter writer)
        {
            writer.WriteStartObject();
            this.EncodeFields(value, writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Encode fields of given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        public abstract void EncodeFields(T value, IJsonWriter writer);

        /// <summary>
        /// Write property of specific type with given encoder.
        /// </summary>
        /// <typeparam name="TProperty">The property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param> 
        /// <param name="encoder">The encoder.</param>
        protected static void WriteProperty<TProperty>(string propertyName, TProperty value, IJsonWriter writer, IEncoder<TProperty> encoder)
        {
            writer.WritePropertyName(propertyName);
            encoder.Encode(value, writer);
        }

        /// <summary>
        /// Write property of list of specific type with given encoder.
        /// </summary>
        /// <typeparam name="TProperty">The property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param> 
        /// <param name="encoder">The encoder.</param>
        protected static void WriteListProperty<TProperty>(string propertyName, IList<TProperty> value, IJsonWriter writer, IEncoder<TProperty> encoder)
        {
            writer.WritePropertyName(propertyName);
            ListEncoder<TProperty>.Encode(value, writer, encoder);
        }
    }

    /// <summary>
    /// Encoder for list type.
    /// </summary>
    /// <typeparam name="T">The list item type.</typeparam>
    internal sealed class ListEncoder<T> : IEncoder<IList<T>>
    {
        /// <summary>
        /// The item encoder.
        /// </summary>
        private readonly IEncoder<T> itemEncoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEncoder{T}"/> class.
        /// </summary>
        /// <param name="itemEncoder">The item encoder.</param>
        public ListEncoder(IEncoder<T> itemEncoder)
        {
            this.itemEncoder = itemEncoder;
        }

        /// <summary>
        /// Encode given list of specific value with given item encoder.
        /// </summary>
        /// <param name="value">The list.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="itemEncoder">The item encoder.</param>
        public static void Encode(IList<T> value, IJsonWriter writer, IEncoder<T> itemEncoder)
        {
            writer.WriteStartArray();

            foreach (var item in value)
            {
                itemEncoder.Encode(item, writer);
            }

            writer.WriteEndArray();
        }

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param> 
        public void Encode(IList<T> value, IJsonWriter writer)
        {
            Encode(value, writer, this.itemEncoder);
        }
    }
}
