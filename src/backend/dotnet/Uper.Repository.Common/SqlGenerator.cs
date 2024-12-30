using System.Text;
using Uper.Domain.Abstraction.Repository.Common;
using Uper.Domain.Request.Dto;

namespace Uper.Repository.Common;

internal class SqlGenerator : ISqlGenerator
{
    public string GenerateInsertParameterizedSql(CreateUpdateDto dto, string userId, IEnumerable<string> columnNames)
    {
        if (string.IsNullOrWhiteSpace(dto.Type))
            throw new ArgumentException("Type is required.", nameof(dto.Type));
        if (dto.Objects == null || !dto.Objects.Any())
            throw new ArgumentException("At least one object must be provided.", nameof(dto.Objects));
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required.", nameof(userId));

        // Ensure Id and UserId columns are included
        var allColumns = new HashSet<string>(columnNames, StringComparer.OrdinalIgnoreCase)
        {
            "Id",
            "UserId"
        };

        // Build the column list for SQL
        var columnList = string.Join(", ", allColumns);

        // Build value placeholders (e.g., @Id, @UserId)
        var valuePlaceholders = string.Join(", ", allColumns.Select(col => $"@{col}"));

        // Construct the base INSERT statement
        var sqlBuilder = new StringBuilder();
        sqlBuilder.AppendLine($"INSERT INTO {dto.Type} ({columnList}) VALUES");

        // Generate value placeholders for each object
        var objectIndex = 0;
        foreach (var obj in dto.Objects)
        {
            if (!obj.ContainsKey("Id"))
                throw new ArgumentException("Each object must contain an Id property.", nameof(dto.Objects));

            // Map object properties to placeholders
            var placeholders = allColumns.Select(col =>
            {
                if (col.Equals("UserId", StringComparison.OrdinalIgnoreCase))
                    return $"@UserId_{objectIndex}";

                return obj.ContainsKey(col) ? $"@{col}_{objectIndex}" : "NULL";
            });

            sqlBuilder.AppendLine($"({string.Join(", ", placeholders)}),");
            objectIndex++;
        }

        // Remove the trailing comma
        sqlBuilder.Length -= 3;
        sqlBuilder.Append(");");

        return sqlBuilder.ToString();
    }

    public string GenerateInsertSql(CreateUpdateDto dto, string userId, IEnumerable<string> columnNames)
    {
        if (string.IsNullOrWhiteSpace(dto.Type))
            throw new ArgumentException("Type is required.", nameof(dto.Type));
        if (dto.Objects == null || !dto.Objects.Any())
            throw new ArgumentException("At least one object must be provided.", nameof(dto.Objects));
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required.", nameof(userId));

        // Ensure Id and UserId columns are included
        var allColumns = new HashSet<string>(columnNames, StringComparer.OrdinalIgnoreCase)
        {
            "Id",
            "UserId"
        };

        // Build the column list for SQL
        var columnList = string.Join(", ", allColumns);

        // Generate value rows without escaping
        var valueRows = new List<string>();
        foreach (var obj in dto.Objects)
        {
            var rowValues = allColumns.Select(col =>
            {
                if (col.Equals("UserId", StringComparison.OrdinalIgnoreCase))
                    return $"'{userId}'";

                return obj.ContainsKey(col) ? $"'{obj[col]?.ToString() ?? "NULL"}'" : "NULL";
            });

            valueRows.Add($"({string.Join(", ", rowValues)})");
        }

        return $@"
        INSERT INTO {dto.Type} ({columnList}) VALUES
        {string.Join(", ", valueRows)};
    ";
    }


    public string GenerateUpdateSql(CreateUpdateDto dto, string userId, IEnumerable<string> columnNames)
    {
        throw new NotImplementedException();
    }

    //private static string EscapeSql(string value) => value.Replace("'", "''"); // Escape single quotes

}
