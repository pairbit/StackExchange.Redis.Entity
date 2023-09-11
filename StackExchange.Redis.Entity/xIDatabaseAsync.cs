﻿using StackExchange.Redis.Entity;

namespace StackExchange.Redis;

public static class xIDatabaseAsync
{
    public static Task EntitySetAsync<T>(this IDatabaseAsync db, RedisKey key, T entity, IRedisEntityReader<T>? reader = null, CommandFlags flags = CommandFlags.None)
        => db.HashSetAsync(key, (reader ?? RedisEntity<T>.Reader).GetEntries(entity), flags);

    public static Task EntitySetAsync<T>(this IDatabaseAsync db, RedisKey key, T entity, RedisValue[] fields, IRedisEntityReader<T>? reader = null, CommandFlags flags = CommandFlags.None)
        => db.HashSetAsync(key, (reader ?? RedisEntity<T>.Reader).GetEntries(entity, fields), flags);

    public static Task<bool> EntitySetAsync<T>(this IDatabaseAsync db, RedisKey key, T entity, RedisValue field, IRedisEntityReader<T>? reader = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSetAsync(key, field, (reader ?? RedisEntity<T>.Reader).Read(entity, in field), when, flags);

    public static async Task<bool> EntityLoadAllAsync<T>(this IDatabaseAsync db, T entity, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
        => (writer ?? RedisEntity<T>.Writer).Write(entity, await db.HashGetAllAsync(key, flags).ConfigureAwait(false));

    public static async Task<bool> EntityLoadAsync<T>(this IDatabaseAsync db, T entity, RedisKey key, RedisValue field, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
        => (writer ?? RedisEntity<T>.Writer).Write(entity, in field, await db.HashGetAsync(key, field, flags).ConfigureAwait(false));

    public static async Task<bool> EntityLoadAsync<T>(this IDatabaseAsync db, T entity, RedisKey key, RedisValue[] fields, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
        => (writer ?? RedisEntity<T>.Writer).Write(entity, fields, await db.HashGetAsync(key, fields, flags).ConfigureAwait(false));

    public static async Task<bool> EntityLoadAsync<T>(this IDatabaseAsync db, T entity, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
    {
        writer ??= RedisEntity<T>.Writer;
        var fields = writer.Fields.All;
        return writer.Write(entity, fields, await db.HashGetAsync(key, fields, flags).ConfigureAwait(false));
    }

    public static async Task<TEntity?> EntityGetAllAsync<TEntity, IEntity>(this IDatabaseAsync db, RedisKey key, IRedisEntityWriter<IEntity>? writer = null, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
        => (writer ?? RedisEntity<IEntity>.Writer).GetEntity<TEntity, IEntity>(await db.HashGetAllAsync(key, flags).ConfigureAwait(false));

    public static async Task<TEntity?> EntityGetAsync<TEntity, IEntity>(this IDatabaseAsync db, RedisKey key, RedisValue field, IRedisEntityWriter<IEntity>? writer = null, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
        => (writer ?? RedisEntity<IEntity>.Writer).GetEntity<TEntity, IEntity>(in field, await db.HashGetAsync(key, field, flags).ConfigureAwait(false));

    public static async Task<TEntity?> EntityGetAsync<TEntity, IEntity>(this IDatabaseAsync db, RedisKey key, RedisValue[] fields, IRedisEntityWriter<IEntity>? writer = null, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
        => (writer ?? RedisEntity<IEntity>.Writer).GetEntity<TEntity, IEntity>(fields, await db.HashGetAsync(key, fields, flags).ConfigureAwait(false));

    public static async Task<TEntity?> EntityGetAsync<TEntity, IEntity>(this IDatabaseAsync db, RedisKey key, IRedisEntityWriter<IEntity>? writer = null, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
    {
        writer ??= RedisEntity<IEntity>.Writer;
        var fields = writer.Fields.All;
        return writer.GetEntity<TEntity, IEntity>(fields, await db.HashGetAsync(key, fields, flags).ConfigureAwait(false));
    }

    public static async Task<T?> EntityGetAllAsync<T>(this IDatabaseAsync db, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
        => (writer ?? RedisEntity<T>.Writer).GetEntity<T, T>(await db.HashGetAllAsync(key, flags).ConfigureAwait(false));

    public static async Task<T?> EntityGetAsync<T>(this IDatabaseAsync db, RedisKey key, RedisValue field, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
        => (writer ?? RedisEntity<T>.Writer).GetEntity<T, T>(in field, await db.HashGetAsync(key, field, flags).ConfigureAwait(false));

    public static async Task<T?> EntityGetAsync<T>(this IDatabaseAsync db, RedisKey key, RedisValue[] fields, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
        => (writer ?? RedisEntity<T>.Writer).GetEntity<T, T>(fields, await db.HashGetAsync(key, fields, flags).ConfigureAwait(false));

    public static async Task<T?> EntityGetAsync<T>(this IDatabaseAsync db, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
    {
        writer ??= RedisEntity<T>.Writer;
        var fields = writer.Fields.All;
        return writer.GetEntity<T, T>(fields, await db.HashGetAsync(key, fields, flags).ConfigureAwait(false));
    }
}