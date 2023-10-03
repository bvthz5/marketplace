using System.Linq;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using MarketPlaceAdmin.Bussiness.Util;

namespace MarketPlaceAdmin.Bussiness.Services;

public class AgentOrderService : IAgentOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly ISecurityUtil _securityUtil;
    private readonly IEmailService _emailService;
    private readonly byte[] _allowedStatus;

    public AgentOrderService(IUnitOfWork uow, ISecurityUtil securityUtil, IEmailService emailService)
    {
        _uow = uow;
        _securityUtil = securityUtil;
        _emailService = emailService;

        _allowedStatus = new byte[] {
            (byte)Orders.OrdersStatus.CONFIRMED,
            (byte) Orders.OrdersStatus.WAITING_FOR_PICKUP,
            (byte) Orders.OrdersStatus.INTRANSIT,
            (byte) Orders.OrdersStatus.CANCELLED,
            (byte) Orders.OrdersStatus.OUTFORDELIVERY,
            (byte) Orders.OrdersStatus.DELIVERED,
        };
    }

    /// <summary>
    /// Retrieves a paginated list of orders for the current agent based on the specified search query and filter parameters.
    /// </summary>
    /// <param name="form">An instance of <c>AgentOrderPaginationParams</c> class containing search and filter parameters.</param>
    /// <returns>A <c>Task</c> object representing the asynchronous operation. The task result contains a <c>ServiceResult</c> object that indicates the success or failure of the operation and contains the paginated list of orders in its <c>Data</c> property if successful.</returns>
    public async Task<ServiceResult> GetOrderList(AgentOrderPaginationParams form)
    {
        ServiceResult result = new();
        // Validate OrderStatus

        byte[]? orderStatus = form.OrderStatus?.Where(orderStatus => orderStatus.HasValue).Cast<byte>().ToArray();

        if (orderStatus != null && !orderStatus.All(status => _allowedStatus.Contains(status)))
        {
            result.ServiceStatus = ServiceStatus.BadRequest;
            result.Message = "Invalid OrderStatus Value";
            return result;
        }


        // Check if SortBy value passed is defined and acceptable
        if (form.SortBy != null && !_uow.OrderRepository.AgentColumnMapForSortBy.ContainsKey(form.SortBy))
        {
            result.ServiceStatus = ServiceStatus.BadRequest;
            result.Message = $"SortBy : Accepts [{string.Join(", ", _uow.OrderRepository.AgentColumnMapForSortBy.Keys)}] values only";
            return result;
        }

        // Get a list of OrderView objects based on the search query and filter parameters
        List<Orders> agentOrders = await _uow.OrderRepository.FindByZipcodeAndOrderStatusInAndAgentIdOrderBy(form.Search?.Trim(), orderStatus?.Cast<Orders.OrdersStatus>().ToArray() ?? _allowedStatus.Cast<Orders.OrdersStatus>().ToArray(), _securityUtil.GetCurrentUserId(), form.MyProductsOnly, form.SortBy, form.SortByDesc);

        // Paginate the orderViews list and assign it to the ServiceResult.Data property
        Pager<AgentOrderView> pager = new(form.PageNumber, form.PageSize, agentOrders.Count);

        pager.SetResult(agentOrders.Skip((form.PageNumber - 1) * form.PageSize)
                                   .Take(form.PageSize)
                                   .Select(agentOrder => new AgentOrderView(agentOrder)));

        result.ServiceStatus = ServiceStatus.Success;
        result.Message = "Paginated Order List";
        result.Data = pager;

        return result;
    }

    /// <summary>
    /// Assigns an order to the current agent.
    /// </summary>
    /// <param name="orderId">The ID of the order to assign.</param>
    /// <returns>A <c>Task</c> object representing the asynchronous operation. The task result contains a <c>ServiceResult</c> object that indicates the success or failure of the operation.</returns>
    public async Task<ServiceResult> AssignOrder(int orderId)
    {
        ServiceResult result = new();

        // Find the order by ID
        Orders? order = await _uow.OrderRepository.FindByOrderIdAsync(orderId);

        // If the order is null return a not found error message
        if (order is null || order.OrderStatus != Orders.OrdersStatus.CONFIRMED)
        {
            result.ServiceStatus = ServiceStatus.NotFound;
            result.Message = "Order Not Found";
            return result;
        }

        List<Orders> orders = await _uow.OrderRepository.FindOrdersByAgentIdandStatusIn(_securityUtil.GetCurrentUserId(), new Orders.OrdersStatus[] { Orders.OrdersStatus.WAITING_FOR_PICKUP, Orders.OrdersStatus.INTRANSIT, Orders.OrdersStatus.OUTFORDELIVERY });

        if (orders.Count >= 10)
        {
            result.ServiceStatus = ServiceStatus.NotFound;
            result.Message = "Max Limit (10) exceed";
            return result;
        }

        for (int i = 0; i < order.OrderDetails.Count; i++)
        {
            if (order.OrderDetails[i].Histories.Last().Status == OrderHistory.HistoryStatus.CONFIRMED)
            {
                order.OrderDetails[i].Histories.Add(
                                    new OrderHistory()
                                    {
                                        Status = OrderHistory.HistoryStatus.WAITING_FOR_PICKUP,
                                        Date = DateTime.Now,
                                    });
            }
        }

        // Update the agent id and update date
        order.AgentId = _securityUtil.GetCurrentUserId();
        order.OrderStatus = Orders.OrdersStatus.WAITING_FOR_PICKUP;

        // Save the changes to the database
        _uow.OrderRepository.Update(order);
        await _uow.SaveAsync();

        result.ServiceStatus = ServiceStatus.Success;
        result.Message = "Agent Assigned";
        return result;
    }

    /// <summary>
    /// Unassigns an order from the current agent.
    /// </summary>
    /// <param name="orderId">The ID of the order to unassign.</param>
    /// <returns>A <c>Task</c> object representing the asynchronous operation. The task result contains a <c>ServiceResult</c> object that indicates the success or failure of the operation.</returns>
    public async Task<ServiceResult> UnAssignOrder(int orderId)
    {
        ServiceResult result = new();

        // Find the order by ID
        Orders? order = await _uow.OrderRepository.FindByOrderIdAsync(orderId);

        // If the order is null return a not found error message
        if (order is null || order.OrderStatus != Orders.OrdersStatus.WAITING_FOR_PICKUP || order.AgentId != _securityUtil.GetCurrentUserId())
        {
            result.ServiceStatus = ServiceStatus.NotFound;
            result.Message = "Order Not Found";
            return result;
        }

        for (int i = 0; i < order.OrderDetails.Count; i++)
        {
            if (order.OrderDetails[i].Histories.Last().Status == OrderHistory.HistoryStatus.WAITING_FOR_PICKUP)
            {
                order.OrderDetails[i].Histories.RemoveAt(order.OrderDetails[i].Histories.Count - 1);
            }
        }

        // Update the agent id and update date
        order.AgentId = null;
        order.OrderStatus = Orders.OrdersStatus.CONFIRMED;

        // Save the changes to the database
        _uow.OrderRepository.Update(order);
        await _uow.SaveAsync();

        result.ServiceStatus = ServiceStatus.Success;
        result.Message = "Agent Removed";
        return result;
    }
    /// <summary>
    /// Get the order details for a given order ID and agent ID.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <param name="agentId">The unique identifier of the agent.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains a <see cref="ServiceResult"/> object that indicates whether the operation was successful and includes the order detail view if applicable.</returns>
    public async Task<ServiceResult> GetOrderDetails(int orderId, int agentId)
    {
        ServiceResult result = new();

        // Find the order by its unique identifier
        Orders? order = await _uow.OrderRepository.FindByOrderIdAsync(orderId);

        // If the order is not found, return a not found status with a message
        if (order is null || !_allowedStatus.Contains((byte)order.OrderStatus))
        {
            result.ServiceStatus = ServiceStatus.NotFound;
            result.Message = "Order Not Found";
            return result;
        }

        if (order.OrderStatus != Orders.OrdersStatus.CONFIRMED && order.AgentId != agentId)
        {
            result.ServiceStatus = ServiceStatus.NotFound;
            result.Message = "Order Not Found";
            return result;
        }

        // Get the list of order details associated with the order
        List<OrderDetails> orderDetailList = await _uow.OrderDetailsRepository.FindByOrderId(orderId);

        // Generate the order detail view
        OrderDetailView orderDetailView = new(order)
        {
            Items = orderDetailList
                        .Where(orderDetails => (orderDetails.Histories.Last().Status != OrderHistory.HistoryStatus.CANCELLED) || (orderDetails.Histories.Last().Status == OrderHistory.HistoryStatus.CANCELLED && order.AgentId == agentId))
                        .Select(orderDetail => new OrderDetailsProductView(orderDetail.OrderDetailsId, new ProductDetailView(orderDetail.Product), orderDetail.Histories.Last().Status, orderDetail.Histories.Last().Remark))
        };

        // Set the result data to the order detail view
        result.Data = orderDetailView;
        result.ServiceStatus = ServiceStatus.Success;
        result.Message = "Order Detail View";
        return result;
    }

    private static Orders ChangeStatusToIntrasit(Orders order)
    {
        for (int i = 0; i < order.OrderDetails.Count; i++)
        {
            if (order.OrderDetails[i].Histories.Last().Status == OrderHistory.HistoryStatus.WAITING_FOR_PICKUP)
            {
                order.OrderDetails[i].Histories.Add(
                                    new OrderHistory()
                                    {
                                        Status = OrderHistory.HistoryStatus.INTRANSIT,
                                        Date = DateTime.Now,
                                    });
            }
        }

        order.OrderStatus = Orders.OrdersStatus.INTRANSIT;
        return order;
    }

    private static Orders ChangeStatusToOutForDelivery(Orders order)
    {
        for (int i = 0; i < order.OrderDetails.Count; i++)
        {
            if (order.OrderDetails[i].Histories.Last().Status == OrderHistory.HistoryStatus.INTRANSIT)
            {
                order.OrderDetails[i].Histories.Add(
                                    new OrderHistory()
                                    {
                                        Status = OrderHistory.HistoryStatus.OUTFORDELIVERY,
                                        Date = DateTime.Now,
                                    });
            }
        }

        order.OrderStatus = Orders.OrdersStatus.OUTFORDELIVERY;
        return order;
    }

    /// <summary>
    /// Changes the delivery status of an order to In-Transit.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <param name="status">The delivery status to be changed to.</param>
    /// <param name="agentId">The unique identifier of the agent.</param>
    /// <returns>A ServiceResult object containing the status of the operation and a message.</returns>
    public async Task<ServiceResult> ChangeDeliveryStatus(int orderId, byte status, int agentId)
    {
        ServiceResult result = new();

        // Find the order by its unique identifier
        Orders? order = await _uow.OrderRepository.FindByOrderIdAsync(orderId);

        // If the order is not found, return a not found status with a message
        if (order is null || order.AgentId != agentId || (order.OrderStatus != Orders.OrdersStatus.WAITING_FOR_PICKUP && order.OrderStatus != Orders.OrdersStatus.INTRANSIT))
        {
            result.ServiceStatus = ServiceStatus.NotFound;
            result.Message = "Order Not Found";
            return result;
        }

        if (status == (byte)Orders.OrdersStatus.INTRANSIT)
        {
            if (order.OrderStatus != Orders.OrdersStatus.WAITING_FOR_PICKUP)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Status";
                return result;
            }

            order = ChangeStatusToIntrasit(order);
        }
        else if (status == (byte)Orders.OrdersStatus.OUTFORDELIVERY)
        {
            if (order.OrderStatus != Orders.OrdersStatus.INTRANSIT)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Status";
                return result;
            }

            order = ChangeStatusToOutForDelivery(order);
        }
        else
        {
            result.ServiceStatus = ServiceStatus.BadRequest;
            result.Message = "Invalid Status";
            return result;
        }

        // Save the changes to the database
        _uow.OrderRepository.Update(order);
        await _uow.SaveAsync();

        result.Message = $"Status Changed to {order.OrderStatus}";
        return result;
    }

    public async Task<ServiceResult> AgentOrdersStatusCount(int agentId)
    {
        ServiceResult result = new();

        Dictionary<Orders.OrdersStatus, int> dict = await _uow.OrderRepository.GetOrderStatusGroupByAgentId(agentId);

        int total = dict.GetValueOrDefault(Orders.OrdersStatus.DELIVERED);

        int cancelled = dict.GetValueOrDefault(Orders.OrdersStatus.CANCELLED);

        int assigned =
                dict.GetValueOrDefault(Orders.OrdersStatus.WAITING_FOR_PICKUP) +
                dict.GetValueOrDefault(Orders.OrdersStatus.INTRANSIT) +
                dict.GetValueOrDefault(Orders.OrdersStatus.OUTFORDELIVERY);

        result.Data = new List<CountView>()
        {
            new CountView("Total", total),
            new CountView("Cancelled", cancelled),
            new CountView("Assigned", assigned),
        };

        result.ServiceStatus = ServiceStatus.Success;
        result.Message = "Agent Order Status Count";
        return result;
    }


    public async Task<ServiceResult> GenerateOtp(int agentId, int orderId)
    {
        ServiceResult result = new();

        Orders? order = await _uow.OrderRepository.FindByOrderIdAsync(orderId);

        if (order is null || order.AgentId != agentId)
        {
            result.ServiceStatus = ServiceStatus.NotFound;
            result.Message = "Order Not Found";
            return result;
        }

        if (order.OrderStatus != Orders.OrdersStatus.OUTFORDELIVERY)
        {
            result.ServiceStatus = ServiceStatus.BadRequest;
            result.Message = $"Order Status : {order.OrderStatus}";
            return result;
        }

        TimeSpan? requestTime = DateTime.Now - order.OtpGeneratedTime;

        if (requestTime < TimeSpan.FromMinutes(1))
        {
            int remainingSec = (int)(60 - requestTime.Value.TotalSeconds);

            result.ServiceStatus = ServiceStatus.BadRequest;
            result.Message = $"Wait for : {remainingSec} Seconds";
            result.Data = remainingSec;
            return result;
        }

        string otp = OtpGenerator.GenerateOTP(6);

        order.Otp = otp;

        order.OtpGeneratedTime = DateTime.Now;

        _uow.OrderRepository.Update(order);

        await _uow.SaveAsync();

        string to = order.User.Email;
        string name = order.User.FirstName + " " + order.User.LastName;
        string[] productName = order.OrderDetails.Where(o => o.Histories.Last().Status == OrderHistory.HistoryStatus.OUTFORDELIVERY).Select(orderDetail => orderDetail.Product.ProductName).ToArray();

        _emailService.DeliveryOtp(to, name, otp, productName);

        result.Message = "Otp Generated Successfully";

        return result;
    }

    public async Task<ServiceResult> VerifyOtp(int agentId, int orderId, string otp)
    {

        ServiceResult result = new();

        Orders? order = await _uow.OrderRepository.FindByOrderIdAsync(orderId);

        if (order is null || order.AgentId != agentId)
        {
            result.ServiceStatus = ServiceStatus.NotFound;
            result.Message = "Order Not Found";
            return result;
        }

        if (order.OrderStatus != Orders.OrdersStatus.OUTFORDELIVERY)
        {
            result.ServiceStatus = ServiceStatus.BadRequest;
            result.Message = $"Order Status : {order.OrderStatus}";
            return result;
        }

        TimeSpan? requestTime = DateTime.Now - order.OtpGeneratedTime;

        if (requestTime > TimeSpan.FromMinutes(10))
        {
            result.ServiceStatus = ServiceStatus.BadRequest;
            result.Message = "Otp Timeout";
            return result;
        }

        if (order.Otp != otp)
        {
            result.ServiceStatus = ServiceStatus.BadRequest;
            result.Message = "Invalid Otp";
            return result;
        }

        for (int i = 0; i < order.OrderDetails.Count; i++)
        {
            if (order.OrderDetails[i].Histories.Last().Status == OrderHistory.HistoryStatus.OUTFORDELIVERY)
            {
                order.OrderDetails[i].Histories.Add(
                                    new OrderHistory()
                                    {
                                        Status = OrderHistory.HistoryStatus.DELIVERED,
                                        Date = DateTime.Now,
                                    });
            }
        }

        order.Otp = null;
        order.OrderStatus = Orders.OrdersStatus.DELIVERED;

        _uow.OrderRepository.Update(order);

        await _uow.SaveAsync();

        result.Message = "Marked As Delivered";

        return result;
    }
}