using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NSubstitute;

namespace MarketPlaceUserTest.Servicetest
{
    public class NotificationServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;
        private readonly INotificationService _notificationService;

        public NotificationServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _logger = Substitute.For<ILogger<NotificationService>>();
            _notificationService = new NotificationService(_unitOfWork, _logger);
        }

        [Fact]
        public async Task DeleteAllNotificationsByUserId_WithNotifications_ReturnsSuccessResult()
        {
            // Arrange
            int userId = 1;
            List<Notification> notifications = new List<Notification>
            {
                new Notification { NotificationId = 1, UserId = userId },
                new Notification { NotificationId = 2, UserId = userId }
            };
            _unitOfWork.NotificationRepository.GetNotificationsByUserId(userId).Returns(notifications);

            // Act
            ServiceResult result = await _notificationService.DeleteAllNotificationsByUserId(userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Notifications Deleted", result.Message);
            _unitOfWork.NotificationRepository.Received().Delete(notifications);
            await _unitOfWork.Received().SaveAsync();
            Assert.True(result.Status);
        }

        [Fact]
        public async Task DeleteAllNotificationsByUserId_WithoutNotifications_ReturnsNotFoundResult()
        {
            // Arrange
            int userId = 1;
            _unitOfWork.NotificationRepository.GetNotificationsByUserId(userId).Returns(new List<Notification>());

            // Act
            ServiceResult result = await _notificationService.DeleteAllNotificationsByUserId(userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Notifications Not Found", result.Message);
            _unitOfWork.NotificationRepository.DidNotReceiveWithAnyArgs().Delete(Arg.Any<Notification>());
            _unitOfWork.NotificationRepository.DidNotReceiveWithAnyArgs().Delete(Arg.Any<List<Notification>>());
            await _unitOfWork.DidNotReceive().SaveAsync();
            Assert.False(result.Status);
        }

        // Delete Notification

        [Fact]
        public async Task DeleteNotification_Found_DeletesAndReturnsSuccessResult()
        {
            // Arrange
            int userId = 1;
            int notificationId = 1;
            Notification notification = new() { NotificationId = notificationId, UserId = userId };
            _unitOfWork.NotificationRepository.GetNotificationByIdAndUserId(notificationId, userId).Returns(notification);

            // Act
            var result = await _notificationService.DeleteNotification(userId, notificationId);

            // Assert
            _unitOfWork.NotificationRepository.Received().Delete(Arg.Any<Notification>());
            await _unitOfWork.Received().SaveAsync();
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Notification Deleted", result.Message);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task DeleteNotification_NotFound_ReturnsNotFoundResult()
        {
            // Arrange
            int userId = 1;
            int notificationId = 1;
            Notification? notification = null;
            _unitOfWork.NotificationRepository.GetNotificationByIdAndUserId(notificationId, userId).Returns(notification);

            // Act
            var result = await _notificationService.DeleteNotification(userId, notificationId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Notification Not Found", result.Message);
            Assert.False(result.Status);
        }

        // GetNotifications

        [Fact]
        public async Task GetNotificationsByUserId_ReturnsSuccessWithData()
        {
            // Arrange
            int userId = 1;
            var notifications = new List<Notification>
            {
                new Notification { NotificationId = 1, UserId = userId, Data = "Notification 1" },
                new Notification { NotificationId = 2, UserId = userId, Data = "Notification 2" }
            };

            _unitOfWork.NotificationRepository.GetNotificationsByUserId(userId).Returns(notifications);

            // Act
            var result = await _notificationService.GetNotificationsByUserId(userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Notifications", result.Message);
            Assert.NotNull(result.Data);
 
            var notificationViews = result.Data as IEnumerable<NotificationView>;
            Assert.NotNull(notificationViews);
            Assert.Equal(2, notificationViews.Count());
            var firstNotificationView = notificationViews.First();
            Assert.Equal(1, firstNotificationView.NotificationId);
            Assert.Equal(0, firstNotificationView.Type);
            Assert.Equal("Notification 1", firstNotificationView.Data);
            Assert.Equal(0, firstNotificationView.Status);
            Assert.Equal(notifications.First().CreatedDate, firstNotificationView.CreatedDate);
            Assert.Equal(notifications.First().UpdatedDate, firstNotificationView.UpdatedDate);
        }

        [Fact]
        public async Task GetNotificationsByUserId_ReturnsSuccessWithEmptyData()
        {
            // Arrange
            int userId = 1;
            var notifications = new List<Notification>();
            _unitOfWork.NotificationRepository.GetNotificationsByUserId(userId).Returns(notifications);

            // Act
            var result = await _notificationService.GetNotificationsByUserId(userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Notifications", result.Message);
            Assert.NotNull(result.Data);

            var notificationViews = result.Data as IEnumerable<NotificationView>;
            Assert.NotNull(notificationViews);
            Assert.Empty(notificationViews);
            Assert.True(result.Status);

            foreach (var notificationView in notificationViews)
            {
                Assert.IsType<NotificationView>(notificationView);
                var notification = notifications.Single(n => n.NotificationId == notificationView.NotificationId);
                Assert.Equal((byte)notification.NotificationType, notificationView.Type);
                Assert.Equal(notification.Data, notificationView.Data);
                Assert.Equal((byte)notification.Status, notificationView.Status);
                Assert.Equal(notification.CreatedDate, notificationView.CreatedDate);
                Assert.Equal(notification.UpdatedDate, notificationView.UpdatedDate);
            }
        }


        // Unread Notification Count

        [Fact]
        public async Task GetUnreadNotificationCount_ShouldReturnUnreadNotificationCount_WhenCalledWithValidUserId()
        {
            // Arrange
            int userId = 123;
            int unreadNotificationCount = 2;
            _unitOfWork.NotificationRepository.GetUnreadNotificationCount(userId).Returns(unreadNotificationCount);

            // Act
            var result = await _notificationService.GetUnreadNotificationCount(userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Unread Notification Count", result.Message);
            Assert.Equal(unreadNotificationCount, result.Data);
            Assert.True(result.Status);
        }

        // Mark As Read

        [Fact]
        public async Task MarkAsRead_NoUnreadNotifications_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            List<Notification> notifications = new List<Notification>();
            _unitOfWork.NotificationRepository.GetNotificationsByUserIdAndStatus(userId, Notification.NotificationStatus.UNREAD).Returns(notifications);

            // Act
            var result = await _notificationService.MarkAsRead(userId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Already Read", result.Message);
            _unitOfWork.NotificationRepository.DidNotReceive().Update(Arg.Any<IEnumerable<Notification>>());
            await _unitOfWork.DidNotReceive().SaveAsync();
            Assert.False(result.Status);
        }


        [Fact]
        public async Task MarkAsRead_UnreadNotificationsExist_ReturnsSuccess()
        {
            // Arrange
            int userId = 1;
            List<Notification> notifications = new List<Notification>()
            {
                new Notification() { UserId = userId, Status = Notification.NotificationStatus.UNREAD },
                new Notification() { UserId = userId, Status = Notification.NotificationStatus.UNREAD },
            };
            _unitOfWork.NotificationRepository.GetNotificationsByUserIdAndStatus(userId, Notification.NotificationStatus.UNREAD).Returns(notifications);

            // Act
            var result = await _notificationService.MarkAsRead(userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("All Notifications Marked As Read", result.Message);
            _unitOfWork.NotificationRepository.Received(1).Update(Arg.Is<IEnumerable<Notification>>(n => n.All(n => n.Status == Notification.NotificationStatus.READ)));
            await _unitOfWork.Received(1).SaveAsync();
            Assert.True(result.Status);
        }

    }
}
