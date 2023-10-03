using MarketPlace.DataAccess.Model;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MarketPlaceUser.Bussiness.Security
{

    public interface ISecurityUtil
    {
        /// <summary>
        /// For authenticated request returns userId from ClaimsPrincipal.<br/>
        /// Returns 0 if Not-Authenticated request
        /// </summary>
        /// <returns>Current UserId or 0 </returns>
        int GetCurrentUserId();

        /// <returns>Current User Role or Null </returns>
        User.UserRole? GetCurrentUserRole();
    }


    public class SecurityUtil : ISecurityUtil
    {
        private readonly ClaimsPrincipal? _user;
        public SecurityUtil(IHttpContextAccessor httpContextAccessor)
        {
            _user = httpContextAccessor.HttpContext?.User;
        }

        /// <summary>
        /// For authenticated request returns userId from ClaimsPrincipal.<br/>
        /// Returns 0 if Not-Authenticated request
        /// </summary>
        /// <returns>Current UserId or 0 </returns>
        public int GetCurrentUserId()
        {
            var userId = _user?.FindFirst(ClaimTypes.NameIdentifier);

            if (userId != null)
                return int.Parse(userId.Value);

            return 0;
        }

        /// <returns>Current User Role or Null </returns>
        public User.UserRole? GetCurrentUserRole()
        {
            var role = _user?.FindFirst(ClaimTypes.Role);

            if (role != null)
                return (User.UserRole)byte.Parse(role.Value);

            return null;
        }
    }
}
