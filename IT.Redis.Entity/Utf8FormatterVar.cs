﻿using IT.Redis.Entity.Internal;
using IT.Redis.Entity.Utf8Formatters;

namespace IT.Redis.Entity;

public class Utf8FormatterVar : IUtf8Formatter
{
    public int GetLength<T>(in T value)
        => Cache<T>.Formatter.GetLength(in value);

    public bool TryFormat<T>(in T value, Span<byte> bytes, out int written)
        => Cache<T>.Formatter.TryFormat(in value, bytes, out written);

    static class Cache<T>
    {
        public static readonly IUtf8Formatter<T> Formatter = GetFormatter();

        static IUtf8Formatter<T> GetFormatter()
        {
            var type = typeof(T);
            if (type == typeof(Guid)) return (IUtf8Formatter<T>)GuidUtf8Formatter.Default;
            if (type == typeof(int)) return (IUtf8Formatter<T>)Int32Utf8Formatter.Var.Default;
            if (type == typeof(uint)) return (IUtf8Formatter<T>)UInt32Utf8Formatter.Var.Default;
            if (type == typeof(short)) return (IUtf8Formatter<T>)Int16Utf8Formatter.Var.Default;
            if (type == typeof(ushort)) return (IUtf8Formatter<T>)UInt16Utf8Formatter.Var.Default;
            if (type == typeof(byte)) return (IUtf8Formatter<T>)ByteUtf8Formatter.Var.Default;
            if (type == typeof(sbyte)) return (IUtf8Formatter<T>)SByteUtf8Formatter.Var.Default;
            if (type == typeof(byte[])) return (IUtf8Formatter<T>)ByteArrayUtf8Formatter.Default;
            if (type == typeof(string)) return (IUtf8Formatter<T>)StringUtf8Formatter.Default;

            return new Utf8FormatterNotFound<T>();
        }
    }
}