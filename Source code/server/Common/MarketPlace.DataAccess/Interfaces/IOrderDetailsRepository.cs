using MarketPlace.DataAccess.Model;

namespace MarketPlace.DataAccess.Interfaces;

public interface IOrderDetailsRepository : IRepository<OrderDetails>
{
    Task<OrderDetails?> FindByOrderDetailsId(int orderDetailsId);

    Task<List<OrderDetails>> GetAll(int userId);

    Task<List<OrderDetails>> FindByOrderId(int orderId);

    Task<List<OrderDetails>> FindByOrderIdAndOrderDetailStatus(int orderId, OrderHistory.HistoryStatus status);

    Task<List<OrderDetails>> FindBySellerId(int sellerId);

    Task<List<OrderDetails>> FindByBuyerIdAndStatus(int buyerId, OrderHistory.HistoryStatus status);

    Task<List<OrderDetails>> FindByBuyerIdAndProductId(int buyerId, int productId);

    Task<Dictionary<DateTime, int>> GetSalesCount(DateTime from, DateTime to);

    Task<List<OrderDetails>> FindByUserIdAndProductIdAndOrderStatus(int userId, int productId);

    Task<OrderDetails?> FindByOrderDetailsIdAndUserId(int orderDetailsId, int userId);

    Task<OrderDetails?> FindByOrderIdAndProductId(int orderId, int productId);
}
