using MarketPlace.DataAccess.Model;

namespace MarketPlace.DataAccess.Interfaces;

public interface IDeliveryAddressRepository : IRepository<DeliveryAddress>
{
    Task<List<DeliveryAddress>> FindByUserIdAsync(int userId);

    DeliveryAddress? FindByUserIdAndStatusAsync(int userId, DeliveryAddress.DeliveryAddressStatus status);

    Task<DeliveryAddress?> FindByUserIdAndAddressIdAsync(int userId, int deliveryAddressId);

    Task<DeliveryAddress?> FindByAddressIdAsync(int deliveryAddressId);

    Task<List<DeliveryAddress>> FindByUserIdAndStatusNotRemovedAsync(int userId);
}