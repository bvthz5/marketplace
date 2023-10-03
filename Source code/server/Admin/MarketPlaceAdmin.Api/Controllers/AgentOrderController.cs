using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceAdmin.Api.Controllers
{
    /// <summary>
    /// Provides a controller for managing orders.
    /// </summary>
    [Authorize(Roles = "Agent")]
    [Route("api/agent-order")]
    [ApiController]
    public class AgentOrderController : ControllerBase
    {
        private readonly IAgentOrderService _orderService;
        private readonly ISecurityUtil _securityUtil;


        /// <summary>
        /// Initializes a new instance of the <see cref="AgentOrderController"/> class with a specified order service.
        /// </summary>
        /// <param name="orderService">The order service to use for managing orders.</param>
        /// <param name="securityUtil">The security utility for performing security-related tasks.</param>
        /// 
        public AgentOrderController(IAgentOrderService orderService, ISecurityUtil securityUtil)
        {
            _orderService = orderService;
            _securityUtil = securityUtil;

        }

        /// <summary>
        /// Gets a paginated list of orders, with optional search, sorting, and filtering.
        /// </summary>
        /// <param name="form">An object containing pagination, search, sorting, and filtering parameters.</param>
        /// <returns>A ServiceResult object containing the paginated list of orders.</returns>
        [HttpGet("page", Name = "Agent Paginated Order List")]
        public async Task<IActionResult> GetOrderList([FromQuery] AgentOrderPaginationParams form)
        {
            ServiceResult result = await _orderService.GetOrderList(form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// HTTP PUT method that assigns an order with a given order ID.
        /// </summary>
        /// <param name="orderId">The ID of the order to be assigned.</param>
        /// <returns>Returns an asynchronous task that represents the HTTP response message, containing the result of the operation.</returns>
        [HttpPut("assign/{orderId:int:min(1)}")]
        public async Task<IActionResult> AssignOrder(int orderId)
        {
            ServiceResult result = await _orderService.AssignOrder(orderId);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// HTTP PUT method that unassigns an order with a given order ID .
        /// </summary>
        /// <param name="orderId">The ID of the order to be unassigned.</param>
        /// <returns>Returns an asynchronous task that represents the HTTP response message, containing the result of the operation.</returns>
        [HttpPut("unassign/{orderId:int:min(1)}")]
        public async Task<IActionResult> UnAssignOrder(int orderId)
        {
            ServiceResult result = await _orderService.UnAssignOrder(orderId);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Gets the details of an order with a specific order ID.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order to retrieve.</param>
        /// <returns>A ServiceResult object containing the list of OrderDetails, if the order is found.</returns>
        [HttpGet("{orderId:int:min(1)}", Name = "Agent Order Details")]
        public async Task<IActionResult> AgentOrderDetailView(int orderId)
        {
            ServiceResult result = await _orderService.GetOrderDetails(orderId, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Changes the delivery status of an order to the specified status for the authenticated agent.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order.</param>
        /// <param name="status">The delivery status to be changed to.</param>
        /// <returns>An IActionResult object containing the result of the operation.</returns>
        [HttpPut("status/{orderId:int:min(1)}", Name = "Agent Change Delivery Status")]
        public async Task<IActionResult> ChangeDeliveryStatus(int orderId, [FromBody] byte status)
        {
            ServiceResult result = await _orderService.ChangeDeliveryStatus(orderId, status, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// HTTP GET method that returns the count of orders based on their status for the current authenticated agent.
        /// </summary>
        /// <returns>Returns a HTTP status code and a ServiceResult object with the order status count as the data property.</returns>
        [HttpGet("order-status-count", Name = "Order Status Count")]
        public async Task<IActionResult> AgentOrdersStatusCount()
        {
            ServiceResult result = await _orderService.AgentOrdersStatusCount(_securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Agent")]
        [HttpPut("generate-otp/{orderId:int:min(1)}")]
        public async Task<IActionResult> GenerateOtp(int orderId)
        {
            ServiceResult result = await _orderService.GenerateOtp(_securityUtil.GetCurrentUserId(), orderId);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="otp"></param>
        /// <returns></returns>
        [Authorize(Roles = "Agent")]
        [HttpPut("verify-otp/{orderId:int:min(1)}")]
        public async Task<IActionResult> VerifyOtp(int orderId, [FromBody] string otp)
        {
            ServiceResult result = await _orderService.VerifyOtp(_securityUtil.GetCurrentUserId(), orderId, otp);
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}