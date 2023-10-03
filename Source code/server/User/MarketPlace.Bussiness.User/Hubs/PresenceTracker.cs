using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace MarketPlaceUser.Bussiness.Hubs
{
    /// <summary>
    /// Represents an interface for tracking the online status of users in a multi-user system.
    /// </summary>
    public interface IPresenceTracker
    {
        /// <summary>
        /// Gets the list of online users.
        /// </summary>
        /// <returns>An array of user ids for all online users.</returns>
        Task<string[]> GetOnlineUsers();

        /// <summary>
        /// Registers a user connection and determines whether to terminate an existing connection.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="connectionId">The id of the new connection.</param>
        /// <returns>The id of the terminated connection if any, or null if no connection was terminated.</returns>
        Task<string?> UserConnected(string? userId, string connectionId);

        /// <summary>
        /// Removes a user connection and determines whether the user is offline.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="connectionId">The id of the connection to remove.</param>
        /// <returns>True if the user is offline, otherwise false.</returns>
        Task<bool> UserDisconnected(string? userId, string connectionId);

    }

    public class PresenceTracker : IPresenceTracker
    {
        private static readonly Dictionary<string, List<string>> _onlineUsers = new();
        private readonly ILogger<PresenceTracker> _logger;

        public PresenceTracker(ILogger<PresenceTracker> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Adds a new connection for the given user ID. If the user already has the maximum allowed number of connections, 
        /// the first connection is terminated and its connection ID is returned.
        /// </summary>
        /// <param name="userId">The ID of the user whose connection is being added</param>
        /// <param name="connectionId">The ID of the new connection to add</param>
        /// <returns>
        /// If a connection was terminated, returns the ID of the terminated connection; 
        /// otherwise, returns null.
        /// </returns>
        public Task<string?> UserConnected(string? userId, string connectionId)
        {

            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            // Initialize terminateFirstConnection to null.
            string? terminateFirstConnection = null;

            // Lock the OnlineUsers dictionary to prevent concurrent access.
            lock (_onlineUsers)
            {
                // If the dictionary contains the user ID, check if the user has the maximum number of connections.
                if (_onlineUsers.ContainsKey(userId))
                {
                    if (_onlineUsers[userId].Count >= 3)
                    {
                        // If the user has the maximum number of connections, terminate the first connection and 
                        // store its connection ID in terminateFirstConnection.
                        terminateFirstConnection = _onlineUsers[userId].ElementAtOrDefault(0);

                        // Log a warning that a connection is being terminated.
                        _logger.LogWarning("Maximum 6 Connection Allowed, Terminating Connection {connectionId} for User {userId}", terminateFirstConnection, userId);

                        // Remove the first connection from the list of connections for this user.
                        _onlineUsers[userId].RemoveAt(0);
                    }

                    // Add the new connection to the list of connections for this user.
                    _onlineUsers[userId].Add(connectionId);
                }
                else
                {
                    // If the dictionary does not contain the user ID, add a new key-value pair with the user ID and a 
                    // new list containing only the new connection ID.
                    _onlineUsers.Add(userId, new List<string> { connectionId });
                }
            }

            // Log that the user has connected.
            _logger.LogInformation("User Connected: {userId}", userId);

            // Return terminateFirstConnection (which may be null).
            return Task.FromResult(terminateFirstConnection);
        }


        /// <summary>
        /// Removes the specified connection ID from the list of connections associated with the specified user ID, and removes the user from the list of online users if they no longer have any connections.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="connectionId">The connection ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a Boolean value indicating whether the user is offline after disconnection.</returns>
        public Task<bool> UserDisconnected(string? userId, string connectionId)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            bool isOffline = false;
            lock (_onlineUsers)
            {
                if (!_onlineUsers.ContainsKey(userId)) return Task.FromResult(isOffline);

                _onlineUsers[userId].Remove(connectionId);
                if (_onlineUsers[userId].Count == 0)
                {
                    _onlineUsers.Remove(userId);
                    isOffline = true;
                }
            }

            return Task.FromResult(isOffline);
        }

        /// <summary>
        /// Gets an array of user IDs representing the online users.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an array of user IDs representing the online users.</returns>
        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock (_onlineUsers)
            {
                onlineUsers = _onlineUsers.Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }
    }
}