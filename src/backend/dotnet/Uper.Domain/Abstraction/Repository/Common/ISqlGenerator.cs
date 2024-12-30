using Uper.Domain.Request.Dto;

namespace Uper.Domain.Abstraction.Repository.Common;

/*
 * ISqlGenerator Interface
 * 
 * Overview:
 * The ISqlGenerator interface provides methods to dynamically generate SQL statements for CRUD operations 
 * in the UPER project. It focuses on creating syntactically correct SQL strings based on the structure of 
 * the CreateUpdateDto and other parameters. However, it does not handle escaping or sanitization of input values.
 * 
 * Responsibility for Escaping:
 * - The SQL generated by this interface must be sanitized by another component or agent before being executed 
 *   against the database.
 * - In UPER, the TursoClient is an example of such an agent. It ensures all SQL generated by ISqlGenerator is 
 *   properly escaped and sanitized before being sent to the Turso HTTP API.
 * 
 * Why Escaping Is Delegated:
 * 1. **Separation of Concerns**: 
 *    ISqlGenerator focuses solely on generating valid SQL syntax. Delegating escaping to a separate agent 
 *    ensures that ISqlGenerator remains lightweight and adaptable for different backends.
 * 2. **Centralized Escaping**:
 *    By centralizing escaping in a specific agent like TursoClient, you ensure consistent handling of SQL 
 *    sanitization across all parts of the application.
 * 3. **Flexibility**:
 *    This design allows ISqlGenerator to be used in scenarios where escaping may not be required, or where 
 *    a different agent handles sanitization.
 * 
 * Escaping Example (TursoClient):
 * Suppose a property value contains a single quote, e.g., "O'Reilly". When generating an INSERT SQL:
 * 
 *     INSERT INTO ExampleType (Id, Name, UserId) VALUES ('uuid-123', 'O'Reilly', 'auth0|user-abc');
 * 
 * Without escaping, this SQL would result in a syntax error or open a vulnerability to SQL injection. The 
 * TursoClient, as the designated escaping agent, automatically escapes the value to:
 * 
 *     INSERT INTO ExampleType (Id, Name, UserId) VALUES ('uuid-123', 'O''Reilly', 'auth0|user-abc');
 * 
 * By delegating escaping, ISqlGenerator ensures its flexibility while relying on the centralized agent 
 * for security.
 * 
 * Usage Notes:
 * - ISqlGenerator is not responsible for escaping or sanitizing inputs.
 * - Ensure that all SQL generated by this interface is passed through a designated agent (e.g., TursoClient) 
 *   for escaping before execution.
 * 
 * Security Considerations:
 * - Any SQL generated by ISqlGenerator must be sanitized by a responsible agent to prevent SQL injection 
 *   vulnerabilities.
 * - Developers must ensure that the agent handling SQL sanitization is applied consistently to all queries.
 * 
 * Example Workflow:
 * 1. Repository calls ISqlGenerator to create an INSERT SQL string:
 *        INSERT INTO ExampleType (Id, Name, UserId) VALUES ('{Id}', '{Name}', '{UserId}');
 * 2. The SQL string is passed to the designated escaping agent (e.g., TursoClient).
 * 3. The escaping agent sanitizes the input and executes the query against the database.
 * 
 * This design allows ISqlGenerator to remain clean and reusable across different backends, while relying 
 * on an external agent for critical security functions like SQL sanitization.
 */
public interface ISqlGenerator
{
    /// <summary>
    /// Generates an INSERT SQL statement for the given entity type and objects.
    /// </summary>
    /// <param name="dto">The CreateUpdateDto containing the type and objects to insert.</param>
    /// <param name="userId">The ID of the authenticated user.</param>
    /// <param name="columnNames">The column names for the given entity type.</param>
    /// <returns>A parameterized SQL INSERT statement as a string.</returns>
    string GenerateInsertParameterizedSql(CreateUpdateDto dto, string userId, IEnumerable<string> columnNames);

    /// <summary>
    /// Generates an INSERT SQL statement for the given entity type and objects not parameterized.
    /// 
    /// </summary>
    /// <param name="dto">The CreateUpdateDto containing the type and objects to insert.</param>
    /// <param name="userId">The ID of the authenticated user.</param>
    /// <param name="columnNames">The column names for the given entity type.</param>
    /// <returns>A SQL INSERT statement as a string.</returns>
    string GenerateInsertSql(CreateUpdateDto dto, string userId, IEnumerable<string> columnNames);

    /// <summary>
    /// Generates an UPDATE SQL statement for the given entity type and objects.
    /// </summary>
    /// <param name="dto">The CreateUpdateDto containing the type and objects to update.</param>
    /// <param name="userId">The ID of the authenticated user.</param>
    /// <param name="columnNames">The column names for the given entity type.</param>
    /// <returns>A parameterized SQL UPDATE statement as a string.</returns>
    string GenerateUpdateSql(CreateUpdateDto dto, string userId, IEnumerable<string> columnNames);
}

