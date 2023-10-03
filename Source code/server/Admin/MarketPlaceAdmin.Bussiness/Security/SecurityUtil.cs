using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MarketPlaceAdmin.Bussiness.Security
{

    /// <summary>
    /// Implements the ISecurityUtil interface using an IHttpContextAccessor to access the current user's ClaimsPrincipal.
    /// </summary>
    public class SecurityUtil : ISecurityUtil
    {
        private readonly ClaimsPrincipal _user;

        /// <summary>
        /// Initializes a new instance of the SecurityUtil class.
        /// </summary>
        /// <param name="httpContextAccessor">The IHttpContextAccessor used to access the current user's ClaimsPrincipal.</param>
        public SecurityUtil(IHttpContextAccessor httpContextAccessor)
        {
            _user = httpContextAccessor.HttpContext.User;
        }

        /// <returns>The ID of the currently authenticated admin user, or 0 if there is no authenticated user.</returns>
        public int GetCurrentUserId()
        {
            var userId = _user.FindFirst(ClaimTypes.NameIdentifier);

            if (userId != null)
                return int.Parse(userId.Value);

            return 0;
        }

        /// <returns>The true if authenticated user is a admin user</returns>
        public bool IsAdmin()
        {
            var role = _user.FindFirst(ClaimTypes.Role);

            return role is not null && role.Value == "Admin";
        }
    }
}