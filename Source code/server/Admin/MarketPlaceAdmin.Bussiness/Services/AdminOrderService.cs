using System.Linq;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;

namespace MarketPlaceAdmin.Bussiness.Services
{
    /// <summary>
    /// Provides services for managing orders.
    /// </summary>
    public class AdminOrderService : IAdminOrderService
    {
        private readonly IUnitOfWork _uow;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminOrderService"/> class.
        /// </summary>
        /// <param name="uow">The unit of work that provides access to the data repositories.</param>
        public AdminOrderService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// Gets the order details for a given order ID, if available.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve the details for.</param>
        /// <returns>A <see cref="ServiceResult"/> that contains a list of order details if successful, or an error message if the order could not be found or its details could not be retrieved.</returns>
        public async Task<ServiceResult> GetOrderDetails(int orderId)
        {
            ServiceResult result = new();

            // Find the order by its unique identifier
            Orders? order = await _uow.OrderRepository.FindByOrderIdAsync(orderId);

            // If the order is not found, return a not found status with a message
            if (order is null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Order Not Found";
                return result;
            }

            // If no order details are found, return a not found status with a message
            if (order.OrderDetails.Count == 0)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Order Details Not Generated";
                return result;
            }

            // Generate the order detail view
            OrderDetailView orderDetailView = new(order)
            {
                Items = order.OrderDetails.Select(orderDetail => new OrderDetailsProductView(orderDetail.OrderDetailsId, new ProductDetailView(orderDetail.Product), orderDetail.Histories.Last().Status, orderDetail.Histories.Last().Remark))
            };

            // Set the result data to the order detail view
            result.Data = orderDetailView;
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Order Detail View";
            return result;
        }

        public async Task<ServiceResult> GetOrderHistory(int orderDetailsId)
        {
            ServiceResult result = new();

            OrderDetails? orderDetails = await _uow.OrderDetailsRepository.FindByOrderDetailsId(orderDetailsId);

            if (orderDetails is null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Order Not Found";
                return result;
            }

            result.Data = orderDetails.Histories.Select(orderHistory => new OrderHistoryView(orderHistory));
            result.Message = "Order History";

            return result;
        }

        /// <summary>
        /// Retrieves a paginated list of orders based on the provided search, sort, and filter parameters.
        /// </summary>
        /// <param name="form">The pagination, search, and sorting parameters to use for the query.</param>
        /// <returns>A <see cref="ServiceResult"/> object containing the paginated list of <see cref="OrderView"/> objects, along with metadata and error messages if applicable.</returns>
        /// <remarks>
        /// This method performs the following operations:
        /// <list type="bullet">
        ///     <item><description>Validates the payment status filter parameter to ensure it contains only valid values from the <see cref="Orders.PaymentsStatus"/> enumeration.</description></item>
        ///     <item><description>Validates the order status filter parameter to ensure it contains only valid values from the <see cref="Orders.OrdersStatus"/> enumeration.</description></item>
        ///     <item><description>Checks that the provided sort-by field is valid and supported by the _uow.OrderRepository column map.</description></item>
        ///     <item><description>Retrieves a list of <see cref="OrderView"/> objects from the _uow.OrderRepository based on the search, sort, and filter parameters.</description></item>
        ///     <item><description>Applies pagination to the <see cref="OrderView"/> list using the <see cref="Pager{T}"/> class.</description></item>
        /// </list>
        /// </remarks>
        public async Task<ServiceResult> GetOrderList(OrderPaginationParams form)
        {
            ServiceResult result = new();

            // Validate Payment Status

            byte[]? paymentStatus = form.PaymentStatus?.Where(paymentStatus => paymentStatus.HasValue).Cast<byte>().ToArray();

            if (paymentStatus != null && !paymentStatus.All(status => Enum.IsDefined(typeof(Orders.PaymentsStatus), status)))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid PaymentStatus Value";
                return result;
            }

            // Validate OrderStatus

            byte[]? orderStatus = form.OrderStatus?.Where(orderStatus => orderStatus.HasValue).Cast<byte>().ToArray();

            if (orderStatus != null && !orderStatus.All(status => Enum.IsDefined(typeof(Orders.OrdersStatus), status)))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid OrderStatus Value";
                return result;
            }

            // Check if SortBy value passed is defined and acceptable
            if (form.SortBy != null && !_uow.OrderRepository.ColumnMapForSortBy.ContainsKey(form.SortBy))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"SortBy : Accepts [{string.Join(", ", _uow.OrderRepository.ColumnMapForSortBy.Keys)}] values only";
                return result;
            }

            // Get a list of OrderView objects based on the search query and filter parameters
            List<OrderView> orderViews = (await _uow.OrderRepository.FindAll(form.Search?.Trim(), form.SortBy, form.SortByDesc, paymentStatus?.Cast<Orders.PaymentsStatus>().ToArray(), form.BuyerId, orderStatus?.Cast<Orders.OrdersStatus>().ToArray()))
                                            .ConvertAll(order => new OrderView(order));

            // Paginate the orderViews list and assign it to the ServiceResult.Data property
            Pager<OrderView> page = new(form.PageNumber, form.PageSize, orderViews);

            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Paginated Order List";
            result.Data = page;

            return result;
        }
    }
}
