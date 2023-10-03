using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using Microsoft.Extensions.Logging;

namespace MarketPlaceUser.Bussiness.Services
{
    public class RefundService : IRefundService
    {
        private readonly IUnitOfWork _uow; // Unit of work for accessing repositories
        private readonly ILogger<OrderService> _logger; // Logger for logging errors
        private readonly IRazorpayService _razorpayService;


        public RefundService(IUnitOfWork uow, ILogger<OrderService> logger, IRazorpayService razorpayService)
        {
            _uow = uow;
            _logger = logger;
            _razorpayService = razorpayService;
        }

        public async Task<ServiceResult> Refund(RefundOrderForm form, int orderDetailsId, int userId)
        {
            ServiceResult result = new();

            OrderDetails? orderDetails = await _uow.OrderDetailsRepository.FindByOrderDetailsId(orderDetailsId);

            if (orderDetails is null || orderDetails.Histories.Last().Status == OrderHistory.HistoryStatus.CREATED)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Order Not Found";
                return result;
            }

            if (orderDetails.Histories.Last().Status == OrderHistory.HistoryStatus.DELIVERED)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"Order Status : {orderDetails.Histories.Last().Status}";
                return result;
            }

            if (orderDetails.Histories.Last().Status == OrderHistory.HistoryStatus.CANCELLED)
            {
                result.ServiceStatus = ServiceStatus.AlreadyExists;
                result.Message = "Order Already Cancelled";
                return result;
            }

            if (_razorpayService.Refund(orderDetails.Product.Price, orderDetails.Order.OrderNumber))
            {
                orderDetails.Histories.Add(new OrderHistory() { Date = DateTime.Now, Status = OrderHistory.HistoryStatus.CANCELLED, Remark = form.Reason });

                List<OrderDetails> all = await _uow.OrderDetailsRepository.FindByOrderId(orderDetails.OrderId);

                // Check if all order details are now cancelled and update order status if necessary
                if (all.All(order => order.Histories.Last().Status == OrderHistory.HistoryStatus.CANCELLED))
                {
                    orderDetails.Order.OrderStatus = Orders.OrdersStatus.CANCELLED;
                    orderDetails.Order.PaymentStatus = Orders.PaymentsStatus.REFUNDED;
                }

                orderDetails.Order.TotalPrice -= orderDetails.Product.Price;

                _uow.OrderRepository.Update(orderDetails.Order);
                _uow.OrderDetailsRepository.Update(orderDetails);

                await _uow.SaveAsync();

                _logger.LogInformation("OrderDetails: {orderDetailsId} Refunded", orderDetailsId);

                result.ServiceStatus = Enums.ServiceStatus.Success;
                result.Message = "Refund processed successfully";
            }
            else
            {
                _logger.LogError("Refund process failed");
                result.ServiceStatus = Enums.ServiceStatus.BadRequest;
                result.Message = "Error processing refund";
            }
            return result;
        }
    }
}

