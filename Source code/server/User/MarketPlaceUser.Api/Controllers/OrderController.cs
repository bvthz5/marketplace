using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceUser.Api.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        private readonly ISecurityUtil _securityUtil;
        private readonly IOrderDetailsService _orderDetailsService;
        private readonly IRefundService _refundService;

        public OrderController(IOrderService orderService, ISecurityUtil securityUtil, IOrderDetailsService orderDetailsService, IRefundService refundService)
        {
            _orderService = orderService;
            _securityUtil = securityUtil;
            _orderDetailsService = orderDetailsService;
            _refundService = refundService;
        }

        /// <summary>
        /// Controller method for initializing an order.
        /// </summary>
        /// <param name="form">An OrderForm containing the necessary information for the order.</param>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> InitializeOrder(OrderForm form)
        {
            // If no products are found in the form, return a BadRequest.
            if (form.ProductIds.Length == 0 || form.ProductIds == null)
            {
                return BadRequest("No products found");
            }

            // Call the service to add the order asynchronously.
            ServiceResult? result = await _orderService.AddOrderAsync(_securityUtil.GetCurrentUserId(), form.DeliveryAddressId, form.ProductIds);

            // Return the appropriate HTTP status code and result.
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Controller method for confirming a payment.
        /// </summary>
        /// <param name="confirmPaymentForm">A ConfirmPaymentForm containing the necessary information to confirm a payment.</param>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> ConfirmPayment(ConfirmPaymentForm confirmPaymentForm)
        {
            // Call the service to confirm a payment.
            ServiceResult result = await _orderService.ConfirmPayment(confirmPaymentForm, _securityUtil.GetCurrentUserId());

            // Return the appropriate HTTP status code and result.
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Controller method for viewing a list of order details.
        /// </summary>
        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> ViewOrderDetailList()
        {
            // Call the service to get all orders.
            ServiceResult result = await _orderDetailsService.GetAllOrders(_securityUtil.GetCurrentUserId());

            // Return the appropriate HTTP status code and result.
            return StatusCode((int)result.ServiceStatus, result);
        }

        [Authorize]
        [HttpGet("{orderDetailsId}")]
        public async Task<IActionResult> ViewOrderDetails(int orderDetailsId)
        {
            ServiceResult result = await _orderDetailsService.GetOrderDetailsById(_securityUtil.GetCurrentUserId(), orderDetailsId);

            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Controller method for canceling an order.
        /// </summary>
        /// <param name="orderNumber">The order number to be canceled.</param>
        [Authorize]
        [HttpPut("cancel")]
        public async Task<IActionResult> CancelOrder(string orderNumber)
        {
            // Call the service to cancel an order.
            ServiceResult result = await _orderService.CancelOrder(orderNumber);

            // Return the appropriate HTTP status code and result.
            return StatusCode((int)result.ServiceStatus, result);
        }

        [Authorize]
        [HttpGet("download-invoice/{orderDetailsId}", Name = "Download Invoice")]
        public async Task<IActionResult> DownloadInvoice(int orderDetailsId)
        {
            object obj = await _orderService.DownloadInvoice(orderDetailsId, _securityUtil.GetCurrentUserId());

            if (obj is ServiceResult result)
            {
                return StatusCode((int)result.ServiceStatus, result);
            }
            else
            {
                return obj is MemoryStream invoice ? File(invoice.ToArray(), "application/pdf", $"invoice_{orderDetailsId}.pdf") : BadRequest();
            }
        }

        [Authorize]
        [HttpGet("email-invoice/{orderDetailsId}", Name = "Email Invoice")]
        public async Task<IActionResult> EmailInvoice(int orderDetailsId)
        {
            ServiceResult result = await _orderService.EmailInvoice(orderDetailsId, _securityUtil.GetCurrentUserId());

            return StatusCode((int)result.ServiceStatus, result);
        }

        [Authorize]
        [HttpPost("refund/{orderDetailsId}")]
        public async Task<IActionResult> RefundOrder([FromBody] RefundOrderForm form, int orderDetailsId)
        {
            ServiceResult result = await _refundService.Refund(form, orderDetailsId, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderDetailsId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("history/{orderDetailsId:int:min(1)}")]
        public async Task<IActionResult> GetOrderHistory(int orderDetailsId)
        {
            ServiceResult result = await _orderService.GetOrderHistory(orderDetailsId, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}
