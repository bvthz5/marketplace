using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MarketPlaceUser.Bussiness.Hubs
{
    /// <summary>
    /// Hub class for managing real-time notifications using SignalR.
    /// </summary>
    public class NotificationHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;
        private readonly ILogger<NotificationHub> _logger;

        /// <summary>
        /// Initializes a new instance of the NotificationHub class.
        /// </summary>
        /// <param name="presenceTracker">The presence tracker service.</param>
        /// <param name="unitOfWork">The unit of work service.</param>
        /// <param name="logger">The logger service.</param>
        public NotificationHub(PresenceTracker presenceTracker, ILogger<NotificationHub> logger)
        {
            _presenceTracker = presenceTracker;
            _logger = logger;
        }

        /// <summary>
        /// Overrides the OnConnectedAsync method and is called when a new client connects to the hub.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Authorize]
        public override async Task OnConnectedAsync()
        {
            string? userId = Context.UserIdentifier;

            _logger.LogInformation("Hub Connected :: UserId : {userId}", userId);

            // Track the connected user and connection id.
            var disconnectId = await _presenceTracker.UserConnected(userId, Context.ConnectionId);

            // Check if the user has more than 3 active connections from other devices or browsers.
            // If so, stop their previous first connection to maintain maximum of 3 connections per user.
            if (disconnectId != null)
            {
                // The method SendAsync is called to trigger the "Stop" method on the previous connection's client-side, so that the UI can stop the previous connection.
                await Clients.Client(disconnectId).SendAsync("Stop");
            }

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Overrides the OnDisconnectedAsync method and is called when a client disconnects from the hub.
        /// </summary>
        /// <param name="exception">The exception that caused the disconnection, if any.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _presenceTracker.UserDisconnected(Context.UserIdentifier, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Sends a notification to the specified list of user IDs.
        /// </summary>
        /// <param name="userIds">The list of user IDs to send the notification to.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendNotification(List<string> userIds)
        {
            await Clients.Users(userIds).SendAsync("Notification");
        }
    }
}
