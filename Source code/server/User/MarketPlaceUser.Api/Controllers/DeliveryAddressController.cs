using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceUser.Api.Controllers
{
    [Route("api/delivery-address")]
    [ApiController]
    public class DeliveryAddressController : ControllerBase
    {
        public readonly IDeliveryAddressService _deliveryAddressService;
        public readonly ISecurityUtil _securityUtil;
        public DeliveryAddressController(IDeliveryAddressService deliveryAddressService, ISecurityUtil securityUtil)
        {
            _deliveryAddressService = deliveryAddressService;
            _securityUtil = securityUtil;
        }

        /// <summary>
        /// Action method for adding a delivery address.
        /// Requires an authenticated user.
        /// </summary>
        /// <param name="deliveryAddressForm">The form data containing the delivery address information.</param>
        /// <returns>The status of the add address operation.</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddAddress(DeliveryAddressForm deliveryAddressForm)
        {
            ServiceResult result = await _deliveryAddressService.AddAddressAsync(deliveryAddressForm, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);

        }

        /// <summary>
        /// Action method for retrieving the delivery addresses for the authenticated user.
        /// Requires an authenticated user.
        /// </summary>
        /// <returns>The delivery addresses for the authenticated user.</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAdresses()
        {
            ServiceResult result = await _deliveryAddressService.GetAddressAsync(_securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);

        }

        /// <summary>
        /// Gets a specific delivery address for the current user.
        /// </summary>
        /// <param name="deliveryAddressId">The ID of the delivery address to retrieve.</param>
        /// <returns>A <see cref="ServiceResult"/> containing the delivery address with the specified ID.</returns>
        [Authorize]
        [HttpGet("{deliveryAddressId}")]
        public async Task<IActionResult> GetAddressById(int deliveryAddressId)
        {
            ServiceResult result = await _deliveryAddressService.GetAddressById(deliveryAddressId, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Sets the specified delivery address as the default for the current user.
        /// </summary>
        /// <param name="deliveryAddressId">The ID of the delivery address to set as the default.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating whether the default address was set successfully.</returns>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int deliveryAddressId)
        {
            ServiceResult result = await _deliveryAddressService.SetAddressDefault(_securityUtil.GetCurrentUserId(), deliveryAddressId);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Updates the specified delivery address for the current user.
        /// </summary>
        /// <param name="deliveryAddressId">The ID of the delivery address to update.</param>
        /// <param name="deliveryAddressForm">The delivery address form.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating whether the address was updated successfully.</returns>
        [Authorize]
        [HttpPut("edit")]
        public async Task<IActionResult> UpdateAddress(int deliveryAddressId, DeliveryAddressForm deliveryAddressForm)
        {
            ServiceResult result = await _deliveryAddressService.EditAddressAsync(deliveryAddressForm, deliveryAddressId, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Controller method for deleting a delivery address.
        /// </summary>
        /// <param name="deliveryAddressId">The ID of the address to remove.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating the result of the operation.</returns>
        [Authorize]
        [HttpPut("delete/{deliveryAddressId}")]
        public async Task<IActionResult> DeleteAddress(int deliveryAddressId)
        {
            ServiceResult result = await _deliveryAddressService.DeleteAddress(_securityUtil.GetCurrentUserId(), deliveryAddressId);
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}
