using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Dto.Forms.Validations;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceUser.Api.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        private readonly ISecurityUtil _securityUtil;

        public UserController(IUserService userService, ISecurityUtil securityUtil)
        {
            _userService = userService;
            _securityUtil = securityUtil;
        }

        /// <summary>
        /// Adds a new user account to the system
        /// </summary>
        /// <param name="form">User registration form data</param>
        /// <returns>ServiceResult object containing the operation status and any relevant data</returns>
        [HttpPost(Name = "PostUser")]
        public async Task<IActionResult> Add(UserRegistrationForm form)
        {
            ServiceResult result = await _userService.AddUserAsync(form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Resends the verification email for a given user account
        /// </summary>
        /// <param name="email">Email address of the user account to resend the verification email for</param>
        /// <returns>ServiceResult object containing the operation status and any relevant data</returns>
        [HttpPut("resend-verification-mail")]
        public async Task<IActionResult> ResendVerificationMail([FromBody][Email] string email)
        {
            ServiceResult result = await _userService.ResendVerificationMailAsync(email);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Resends the verification email for a given user account
        /// </summary>
        /// <param name="token">Token of the user account to resend the verification email for</param>
        /// <returns>ServiceResult object containing the operation status and any relevant data</returns>
        [HttpPut("resend-verification-mail-token")]
        public async Task<IActionResult> ResendVerificationMailByToken([FromBody] string token)
        {
            ServiceResult result = await _userService.ResendVerificationMailByToken(token);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Verifies a user account using a verification token
        /// </summary>
        /// <param name="token">Verification token to use for account verification</param>
        /// <returns>ServiceResult object containing the operation status and any relevant data</returns>
        [HttpPut("verify")]
        public async Task<IActionResult> Verify([FromBody] string token)
        {
            ServiceResult result = await _userService.VerifyUserAsync(token);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Gets the user account information for a given user ID
        /// </summary>
        /// <param name="userId">ID of the user account to retrieve information for</param>
        /// <returns>ServiceResult object containing the operation status and any relevant data</returns>
        [HttpGet("{userId:int:min(1)}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            ServiceResult result = await _userService.GetUserAsync(userId);
            return StatusCode((int)result.ServiceStatus, result);
        }
        /// <summary>
        /// Action method for getting the current user's information.
        /// Requires an authenticated user.
        /// </summary>
        /// <returns>The current user's information.</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            ServiceResult result = await _userService.GetUserAsync(_securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Action method for updating the current user's information.
        /// Requires an authenticated user.
        /// </summary>
        /// <param name="form">The updated user information.</param>
        /// <returns>The updated user information.</returns>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserUpdateForm form)
        {
            ServiceResult result = await _userService.EditAsync(_securityUtil.GetCurrentUserId(), form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Action method for initiating the forgot password process.
        /// Does not require an authenticated user.
        /// </summary>
        /// <param name="email">The email address associated with the user's account.</param>
        /// <returns>The status of the forgot password request.</returns>
        [AllowAnonymous]
        [HttpPut("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody][Email] string email)
        {
            ServiceResult result = await _userService.ForgotPasswordRequestAsync(email);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Action method for resetting the user's password.
        /// Does not require an authenticated user.
        /// </summary>
        /// <param name="form">The form containing the password reset information.</param>
        /// <returns>The status of the password reset request.</returns>
        [AllowAnonymous]
        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordForm form)
        {
            ServiceResult result = await _userService.ResetPasswordAsync(form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Action method for setting the user's profile picture.
        /// Requires an authenticated user.
        /// </summary>
        /// <param name="image">The form data containing the image to upload.</param>
        /// <returns>The status of the upload operation.</returns>
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> SetProfilePic([FromForm] ImageForm image)
        {
            ServiceResult result = await _userService.UploadImageAsync(_securityUtil.GetCurrentUserId(), image);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Action method for changing the user's password.
        /// Requires an authenticated user.
        /// </summary>
        /// <param name="form">The form data containing the password change information.</param>
        /// <returns>The status of the password change operation.</returns>
        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordForm form)
        {
            ServiceResult result = await _userService.ChangePasswordAsync(form, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Action method for requesting to become a seller.
        /// Requires an authenticated user with a role of "0".
        /// </summary>
        /// <returns>The status of the seller request operation.</returns>
        [Authorize(Roles = "0")]
        [HttpPut("requset-to-seller")]
        public async Task<IActionResult> RequsetToSeller()
        {
            ServiceResult result = await _userService.RequsetToSeller(_securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Action method for getting the user's profile picture.
        /// Does not require an authenticated user.
        /// </summary>
        /// <param name="fileName">The name of the file to retrieve.</param>
        /// <returns>The user's profile picture file.</returns>
        [HttpGet("profile/{fileName}")]
        public async Task<IActionResult> GetProfile(string fileName)
        {
            FileStream? fileStream = await _userService.GetProfilePic(fileName);

            if (fileStream is null)
                return NotFound();

            return File(fileStream, "image/jpeg");
        }
    }
}