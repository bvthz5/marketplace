using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceUser.Api.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishListService _wishListService;

        private readonly ISecurityUtil _securityUtil;

        public WishListController(IWishListService wishListService, ISecurityUtil securityUtil)
        {
            _wishListService = wishListService;
            _securityUtil = securityUtil;
        }

        /// <summary>
        /// Adds a product to the user's wish list.
        /// </summary>
        /// <param name="productId">The ID of the product to add.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating the result of the operation.</returns>
        [Authorize]
        [HttpPost(Name = "AddWishList")]
        public async Task<IActionResult> Add([FromBody] int productId)
        {
            ServiceResult result = await _wishListService.AddToWishListAsync(_securityUtil.GetCurrentUserId(), productId);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Gets the user's wish list.
        /// </summary>
        /// <returns>A <see cref="ServiceResult"/> containing the user's wish list.</returns>
        [Authorize]
        [HttpGet(Name = "GetWishList")]
        public async Task<IActionResult> Get()
        {
            ServiceResult result = await _wishListService.GetWishListAsync(_securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Removes a product from the user's wish list.
        /// </summary>
        /// <param name="productId">The ID of the product to remove.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating the result of the operation.</returns>
        [Authorize]
        [HttpDelete("{productId:int:min(1)}")]
        public async Task<IActionResult> Delete(int productId)
        {
            ServiceResult result = await _wishListService.RemoveFromWishListAsync(_securityUtil.GetCurrentUserId(), productId);
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}
