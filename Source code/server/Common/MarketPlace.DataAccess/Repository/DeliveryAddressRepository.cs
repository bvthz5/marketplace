using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.DataAccess.Repository;

public class DeliveryAddressRepository : Repository<DeliveryAddress>, IDeliveryAddressRepository
{
    private readonly MarketPlaceDbContext _dbContext;

    public DeliveryAddressRepository(MarketPlaceDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeliveryAddress?> FindByAddressIdAsync(int deliveryAddressId)
    {
        return await _dbContext.DeliveryAddresses.SingleOrDefaultAsync(deliveryAddress => deliveryAddress.DeliveryAddressId == deliveryAddressId);

    }

    public async Task<DeliveryAddress?> FindByUserIdAndAddressIdAsync(int userId, int deliveryAddressId)
    {
        return await _dbContext.DeliveryAddresses.SingleOrDefaultAsync(deliveryAddress => deliveryAddress.CreatedUserId == userId && deliveryAddress.DeliveryAddressId == deliveryAddressId);
    }

    public DeliveryAddress? FindByUserIdAndStatusAsync(int userId, DeliveryAddress.DeliveryAddressStatus status)
    {
        return _dbContext.DeliveryAddresses.SingleOrDefault(deliveryAddress => deliveryAddress.CreatedUserId == userId && deliveryAddress.Status == status);
    }

    public async Task<List<DeliveryAddress>> FindByUserIdAndStatusNotRemovedAsync(int userId)
    {
        return await _dbContext.DeliveryAddresses.Where(deliveryAddress => deliveryAddress.CreatedUserId == userId && deliveryAddress.Status != DeliveryAddress.DeliveryAddressStatus.REMOVED).ToListAsync();
    }

    public async Task<List<DeliveryAddress>> FindByUserIdAsync(int userId)
    {
        return await _dbContext.DeliveryAddresses.Where(deliveryAddress => deliveryAddress.CreatedUserId == userId).ToListAsync();
    }
}
