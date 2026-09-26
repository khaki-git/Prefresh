using Microsoft.Data.Sqlite;

namespace Refresh.Core.Storage;

public sealed record ArchiveLevelRecord(long Id, string Title, string Author, string RootHash, int Game);

public sealed class ArchiveLevelCatalog
{
    private readonly string _path;

    public ArchiveLevelCatalog(string path)
    {
        this._path = path;
    }

    public bool IsAvailable => File.Exists(this._path);

    private SqliteConnection Open()
    {
        SqliteConnectionStringBuilder builder = new()
        {
            DataSource = this._path,
            Mode = SqliteOpenMode.ReadOnly,
            Cache = SqliteCacheMode.Shared,
        };
        SqliteConnection connection = new(builder.ToString());
        connection.Open();
        return connection;
    }

    public IReadOnlyList<ArchiveLevelRecord> Search(string query, int limit = 30)
    {
        if (!this.IsAvailable || string.IsNullOrWhiteSpace(query))
            return [];

        using SqliteConnection connection = this.Open();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, npHandle, rootLevel, game
            FROM slot
            WHERE rootLevel IS NOT NULL AND length(rootLevel) = 20
              AND (instr(lower(name), lower($query)) > 0
                   OR instr(lower(npHandle), lower($query)) > 0)
            ORDER BY heartCount DESC
            LIMIT $limit
            """;
        command.Parameters.AddWithValue("$query", query.Trim());
        command.Parameters.AddWithValue("$limit", Math.Clamp(limit, 1, 50));

        using SqliteDataReader reader = command.ExecuteReader();
        List<ArchiveLevelRecord> levels = [];
        while (reader.Read())
            levels.Add(ReadLevel(reader));
        return levels;
    }

    public ArchiveLevelRecord? FindById(long id)
    {
        if (!this.IsAvailable)
            return null;

        using SqliteConnection connection = this.Open();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, npHandle, rootLevel, game
            FROM slot
            WHERE id = $id AND rootLevel IS NOT NULL AND length(rootLevel) = 20
            LIMIT 1
            """;
        command.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? ReadLevel(reader) : null;
    }

    private static ArchiveLevelRecord ReadLevel(SqliteDataReader reader)
        => new(
            reader.GetInt64(0),
            reader.IsDBNull(1) ? "" : reader.GetString(1),
            reader.IsDBNull(2) ? "" : reader.GetString(2),
            Convert.ToHexString(reader.GetFieldValue<byte[]>(3)).ToLowerInvariant(),
            reader.GetInt32(4));
}
