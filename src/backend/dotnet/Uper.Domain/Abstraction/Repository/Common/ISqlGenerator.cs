using System;
using Uper.Domain.Request.Dto;

namespace Uper.Domain.Abstraction.Repository.Common;

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

    /// <summary>
    /// Generates a DELETE SQL statement for the given entity type and ID.
    /// </summary>
    /// <param name="type">The type of the entity to delete.</param>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <param name="userId">The ID of the authenticated user.</param>
    /// <returns>A parameterized SQL DELETE statement as a string.</returns>
    string GenerateDeleteSql(string type, string id, string userId);

    /// <summary>
    /// Generates a SELECT ALL SQL statement for the given entity type scoped to the user.
    /// </summary>
    /// <param name="type">The type of the entity to select.</param>
    /// <param name="userId">The ID of the authenticated user.</param>
    /// <returns>A parameterized SQL SELECT ALL statement as a string.</returns>
    string GenerateSelectAllSql(string type, string userId);
}

