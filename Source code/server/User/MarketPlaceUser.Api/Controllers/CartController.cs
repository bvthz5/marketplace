using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceUser.Api.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        private readonly ISecurityUtil _securityUtil;

        public CartController(ICartService cartService, ISecurityUtil securityUtil)
        {
            _cartService = cartService;
            _securityUtil = securityUtil;
        }

        /// <summary>
        /// Adds a product to the user's cart.
        /// </summary>
        /// <param name="productId">The ID of the product to add.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating the result of the operation.</returns>
        [Authorize]
        [HttpPost(Name = "AddCart")]
        public async Task<IActionResult> Add([FromBody] int productId)
        {
            ServiceResult result = await _cartService.AddToCartAsync(_securityUtil.GetCurrentUserId(), productId);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Gets the user's cart.
        /// </summary>
        /// <returns>A <see cref="ServiceResult"/> containing the user's cart.</returns>
        [Authorize]
        [HttpGet(Name = "GetCart")]
        public async Task<IActionResult> Get()
        {
            ServiceResult result = await _cartService.GetCartAsync(_securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Removes a product from the user's cart.
        /// </summary>
        /// <param name="productId">The ID of the product to remove.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating the result of the operation.</returns>
        [Authorize]
        [HttpDelete("{productId:int:min(1)}")]
        public async Task<IActionResult> Delete(int productId)
        {
            ServiceResult result = await _cartService.RemoveFromCartAsync(_securityUtil.GetCurrentUserId(), productId);
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}
