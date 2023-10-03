using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceUser.Api.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        private readonly ISecurityUtil _securityUtil;

        public ProductController(IProductService productService, ISecurityUtil securityUtil)
        {
            _productService = productService;
            _securityUtil = securityUtil;
        }

        /// <summary>
        /// Add a new product
        /// </summary>
        /// <param name="form">The form data for the new product</param>
        /// <returns>The result of the service operation</returns>
        [Authorize(Roles = "2")]
        [HttpPost(Name = "PostProduct")]
        public async Task<IActionResult> Add(ProductForm form)
        {
            ServiceResult result = await _productService.AddProductAsync(form, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Get a product by ID
        /// </summary>
        /// <param name="productId">The ID of the product to get</param>
        /// <returns>The result of the service operation</returns>
        [AllowAnonymous]
        [HttpGet("{productId:int:min(1)}")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            ServiceResult result = await _productService.GetProductAsync(productId, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Update a product
        /// </summary>
        /// <param name="productId">The ID of the product to update</param>
        /// <param name="form">The form data for the updated product</param>
        /// <returns>The result of the service operation</returns>
        [Authorize(Roles = "2")]
        [HttpPut("{productId:int:min(1)}")]
        public async Task<IActionResult> Update(int productId, [FromBody] ProductForm form)
        {
            ServiceResult result = await _productService.EditProductAsync(form, productId, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="productId">The ID of the product to delete</param>
        /// <returns>The result of the service operation</returns>
        [Authorize(Roles = "2")]
        [HttpPut("delete/{productId:int:min(1)}")]
        public async Task<IActionResult> Delete(int productId)
        {
            ServiceResult result = await _productService.DeleteProductAsync(productId, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Get a paginated list of products
        /// </summary>
        /// <param name="form">The pagination parameters</param>
        /// <returns>The result of the service operation</returns>
        [AllowAnonymous]
        [HttpGet("page")]
        public async Task<IActionResult> PaginatedProductList([FromQuery] ProductPaginationParams form)
        {
            ServiceResult result = await _productService.ProductListAsync(form, _securityUtil.GetCurrentUserRole());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Get a list of products by user ID
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>The result of the service operation</returns>
        [HttpGet("by-user/{userId:int:min(0)}")]
        public async Task<IActionResult> GetProductByUserId(int userId)
        {
            ServiceResult result = await _productService.GetProductByUserIdAsync(userId == 0 ? _securityUtil.GetCurrentUserId() : userId);
            return StatusCode((int)result.ServiceStatus, result);
        }
    }

}
