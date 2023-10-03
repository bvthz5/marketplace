using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Extensions;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MarketPlace.DataAccess.Repository
{
    public class OrderRepository : Repository<Orders>, IOrderRepository
    {
        private readonly MarketPlaceDbContext _dbContext;

        private static readonly string _primaryKey = "OrdersId";

        public Dictionary<string, Expression<Func<Orders, object>>> ColumnMapForSortBy { get; } = new()
        {
            [_primaryKey] = orders => orders.OrdersId,
            ["Price"] = orders => orders.TotalPrice,
            ["OrderDate"] = orders => orders.OrderDate,
            ["PaymentDate"] = orders => orders.PaymentDate
        };

        public Dictionary<string, Expression<Func<Orders, object>>> AgentColumnMapForSortBy { get; } = new()
        {
            ["OrderDate"] = orders => orders.OrderDate,
        };

        public OrderRepository(MarketPlaceDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Orders>> FindAll(string? search, string? sortBy, bool sortByDesc, Orders.PaymentsStatus[]? paymentStatus, int buyerId, Orders.OrdersStatus[]? orderStatus)
        {
            return await _dbContext.Orders
                                        .AsNoTracking()
                                        .Include(order => order.User)
                                        .Where(order =>
                                                (string.IsNullOrWhiteSpace(search)
                                                        || order.OrderNumber.Contains(search)
                                                        || new { Name = order.User.FirstName + " " + order.User.LastName }.Name.Contains(search))
                                                && (paymentStatus == null || !paymentStatus.Any() || paymentStatus.Contains(order.PaymentStatus))
                                                && (orderStatus == null || !orderStatus.Any() || orderStatus.Contains(order.OrderStatus))
                                                && (buyerId == 0 || order.UserId == buyerId)
                                         ).ApplyOrdering(sortBy ?? _primaryKey, sortByDesc, ColumnMapForSortBy)
                                        .ToListAsync();
        }

        public async Task<Orders?> FindByOrderIdAsync(int orderId)
        {
            return await _dbContext.Orders
                                    .Include(order => order.User)
                                    .Include(order => order.OrderDetails)
                                        .ThenInclude(orderDetails => orderDetails.Product)
                                            .ThenInclude(product => product.Category)
                                    .Include(order => order.OrderDetails)
                                        .ThenInclude(orderDetails => orderDetails.Product)
                                            .ThenInclude(product => product.Photos)
                                     .Include(order => order.OrderDetails)
                                        .ThenInclude(orderDetails => orderDetails.Product)
                                            .ThenInclude(product => product.CreatedUser)
                                    .Include(order => order.OrderDetails)
                                        .ThenInclude(orderDetails => orderDetails.Histories)
                                    .FirstOrDefaultAsync(order => order.OrdersId == orderId);
        }

        public async Task<Orders?> FindByOrderNumberAsync(string orderNumber)
        {
            return await _dbContext.Orders.FirstOrDefaultAsync(order => order.OrderNumber == orderNumber);
        }

        public async Task<List<Orders>> FindByUserIdAsync(int userId)
        {
            return await _dbContext.Orders
                                        .Include(order => order.User)
                                        .Where(order => order.UserId == userId).ToListAsync();
        }

        public async Task<List<Orders>> FindByZipcodeAndOrderStatusInAndAgentIdOrderBy(string? search, Orders.OrdersStatus[] orderStatus, int agentId, bool myProductsOnly, string? sortBy, bool sortByDesc)
        {
            IQueryable<Orders> query = _dbContext.Orders.Include(order => order.Agent).AsQueryable();

            query = query.Where(order => myProductsOnly ? order.AgentId == agentId : order.AgentId == null);

            query = query.Where(order => string.IsNullOrWhiteSpace(search) || order.DeliveryAddress.Contains($"\b{search}\b"));

            query = query.Where(order => orderStatus.Contains(order.OrderStatus));

            query = query.Where(order => order.OrderStatus != Orders.OrdersStatus.CANCELLED || (order.OrderStatus == Orders.OrdersStatus.CANCELLED && order.AgentId == agentId));

            query = query.ApplyOrdering(sortBy ?? "OrderDate", sortByDesc, AgentColumnMapForSortBy);


            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<Orders>> FindOrdersByAgentIdandStatusIn(int agentId, Orders.OrdersStatus[] status)
        {
            return await _dbContext.Orders.Where(order => order.AgentId == agentId && status.Contains(order.OrderStatus)).ToListAsync();
        }

        public async Task<Dictionary<Orders.OrdersStatus, int>> GetOrderStatusGroupByAgentId(int agnetId)
        {
            return await _dbContext.Orders
                                               .Where(order => order.AgentId == agnetId)
                                               .GroupBy(
                                                   order => order.OrderStatus,
                                                   order => order.AgentId)
                                              .Select(g => new { status = g.Key, count = g.Count() })
                                              .ToDictionaryAsync(obj => obj.status, obj => obj.count);
        }
    }
}