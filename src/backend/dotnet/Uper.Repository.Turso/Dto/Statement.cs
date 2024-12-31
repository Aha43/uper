using System.Text.Json.Serialization;

namespace Uper.Repository.Turso.Dto;

public class Statement
{
    [JsonPropertyName("sql")]
    public string Sql { get; set; } = string.Empty;
}
