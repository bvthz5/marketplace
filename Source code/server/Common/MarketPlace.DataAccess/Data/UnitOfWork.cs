using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Repository;

namespace MarketPlace.DataAccess.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MarketPlaceDbContext _dbContext;

        public UnitOfWork(MarketPlaceDbContext marketPlaceDbContext)
        {
            this._dbContext = marketPlaceDbContext;
        }

        public IAdminRepository AdminRepository => new AdminRepository(_dbContext);

        public IUserRepository UserRepository => new UserRepository(_dbContext);

        public IProductRepostory ProductRepostory => new ProductRepository(_dbContext);

        public ICategoryRepository CategoryRepository => new CategoryRepository(_dbContext);

        public IPhotoRepository PhotoRepository => new PhotoRepository(_dbContext);

        public IWishListRepository WishListRepository => new WishListRepository(_dbContext);

        public IOrderRepository OrderRepository => new OrderRepository(_dbContext);

        public IDeliveryAddressRepository DeliveryAddressRepository => new DeliveryAddressRepository(_dbContext);

        public IOrderDetailsRepository OrderDetailsRepository => new OrderDetailsRepository(_dbContext);

        public INotificationRepository NotificationRepository => new NotificationRepository(_dbContext);

        public ICartRepository CartRepository => new CartRepository(_dbContext);

        public IAgentRepository AgentRepository => new AgentRepository(_dbContext);

        public IOrderHistoryRepository OrderHistoryRepository => new OrderHistoryRepository(_dbContext);

        public async Task<bool> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
