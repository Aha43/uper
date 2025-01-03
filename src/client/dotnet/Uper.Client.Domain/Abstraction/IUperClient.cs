namespace Uper.Client.Domain.Abstraction;

public interface IUperClient
{
    Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
    Task GetByIdAsync<T>(int id) where T : class;
    Task CreateAsync<T>(T entity) where T : class;
    Task UpdateAsync<T>(T entity) where T : class;
    Task DeleteAsync<T>(int id) where T : class;
}
