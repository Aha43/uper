using Uper.Repository.Turso;

namespace Uper.IntegrationTest.Turso.Tools;

public sealed class TestHelper(TursoClient client)
{
    public string TableName { get; private set; } = string.Empty;

    public bool IsInitialized => TableName != string.Empty;

    public bool DropTable { get; private set; } = true;

    public async Task InitializeAsync(string tableName = "TestTable", bool dropTable = true)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException("Table name cannot be null or empty", nameof(tableName));
        }
        if (IsInitialized)
        {
            throw new InvalidOperationException("TestHelper is already initialized");
        }

        TableName = tableName;

        DropTable = dropTable;

        await DropTableAsync();
        await CreateTableAsync();
    }
    
    public async Task DisposeAsync()
    {
        if (DropTable)
        {
            await DropTableAsync();
            TableName = string.Empty;
        }
    }

    private async Task DropTableAsync()
    {
        var sql = $"DROP TABLE IF EXISTS {TableName}";
        await client.ExecuteQueryAsync(sql);
    }

    private async Task CreateTableAsync()
    {
        var sql = $"CREATE TABLE IF NOT EXISTS {TableName} (Id TEXT PRIMARY KEY, Name TEXT, Description TEXT, UserId TEXT)";
        await client.ExecuteQueryAsync(sql);
    }
}
