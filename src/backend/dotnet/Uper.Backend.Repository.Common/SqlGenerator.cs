using System.Text;
using Uper.Backend.Domain.Abstraction.Repository.Common;
using Uper.Backend.Domain.Request.Dto;

namespace Uper.Backend.Repository.Common;

internal class SqlGenerator : ISqlGenerator
{
    public string GenerateInsertSql(CreateUpdateDto dto, string userId)
    {
        if (dto.Objects.Count == 0)
            throw new ArgumentException("At least one object must be provided.", "dto.Objects");

        var columnNames = dto.Objects.FirstOrDefault()?.Keys ?? Enumerable.Empty<string>();

        var allColumns = new HashSet<string>(columnNames, StringComparer.OrdinalIgnoreCase)
        {
            "Id",
            "UserId"
        };

        var columnList = string.Join(", ", allColumns);

        var valueRows = new List<string>();
        foreach (var obj in dto.Objects)
        {
            var rowValues = allColumns.Select(col =>
            {
                if (col.Equals("UserId", StringComparison.OrdinalIgnoreCase))
                    return $"'{userId}'";

                return obj.TryGetValue(col, out var value)
                    ? value == null ? "NULL" : $"'{value}'"
                    : "NULL";
            });

            valueRows.Add($"({string.Join(", ", rowValues)})");
        }

        return $@"
        INSERT INTO {dto.Type} ({columnList}) VALUES
        {string.Join(", ", valueRows)};
    ";
    }

    public string GenerateUpdateSql(CreateUpdateDto dto, string userId)
    {
        if (dto.Objects.Count == 0)
            throw new ArgumentException("At least one object must be provided.", "dto.Objects");

        var columnNames = dto.Objects.FirstOrDefault()?.Keys ?? Enumerable.Empty<string>();

        var sb = new StringBuilder();

        foreach (var obj in dto.Objects)
        {
            if (!obj.TryGetValue("Id", out var idValue) || idValue == null)
                throw new ArgumentException("Each object must include a non-null 'Id' key.", "dto.Objects");

            sb.Append("UPDATE ").Append(dto.Type).Append(" SET ");

            var setClauses = columnNames
                .Where(col => col != "Id") // Exclude "Id" from the SET clause
                .Select(col =>
                {
                    if (obj.TryGetValue(col, out var value))
                    {
                        return value == null ? $"{col} = NULL" : $"{col} = {FormatValue(value)}";
                    }
                    else
                    {
                        return $"{col} = NULL";
                    }
                });

            sb.Append(string.Join(", ", setClauses));
            sb.Append(" WHERE Id = '").Append(idValue).Append("';");
        }

        return sb.ToString();
    }

    private static string? FormatValue(object? value)
    {
        return value switch
        {
            string str => $"'{str}'",
            int or long or double or decimal => $"'{value}'",
            bool boolVal => boolVal ? "1" : "0",
            null => "NULL",
            _ => throw new ArgumentException($"Unsupported value type: {value.GetType()}.")
        };
    }

}
