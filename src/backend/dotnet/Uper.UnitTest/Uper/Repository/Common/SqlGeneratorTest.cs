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
        var columnNames = new[] { "Id", "Name", "Description", "UserId" };

        // Act
        var sql = _sqlGenerator.GenerateInsertSql(dto, userId, columnNames);

        // Assert
        var expectedSql = @"
            INSERT INTO ExampleType (Id, Name, Description, UserId) VALUES
            ('uuid-123', 'Sample Object', 'A test object', 'auth0|user-abc'),
            ('uuid-124', 'Another Object', NULL, 'auth0|user-abc');
        ";

        Assert.NotNull(sql);
        Assert.Equal(NormalizeSql(expectedSql), NormalizeSql(sql));
    }

    private static string NormalizeSql(string sql)
        => string.Join(" ", sql.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));

}
