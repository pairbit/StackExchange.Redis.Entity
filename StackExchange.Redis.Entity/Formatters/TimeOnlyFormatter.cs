﻿namespace StackExchange.Redis.Entity.Formatters;

public class TimeOnlyFormatter : NullableFormatter<TimeOnly>
{
    public static readonly TimeOnlyFormatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref TimeOnly value) => value = new TimeOnly((long)redisValue);

    public override RedisValue Serialize(in TimeOnly value) => value.Ticks;
}