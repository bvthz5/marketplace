using MarketPlaceUser.Api.Controllers;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace MarketPlaceUserTest.TestController
{
    public class NotificationControllerTests
    {
        private readonly INotificationService _notificationService;
        private readonly ISecurityUtil _securityUtil;
        private readonly NotificationController _notificationController;

        public NotificationControllerTests()
        {
            _notificationService = Substitute.For<INotificationService>();
            _securityUtil = Substitute.For<ISecurityUtil>();
            _notificationController = new NotificationController(_notificationService, _securityUtil);
        }

        [Fact]
        public async Task GetNotifications_Should_Return_OkResult_With_ServiceResult()
        {
            // Arrange
            var expected = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _notificationService.GetNotificationsByUserId(Arg.Any<int>()).Returns(expected);
            _securityUtil.GetCurrentUserId().Returns(1);

            // Act
            var result = await _notificationController.GetNotifications();

            // Assert
            Assert.IsType<ObjectResult>(result);
            var okResult = (ObjectResult)result;
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public async Task GetUnreadNotificationCount_Should_Return_OkResult_With_ServiceResult()
        {
            // Arrange
            var expected = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _notificationService.GetUnreadNotificationCount(Arg.Any<int>()).Returns(expected);
            _securityUtil.GetCurrentUserId().Returns(1);

            // Act
            var result = await _notificationController.GetUnreadNotificationCount();

            // Assert
            Assert.IsType<ObjectResult>(result);
            var okResult = (ObjectResult)result;
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public async Task MarkAsRead_Should_Return_OkResult_With_ServiceResult()
        {
            // Arrange
            var expected = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _notificationService.MarkAsRead(Arg.Any<int>()).Returns(expected);
            _securityUtil.GetCurrentUserId().Returns(1);

            // Act
            var result = await _notificationController.MarkAsRead();

            // Assert
            Assert.IsType<ObjectResult>(result);
            var okResult = (ObjectResult)result;
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public async Task DeleteAllNotifications_Should_Return_OkResult_With_ServiceResult()
        {
            // Arrange
            var expected = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _notificationService.DeleteAllNotificationsByUserId(Arg.Any<int>()).Returns(expected);
            _securityUtil.GetCurrentUserId().Returns(1);

            // Act
            var result = await _notificationController.DeleteAllNotifications();

            // Assert
            Assert.IsType<ObjectResult>(result);
            var okResult = (ObjectResult)result;
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public async Task DeleteNotification_Should_Return_OkResult_With_ServiceResult()
        {
            // Arrange
            var expected = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _notificationService.DeleteNotification(Arg.Any<int>(), Arg.Any<int>()).Returns(expected);
            _securityUtil.GetCurrentUserId().Returns(1);

            // Act
            var result = await _notificationController.DeleteNotification(1);

            // Assert
            Assert.IsType<ObjectResult>(result);
            var okResult = (ObjectResult)result;
            Assert.Equal(expected, okResult.Value);
        }
    }
}
