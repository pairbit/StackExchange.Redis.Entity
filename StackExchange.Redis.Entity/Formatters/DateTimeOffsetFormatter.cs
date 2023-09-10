﻿namespace StackExchange.Redis.Entity.Formatters;

public class DateTimeOffsetFormatter : NullableFormatter<DateTimeOffset>
{
    public static readonly DateTimeOffsetFormatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref DateTimeOffset value) => value = new DateTimeOffset((long)redisValue, TimeSpan.Zero);

    public override RedisValue Serialize(in DateTimeOffset value) => value.UtcTicks;
}