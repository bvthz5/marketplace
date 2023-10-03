using MarketPlace.DataAccess.Model;
using System.Linq.Expressions;

namespace MarketPlace.DataAccess.Interfaces;

public interface IOrderRepository : IRepository<Orders>
{
    Dictionary<string, Expression<Func<Orders, object>>> ColumnMapForSortBy { get; }
    Dictionary<string, Expression<Func<Orders, object>>> AgentColumnMapForSortBy { get; }

    Task<Orders?> FindByOrderIdAsync(int orderId);

    Task<List<Orders>> FindOrdersByAgentIdandStatusIn(int agentId, Orders.OrdersStatus[] status);

    Task<Orders?> FindByOrderNumberAsync(string orderNumber);

    Task<List<Orders>> FindAll(string? search, string? sortBy, bool sortByDesc, Orders.PaymentsStatus[]? paymentStatus, int buyerId, Orders.OrdersStatus[]? orderStatus);

    Task<List<Orders>> FindByZipcodeAndOrderStatusInAndAgentIdOrderBy(string? search, Orders.OrdersStatus[] orderStatus, int agentId, bool myProductsOnly, string? sortBy, bool sortByDesc);

    Task<List<Orders>> FindByUserIdAsync(int userId);

    Task<Dictionary<Orders.OrdersStatus, int>> GetOrderStatusGroupByAgentId(int agnetId);
}