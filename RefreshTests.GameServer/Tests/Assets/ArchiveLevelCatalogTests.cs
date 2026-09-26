using Microsoft.Data.Sqlite;
using Refresh.Core.Storage;

namespace RefreshTests.GameServer.Tests.Assets;

public class ArchiveLevelCatalogTests
{
    [Test]
    public void FindsLevelsByTitleAndCreatorAndReturnsRootHashes()
    {
        string path = Path.GetTempFileName();
        try
        {
            using (SqliteConnection connection = new($"Data Source={path}"))
            {
                connection.Open();
                using SqliteCommand command = connection.CreateCommand();
                command.CommandText = """
                    CREATE TABLE slot (id INTEGER, name TEXT, npHandle TEXT, rootLevel BLOB, game INTEGER, heartCount INTEGER);
                    INSERT INTO slot VALUES (42, 'Old Castle', 'SackBuilder', X'0123456789abcdef0123456789abcdef01234567', 2, 10);
                    """;
                command.ExecuteNonQuery();
            }

            ArchiveLevelCatalog catalog = new(path);
            ArchiveLevelRecord result = catalog.Search("castle").Single();
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(42));
                Assert.That(result.Author, Is.EqualTo("SackBuilder"));
                Assert.That(result.RootHash, Is.EqualTo("0123456789abcdef0123456789abcdef01234567"));
                Assert.That(catalog.Search("sackbuilder").Single().Id, Is.EqualTo(42));
                Assert.That(catalog.FindById(42)?.Title, Is.EqualTo("Old Castle"));
                Assert.That(catalog.FindById(43), Is.Null);
                Assert.That(catalog.Search("'; DROP TABLE slot; --"), Is.Empty);
            });
        }
        finally
        {
            File.Delete(path);
        }
    }
}
