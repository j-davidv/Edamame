using LiteDB;
using TEST.Domain.Interfaces;

namespace TEST.Infrastructure.Persistence;

/// <summary>
/// Generic repository implementation for LiteDB with thread-safe operations.
/// Implements Dependency Inversion through IRepository interface.
/// </summary>
public class LiteDbRepository<T> : IRepository<T>
{
    private readonly ILiteDatabase _database;
    private readonly ILiteCollection<T> _collection;
    private static readonly ReaderWriterLockSlim _lock = new();
    private readonly string _collectionName;

    public LiteDbRepository(ILiteDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _collectionName = typeof(T).Name;
        _collection = _database.GetCollection<T>(_collectionName);
    }

    /// <summary>
    /// Gets an entity by its ID with thread-safe read operation.
    /// </summary>
    public async Task<T?> GetByIdAsync(string id)
    {
        return await Task.Run(() =>
        {
            _lock.EnterReadLock();
            try
            {
                try
                {
                    var objectId = new ObjectId(id);
                    return _collection.FindById(objectId);
                }
                catch (ArgumentException)
                {
                    return default;
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }
        });
    }

    /// <summary>
    /// Gets all entities with thread-safe read operation.
    /// </summary>
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Task.Run(() =>
        {
            _lock.EnterReadLock();
            try
            {
                return _collection.FindAll().ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        });
    }

    /// <summary>
    /// Creates a new entity with thread-safe write operation.
    /// </summary>
    public async Task<string> CreateAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        return await Task.Run(() =>
        {
            _lock.EnterWriteLock();
            try
            {
                var result = _collection.Insert(entity);
                // Return the ObjectId in its standard 24-character hex format
                return result.ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create {_collectionName}.", ex);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        });
    }

    /// <summary>
    /// Updates an existing entity with thread-safe write operation.
    /// </summary>
    public async Task<bool> UpdateAsync(string id, T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        return await Task.Run(() =>
        {
            _lock.EnterWriteLock();
            try
            {
                try
                {
                    var objectId = new ObjectId(id);
                    return _collection.Update(entity);
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update {_collectionName}.", ex);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        });
    }

    /// <summary>
    /// Deletes an entity with thread-safe write operation.
    /// </summary>
    public async Task<bool> DeleteAsync(string id)
    {
        return await Task.Run(() =>
        {
            _lock.EnterWriteLock();
            try
            {
                try
                {
                    var objectId = new ObjectId(id);
                    return _collection.Delete(objectId);
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to delete {_collectionName}.", ex);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        });
    }
}
