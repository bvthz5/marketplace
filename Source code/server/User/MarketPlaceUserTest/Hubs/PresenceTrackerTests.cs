using MarketPlaceUser.Bussiness.Hubs;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace MarketPlaceUserTest.Hubs
{
    public class PresenceTrackerTests
    {
        [Fact]
        public async Task UserConnected_AddsUserToOnlineUsers()
        {
            // Arrange
            var userId = "user1";
            var connectionId = "conn1";
            var logger = Substitute.For<ILogger<PresenceTracker>>();
            var presenceTracker = new PresenceTracker(logger);

            // Act
            await presenceTracker.UserConnected(userId, connectionId);

            // Assert
            var onlineUsers = await presenceTracker.GetOnlineUsers();
            Assert.Contains(userId, onlineUsers);
        }

        [Fact]
        public async Task UserConnected_NullUserId()
        {
            // Arrange
            string? userId = null;
            var connectionId = "conn1";
            var logger = Substitute.For<ILogger<PresenceTracker>>();
            var presenceTracker = new PresenceTracker(logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await presenceTracker.UserConnected(userId, connectionId));
        }

        [Fact]
        public async Task UserConnected_WhenMaxConnectionsReached_RemovesOldestConnection()
        {
            // Arrange
            var userId = "user1";
            var logger = Substitute.For<ILogger<PresenceTracker>>();
            var presenceTracker = new PresenceTracker(logger);

            // Add 3 connections for user1
            await presenceTracker.UserConnected(userId, "conn1");
            await presenceTracker.UserConnected(userId, "conn2");
            await presenceTracker.UserConnected(userId, "conn3");

            // Act
            await presenceTracker.UserConnected(userId, "conn4");

            // Assert
            var onlineUsers = await presenceTracker.GetOnlineUsers();
            Assert.Contains(userId, onlineUsers);
            Assert.DoesNotContain("conn1", onlineUsers);
        }

        [Fact]
        public async Task UserDisconnected_RemovesUserFromOnlineUsers()
        {
            // Arrange
            var userId = "user2";
            var connectionId = "conn1";
            var logger = Substitute.For<ILogger<PresenceTracker>>();
            var presenceTracker = new PresenceTracker(logger);

            await presenceTracker.UserConnected(userId, connectionId);

            // Act
            await presenceTracker.UserDisconnected(userId, connectionId);

            // Assert
            var onlineUsers = await presenceTracker.GetOnlineUsers();
            Assert.DoesNotContain(userId, onlineUsers);
        }

        [Fact]
        public async Task UserDisconnected_NullUserId()
        {
            // Arrange
            string? userId = null;
            var connectionId = "conn1";
            var logger = Substitute.For<ILogger<PresenceTracker>>();
            var presenceTracker = new PresenceTracker(logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await presenceTracker.UserDisconnected(userId, connectionId));
        }

        [Fact]
        public async Task UserDisconnected_RemovesUserFromOnlineUsers2()
        {
            // Arrange
            var userId = "user2";
            var connectionId = "conn1";
            var logger = Substitute.For<ILogger<PresenceTracker>>();
            var presenceTracker = new PresenceTracker(logger);

            // Act
            await presenceTracker.UserDisconnected(userId, connectionId);

            // Assert
            var onlineUsers = await presenceTracker.GetOnlineUsers();
            Assert.DoesNotContain(userId, onlineUsers);
        }


    }
}
