﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DocLib;
using DocLib.RedisEntity;

namespace StackExchange.Redis.Entity.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class Benchmark
{
    private static readonly IRedisEntityReaderWriter<Document> _rw3 = new RedisDocumentArrayExpression();
    private static readonly IRedisEntityReaderWriter<Document> _rw2 = new RedisDocumentArray();
    private static readonly IRedisEntityReaderWriter<Document> _rw1 = new RedisDocument();
    private static readonly IRedisEntityReaderWriter<Document> _rw = RedisEntity<Document>.ReaderWriter;

    private static readonly HashEntry[] _entries = _rw.GetEntries(Document.Data);

    public Benchmark()
    {

    }

    [Benchmark]
    public HashEntry[] GetEntries() => _rw.GetEntries(Document.Data);

    [Benchmark]
    public HashEntry[] GetEntries_Manual() => _rw1.GetEntries(Document.Data);

    [Benchmark]
    public HashEntry[] GetEntries_Array() => _rw2.GetEntries(Document.Data);

    [Benchmark]
    public HashEntry[] GetEntries_ArrayExpression() => _rw3.GetEntries(Document.Data);

    [Benchmark]
    public Document? GetEntity() => _rw.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity_Manual() => _rw1.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity_Array() => _rw2.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity_ArrayExpression() => _rw3.GetEntity(_entries);
}