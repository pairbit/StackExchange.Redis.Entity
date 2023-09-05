﻿namespace StackExchange.Redis.Entity;

public static class xIRedisEntityWriter
{
    public static void Write<T>(this IRedisEntityWriter<T> writer, T entity, HashEntry[] entries)
    {
        for (int i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            writer.Write(entity, entry.Name, entry.Value);
        }
    }

    public static void Write<T>(this IRedisEntityWriter<T> writer, T entity, RedisValue[] fields, RedisValue[] values)
    {
        if (fields.Length != values.Length) throw new ArgumentOutOfRangeException(nameof(values));

        for (int i = 0; i < fields.Length; i++)
        {
            writer.Write(entity, fields[i], values[i]);
        }
    }
}