using MarketPlaceAdmin.Bussiness.Security;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Security.Claims;
using Xunit;

namespace MarketplaceAdminTest.Security
{
    public class SecurityUtilTests
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecurityUtilTests()
        {
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        }

        [Fact]
        public void GetCurrentAdminId_WhenUserIdClaimIsMissing_ReturnsZero()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal();
            Assert.NotNull(_httpContextAccessor.HttpContext);
            _httpContextAccessor.HttpContext.User.Returns(claimsPrincipal);
            var securityUtil = new SecurityUtil(_httpContextAccessor);

            // Act
            var result = securityUtil.GetCurrentUserId();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetCurrentAdminId_WhenUserIdClaimIsPresent_ReturnsUserId()
        {
            // Arrange
            var userId = 123;
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));
            Assert.NotNull(_httpContextAccessor.HttpContext);
            _httpContextAccessor.HttpContext?.User.Returns(claimsPrincipal);
            var securityUtil = new SecurityUtil(_httpContextAccessor);

            // Act
            var result = securityUtil.GetCurrentUserId();

            // Assert
            Assert.Equal(userId, result);
        }

        [Fact]
        public void IsAdmin_Success()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Role, "Admin")
            }));

            Assert.NotNull(_httpContextAccessor.HttpContext);
            _httpContextAccessor.HttpContext?.User.Returns(claimsPrincipal);
            var securityUtil = new SecurityUtil(_httpContextAccessor);

            // Act
            var result = securityUtil.IsAdmin();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsAdmin_False()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Role, "Agent")
            }));

            Assert.NotNull(_httpContextAccessor.HttpContext);
            _httpContextAccessor.HttpContext?.User.Returns(claimsPrincipal);
            var securityUtil = new SecurityUtil(_httpContextAccessor);

            // Act
            var result = securityUtil.IsAdmin();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsAdmin_False2()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            }));

            Assert.NotNull(_httpContextAccessor.HttpContext);
            _httpContextAccessor.HttpContext?.User.Returns(claimsPrincipal);
            var securityUtil = new SecurityUtil(_httpContextAccessor);

            // Act
            var result = securityUtil.IsAdmin();

            // Assert
            Assert.False(result);
        }
    }
}