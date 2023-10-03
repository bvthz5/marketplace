using MarketPlace.DataAccess.Model;

namespace MarketPlace.DataAccess.Interfaces;

public interface IWishListRepository : IRepository<WishList>
{
    Task<WishList?> FindByProductIdAndUserIdAsync(int productId, int userId);

    Task DeleteByProductIdAndUserIdAsync(int productId, int userId);

    Task DeleteByProductId(int productId);

    Task<List<int>> FindUserIdsByProductIdAndNotUserId(int productId, int userId);

    Task<List<WishList>> FindByUserIdAsync(int userId);
}