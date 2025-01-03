using Uper.Backend.Domain.Abstraction.Repository;
using Uper.Backend.Domain.Abstraction.Repository.Common;
using Uper.Backend.Domain.Request.Dto;

namespace Uper.Backend.Repository.Turso;

public sealed class TursoRepository(TursoClient tursoClient, ISqlGenerator sqlGenerator) : IRepository
{
    public async Task CreateAsync(CreateUpdateDto dto, string userId)
    {
        var sql = sqlGenerator.GenerateInsertSql(dto, userId);
        await tursoClient.ExecuteQueryAsync(sql);
    }

    public async Task DeleteAsync(string type, string id, string userId)
    {
        var sql = $"DELETE FROM {type} WHERE Id = '{id}' AND UserId = '{userId}';";
        await tursoClient.ExecuteQueryAsync(sql);
    }

    public async Task<IEnumerable<Dictionary<string, object?>>> GetAllAsync(string type, string userId)
    {
        var sql = $@"
        SELECT *
        FROM {type}
        WHERE UserId = '{userId}';";

        var jsonResponse = await tursoClient.ExecuteQueryAsync(sql);
        var parsedResponse = TursoResponseParser.Parse(jsonResponse);
        return parsedResponse.Rows;
    }

    public async Task<Dictionary<string, object?>?> GetByIdAsync(string type, string id, string userId)
    {
        var sql = $"SELECT * FROM {type} WHERE Id = '{id}' AND UserId = '{userId}';";
        var jsonResponse = await tursoClient.ExecuteQueryAsync(sql);
        var parsedResponse = TursoResponseParser.Parse(jsonResponse);
        return parsedResponse.Rows.FirstOrDefault();
    }

    public async Task UpdateAsync(CreateUpdateDto dto, string userId)
    {
        var sql = sqlGenerator.GenerateUpdateSql(dto, userId);
        await tursoClient.ExecuteQueryAsync(sql);
    }
}
