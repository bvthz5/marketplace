using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Security;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlaceUserTest.Security
{
    public class SecurityUtilTests
    {
        [Fact]
        public void GetCurrentUserId_WhenUserNotAuthenticated_ReturnsZero()
        {
            // Arrange
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.ReturnsNull();
            var securityUtil = new SecurityUtil(httpContextAccessor);

            // Act
            var result = securityUtil.GetCurrentUserId();

            // Assert
            Assert.Equal(0, result);
        }


        [Fact]
        public void GetCurrentUserId_WhenUserAuthenticated_ReturnsCorrectUserId()
        {
            // Arrange
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var userIdClaim = new Claim(ClaimTypes.NameIdentifier, "1234");
            var identity = new ClaimsIdentity(new[] { userIdClaim });
            var claimsPrincipal = new ClaimsPrincipal(identity);
            httpContextAccessor.HttpContext.Returns(new DefaultHttpContext { User = claimsPrincipal });
            var securityUtil = new SecurityUtil(httpContextAccessor);

            // Act
            var result = securityUtil.GetCurrentUserId();

            // Assert
            Assert.Equal(1234, result);
        }


        [Fact]
        public void GetCurrentUserRole_WhenUserNotAuthenticated_ReturnsNull()
        {
            // Arrange
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.ReturnsNull();
            var securityUtil = new SecurityUtil(httpContextAccessor);

            // Act
            var result = securityUtil.GetCurrentUserRole();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetCurrentUserRole_WhenUserAuthenticated_ReturnsCorrectRole()
        {
            // Arrange
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var roleClaim = new Claim(ClaimTypes.Role, "2");
            var identity = new ClaimsIdentity(new[] { roleClaim });
            var claimsPrincipal = new ClaimsPrincipal(identity);
            httpContextAccessor.HttpContext.Returns(new DefaultHttpContext { User = claimsPrincipal });
            var securityUtil = new SecurityUtil(httpContextAccessor);

            // Act
            var result = securityUtil.GetCurrentUserRole();

            // Assert
            Assert.Equal(User.UserRole.SELLER, result); // replace "SomeRole" with the actual value of the UserRole enum corresponding to the role claim in your code
        }



    }
}
