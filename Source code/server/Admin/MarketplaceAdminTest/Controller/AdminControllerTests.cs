using MarketPlaceAdmin.Api.Controllers;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace MarketplaceAdminTest.Controller
{
    public class AdminControllerTests
    {
        private readonly IAdminService _adminService;
        private readonly ISecurityUtil _securityUtil;
        private readonly AdminController _adminController;

        public AdminControllerTests()
        {
            _adminService = Substitute.For<IAdminService>();
            _securityUtil = Substitute.For<ISecurityUtil>();
            _adminController = new AdminController(_adminService, _securityUtil);
        }

        [Fact]
        public async Task ForgotPassword_WithValidEmail_ReturnsOk()
        {
            // Arrange
            var email = "test@example.com";
            var expectedServiceResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _adminService.ForgotPasswordRequest(email).Returns(expectedServiceResult);

            // Act
            var result = await _adminController.ForgotPassword(email) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedServiceResult, result.Value);
        }

        [Fact]
        public async Task ResetPassword_WithValidForm_ReturnsOk()
        {
            // Arrange
            var form = new ForgotPasswordForm { Token = "valid_token", Password = "new_password" };
            var expectedServiceResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _adminService.ResetPassword(form).Returns(expectedServiceResult);

            // Act
            var result = await _adminController.ResetPassword(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedServiceResult, result.Value);
        }

        [Fact]
        public async Task ChangePassword_WithValidForm_ReturnsOk()
        {
            // Arrange
            var form = new ChangePasswordForm { CurrentPassword = "current_password", NewPassword = "new_password" };
            var expectedServiceResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _securityUtil.GetCurrentUserId().Returns(1);
            _adminService.ChangePassword(form, 1).Returns(expectedServiceResult);

            // Act
            var result = await _adminController.ChangePassword(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedServiceResult, result.Value);
        }

    }

}
