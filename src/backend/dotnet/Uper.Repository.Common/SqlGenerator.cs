using System.Text;
using Uper.Domain.Abstraction.Repository.Common;
using Uper.Domain.Request.Dto;

namespace Uper.Repository.Common;

internal class SqlGenerator : ISqlGenerator
{
    public string GenerateInsertSql(CreateUpdateDto dto, string userId, IEnumerable<string> columnNames)
    {
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

                return obj.ContainsKey(col)
                    ? (obj[col] == null ? "NULL" : $"'{obj[col].ToString()}'")
                    : "NULL";
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
        var sb = new StringBuilder();

        foreach (var obj in dto.Objects)
        {
            if (!obj.ContainsKey("Id"))
                throw new ArgumentException("Each object must include an 'Id' key.", nameof(dto.Objects));

            sb.Append("UPDATE ").Append(dto.Type).Append(" SET ");

            var setClauses = columnNames
                .Where(col => col != "Id") // Exclude "Id" from the SET clause
                .Select(col =>
                {
                    var value = obj.ContainsKey(col) && obj[col] != null
                        ? FormatValue(obj[col])
                        : "NULL";
                    return $"{col} = {value}";
                });

            sb.Append(string.Join(", ", setClauses));

            var idValue = FormatValue(obj["Id"]);
            sb.Append(" WHERE Id = ").Append(idValue).Append(";");
        }

        return sb.ToString();
    }

    private static string? FormatValue(object value)
    {
        return value switch
        {
            string str => $"'{str}'",
            int or long or double or decimal => value.ToString(),
            bool boolVal => boolVal ? "1" : "0",
            null => "NULL",
            _ => throw new ArgumentException($"Unsupported value type: {value.GetType()}.")
        };
    }

}
