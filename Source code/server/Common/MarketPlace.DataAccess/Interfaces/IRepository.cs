namespace MarketPlace.DataAccess.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> Add(T entity);

    Task Add(IEnumerable<T> entities);

    T Update(T entity);

    void Update(IEnumerable<T> entities);

    void Delete(T entity);

    void Delete(IEnumerable<T> entities);

    Task<T?> FindById(int id);
}