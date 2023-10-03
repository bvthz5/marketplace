using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceAdmin.Api.Controllers
{
    /// <summary>
    /// This controller handles all API requests related to users, including retrieving user details, changing user status,
    /// accepting/rejecting seller requests, and retrieving a paginated list of users based on specified parameters. 
    /// </summary>
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor for the UserController. Takes an instance of an IUserService as a parameter, which is used to retrieve 
        /// and modify user information.
        /// </summary>
        /// <param name="userService">An instance of an IUserService.</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves user details by ID
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        [Authorize(Roles = "Admin")]
        [HttpGet("{userId:int:min(1)}", Name = "Get User By Id")]
        public async Task<IActionResult> GetUser(int userId)
        {
            ServiceResult result = await _userService.GetUser(userId);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Retrieves a user's profile picture by filename.
        /// </summary>
        /// <param name="fileName">The filename of the profile picture to retrieve.</param>
        /// <returns>A FileStream containing the user's profile picture.</returns>
        [AllowAnonymous]
        [HttpGet("profile/{fileName}", Name = "Get User Profile Picture")]
        public async Task<IActionResult> GetProfile(string fileName)
        {
            FileStream? fileStream = await _userService.GetProfilePic(fileName);

            if (fileStream is null)
                return NotFound();

            return File(fileStream, "image/jpeg");
        }

        /// <summary>
        /// Changes a user's status.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="status">The new status to set for the user.</param>
        [Authorize(Roles = "Admin")]
        [HttpPut("status/{userId:int:min(1)}", Name = "Change User Status")]
        public async Task<IActionResult> ChangeUserStatus(int userId, [FromBody] byte status)
        {
            ServiceResult result = await _userService.ChangeStatusAsync(userId, status);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Retrieves a paginated list of users based on the provided search and sort parameters.
        /// </summary>
        /// <param name="form">A UserPaginationParams object containing pagination, search, and sorting parameters.</param>
        /// <returns>A ServiceResult containing the paginated list of users.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("page", Name = "User List")]
        public async Task<IActionResult> PaginatedUserList([FromQuery] UserPaginationParams form)
        {
            ServiceResult result = await _userService.UserListAsync(form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Accepts or rejects a seller request from a user.
        /// </summary>
        /// <param name="userId">The ID of the user making the seller request.</param>
        /// <param name="form">A SellerRequestForm containing information about the seller request.</param>
        [Authorize(Roles = "Admin")]
        [HttpPut("seller-request/{userId:int:min(1)}", Name = "Seller Request")]
        public async Task<IActionResult> SellerRequest(int userId, [FromBody] RequestForm form)
        {
            ServiceResult result = await _userService.SellerRequest(userId, form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Gets the count of products for the authenticated seller.
        /// </summary>
        /// <returns>Returns a <see cref="ServiceResult"/> object containing the result of the operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("seller-product-count", Name = "Seller Product Count")]
        public async Task<IActionResult> SellerProductCount()
        {
            ServiceResult result = await _userService.SellerProductCount();
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Gets the count of products for the specified seller with their status.
        /// </summary>
        /// <param name="userId">The ID of the seller whose product count is being requested.</param>
        /// <returns>Returns a <see cref="ServiceResult"/> object containing the result of the operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("seller-product-status-count/{userId:int:min(1)}", Name = "Seller Product Status Count")]
        public async Task<IActionResult> SellerProductCount(int userId)
        {
            ServiceResult result = await _userService.SellerProductStatusCount(userId);
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}