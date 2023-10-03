using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceAdmin.Api.Controllers
{
    /// <summary>
    /// Represents a controller for handling dashboard-related HTTP requests.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="dashboardService">The dashboard service.</param>
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Gets the category wise product count.
        /// </summary>
        /// <returns>
        /// The task result contains the category name with corresponding count.
        /// </returns>
        [HttpGet("category-product-count", Name = "Category Wise Product Count")]
        public async Task<IActionResult> GetCategoryProductCount()
        {
            ServiceResult result = await _dashboardService.GetActiveProductCountGroupByCategory();
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Gets the order count by date.
        /// </summary>
        /// <param name="form">The form containing the start and end date for the time period.</param>
        /// <returns>
        /// The task result contains the date with corresponding sales count.
        /// </returns>
        [HttpGet("order-count", Name = "Order Count By Date")]
        public async Task<IActionResult> GetOrderCount([FromQuery] FromToDateForm form)
        {
            ServiceResult result = await _dashboardService.GetSalesCount(form);
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}
