using MarketPlaceAdmin.Api.Controllers;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace MarketplaceAdminTest.Controller
{
    public class LoginControllerTest
    {
        private readonly IAdminService _adminService;
        private readonly IAgentService _agentService;
        private readonly LoginController _loginController;

        public LoginControllerTest()
        {
            _adminService = Substitute.For<IAdminService>();
            _agentService = Substitute.For<IAgentService>();
            _loginController = new LoginController(_adminService, _agentService);
        }

        [Fact]
        public async Task LoginSuccessTest()
        {
            // Arrange
            var loginForm = new LoginForm
            {
                Email = "snehil.s@innovaturelabs.com",
                Password = "Password@123"
            };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _adminService.Login(loginForm).Returns(expectedResult);

            // Act
            var actualResult = await _loginController.Login(loginForm) as ObjectResult;

            // Assert
            Assert.NotNull(actualResult);
            Assert.NotNull(actualResult.Value);
            Assert.IsType<ServiceResult>(actualResult.Value);
            Assert.Equal(StatusCodes.Status200OK, actualResult.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)actualResult.Value).ServiceStatus);
        }


        [Fact]
        public async Task RefreshSuccessTest()
        {
            // Arrange
            var refreshToken = "Valid Refresh Token";
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _adminService.Refresh(refreshToken).Returns(expectedResult);

            // Act
            var actualResult = await _loginController.Refresh(refreshToken) as ObjectResult;

            // Assert
            Assert.NotNull(actualResult);
            Assert.NotNull(actualResult.Value);
            Assert.IsType<ServiceResult>(actualResult.Value);
            Assert.Equal(StatusCodes.Status200OK, actualResult.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)actualResult.Value).ServiceStatus);
        }

        [Fact]
        public async Task AgentLogin_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var form = new LoginForm
            {
                Email = "binilvincent80@gmail.com",
                Password = "cQ1?B&9E"
            };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _agentService.Login(form.Email, form.Password).Returns(expectedResult);

            // Act
            var result = await _loginController.AgentLogin(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task AgentLogin_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var form = new LoginForm
            {
                Email = "test@test.com",
                Password = "password"
            };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _agentService.Login(form.Email, form.Password).Returns(expectedResult);

            // Act
            var result = await _loginController.AgentLogin(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task AgentRefreshSuccessTest()
        {
            // Arrange
            var refreshToken = "Valid Agent Refresh Token";
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _agentService.AgentRefresh(refreshToken).Returns(expectedResult);

            // Act
            var actualResult = await _loginController.AgentRefresh(refreshToken) as ObjectResult;

            // Assert
            Assert.NotNull(actualResult);
            Assert.NotNull(actualResult.Value);
            Assert.IsType<ServiceResult>(actualResult.Value);
            Assert.Equal(StatusCodes.Status200OK, actualResult.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)actualResult.Value).ServiceStatus);
        }
    }
}
