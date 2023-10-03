using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceAdmin.Api.Controllers
{
    /// <summary>
    /// API controller for Google authentication.
    /// </summary>
    [Route("api/google-login")]
    [ApiController]
    public class GoogleController : ControllerBase
    {
        private readonly IGoogleAuthService _googleAuthService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleController"/> class.
        /// </summary>
        /// <param name="googleAuthService">The Google authentication service.</param>
        public GoogleController(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;
        }

        /// <summary>
        /// Google user login using IdToken
        /// </summary>
        /// <param name="idToken">The Google IdToken obtained from the client-side authentication flow.</param>
        [AllowAnonymous]
        [HttpPost("admin", Name = "Google Login")]
        public async Task<IActionResult> Login([FromBody] string idToken)
        {
            ServiceResult result = await _googleAuthService.Login(idToken);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Agent Google  login using IdToken
        /// </summary>
        /// <param name="idToken">The Google IdToken obtained from the client-side authentication flow.</param>
        [AllowAnonymous]
        [HttpPost("agent")]
        public async Task<IActionResult> GoogleLogin([FromBody] string idToken)
        {
            ServiceResult result = await _googleAuthService.AgentLogin(idToken);
            return StatusCode((int)result.ServiceStatus, result);
        }

    }
}
