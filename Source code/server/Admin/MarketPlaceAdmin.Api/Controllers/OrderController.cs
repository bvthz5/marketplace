using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceAdmin.Api.Controllers
{
    /// <summary>
    /// Provides a controller for managing orders.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IAdminOrderService _orderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderController"/> class with a specified order service.
        /// </summary>
        /// <param name="orderService">The order service to use for managing orders.</param>
        public OrderController(IAdminOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Gets the details of an order with a specific order ID.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order to retrieve.</param>
        /// <returns>A ServiceResult object containing the list of OrderDetails, if the order is found.</returns>
        [HttpGet("{orderId:int:min(1)}", Name = "Order Details")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            ServiceResult result = await _orderService.GetOrderDetails(orderId);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Gets a paginated list of orders, with optional search, sorting, and filtering.
        /// </summary>
        /// <param name="form">An object containing pagination, search, sorting, and filtering parameters.</param>
        /// <returns>A ServiceResult object containing the paginated list of orders.</returns>
        [HttpGet("page", Name = "Paginated Order List")]
        public async Task<IActionResult> GetOrderList([FromQuery] OrderPaginationParams form)
        {
            ServiceResult result = await _orderService.GetOrderList(form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderDetailsId"></param>
        /// <returns></returns>
        [HttpGet("history/{orderDetailsId:int:min(1)}")]
        public async Task<IActionResult> GetOrderHistory(int orderDetailsId)
        {
            ServiceResult result = await _orderService.GetOrderHistory(orderDetailsId);
            return StatusCode((int)result.ServiceStatus, result);
        }

    }
}