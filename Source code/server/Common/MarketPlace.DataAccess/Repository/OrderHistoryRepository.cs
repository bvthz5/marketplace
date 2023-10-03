using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;

namespace MarketPlace.DataAccess.Repository;
public class OrderHistoryRepository : Repository<OrderHistory>, IOrderHistoryRepository
{
    public OrderHistoryRepository(MarketPlaceDbContext dbContext) : base(dbContext)
    {

    }
}