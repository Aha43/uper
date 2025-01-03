namespace Uper.Client.Domain.Abstraction;

public interface IUperClient
{
    Task<IEnumerable<T>?> GetAllAsync<T>() where T : IEntity;
    Task<T?> GetByIdAsync<T>(string id) where T : IEntity;
    Task CreateAsync<T>(T entity) where T : IEntity;
    Task UpdateAsync<T>(T entity) where T : IEntity;
    Task DeleteAsync<T>(string id) where T : IEntity;
}
