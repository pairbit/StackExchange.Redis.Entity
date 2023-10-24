﻿namespace IT.Redis.Entity.Internal;

internal class KeyBuilder
{
    private readonly List<object> _serializers = new(5);
    private readonly byte[] _prefix;
    private readonly byte _separator = (byte)':';

    public KeyBuilder(byte[]? prefix)
    {
        _prefix = prefix ?? Array.Empty<byte>();
    }

    public void AddSerializer(object serializer) => _serializers.Add(serializer);

    public byte[] Build<TKey>(in TKey key)
    {
        var f = GetFormatter<TKey>(0);

        var prefix = _prefix;
        var offset = prefix.Length;

        var bytes = new byte[offset + f.GetLength(in key)];
        var span = bytes.AsSpan();

        if (offset > 0)
        {
            prefix.CopyTo(span);
            span = span.Slice(offset);
        }

        f.Format(in key, span);

        return bytes;
    }

    public byte[] Build<TKey1, TKey2>(in TKey1 key1, in TKey2 key2)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);

        var sep = _separator;
        var prefix = _prefix;
        var offset = prefix.Length;

        var bytes = new byte[1 + offset +
            f1.GetLength(in key1) +
            f2.GetLength(in key2)];

        var span = bytes.AsSpan();

        if (offset > 0)
        {
            prefix.CopyTo(span);
            span = span.Slice(offset);
            offset = 0;
        }

        offset += f1.Format(in key1, span);
        span[offset++] = sep;
        f2.Format(in key2, span.Slice(offset));

        return bytes;
    }

    public byte[] Build<TKey1, TKey2, TKey3>(in TKey1 key1, in TKey2 key2, in TKey3 key3)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);

        var sep = _separator;
        var prefix = _prefix;
        var offset = prefix.Length;

        var bytes = new byte[2 + offset +
            f1.GetLength(in key1) +
            f2.GetLength(in key2) +
            f3.GetLength(in key3)];

        var span = bytes.AsSpan();

        if (offset > 0)
        {
            prefix.CopyTo(span);
            span = span.Slice(offset);
            offset = 0;
        }

        offset += f1.Format(in key1, span);
        span[offset++] = sep;
        offset += f2.Format(in key2, span.Slice(offset));
        span[offset++] = sep;
        f3.Format(in key3, span.Slice(offset));

        return bytes;
    }

    private IUtf8Formatter<TKey> GetFormatter<TKey>(int index)
    {
        return (IUtf8Formatter<TKey>)_serializers[index];
    }
}