using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.DataAccess.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly MarketPlaceDbContext _dbContext;

        public CartRepository(MarketPlaceDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteByProductIdAndUserIdAsync(int productId, int userId)
        {
            _dbContext.Cart.RemoveRange(await _dbContext.Cart.Where(cart => cart.ProductId == productId && cart.UserId == userId).ToListAsync());
        }

        public async Task DeleteByProductId(int productId)
        {
            _dbContext.Cart.RemoveRange(
                                    await _dbContext.Cart.Where(cart => cart.ProductId == productId).ToListAsync());
        }

        public async Task<Cart?> FindByProductIdAndUserIdAsync(int productId, int userId)
        {
            return await _dbContext.Cart.FirstOrDefaultAsync(cart => cart.ProductId == productId && cart.UserId == userId);
        }

        public async Task<List<Cart>> FindByUserIdAsync(int userId)
        {
            return await _dbContext.Cart
                                   .Include(cart => cart.Product)
                                        .ThenInclude(product => product.Category)
                                   .Include(cart => cart.Product)
                                         .ThenInclude(product => product.CreatedUser)

                                   .Include(cart => cart.Product)
                                   .Include(cart => cart.User)
                                   .Where(cart => cart.UserId == userId)
                                   .OrderByDescending(cart => cart.CartId)
                                   .ToListAsync();
        }

        public async Task<List<Cart>> FindByUserIdAndProductStatusAsync(int userId, Product.ProductStatus status)
        {
            return await _dbContext.Cart
                                   .Include(cart => cart.Product)
                                        .ThenInclude(product => product.Category)
                                   .Include(cart => cart.Product)
                                         .ThenInclude(product => product.CreatedUser)
                                   .Include(cart => cart.User)
                                   .Where(cart => cart.UserId == userId && cart.Product.Status == status)
                                   .ToListAsync();
        }

        public async Task DeleteByUserId(int userId)
        {

            List<Cart> soldCartItems = await _dbContext.Cart.Where(cart => cart.UserId == userId &&
                                            cart.Product.Status == Product.ProductStatus.SOLD).ToListAsync();

            if (soldCartItems.Count != 0)
                _dbContext.Cart.RemoveRange(soldCartItems);

        }
    }
}