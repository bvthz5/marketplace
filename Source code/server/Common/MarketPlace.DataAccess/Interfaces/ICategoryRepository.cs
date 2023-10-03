using MarketPlace.DataAccess.Model;

namespace MarketPlace.DataAccess.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<List<Category>> FindAllAsync();

    Task<Category?> FindByIdAndStatusAsync(int categoryId, Category.CategoryStatus status);

    Task<List<Category>> FindAllByStatusAsync(Category.CategoryStatus status);

    Task<Category?> FindByCategoryNameAsync(string categoryName);
}