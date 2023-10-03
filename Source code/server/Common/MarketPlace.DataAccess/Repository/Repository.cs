using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbSet<T> _dbSet;

    public Repository(MarketPlaceDbContext dbContext)
    {
        _dbSet = dbContext.Set<T>();
    }

    public async Task<T> Add(T entity)
    {
        return (await _dbSet.AddAsync(entity)).Entity;
    }

    public async Task Add(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void Delete(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<T?> FindById(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public T Update(T entity)
    {
        return _dbSet.Update(entity).Entity;
    }

    public void Update(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }
}
