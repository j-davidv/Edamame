namespace TEST.Domain.Interfaces;

/// <summary>
/// Abstraction for generic repository operations (Dependency Inversion).
/// </summary>
public interface IRepository<T>
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<string> CreateAsync(T entity);
    Task<bool> UpdateAsync(string id, T entity);
    Task<bool> DeleteAsync(string id);
}
