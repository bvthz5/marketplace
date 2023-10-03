using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.DataAccess.Repository;

public class WishListRepository : Repository<WishList>, IWishListRepository
{
    private readonly MarketPlaceDbContext _dbContext;

    public WishListRepository(MarketPlaceDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task DeleteByProductId(int productId)
    {
        _dbContext.WishLists.RemoveRange(
                                await _dbContext.WishLists.Where(wishList => wishList.ProductId == productId).ToListAsync());
    }

    public async Task DeleteByProductIdAndUserIdAsync(int productId, int userId)
    {
        _dbContext.WishLists.RemoveRange(await _dbContext.WishLists.Where(wishList => wishList.ProductId == productId && wishList.UserId == userId).ToListAsync());
    }

    public async Task<List<int>> FindUserIdsByProductIdAndNotUserId(int productId, int userId)
    {
        return await _dbContext.WishLists.Where(wishList => wishList.ProductId == productId && wishList.UserId != userId).Select(wishList => wishList.UserId).ToListAsync();
    }

    public async Task<WishList?> FindByProductIdAndUserIdAsync(int productId, int userId)
    {
        return await _dbContext.WishLists.FirstOrDefaultAsync(wishList => wishList.ProductId == productId && wishList.UserId == userId);
    }

    public async Task<List<WishList>> FindByUserIdAsync(int userId)
    {
        return await _dbContext.WishLists
                                    .Include(wishList => wishList.Product)
                                        .ThenInclude(product => product.Category)
                                    .Include(cart => cart.Product)
                                        .ThenInclude(product => product.CreatedUser)

                                    .Include(wishList => wishList.Product)
                                    .Include(wishList => wishList.User)
                                    .Where(wishList => wishList.UserId == userId)
                                    .OrderByDescending(wishList => wishList.WishListId)
                                    .ToListAsync();
    }
}
