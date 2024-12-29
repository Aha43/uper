using Microsoft.Extensions.DependencyInjection;
using Uper.Domain.Abstraction.Repository.Common;
using Uper.Domain.Request.Dto;
using Uper.Repository.Common;

namespace Uper.UnitTest.Uper.Repository.Common;

public class SqlGeneratorTest
{
    private ISqlGenerator _sqlGenerator;

    public SqlGeneratorTest()
    {
        var serviceProvider = new ServiceCollection()
            .AddCommonRepositoryServices()
            .BuildServiceProvider();

        _sqlGenerator = serviceProvider.GetRequiredService<ISqlGenerator>();
    }

    [Fact]
    public void GenerateInsertSql_ShouldGenerateCorrectSql()
    {
        // Arrange
        var dto = new CreateUpdateDto
        {
            Type = "ExampleType",
            Objects = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                { "Id", "uuid-123" },
                { "Name", "Sample Object" },
                { "Description", "A test object" }
            },
            new Dictionary<string, object>
            {
                { "Id", "uuid-124" },
                { "Name", "Another Object" }
            }
        }
        };

        var userId = "auth0|user-abc";
        var columnNames = new[] { "Id", "Name", "Description", "UserId" };

        // Act
        var sql = _sqlGenerator.GenerateInsertSql(dto, userId, columnNames);

        // Assert
        var expectedSql = @"
        INSERT INTO ExampleType (Id, Name, Description, UserId) VALUES
        (@Id_0, @Name_0, @Description_0, @UserId_0),
        (@Id_1, @Name_1, NULL, @UserId_1);";

        var normalizedSql = NormalizeSql(sql);
        var normalizedExpectedSql = NormalizeSql(expectedSql);

        Assert.NotNull(sql);
        Assert.Equal(normalizedExpectedSql, normalizedSql);
    }

    private static string NormalizeSql(string sql)
    {
        return string.Join(" ", sql.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));
    }

}
