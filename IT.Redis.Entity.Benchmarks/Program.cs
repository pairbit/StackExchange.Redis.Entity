﻿using IT.Redis.Entity.Benchmarks;

var bench = new RedisBenchmark();
bench.KES();
bench.KEB();
bench.KE_String();
bench.KE_Default();
bench.KE_Fixed();

//BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(Benchmark));
BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(RedisBenchmark));

//using DocLib;
//using DocLib.RedisEntity;
//using IT.Redis.Entity;

//var doc = Document.Data;

//RedisEntity<Document>.ReaderFactory = () => new RedisDocument();

//var redisValue = RedisEntity<Document>.Reader.Read(doc, 0);

//RedisEntity<Document>.ReaderFactory = () => new RedisDocumentArray();

//redisValue = RedisEntity<Document>.Reader.Read(doc, 1);

//RedisEntity<Document>.ReaderFactory = () => RedisEntity<Document>.Default;

//redisValue = RedisEntity<Document>.Reader.Read(doc, 2);

//Console.WriteLine(redisValue);