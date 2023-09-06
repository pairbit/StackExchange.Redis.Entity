using ExternalLib;

namespace StackExchange.Redis.Entity.Tests;

public class RedisEntityTest
{
    private IDatabase _db;

    [SetUp]
    public void Setup()
    {
        var connection = ConnectionMultiplexer.Connect(GetConnectionString(6381));
        _db = connection.GetDatabase()!;
    }

    //[Test]
    public void ReadOnlyDocument()
    {
        var reader = RedisEntity<ReadOnlyDocument>.Reader;
        var writer = RedisEntity<Document>.Writer;

        _db.HashSet(Doc.Key1, reader.GetEntries(Doc.ReadOnlyData1));

        var doc = new Document();

        writer.Write(doc, _db.HashGetAll(Doc.Key1));

        Assert.That(doc, Is.EqualTo(Doc.Data1));

        Assert.IsTrue(_db.KeyDelete(Doc.Key1));
    }

    //[Test]
    public void HashSetGet_Default()
    {
        HashSetGet(RedisEntity<Document>.Writer, RedisEntity<Document>.Reader);
    }

    [Test]
    public void HashSet_Multi()
    {
        var readerWriter = new RedisDocumentReaderWriter();
        var entries = new HashEntry[readerWriter.Fields.All.Length];
        var document = new Document();
        var keys = new RedisKey[1000];

        for (int i = 0; i < keys.Length; i++)
        {
            Doc.New(document, i);

            readerWriter.Read(entries, document);

            var key = Doc.Prefix.Append(i.ToString());

            _db.HashSet(key, entries);

            Assert.That(readerWriter.GetEntity(_db.HashGetAll(key)), Is.EqualTo(document));

            keys[i] = key;
        }

        Assert.That(_db.KeyDelete(keys), Is.EqualTo(keys.Length));
    }

    [Test]
    public void HashSetGet_Custom()
    {
        var readerWriter = new RedisDocumentReaderWriter();
        HashSetGet(readerWriter, readerWriter);
    }

    [Test]
    public void EntitySetGet_Custom()
    {
        var readerWriter = new RedisDocumentReaderWriter();
        EntitySetGet(readerWriter, readerWriter);
    }

    private void HashSetGet(IRedisEntityWriter<Document> writer, IRedisEntityReader<Document> reader)
    {
        var EndDate_Modified = new RedisValue[] { reader.Fields[nameof(Document.EndDate)], reader.Fields[nameof(Document.Modified)] };
        var Field_IsDeleted = reader.Fields[nameof(Document.IsDeleted)];

        Assert.IsNull(writer.GetEntity(_db.HashGetAll(Doc.Key1)));
        Assert.IsNull(writer.GetEntity(EndDate_Modified, _db.HashGet(Doc.Key1, EndDate_Modified)));
        Assert.IsNull(writer.GetEntity(writer.Fields.All, _db.HashGet(Doc.Key1, writer.Fields.All)));

        var doc2 = new Document();

        Assert.That(writer.Write(doc2, _db.HashGetAll(Doc.Key1)), Is.False);
        Assert.That(writer.Write(doc2, EndDate_Modified, _db.HashGet(Doc.Key1, EndDate_Modified)), Is.False);
        Assert.That(writer.Write(doc2, Field_IsDeleted, _db.HashGet(Doc.Key1, Field_IsDeleted)), Is.False);

        _db.HashSet(Doc.Key1, reader.GetEntries(Doc.Data1));

        Assert.That(writer.Write(doc2, _db.HashGetAll(Doc.Key1)), Is.True);

        Assert.That(doc2, Is.EqualTo(Doc.Data1));
        Assert.That(writer.GetEntity(_db.HashGetAll(Doc.Key1)), Is.EqualTo(Doc.Data1));

        doc2.EndDate = new DateOnly(2022, 03, 20);
        doc2.Modified = DateTime.UtcNow;

        _db.HashSet(Doc.Key1, reader.GetEntries(doc2, EndDate_Modified));

        var doc3 = new Document();

        Assert.That(writer.Write(doc3, EndDate_Modified, _db.HashGet(Doc.Key1, EndDate_Modified)), Is.True);

        Assert.That(doc2, Is.Not.EqualTo(doc3));
        Assert.That(doc2.EndDate, Is.EqualTo(doc3.EndDate));
        Assert.That(doc2.Modified, Is.EqualTo(doc3.Modified));

        Assert.That(writer.Write(doc3, writer.Fields.All, _db.HashGet(Doc.Key1, writer.Fields.All)), Is.True);

        Assert.That(doc2, Is.EqualTo(doc3));

        doc2.IsDeleted = true;
 
        _db.HashSet(Doc.Key1, Field_IsDeleted, reader.Read(doc2, Field_IsDeleted));

        Assert.That(writer.Write(doc3, Field_IsDeleted, _db.HashGet(Doc.Key1, Field_IsDeleted)), Is.True);

        Assert.That(doc2, Is.EqualTo(doc3));

        Assert.IsTrue(_db.KeyDelete(Doc.Key1));
    }

    private void EntitySetGet(IRedisEntityWriter<Document> writer, IRedisEntityReader<Document> reader)
    {
        var EndDate_Modified = new RedisValue[] { reader.Fields[nameof(Document.EndDate)], reader.Fields[nameof(Document.Modified)] };
        
        Assert.IsNull(_db.EntityGetAll<Document>(Doc.Key1));
        Assert.IsNull(_db.EntityGet<Document>(Doc.Key1, EndDate_Modified));
        Assert.IsNull(_db.EntityGet<Document>(Doc.Key1, writer));

        var doc2 = new Document();

        Assert.That(_db.EntityLoadAll(doc2, Doc.Key1), Is.False);
        Assert.That(_db.EntityLoad(doc2, Doc.Key1, EndDate_Modified), Is.False);
        Assert.That(_db.EntityLoad(doc2, Doc.Key1, writer), Is.False);

        _db.EntitySet(Doc.Key1, Doc.Data1, reader);

        Assert.That(_db.EntityLoadAll(doc2, Doc.Key1, writer), Is.True);

        Assert.That(doc2, Is.EqualTo(Doc.Data1));
        Assert.That(_db.EntityGetAll(Doc.Key1, writer), Is.EqualTo(Doc.Data1));

        doc2.EndDate = new DateOnly(2022, 03, 20);
        doc2.Modified = DateTime.UtcNow;

        _db.EntitySet(Doc.Key1, doc2, EndDate_Modified, reader);

        var doc3 = new Document();

        Assert.That(_db.EntityLoad(doc3, Doc.Key1, EndDate_Modified, writer), Is.True);
        Assert.That(doc2, Is.Not.EqualTo(doc3));
        Assert.That(doc2.EndDate, Is.EqualTo(doc3.EndDate));
        Assert.That(doc2.IsDeleted, Is.EqualTo(doc3.IsDeleted));

        Assert.That(_db.EntityGet(Doc.Key1, EndDate_Modified, writer), Is.EqualTo(doc3));
        //Assert.That(_db.EntityGet<Document, IDocumentView>(Doc.Key1), Is.EqualTo(doc3));

        Assert.That(_db.EntityLoad(doc3, Doc.Key1, writer), Is.True);

        Assert.That(doc3, Is.EqualTo(doc2));
        Assert.That(_db.EntityGet(Doc.Key1, writer), Is.EqualTo(doc2));

        Assert.IsTrue(_db.KeyDelete(Doc.Key1));
    }

    static string GetConnectionString(int port = 6379, int db = 0) => $"localhost:{port},defaultDatabase={db},syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False";
}