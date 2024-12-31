using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Uper.Common;
using Uper.Repository.Turso;

namespace Uper.IntegrationTest.Turso;

public class TursoClientTest : IAsyncLifetime
{
    private readonly TursoClient _tursoClient;

    public TursoClientTest()
    {
        // Build configuration to include user secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TursoClientTest>() // Use the test class to locate the secrets file
            .Build();
        var tursoClientConfiguration = configuration.GetRequiredAs<TursoClientConfiguration>();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(tursoClientConfiguration.BaseUrl),
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", 
                    tursoClientConfiguration.BearerToken),
            }
        };

        _tursoClient = new TursoClient(httpClient);
    }

    public async Task InitializeAsync()
    {
        // Setup: Create a test table
        var createTableSql = @"
            CREATE TABLE IF NOT EXISTS TursoClientTestTable (
                Id TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                Description TEXT,
                UserId TEXT NOT NULL
            );
        ";
        await _tursoClient.ExecuteQueryAsync(createTableSql);
    }

    public async Task DisposeAsync()
    {
        // Teardown: Drop the test table
        var dropTableSql = "DROP TABLE IF EXISTS TursoClientTestTable;";
        await _tursoClient.ExecuteQueryAsync(dropTableSql);
    }

    [Fact]
    public async Task InsertAndQueryTest()
    {
        // Example test using the TursoClientTestTable
        var insertSql = @"
            INSERT INTO TursoClientTestTable (Id, Name, Description, UserId)
            VALUES ('uuid-125', 'Test Name', 'Test Description', 'auth0|user-abc');
        ";
        await _tursoClient.ExecuteQueryAsync(insertSql);

        var selectSql = "SELECT * FROM TursoClientTestTable WHERE Id = 'uuid-125';";
        var result = await _tursoClient.ExecuteQueryAsync(selectSql);

        Assert.Contains("Test Name", result);
    }
}
