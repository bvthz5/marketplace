using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly MarketPlaceDbContext _dbContext;

        public CategoryRepository(MarketPlaceDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Category>> FindAllAsync()
        {
            return await _dbContext.Categories.AsNoTracking().OrderByDescending(category => category.CategoryId).ToListAsync();
        }

        public async Task<List<Category>> FindAllByStatusAsync(Category.CategoryStatus status)
        {
            return await _dbContext.Categories
                                        .Where(category => category.Status == status)
                                        .OrderBy(category => category.CategoryName)
                                        .ToListAsync();
        }

        public async Task<Category?> FindByIdAndStatusAsync(int categoryId, Category.CategoryStatus status)
        {
            return await _dbContext.Categories.SingleOrDefaultAsync(category => category.CategoryId == categoryId && category.Status == status);
        }

        public async Task<Category?> FindByCategoryNameAsync(string categoryName)
        {
            return await _dbContext.Categories.SingleOrDefaultAsync(category => category.CategoryName.ToLower() == categoryName);
        }
    }
}
