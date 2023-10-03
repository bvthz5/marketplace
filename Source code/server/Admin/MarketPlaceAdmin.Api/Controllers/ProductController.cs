using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MarketPlaceAdmin.Api.Controllers
{
    /// <summary>
    /// Controller for managing product-related operations.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        /// <summary>
        /// Constructor for the ProductController.
        /// </summary>
        /// <param name="productService">The service for managing products.</param>
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Gets the details for a specific product.
        /// </summary>
        /// <param name="productId">The ID of the product to retrieve.</param>
        /// <returns>The result of the service operation.</returns>
        [HttpGet("{productId:int:min(1)}", Name = "Get Product By Id")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            ServiceResult result = await _productService.GetProduct(productId);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Edits the name of a product.
        /// </summary>
        /// <param name="productId">The ID of the product to edit.</param>
        /// <param name="productName">The new name of the product.</param>
        /// <returns>The result of the service operation.</returns>
        [HttpPut("{productId:int:min(1)}", Name = "Edit Product Name")]
        public async Task<IActionResult> AdminUpdate(int productId,
            [FromBody]
        [StringLength(200)]
        [Required(AllowEmptyStrings = false)]
        [RegularExpression("^[A-Za-z0-9!@%&()_\\-,.\"'+|:/\\n\\s]+$", ErrorMessage = "Invalid character present.")]
        string productName)
        {
            ServiceResult result = await _productService.AdminEditProduct(productName, productId);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Retrieves a paginated list of products with filtering, sorting, and searching.
        /// </summary>
        /// <param name="form">The pagination parameters for the list.</param>
        /// <returns>The result of the service operation.</returns>
        [HttpGet("offset", Name = "Offset Paginated Product List")]
        public async Task<IActionResult> OffsetPaginatedProductList([FromQuery] ProductOffsetPaginationParams form)
        {
            ServiceResult result = await _productService.ProductListOffset(form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Retrieves a paginated list of products with filtering, sorting, and searching.
        /// </summary>
        /// <param name="form">The pagination parameters for the list.</param>
        /// <returns>The result of the service operation.</returns>
        [HttpGet("page", Name = "Paginated Product List")]
        public async Task<IActionResult> PaginatedProductList([FromQuery] ProductPaginationParams form)
        {
            ServiceResult result = await _productService.ProductList(form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Changes the status of a product from pending to active/inactive.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="form">The request form containing the new status information.</param>
        /// <returns>The result of the service operation.</returns>
        [HttpPut("status/{productId:int:min(1)}", Name = "Product Status Change")]
        public async Task<IActionResult> ChangeProductStatus(int productId, [FromBody] RequestForm form)
        {
            ServiceResult result = await _productService.ChangeStatusAsync(productId, form);
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}
