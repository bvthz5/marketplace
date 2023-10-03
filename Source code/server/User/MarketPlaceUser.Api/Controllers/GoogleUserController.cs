using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceUser.Api.Controllers
{
    [Route("api/google")]
    [ApiController]
    public class GoogleUserController : ControllerBase
    {
        private readonly IGoogleAuthService _googleAuthService;

        public GoogleUserController(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;
        }

        /// <summary>
        /// Controller method for logging in with a Google account.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] string idToken)
        {
            // Call the service to register and login with the Google account.
            ServiceResult result = await _googleAuthService.RegisterAndLogin(idToken);

            // Return the appropriate HTTP status code and result.
            return StatusCode((int)result.ServiceStatus, result);
        }

    }
}
