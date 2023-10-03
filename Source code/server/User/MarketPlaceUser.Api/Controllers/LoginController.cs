using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceUser.Api.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Controller method for logging in with user credentials.
        /// </summary>
        /// <param name="form">LoginForm containing user credentials.</param>
        [AllowAnonymous]
        [HttpPost(Name = "Login")]
        public async Task<IActionResult> Login(LoginForm form)
        {
            // Call the service to log in with user credentials.
            ServiceResult result = await _userService.Login(form);

            // Return the appropriate HTTP status code and result.
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Controller method for refreshing an authentication token.
        /// </summary>
        /// <param name="refreshToken">The refresh token used to obtain a new access token.</param>
        [AllowAnonymous]
        [HttpPut(Name = "Refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            // Call the service to refresh the authentication token.
            ServiceResult result = await _userService.RefreshAsync(refreshToken);

            // Return the appropriate HTTP status code and result.
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}
