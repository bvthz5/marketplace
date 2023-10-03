using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Extensions;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MarketPlace.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepostory
{
    private readonly MarketPlaceDbContext _dbContext;

    private static readonly string _primaryKey = "ProductId";

    public Dictionary<string, Expression<Func<Product, object>>> ColumnMapForSortBy { get; } = new()
    {
        [_primaryKey] = product => product.ProductId,
        ["Price"] = product => product.Price,
        ["CreatedDate"] = product => product.CreatedDate,
        ["UpdatedDate"] = product => product.UpdatedDate
    };

    public ProductRepository(MarketPlaceDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public new async Task<Product?> FindById(int id)
    {
        return await _dbContext.Products
                                    .Include(product => product.CreatedUser)
                                    .Include(product => product.Photos)
                                    .Include(product => product.Category)
                                    .SingleOrDefaultAsync(product => product.ProductId == id);
    }

    public async Task<Product?> FindByIdAndStatusAsync(int productId, Product.ProductStatus status)
    {
        return await _dbContext.Products
                                    .Include(product => product.CreatedUser)

                                    .Include(product => product.Category)
                                    .SingleOrDefaultAsync(p => p.ProductId == productId && p.Status == status);
    }

    public async Task<List<Product>> FindAllByCategoryOrBrandLikeAndPriceAndLocationBetweenAsync(int[] categoryIds, float startPrice, float endPrice, bool SortByDesc, Product.ProductStatus[] status, Dictionary<string, string?> args)
    {
        string? Search = args.GetValueOrDefault("Search");
        string? Location = args.GetValueOrDefault("Location");
        string? SortBy = args.GetValueOrDefault("SortBy");
        return await _dbContext.Products
                                     .AsNoTracking()
                                     .Include(product => product.Category)
                                     .Include(product => product.CreatedUser)
                                     .Include(product => product.Photos)

                                     .Where(product => (categoryIds.Length == 0 || categoryIds.Contains(product.CategoryId)) &&
                                                        (status.Length == 0 || status.Contains(product.Status)) &&
                                                        product.Price >= startPrice && (endPrice == 0.0 || product.Price <= endPrice) &&
                                                        (string.IsNullOrWhiteSpace(Location) || product.Address.ToLower().Contains(Location.Trim().ToLower())) &&
                                                        product.Status != Product.ProductStatus.DRAFT &&
                                                        (string.IsNullOrWhiteSpace(Search) || product.ProductName.Contains(Search) || product.Category.CategoryName.Contains(Search) || (product.ProductDescription != null && product.ProductDescription.Contains(Search))))
                                     .ApplyOrdering(SortBy ?? _primaryKey, SortByDesc, ColumnMapForSortBy)
                                     .ToListAsync();
    }

    public async Task<List<Product>> FindByUserIAsync(int userId)
    {
        return await _dbContext.Products
                                    .Include(product => product.CreatedUser)
                                    .Include(product => product.Category)
                                    .Include(product => product.Photos)
                                    .Where(product => product.CreatedUserId == userId)
                                    .OrderByDescending(product => product.CreatedDate)
                                    .ToListAsync();
    }

    public async Task DeleteProductAsync(int userId)
    {
        var products = await _dbContext.Products.Where(product => product.CreatedUserId == userId).ToListAsync();

        foreach (var product in products)
        {
            product.Status = Product.ProductStatus.DELETED;
            _dbContext.Update(product);
        }
    }

    public async Task<Dictionary<string, int>> GetActiveProductCountGroupByCategory()
    {
        return await _dbContext.Products
                     .Where(product => product.Status == Product.ProductStatus.ACTIVE)
                     .GroupBy(
                         product => product.Category.CategoryName,
                         product => product.ProductId)
                     .Select(g => new { categoryName = g.Key, count = g.Count() })
                     .ToDictionaryAsync(obj => obj.categoryName, obj => obj.count);
    }

    public async Task<List<Product>> FindAllByUserIdAndCategoryAndLocationAndPriceBetweenAsync(int? userId, int[]? categoryIds, float startPrice, float endPrice, bool SortByDesc, Product.ProductStatus[]? status, Dictionary<string, string?> args)
    {
        string? location = args.GetValueOrDefault("Location");

        string? search = args.GetValueOrDefault("Search");

        string? sortBy = args.GetValueOrDefault("SortBy");

        IQueryable<Product> baseQuery = _dbContext.Products
                                                    .AsNoTracking()
                                                    .Include(product => product.Photos)
                                                    .Include(product => product.Category);

        if (userId != null)
            baseQuery = baseQuery.Include(product => product.CreatedUser);

        return await baseQuery
                        .Where(product => (categoryIds == null || !categoryIds.Any() || categoryIds.Contains(product.CategoryId)) &&
                                            (userId == null || product.CreatedUserId == userId) &&
                                            (status == null || !status.Any() || status.Contains(product.Status)) &&
                                            product.Price >= startPrice && (endPrice == 0.0 || product.Price <= endPrice) &&
                                            (string.IsNullOrWhiteSpace(location) || product.Address.ToLower().Contains(location.Trim().ToLower())) &&
                                            product.Status != Product.ProductStatus.DRAFT &&
                                            (string.IsNullOrWhiteSpace(search) || product.ProductName.Contains(search) || product.Category.CategoryName.Contains(search) || (product.ProductDescription != null && product.ProductDescription.Contains(search))))
                        .ApplyOrdering(sortBy ?? _primaryKey, SortByDesc, ColumnMapForSortBy)
                        .ToListAsync();
    }

    public async Task<List<Product>> FindByProductIdsAndStatusAsync(int[] productIds, Product.ProductStatus status)
    {
        return await _dbContext.Products
            .Include(product => product.CreatedUser)
            .Where(product => (productIds.Length == 0 || productIds.Contains(product.ProductId)) &&
                                                            product.Status == status).ToListAsync();
    }
}