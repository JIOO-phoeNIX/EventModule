

namespace EvenrModule.Persistence.Repository.Interfaces;

/// <summary>
/// This is the Base repository for CRUD operations
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public interface IBaseRepository<T> where T : class
{
    /// <summary>
    /// Used to perform Create operation
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task AddAsync(T entity);
    /// <summary>
    /// Used to perform delete operation
    /// </summary>
    /// <param name="entity"></param>
    public void Delete(T entity);
    /// <summary>
    /// Used to fetch all the records of the entity
    /// </summary>
    /// <returns></returns>
    public IEnumerable<T> GetAll();
    /// <summary>
    /// Used to Get an entity
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<T> GetByIdAsync(object id);
    /// <summary>
    /// Call this to commit the changes to the data store
    /// </summary>
    /// <returns></returns>
    public Task SaveChangesAsync();
    /// <summary>
    /// Used to Update an entity
    /// </summary>
    /// <param name="entity"></param>
    public void Update(T entity);
}