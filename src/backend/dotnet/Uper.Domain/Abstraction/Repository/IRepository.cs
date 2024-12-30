using Uper.Domain.Request.Dto;

namespace Uper.Domain.Abstraction.Repository;

public interface IRepository
{
    /// <summary>
    /// Retrieves all entities of a specified type scoped to the authenticated user.
    /// </summary>
    /// <param name="type">The type of the entities to retrieve.</param>
    /// <param name="userId">The ID of the authenticated user.</param>
    /// <returns>A collection of entities as dictionaries representing the retrieved data.</returns>
    Task<IEnumerable<Dictionary<string, object>>> GetAllAsync(string type, string userId);

    /// <summary>
    /// Retrieves a single entity by its ID and type scoped to the authenticated user.
    /// </summary>
    /// <param name="type">The type of the entity to retrieve.</param>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <param name="userId">The ID of the authenticated user.</param>
    /// <returns>The entity as a dictionary if found, or null if not found.</returns>
    Task<Dictionary<string, object>?> GetByIdAsync(string type, string id, string userId);

    /// <summary>
    /// Creates new entities of the specified type scoped to the authenticated user.
    /// </summary>
    /// <param name="dto">The CreateUpdateDto containing the entities to create.</param>
    /// <param name="userId">The ID of the authenticated user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateAsync(CreateUpdateDto dto, string userId);

    /// <summary>
    /// Updates existing entities of the specified type scoped to the authenticated user.
    /// </summary>
    /// <param name="dto">The CreateUpdateDto containing the entities to update.</param>
    /// <param name="userId">The ID of the authenticated user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(CreateUpdateDto dto, string userId);

    /// <summary>
    /// Deletes an entity by its ID and type scoped to the authenticated user.
    /// </summary>
    /// <param name="type">The type of the entity to delete.</param>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <param name="userId">The ID of the authenticated user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(string type, string id, string userId);
}

