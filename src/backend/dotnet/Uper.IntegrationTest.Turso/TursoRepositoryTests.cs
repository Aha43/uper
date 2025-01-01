using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Uper.Domain.Abstraction.Repository;
using Uper.Domain.Request.Dto;
using Uper.Repository.Turso;

namespace Uper.IntegrationTest.Turso;

public class TursoRepositoryTests : IAsyncLifetime
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRepository _repository;

    public TursoRepositoryTests()
    {
        // Build configuration to include user secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TursoRepositoryTests>() // Use the test class to locate the secrets file
            .Build();

        var services = new ServiceCollection();
        services.AddTursoRepository(configuration); // Assume this method registers the TursoClient and TursoRepository
        _serviceProvider = services.BuildServiceProvider();

        _repository = _serviceProvider.GetRequiredService<IRepository>();
    }

    public async Task InitializeAsync()
    {
        // Create test table
        var client = _serviceProvider.GetRequiredService<TursoClient>();
        await client.ExecuteQueryAsync("CREATE TABLE IF NOT EXISTS TestTable (Id TEXT PRIMARY KEY, Name TEXT, Description TEXT, UserId TEXT)");
    }

    public async Task DisposeAsync()
    {
        // Drop test table
        var client = _serviceProvider.GetRequiredService<TursoClient>();
        await client.ExecuteQueryAsync("DROP TABLE IF EXISTS TestTable");
    }

    [Fact]
    public async Task Create_ShouldInsertObject()
    {
        var dto = new CreateUpdateDto
        {
            Type = "TestTable",
            Objects =
            [
                new() { ["Id"] = "1", ["Name"] = "Test1", ["Description"] = "First test", ["UserId"] = "User1" }
            ]
        };

        await _repository.CreateAsync(dto, "User1");

        var results = await _repository.GetAllAsync("TestTable", "User1");
        Assert.Single(results);
        var result = results.FirstOrDefault();
        Assert.NotNull(result);
        Assert.Equal("Test1", result["Name"]);
    }

    [Fact]
    public async Task Update_ShouldModifyObject()
    {
        var dto = new CreateUpdateDto
        {
            Type = "TestTable",
            Objects =
            [
                new() { ["Id"] = "1", ["Name"] = "Test1", ["Description"] = "First test", ["UserId"] = "User1" }
            ]
        };

        await _repository.CreateAsync(dto, "User1");

        dto.Objects[0]["Description"] = "Updated description";
        await _repository.UpdateAsync(dto, "User1");

        var result = await _repository.GetByIdAsync("TestTable", "1", "User1");
        Assert.NotNull(result);
        Assert.Equal("Updated description", result["Description"]);
    }

    [Fact]
    public async Task Delete_ShouldRemoveObject()
    {
        var dto = new CreateUpdateDto
        {
            Type = "TestTable",
            Objects =
            [
                new() { ["Id"] = "1", ["Name"] = "Test1", ["Description"] = "First test", ["UserId"] = "User1" }
            ]
        };

        await _repository.CreateAsync(dto, "User1");

        await _repository.DeleteAsync("TestTable", "1", "User1");

        var result = await _repository.GetAllAsync("TestTable", "User1");
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllObjects()
    {
        var dto = new CreateUpdateDto
        {
            Type = "TestTable",
            Objects =
            [
                new() { ["Id"] = "1", ["Name"] = "Test1", ["Description"] = "First test", ["UserId"] = "User1" },
                new() { ["Id"] = "2", ["Name"] = "Test2", ["Description"] = "Second test", ["UserId"] = "User1" }
            ]
        };

        await _repository.CreateAsync(dto, "User1");

        var result = await _repository.GetAllAsync("TestTable", "User1");
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}
