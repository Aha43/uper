namespace Uper.Backend.Repository.Turso;

public sealed class TursoResponseParser
{
    public class TursoResponse
    {
        public List<ColumnMetadata> Columns { get; set; } = [];
        public List<Dictionary<string, object?>> Rows { get; set; } = [];
    }

    public class ColumnMetadata
    {
        public string Name { get; set; } = string.Empty;
        public string? Decltype { get; set; } // Optional, may be null
    }

    public static TursoResponse Parse(string jsonResponse)
    {
        var response = new TursoResponse();

        var root = System.Text.Json.JsonDocument.Parse(jsonResponse).RootElement;
        var results = root.GetProperty("results").EnumerateArray();

        foreach (var result in results)
        {
            if (result.GetProperty("response").GetProperty("type").GetString() == "execute")
            {
                var executeResult = result.GetProperty("response").GetProperty("result");

                // Parse columns
                foreach (var column in executeResult.GetProperty("cols").EnumerateArray())
                {
                    response.Columns.Add(new ColumnMetadata
                    {
                        Name = column.GetProperty("name").GetString()!,
                        Decltype = column.TryGetProperty("decltype", out var decltype) ? decltype.GetString() : null
                    });
                }

                // Parse rows
                foreach (var row in executeResult.GetProperty("rows").EnumerateArray())
                {
                    var rowDict = new Dictionary<string, object?>();
                    for (int i = 0; i < row.GetArrayLength(); i++)
                    {
                        var cell = row[i];
                        var columnName = response.Columns[i].Name;

                        rowDict[columnName] = cell.GetProperty("value").ValueKind switch
                        {
                            System.Text.Json.JsonValueKind.String => cell.GetProperty("value").GetString(),
                            System.Text.Json.JsonValueKind.Number => cell.GetProperty("value").GetDecimal(),
                            System.Text.Json.JsonValueKind.Null => null,
                            _ => throw new InvalidOperationException($"Unexpected value type: {cell.GetProperty("value").ValueKind}")
                        };
                    }
                    response.Rows.Add(rowDict);
                }
            }
        }

        return response;
    }
}
