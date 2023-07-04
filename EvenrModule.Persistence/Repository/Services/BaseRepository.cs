using EvenrModule.Persistence.Contexts;
using EvenrModule.Persistence.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvenrModule.Persistence.Repository.Services;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;
    private DbSet<T> _entities;

    public BaseRepository(ApplicationDbContext context)
    {
        _dbContext = context;
        _entities = context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }
        await _entities.AddAsync(entity);
    }

    public void Delete(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }
        _entities.Remove(entity);
    }

    public IEnumerable<T> GetAll()
    {
        return _entities.AsEnumerable();
    }

    public async Task<T> GetByIdAsync(object id)
    {
        var result = await _entities.FindAsync(id);
        return result;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public void Update(T entity)
    {
        _entities.Update(entity);
    }
}