using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceAdmin.Api.Controllers
{

    /// <summary>
    /// Controller for handling admin login and token refresh
    /// </summary>
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAgentService _agentService;

        /// <summary>
        /// Initializes a new instance of the LoginController class with an injected IAdminService instance
        /// </summary>
        /// <param name="adminService">The IAdminService instance used for handling admin login and token refresh</param>
        /// <param name="agentService">The IAdminService instance used for handling admin login and token refresh</param>
        /// 
        public LoginController(IAdminService adminService, IAgentService agentService)
        {
            _adminService = adminService;
            _agentService = agentService;
        }

        /// <summary>
        /// Admin Login using valid email and password
        /// </summary>
        /// <param name="form">The login form containing the email and password of the admin user</param>
        /// <returns>
        /// A ServiceResult containing the user details and JWT access and refresh tokens with expiry time
        /// </returns>
        [AllowAnonymous]
        [HttpPost(Name = "Login")]
        public async Task<IActionResult> Login(LoginForm form)
        {
            ServiceResult result = await _adminService.Login(form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Generate access token using refresh token
        /// </summary>
        /// <remarks>
        /// The refresh token will be invalid after expiry time or if the account password has changed.
        /// </remarks>
        /// <param name="refreshToken">The refresh token to use for generating a new access token</param>
        /// <returns>
        /// A ServiceResult containing the new JWT access and refresh tokens with expiry time
        /// </returns>
        [AllowAnonymous]
        [HttpPut(Name = "Refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            ServiceResult result = await _adminService.Refresh(refreshToken);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// This method handles Agent Login using a valid email and password.
        /// </summary>
        /// <param name="form">The login form containing the email and password of the agent user</param>
        /// <returns>
        /// Returns a ServiceResult containing the Agent user's details and JWT access and refresh tokens with an expiry time.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("agent")]
        public async Task<IActionResult> AgentLogin([FromBody] LoginForm form)
        {
            ServiceResult result = await _agentService.Login(form.Email, form.Password);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Generate access token using refresh token
        /// </summary>
        /// <remarks>
        /// The refresh token will be invalid after expiry time or if the account password has changed.
        /// </remarks>
        /// <param name="refreshToken">The refresh token to use for generating a new access token</param>
        /// <returns>
        /// A ServiceResult containing the new JWT access and refresh tokens with expiry time
        /// </returns>
        [AllowAnonymous]
        [HttpPut("agent")]
        public async Task<IActionResult> AgentRefresh([FromBody] string refreshToken)
        {
            ServiceResult result = await _agentService.AgentRefresh(refreshToken);
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}
