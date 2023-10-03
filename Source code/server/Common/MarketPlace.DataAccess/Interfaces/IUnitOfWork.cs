namespace MarketPlace.DataAccess.Interfaces;

public interface IUnitOfWork
{
    IAdminRepository AdminRepository { get; }

    IUserRepository UserRepository { get; }

    IProductRepostory ProductRepostory { get; }

    ICategoryRepository CategoryRepository { get; }

    IOrderRepository OrderRepository { get; }

    IPhotoRepository PhotoRepository { get; }

    IWishListRepository WishListRepository { get; }

    ICartRepository CartRepository { get; }

    IDeliveryAddressRepository DeliveryAddressRepository { get; }

    IOrderDetailsRepository OrderDetailsRepository { get; }

    IOrderHistoryRepository OrderHistoryRepository { get; }

    INotificationRepository NotificationRepository { get; }

    IAgentRepository AgentRepository { get; }

    Task<bool> SaveAsync();
}