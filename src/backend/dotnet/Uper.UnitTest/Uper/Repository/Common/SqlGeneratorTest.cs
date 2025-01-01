using Microsoft.Extensions.DependencyInjection;
using Uper.Domain.Abstraction.Repository.Common;
using Uper.Domain.Request.Dto;
using Uper.Repository.Common;

namespace Uper.UnitTest.Uper.Repository.Common;

public class SqlGeneratorTest
{
    private readonly ISqlGenerator _sqlGenerator;

    public SqlGeneratorTest()
    {
        var serviceProvider = new ServiceCollection()
            .AddCommonRepositoryServices()
            .BuildServiceProvider();

        _sqlGenerator = serviceProvider.GetRequiredService<ISqlGenerator>();
    }

    [Fact]
    public void GenerateInsertSql_ShouldGenerateNonParameterizedSql()
    {
        // Arrange
        var dto = new CreateUpdateDto
        {
            Type = "ExampleType",
            Objects =
            [
                new() {
                    { "Id", "uuid-123" },
                    { "Name", "Sample Object" },
                    { "Description", "A test object" }
                },
                new() {
                    { "Id", "uuid-124" },
                    { "Name", "Another Object" },
                    { "Description", null }
                }
            ]
        };

        var userId = "auth0|user-abc";

        // Act
        var sql = _sqlGenerator.GenerateInsertSql(dto, userId);

        // Assert
        var expectedSql = @"
            INSERT INTO ExampleType (Id, Name, Description, UserId) VALUES
            ('uuid-123', 'Sample Object', 'A test object', 'auth0|user-abc'),
            ('uuid-124', 'Another Object', NULL, 'auth0|user-abc');
        ";

        Assert.NotNull(sql);
        Assert.Equal(NormalizeSql(expectedSql), NormalizeSql(sql));
    }

    [Fact]
    public void GenerateUpdateSql_ShouldGenerateCorrectSql()
    {
        // Arrange
        var dto = new CreateUpdateDto
        {
            Type = "TestTable",
            Objects =
            [
                new()
                {
                    ["Id"] = "1",
                    ["Name"] = "Updated Name",
                    ["Description"] = null,
                    ["UserId"] = "auth0|user-abc"
                },
                new()
                {
                    ["Id"] = "2",
                    ["Name"] = "Another Update",
                    ["Description"] = "Updated Description",
                    ["UserId"] = "User2"
                }
            ]
        };
        var userId = "auth0|user-abc";

        // Act
        var sql = _sqlGenerator.GenerateUpdateSql(dto, userId);

        // Assert
        var expectedSql =
            "UPDATE TestTable SET Name = 'Updated Name', Description = NULL, UserId = 'auth0|user-abc' WHERE Id = '1';" +
            "UPDATE TestTable SET Name = 'Another Update', Description = 'Updated Description', UserId = 'User2' WHERE Id = '2';";

        Assert.Equal(expectedSql, sql);
    }

    [Fact]
    public void GenerateUpdateSql_ShouldThrowException_WhenObjectsAreEmpty()
    {
        // Arrange
        var dto = new CreateUpdateDto
        {
            Type = "TestTable",
            Objects = []
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _sqlGenerator.GenerateUpdateSql(dto, "user"));
    }

    [Fact]
    public void GenerateUpdateSql_ShouldThrowException_WhenIdIsMissing()
    {
        // Arrange
        var dto = new CreateUpdateDto
        {
            Type = "TestTable",
            Objects =
            [
                new() { ["Name"] = "Test Name" } // Missing Id
            ]
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _sqlGenerator.GenerateUpdateSql(dto, "user"));
    }

    private static string NormalizeSql(string sql)
        => string.Join(" ", sql.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));

}
