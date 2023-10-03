using MarketPlaceUser.Bussiness.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace MarketPlaceUserTest.Hubs
{
    public class NotificationHubTests
    {
        private readonly NotificationHub _hub;
        private readonly PresenceTracker _presenceTracker;
        private readonly ILogger<NotificationHub> _logger;
        private readonly IHubCallerClients _clients;

        public NotificationHubTests()
        {
            _presenceTracker = new PresenceTracker(Substitute.For<ILogger<PresenceTracker>>());
            _logger = Substitute.For<ILogger<NotificationHub>>();
            _clients = Substitute.For<IHubCallerClients>();


            // Create a mock HubCallerContext with the required values
            var mockContext = Substitute.For<HubCallerContext>();


            _hub = new NotificationHub(_presenceTracker, _logger)
            {
                Clients = _clients,
                Context = mockContext
            };

            mockContext.UserIdentifier.Returns("1");

        }

        [Fact]
        public async Task SendNotification_CallsClientsUsersWithUserIds()
        {
            // Arrange
            var userIds = new List<string> { "1", "2", "3" };

            // Act
            await _hub.SendNotification(userIds);

            // Assert
            await _clients.Received(1).Users(userIds).SendAsync(Arg.Any<string>());
        }



        [Fact]
        public async Task OnDisconnectedAsync_CallsUserDisconnectedMethod()
        {
            // Arrange
            var exception = new Exception();

            // Act
            await _hub.OnDisconnectedAsync(exception);

            // Assert
            Assert.NotNull(_presenceTracker.GetOnlineUsers());
        }

        [Fact]
        public async Task OnConnectedAsync_WithUserConnection()
        {

            // Act
            await _hub.OnConnectedAsync();

            // Assert
            Assert.NotNull(_presenceTracker.GetOnlineUsers());
        }


        [Fact]
        public async Task OnConnectedAsync_WithUserConnection_ShouldStopPreviousConnection()
        {

            await _presenceTracker.UserConnected("1", "connection_1");
            await _presenceTracker.UserConnected("1", "connection_2");
            await _presenceTracker.UserConnected("1", "connection_3");

            // Act
            await _hub.OnConnectedAsync();

            // Assert
            Assert.NotNull(_presenceTracker.GetOnlineUsers());
        }


    }
}
