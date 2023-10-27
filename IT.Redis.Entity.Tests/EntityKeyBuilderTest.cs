﻿using DocLib;
using DocLib.RedisEntity;

namespace IT.Redis.Entity.Tests;

public class EntityKeyBuilderTest
{
    [Test]
    public void ValidationTest()
    {
        Assert.That(Assert.Throws<ArgumentException>(() => RedisEntity<DocumentDepend>.Reader.KeyBuilder.
            BuildKey(null, 0, 1)).Message,
            Is.EqualTo($"Entity '{typeof(DocumentDepend).FullName}' has no keys"));

        Assert.That(Assert.Throws<ArgumentException>(() => RedisEntity<DocumentWithReadOnlyKeys>.Reader.KeyBuilder.
            BuildKey(null, 0, Guid.NewGuid(), Guid.NewGuid(), "name", "key4", 44)).Message,
            Is.EqualTo($"Entity '{typeof(DocumentWithReadOnlyKeys).FullName}' contains 4 keys 'Id, ClientId, Name, Key4'"));

        Assert.That(Assert.Throws<ArgumentException>(() => RedisEntity<DocumentAnnotation>.Reader.KeyBuilder.
            BuildKey(null, 0, 5)).Message,
            Is.EqualTo($"Type '{typeof(int).FullName}' is not the type of key '{nameof(DocumentAnnotation.Id)}' of entity '{typeof(DocumentAnnotation).FullName}'"));

        Assert.That(Assert.Throws<ArgumentException>(() => RedisEntity<DocumentAnnotation>.Reader.KeyBuilder.
            BuildKey(null, 0, Guid.NewGuid(), 6)).Message,
            Is.EqualTo($"Entity '{typeof(DocumentAnnotation).FullName}' contains one key '{nameof(DocumentAnnotation.Id)}'"));
    }
}