﻿namespace StackExchange.Redis.Entity.Formatters;

public class TimeSpanFormatter : NullableFormatter<TimeSpan>
{
    public static readonly TimeSpanFormatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref TimeSpan value) => value = new TimeSpan((long)redisValue);

    public override RedisValue Serialize(in TimeSpan value) => value.Ticks;
}