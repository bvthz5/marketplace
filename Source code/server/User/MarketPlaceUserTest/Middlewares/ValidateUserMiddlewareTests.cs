using MarketPlaceUser.Api.Middlewares;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;

namespace MarketPlaceUserTest.Middlewares
{
    public class ValidateUserMiddlewareTests
    {
        private ILogger<ValidateUserMiddleware> _logger;
        private ISecurityUtil _securityUtil;
        private IUserService _userService;
        private ValidateUserMiddleware _middleware;
        private readonly Mock<ISecurityUtil> _mockSecurityUtil = new();
        private readonly Mock<IUserService> _mockUserService = new();
        private readonly Mock<ILogger<ValidateUserMiddleware>> _mockLogger = new();

        public ValidateUserMiddlewareTests()
        {
            _securityUtil = Substitute.For<ISecurityUtil>();
            _userService = Substitute.For<IUserService>();
            _logger = Substitute.For<ILogger<ValidateUserMiddleware>>();
            _middleware = new ValidateUserMiddleware(_logger, _securityUtil, _userService);
        }

        [Fact]
        public async Task InvokeAsync_ValidUser_NextMiddleware()
        {
            // Arrange
            var userId = 123;
            _securityUtil.GetCurrentUserId().Returns(userId);
            _userService.IsValidActiveUser(userId).Returns(true);

            var context = new DefaultHttpContext();
            var nextMiddleware = Substitute.For<RequestDelegate>();

            // Act
            await _middleware.InvokeAsync(context, nextMiddleware);

            // Assert
            await nextMiddleware.Received(1).Invoke(context);
        }

        [Fact]
        public async Task InvokeAsync_NoUserId_NextMiddleware()
        {
            // Arrange
            _securityUtil.GetCurrentUserId().Returns(0);

            var context = new DefaultHttpContext();
            var nextMiddleware = Substitute.For<RequestDelegate>();

            // Act
            await _middleware.InvokeAsync(context, nextMiddleware);

            // Assert
            await nextMiddleware.Received(1).Invoke(context);
        }

        [Fact]
        public async Task InvokeAsync_ValidActiveUserId_CallsNextMiddleware()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ValidateUserMiddleware>>();
            var securityUtilMock = new Mock<ISecurityUtil>();
            var userServiceMock = new Mock<IUserService>();
            var middleware = new ValidateUserMiddleware(loggerMock.Object, securityUtilMock.Object, userServiceMock.Object);
            var httpContextMock = new Mock<HttpContext>();
            var requestDelegateMock = new Mock<RequestDelegate>();
            securityUtilMock.Setup(x => x.GetCurrentUserId()).Returns(1);
            userServiceMock.Setup(x => x.IsValidActiveUser(1)).ReturnsAsync(true);

            // Act
            await middleware.InvokeAsync(httpContextMock.Object, requestDelegateMock.Object);

            // Assert
            userServiceMock.Verify(x => x.IsValidActiveUser(1), Times.Once);
            requestDelegateMock.Verify(x => x(httpContextMock.Object), Times.Once);

        }
       

        [Fact]
        public async Task ValidUserId_CallsNext()
        {
            // Arrange
            var mockContext = new DefaultHttpContext();
            mockContext.Items["userId"] = 1;

            _mockSecurityUtil.Setup(x => x.GetCurrentUserId()).Returns(1);
            _mockUserService.Setup(x => x.IsValidActiveUser(It.IsAny<int>())).ReturnsAsync(true);

            var middleware = new ValidateUserMiddleware(_mockLogger.Object, _mockSecurityUtil.Object, _mockUserService.Object);

            // Act
            await middleware.InvokeAsync(mockContext, context => Task.CompletedTask);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, mockContext.Response.StatusCode);
            _mockUserService.Verify(u => u.IsValidActiveUser(1), Times.Once);
        }

    }

}


