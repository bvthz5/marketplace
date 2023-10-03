using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlace.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceUserTest.Data
{
    public class NotificationRepositoryTest
    {
        private readonly MarketPlaceDbContext _dbContext;
        private readonly INotificationRepository _notification;

        public NotificationRepositoryTest()
        {
            _dbContext = new MarketPlaceDbContext(new DbContextOptionsBuilder<MarketPlaceDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options);
            _notification = new NotificationRepository(_dbContext);
        }

        [Fact]
        public async Task Add_ValidNotification_ShouldAddAndReturnNotification()
        {
            // Arrange
            var notification = new Notification
            {
                NotificationId = 1,
                NotificationType = 0,
                UserId = 1,
                Data = "gg"
            };

            var repository = new NotificationRepository(_dbContext);

            // Act
            var result = await repository.Add(notification);

            // Assert
            Assert.Equal(notification, result);
            Assert.Contains(notification, _dbContext.Notifications);
        }

        [Fact]
        public async Task Add_NullNotification_ShouldThrowArgumentNullException()
        {
            // Arrange
            var repository = new NotificationRepository(_dbContext);

            // Act and Assert
            Assert.NotNull(repository.Add((Notification)null));
        }


        [Fact]
        public void Update_UpdatesNotificationInDbContext()
        {
            // Arrange
            var notification = new Notification
            {
                NotificationId = 1,
                NotificationType = 0,
                UserId = 1,
                Data = "Notification 1"
            };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            var repository = new NotificationRepository(dbContext);

            // Add the notification to the database
            dbContext.Notifications.Add(notification);
            dbContext.SaveChanges();

            // Modify the notification
            notification.Data = "Updated Notification";

            // Act
            var updatedNotification = repository.Update(notification);

            // Assert
            Assert.NotNull(updatedNotification);
            Assert.Equal(notification.NotificationId, updatedNotification.NotificationId);
            Assert.Equal(notification.Data, updatedNotification.Data);

            // Check if the notification is updated in the database context
            var dbNotification = dbContext.Notifications.Find(updatedNotification.NotificationId);
            Assert.NotNull(dbNotification);
            Assert.Equal(notification.NotificationId, dbNotification.NotificationId);
            Assert.Equal(notification.Data, dbNotification.Data);
        }

        [Fact]
        public async Task GetNotificationByIdAndUserId_ReturnsMatchingNotification()
        {
            // Arrange
            var userId = 1;
            var notificationId = 1;
            var notification = new Notification
            {
                NotificationId = notificationId,
                NotificationType = 0,
                UserId = userId,
                Data = "Notification 1"
            };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                dbContext.Notifications.Add(notification);
                await dbContext.SaveChangesAsync();

                var repository = new NotificationRepository(dbContext);

                // Act
                var result = await repository.GetNotificationByIdAndUserId(notificationId, userId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(notificationId, result.NotificationId);
                Assert.Equal(userId, result.UserId);
                Assert.Equal(notification.Data, result.Data);
            }
        }


        [Fact]
        public async Task GetNotificationByIdAndUserId_ReturnsNull_WhenNoMatchingNotification()
        {
            // Arrange
            var userId = 1;
            var notificationId = 1;

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                var repository = new NotificationRepository(dbContext);

                // Act
                var result = await repository.GetNotificationByIdAndUserId(notificationId, userId);

                // Assert
                Assert.NotNull(result);
            }
        }

        [Fact]
        public async Task Delete_RemovesNotificationsFromDatabase()
        {
            // Arrange
            var notifications = new List<Notification>
    {
        new Notification { NotificationId = 1 },
        new Notification { NotificationId = 2 },
        new Notification { NotificationId = 3 }
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                dbContext.Notifications.AddRange(notifications);
                await dbContext.SaveChangesAsync();

                var repository = new NotificationRepository(dbContext);

                // Act
                repository.Delete(notifications);
                await dbContext.SaveChangesAsync();

                // Assert
                var remainingNotifications = await dbContext.Notifications.ToListAsync();
                Assert.Empty(remainingNotifications); // Expecting no notifications remaining in the database
            }
        }

        [Fact]
        public async Task Delete_RemovesSelectedNotificationsFromDatabase()
        {
            // Arrange
            var notifications = new List<Notification>
    {
        new Notification { NotificationId = 1 },
        new Notification { NotificationId = 2 },
        new Notification { NotificationId = 3 }
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                dbContext.Notifications.AddRange(notifications);
                await dbContext.SaveChangesAsync();

                var repository = new NotificationRepository(dbContext);

                // Act
                var notificationsToDelete = new List<Notification> { notifications[1], notifications[2] };
                repository.Delete(notificationsToDelete);
                await dbContext.SaveChangesAsync();

                // Assert
                var remainingNotifications = await dbContext.Notifications.ToListAsync();
                Assert.Single(remainingNotifications); // Expecting one notification remaining in the database
                Assert.Equal(notifications[0].NotificationId, remainingNotifications[0].NotificationId); // Ensure the correct notification is still in the database
            }
        }

        [Fact]
        public async Task Delete_RemovesNotificationFromDatabase()
        {
            // Arrange
            var notification = new Notification { NotificationId = 1 };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                dbContext.Notifications.Add(notification);
                await dbContext.SaveChangesAsync();

                var repository = new NotificationRepository(dbContext);

                // Act
                repository.Delete(notification);
                await dbContext.SaveChangesAsync();

                // Assert
                var remainingNotifications = await dbContext.Notifications.ToListAsync();
                Assert.Empty(remainingNotifications); // Expecting no notifications remaining in the database
            }
        }

        [Fact]
        public async Task GetUnreadNotificationCount_ReturnsCorrectCount()
        {
            // Arrange
            var userId = 1;
            var unreadNotifications = new List<Notification>
    {
        new Notification { NotificationId = 1, UserId = userId, Status = Notification.NotificationStatus.UNREAD },
        new Notification { NotificationId = 2, UserId = userId, Status = Notification.NotificationStatus.UNREAD },
        new Notification { NotificationId = 3, UserId = userId, Status = Notification.NotificationStatus.READ },
        new Notification { NotificationId = 4, UserId = userId, Status = Notification.NotificationStatus.UNREAD }
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                dbContext.Notifications.AddRange(unreadNotifications);
                await dbContext.SaveChangesAsync();

                var repository = new NotificationRepository(dbContext);

                // Act
                var result = await repository.GetUnreadNotificationCount(userId);

                // Assert
                var expectedCount = unreadNotifications.Count(n => n.Status == Notification.NotificationStatus.UNREAD);
                Assert.Equal(expectedCount, result);
            }
        }


        [Fact]
        public async Task GetNotificationsByUserIdAndStatus_ReturnsEmptyListWhenNoMatchingNotifications()
        {
            // Arrange
            int userId = 1;
            Notification.NotificationStatus status = Notification.NotificationStatus.UNREAD;

            var notifications = new List<Notification>
    {
        new Notification { NotificationId = 1, UserId = 1, Status = Notification.NotificationStatus.READ, Data = "Notification 1", CreatedDate = DateTime.Now },
        new Notification { NotificationId = 2, UserId = 2, Status = Notification.NotificationStatus.READ, Data = "Notification 2", CreatedDate = DateTime.Now },
        new Notification { NotificationId = 3, UserId = 1, Status = Notification.NotificationStatus.READ, Data = "Notification 3", CreatedDate = DateTime.Now },
        // Add more notifications with different user IDs and statuses
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var dbContext = new MarketPlaceDbContext(dbContextOptions))
            {
                dbContext.Notifications.AddRange(notifications);
                await dbContext.SaveChangesAsync();

                var repository = new NotificationRepository(dbContext);

                // Act
                var result = await repository.GetNotificationsByUserIdAndStatus(userId, status);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result); // Expecting an empty list when no matching notifications are found
            }
        }





    }
}
