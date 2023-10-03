using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceAdmin.Api.Controllers
{
    /// <summary>
    /// API controller for managing admin account related tasks.
    /// </summary>
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ISecurityUtil _securityUtil;

        /// <summary>
        /// Constructor for AdminController.
        /// </summary>
        /// <param name="adminService">The service for managing admin account related tasks.</param>
        /// <param name="securityUtil">The security utility for performing security-related tasks.</param>
        public AdminController(IAdminService adminService, ISecurityUtil securityUtil)
        {
            _adminService = adminService;
            _securityUtil = securityUtil;
        }

        /// <summary>
        /// Generates a reset password token using the registered email address.
        /// </summary>
        /// <param name="email">The email address of the user requesting a password reset.</param>
        [AllowAnonymous]
        [HttpPut("forgot-password", Name = "Forgot Password")]
        public async Task<IActionResult> ForgotPassword([FromBody][Email] string email)
        {
            ServiceResult result = await _adminService.ForgotPasswordRequest(email);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Resets the password using the received token via email.
        /// </summary>
        /// <param name="form">The form containing the token and new password.</param>
        [AllowAnonymous]
        [HttpPut("reset-password", Name = "Reset Password")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordForm form)
        {
            ServiceResult result = await _adminService.ResetPassword(form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Changes the password using the current password.
        /// </summary>
        /// <param name="form">The form containing the current and new passwords.</param>
        [Authorize(Roles = "Admin")]
        [HttpPut("change-password", Name = "Change Password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordForm form)
        {
            ServiceResult result = await _adminService.ChangePassword(form, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}