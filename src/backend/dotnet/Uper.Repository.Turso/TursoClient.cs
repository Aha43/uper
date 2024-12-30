using System.Text;

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
            throw new ArgumentException("SQL query cannot be null or empty.", nameof(sql));

        var content = new StringContent($"{{\"sql\": \"{EscapeSql(sql)}\"}}", Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("/query", content);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Escapes special characters in the SQL query for safe usage.
    /// </summary>
    /// <param name="sql">The SQL query to escape.</param>
    /// <returns>The escaped SQL query.</returns>
    private static string EscapeSql(string sql) => sql.Replace("\"", "\\\"");
}
