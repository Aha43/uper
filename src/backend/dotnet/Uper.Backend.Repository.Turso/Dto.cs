using System.Text.Json.Serialization;
using Uper.Backend.Repository.Common;

namespace Uper.Backend.Repository.Turso;

public class Statement
{
    [JsonPropertyName("sql")]
    public string Sql { get; set; } = string.Empty;
}

public class Request
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "close";

    [JsonPropertyName("stmt")]
    public Statement? Statement { get; set; }
}

public class TursoRequests
{
    [JsonPropertyName("requests")]
    public Request[] Requests { get; set; } = [];
}

public static class TursoRequest
{
    public static TursoRequests CreateTursoRequest(this string sql)
    {
        return new TursoRequests
        {
            Requests =
            [
                new Request
                {
                    Type = "execute",
                    Statement = new Statement
                    {
                        Sql = sql.EscapeSql()
                    }
                },
                new Request
                {
                    Type = "close"
                }
            ]
        };
    }

}