//-----------------------------------------------------------------------------
// <copyright file="Encoder.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Stone
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

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
    /// Encoder for nullable struct.
    /// </summary>
    /// <typeparam name="T">Type of the struct.</typeparam>
    internal sealed class NullableEncoder<T> : IEncoder<T?>
        where T : struct
    {
        /// <summary>
        /// The encoder.
        /// </summary>
        private readonly IEncoder<T> encoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableEncoder{T}"/> class.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        public NullableEncoder(IEncoder<T> encoder)
        {
            this.encoder = encoder;
        }

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task Encode(T? value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            if (value == null)
            {
                await writer.WriteNull(cancellationToken);
                return;
            }

            await this.encoder.Encode(value.Value, writer, cancellationToken);
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
        /// The nullable instance.
        /// </summary>
        public static readonly IEncoder<int?> NullableInstance = new NullableEncoder<int>(Instance);

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task Encode(int value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            await writer.WriteInt32(value, cancellationToken);
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
        /// The nullable instance.
        /// </summary>
        public static readonly IEncoder<long?> NullableInstance = new NullableEncoder<long>(Instance);

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task Encode(long value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            await writer.WriteInt64(value, cancellationToken);
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
        /// The nullable instance.
        /// </summary>
        public static readonly IEncoder<uint?> NullableInstance = new NullableEncoder<uint>(Instance);

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task Encode(uint value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            await writer.WriteUInt32(value, cancellationToken);
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
        /// The nullable instance.
        /// </summary>
        public static readonly IEncoder<ulong?> NullableInstance = new NullableEncoder<ulong>(Instance);

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task Encode(ulong value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            await writer.WriteUInt64(value, cancellationToken);
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
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task Encode(float value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            await writer.WriteSingle(value, cancellationToken);
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
        /// The nullable instance.
        /// </summary>
        public static readonly IEncoder<double?> NullableInstance = new NullableEncoder<double>(Instance);

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task Encode(double value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            await writer.WriteDouble(value, cancellationToken);
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
        /// The nullable instance.
        /// </summary>
        public static readonly IEncoder<bool?> NullableInstance = new NullableEncoder<bool>(Instance);

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task Encode(bool value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            await writer.WriteBoolean(value, cancellationToken);
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
        /// The nullable instance.
        /// </summary>
        public static readonly IEncoder<DateTime?> NullableInstance = new NullableEncoder<DateTime>(Instance);

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task Encode(DateTime value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            await writer.WriteDateTime(value, cancellationToken);
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
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task Encode(byte[] value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            await writer.WriteBytes(value, cancellationToken);
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
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task Encode(string value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            await writer.WriteString(value, cancellationToken);
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
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task Encode(Empty value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Encoder for struct type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    internal abstract class StructEncoder<T> : IEncoder<T>
        where T : class
    {
        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task Encode(T value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            if (value == null)
            {
                await writer.WriteNull(cancellationToken);
                return;
            }

            await writer.WriteStartObject(cancellationToken);
            this.EncodeFields(value, writer);
            await writer.WriteEndObject(cancellationToken);
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
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        protected static async Task WriteProperty<TProperty>(string propertyName, TProperty value, IJsonWriter writer, IEncoder<TProperty> encoder, CancellationToken cancellationToken = default)
        {
            await writer.WritePropertyName(propertyName, cancellationToken);
            await encoder.Encode(value, writer, cancellationToken);
        }

        /// <summary>
        /// Write property of list of specific type with given encoder.
        /// </summary>
        /// <typeparam name="TProperty">The property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="encoder">The encoder.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        protected static async Task WriteListProperty<TProperty>(string propertyName, IList<TProperty> value, IJsonWriter writer, IEncoder<TProperty> encoder, CancellationToken cancellationToken = default)
        {
            await writer.WritePropertyName(propertyName, cancellationToken);
            await ListEncoder<TProperty>.Encode(value, writer, encoder, cancellationToken);
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
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public static async Task Encode(IList<T> value, IJsonWriter writer, IEncoder<T> itemEncoder, CancellationToken cancellationToken = default)
        {
            await writer.WriteStartArray(cancellationToken);

            foreach (var item in value)
            {
                await itemEncoder.Encode(item, writer, cancellationToken);
            }

            await writer.WriteEndArray(cancellationToken);
        }

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task Encode(IList<T> value, IJsonWriter writer, CancellationToken cancellationToken = default)
        {
            await Encode(value, writer, this.itemEncoder, cancellationToken);
        }
    }
}
