using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.DataAccess.Repository;
public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
{
    private readonly MarketPlaceDbContext _dbContext;

    public OrderDetailsRepository(MarketPlaceDbContext dbContext) : base(dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<List<OrderDetails>> GetAll(int userId)
    {
        return await _dbContext.OrderDetails.Include(orderDetails => orderDetails.Product)
                                                .ThenInclude(product => product.CreatedUser)
                                            .Include(orderDetails => orderDetails.Product)
                                                .ThenInclude(product => product.Category)
                                            .Include(orderDetails => orderDetails.Order)
                                            .Include(orderDetails => orderDetails.Histories)
                                            .Where(order => order.Order.UserId == userId)
                                            .OrderByDescending(orderDetails => orderDetails.OrderDetailsId)
                                            .ToListAsync();
    }


    public async Task<List<OrderDetails>> FindByOrderId(int orderId)

    {
        return await _dbContext.OrderDetails
                            .Include(orderDetails => orderDetails.Histories)
                            .Include(orderDetails => orderDetails.Product)
                                .ThenInclude(product => product.CreatedUser)
                            .Include(orderDetails => orderDetails.Product)
                                .ThenInclude(product => product.Photos)
                            .Include(orderDetails => orderDetails.Product)
                                .ThenInclude(product => product.Category)
                            .Include(orderDetails => orderDetails.Order)
                                .ThenInclude(order => order.User)
                            .Where(orderDetails => orderDetails.OrderId == orderId).ToListAsync();
    }

    public async Task<List<OrderDetails>> FindBySellerId(int sellerId)
    {
        return await _dbContext.OrderDetails
                           .Include(orderDetails => orderDetails.Histories)
                           .Include(orderDetails => orderDetails.Product)
                               .ThenInclude(product => product.CreatedUser)
                           .Include(orderDetails => orderDetails.Order)
                           .Where(orderDetails => orderDetails.Product.CreatedUserId == sellerId).ToListAsync();

    }

    public async Task<Dictionary<DateTime, int>> GetSalesCount(DateTime from, DateTime to)
    {
        to = to.AddDays(1);

        return await _dbContext.OrderDetails
                                .Include(orderDetails => orderDetails.Order)
                                .Include(orderDetails => orderDetails.Histories)
                                .Where(orderDetails => orderDetails.CreatedDate > from &&
                                                        orderDetails.CreatedDate < to &&
                                                        orderDetails.Histories.OrderBy(o => o.OrderHistoryId).Last().Status != OrderHistory.HistoryStatus.CREATED &&
                                                        orderDetails.Order.PaymentStatus == Orders.PaymentsStatus.PAID)
                                .GroupBy(
                                     orderDetails => orderDetails.CreatedDate.Date,
                                     orderDetails => orderDetails.ProductId)
                                .Select(g => new { date = g.Key, count = g.Count() })
                                .ToDictionaryAsync(obj => obj.date, obj => obj.count);
    }
    public async Task<List<OrderDetails>> FindByBuyerIdAndStatus(int buyerId, OrderHistory.HistoryStatus status)
    {
        return await _dbContext.OrderDetails
                                .Include(orderDetails => orderDetails.Histories)
                                .Include(orderDetails => orderDetails.Product)
                                    .ThenInclude(product => product.CreatedUser)
                                .Include(orderDetails => orderDetails.Order)
                                    .ThenInclude(order => order.User)
                                .Where(order => order.Order.UserId == buyerId && order.Histories.OrderBy(o => o.OrderHistoryId).Last().Status == status).ToListAsync();
    }

    public async Task<List<OrderDetails>> FindByUserIdAndProductIdAndOrderStatus(int userId, int productId)
    {
        return await _dbContext.OrderDetails
                                .Include(orderDetails => orderDetails.Histories)
                                .Include(orderDetails => orderDetails.Product)
                                .Include(orderDetails => orderDetails.Order)
                                .Where(order => order.ProductId == productId &&
                                        order.Product.Status == Product.ProductStatus.ONPROCESS &&

                                        order.Order.UserId == userId &&
                                        order.Order.OrderStatus == Orders.OrdersStatus.CREATED).ToListAsync();


    }

    public async Task<List<OrderDetails>> FindByBuyerIdAndProductId(int buyerId, int productId)
    {
        return await _dbContext.OrderDetails
                                    .Include(orderDetails => orderDetails.Histories)
                                    .Include(orderDetails => orderDetails.Product)
                                        .ThenInclude(product => product.CreatedUser)
                                    .Include(orderDetails => orderDetails.Order)
                                        .ThenInclude(order => order.User)
                                    .Where(order => order.Order.UserId == buyerId && order.ProductId == productId)
                                    .ToListAsync();
    }

    public async Task<OrderDetails?> FindByOrderDetailsIdAndUserId(int orderDetailsId, int userId)
    {
        return await _dbContext.OrderDetails
                                    .Include(orderDetails => orderDetails.Product)
                                        .ThenInclude(product => product.CreatedUser)
                                    .Include(orderDetails => orderDetails.Product)
                                        .ThenInclude(product => product.Category)
                                    .Include(orderDetails => orderDetails.Histories)
                                    .Include(orderDetails => orderDetails.Order)
                                        .ThenInclude(order => order.User)
                                    .SingleOrDefaultAsync(order => order.Order.UserId == userId && order.OrderDetailsId == orderDetailsId);
    }

    public async Task<OrderDetails?> FindByOrderDetailsId(int orderDetailsId)
    {
        return await _dbContext.OrderDetails
                                .Include(orderDetails => orderDetails.Histories)
                                .Include(orderDetails => orderDetails.Product)
                                    .ThenInclude(product => product.CreatedUser)
                                .Include(orderDetails => orderDetails.Order)
                                    .ThenInclude(order => order.User)
                                .FirstOrDefaultAsync(orderDetail => orderDetail.OrderDetailsId == orderDetailsId);
    }

    public async Task<OrderDetails?> FindByOrderIdAndProductId(int orderId, int productId)
    {
        return await _dbContext.OrderDetails
                                      .Include(orderDetails => orderDetails.Product)
                                      .Include(orderDetails => orderDetails.Histories)
                                      .FirstOrDefaultAsync(orderDetails => orderDetails.OrderId == orderId && orderDetails.ProductId == productId);

    }

    public async Task<List<OrderDetails>> FindByOrderIdAndOrderDetailStatus(int orderId, OrderHistory.HistoryStatus status)
    {
        return await _dbContext.OrderDetails
                                      .Include(orderDetails => orderDetails.Product)
                                      .Include(orderDetails => orderDetails.Histories)
                                      .Where(orderDetails => orderDetails.OrderId == orderId && orderDetails.Histories.Last().Status == status)
                                      .ToListAsync();
    }
}