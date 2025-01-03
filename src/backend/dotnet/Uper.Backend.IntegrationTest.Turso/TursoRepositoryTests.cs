using Microsoft.Extensions.DependencyInjection;
using Uper.Backend.Domain.Abstraction.Repository;
using Uper.Backend.IntegrationTest.Turso.Tools;
using Uper.Backend.Domain.Request.Dto;
using Uper.Backend.Repository.Turso;

namespace Uper.Backend.IntegrationTest.Turso;

public class TursoRepositoryTests
{
    private readonly TursoClient _client; // Used for set up and tear down

    private readonly IRepository _repository; // Being tested

    public TursoRepositoryTests()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.ConfigureServicesForIntegrationTest().BuildServiceProvider();
        _client = serviceProvider.GetRequiredService<TursoClient>();
        _repository = serviceProvider.GetRequiredService<IRepository>();
    }

    private string GetTableName(string test) => $"{nameof(TursoRepositoryTests).GetInitials()}_{test}";

    [Fact]
    public async Task Create_ShouldInsertObject()
    {
        var testHelper = new TestHelper(_client);
        await testHelper.InitializeAsync(GetTableName(nameof(Create_ShouldInsertObject)), false);

        try
        {
            var dto = new CreateUpdateDto
            {
                Type = testHelper.TableName,
                Objects =
                [
                    new() { ["Id"] = "1", ["Name"] = "Test1", ["Description"] = "First test", ["UserId"] = "User1" }
                ]
            };

            await _repository.CreateAsync(dto, "User1");

            var results = await _repository.GetAllAsync(testHelper.TableName, "User1");
            Assert.Single(results);
            var result = results.FirstOrDefault();
            Assert.NotNull(result);
            Assert.Equal("Test1", result["Name"]);
        }
        finally
        {
            await testHelper.DisposeAsync();
        }
    }

    [Fact]
    public async Task Update_ShouldModifyObject()
    {
        var testHelper = new TestHelper(_client);
        await testHelper.InitializeAsync(GetTableName(nameof(Update_ShouldModifyObject)), false);

        try
        {
            var dto = new CreateUpdateDto
            {
                Type = testHelper.TableName,
                Objects =
                [
                    new() { ["Id"] = "1", ["Name"] = "Test1", ["Description"] = "First test", ["UserId"] = "User1" }
                ]
            };

            await _repository.CreateAsync(dto, "User1");

            dto.Objects[0]["Description"] = "Updated description";
            await _repository.UpdateAsync(dto, "User1");

            var result = await _repository.GetByIdAsync(testHelper.TableName, "1", "User1");
            Assert.NotNull(result);
            Assert.Equal("Updated description", result["Description"]);
        }
        finally
        {
            await testHelper.DisposeAsync();
        }
    }

    [Fact]
    public async Task Delete_ShouldRemoveObject()
    {
        var testHelper = new TestHelper(_client);
        await testHelper.InitializeAsync(GetTableName(nameof(Delete_ShouldRemoveObject)), false);

        try
        {
            var dto = new CreateUpdateDto
            {
                Type = testHelper.TableName,
                Objects =
                [
                    new() { ["Id"] = "1", ["Name"] = "Test1", ["Description"] = "First test", ["UserId"] = "User1" }
                ]
            };

            await _repository.CreateAsync(dto, "User1");

            await _repository.DeleteAsync(testHelper.TableName, "1", "User1");

            var result = await _repository.GetAllAsync(testHelper.TableName, "User1");
            Assert.Empty(result);
        }
        finally
        {
            await testHelper.DisposeAsync();
        }
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllObjects()
    {
        var testHelper = new TestHelper(_client);
        await testHelper.InitializeAsync(GetTableName(nameof(GetAll_ShouldReturnAllObjects)), false);

        try
        {
            var dto = new CreateUpdateDto
            {
                Type = testHelper.TableName,
                Objects =
                [
                    new() { ["Id"] = "1", ["Name"] = "Test1", ["Description"] = "First test", ["UserId"] = "User1" },
                    new() { ["Id"] = "2", ["Name"] = "Test2", ["Description"] = "Second test", ["UserId"] = "User1" }
                ]
            };

            await _repository.CreateAsync(dto, "User1");

            var result = await _repository.GetAllAsync(testHelper.TableName, "User1");
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
        finally
        {
            await testHelper.DisposeAsync();
        }
    }
}
