namespace Uper.Backend.Repository.Common;

public static class SqlExtension
{
    /// <summary>
    /// Escapes special characters in the SQL query for safe usage.
    /// </summary>
    /// <param name="sql">The SQL query to escape.</param>
    /// <returns>The escaped SQL query.</returns>
    public static string EscapeSql(this string sql) => sql.Replace("\"", "\\\"");
}
