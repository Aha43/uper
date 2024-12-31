using System.Text;
using System.Text.Json;

namespace Uper.Repository.Turso;

public sealed class TursoClient(HttpClient httpClient)
{
    /// <summary>
    /// Executes a SQL query on the Turso database.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <returns>The raw JSON response from the Turso server.</returns>
    public async Task<string> ExecuteQueryAsync(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
        {
            throw new ArgumentException("SQL query cannot be null or empty.", nameof(sql));
        }

        var tursoRequests = sql.CreateTursoRequest();
        var json = JsonSerializer.Serialize(tursoRequests);

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await httpClient.PostAsync("/v2/pipeline", content);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

}
