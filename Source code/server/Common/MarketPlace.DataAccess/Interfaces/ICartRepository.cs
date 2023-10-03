using MarketPlace.DataAccess.Model;

namespace MarketPlace.DataAccess.Interfaces;

public interface ICartRepository : IRepository<Cart>
{
    Task DeleteByProductIdAndUserIdAsync(int productId, int userId);

    Task<Cart?> FindByProductIdAndUserIdAsync(int productId, int userId);

    Task DeleteByProductId(int productId);

    Task<List<Cart>> FindByUserIdAsync(int userId);

    Task<List<Cart>> FindByUserIdAndProductStatusAsync(int userId, Product.ProductStatus status);

    Task DeleteByUserId(int userId);
}