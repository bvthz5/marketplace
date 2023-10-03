using System.Linq.Expressions;
using MarketPlace.DataAccess.Model;

namespace MarketPlace.DataAccess.Interfaces;

public interface IProductRepostory : IRepository<Product>
{
    Dictionary<string, Expression<Func<Product, object>>> ColumnMapForSortBy { get; }

    Task<Product?> FindByIdAndStatusAsync(int productId, Product.ProductStatus status);

    Task<List<Product>> FindAllByCategoryOrBrandLikeAndPriceAndLocationBetweenAsync(int[] categoryIds, float startPrice, float endPrice, bool SortByDesc, Product.ProductStatus[] status, Dictionary<string, string?> args);

    Task<List<Product>> FindAllByUserIdAndCategoryAndLocationAndPriceBetweenAsync(int? userId, int[]? categoryIds, float startPrice, float endPrice, bool SortByDesc, Product.ProductStatus[]? status, Dictionary<string, string?> args);

    Task<List<Product>> FindByUserIAsync(int userId);

    Task DeleteProductAsync(int userId);

    Task<Dictionary<string, int>> GetActiveProductCountGroupByCategory();

    Task<List<Product>> FindByProductIdsAndStatusAsync(int[] productIds, Product.ProductStatus status);
}