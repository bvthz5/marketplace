using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using Microsoft.Extensions.Logging;

namespace MarketPlaceUser.Bussiness.Services
{
    /// <summary>
    /// Service responsible for managing notifications.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<NotificationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="logger">The logger.</param>
        public NotificationService(IUnitOfWork unitOfWork, ILogger<NotificationService> logger)
        {
            _uow = unitOfWork;
            _logger = logger;
        }

        /// </summary>
        /// <param name="userId">The ID of the user whose notifications should be deleted.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating the outcome of the operation.</returns>
        public async Task<ServiceResult> DeleteAllNotificationsByUserId(int userId)
        {
            ServiceResult result = new();

            List<Notification> notifications = await _uow.NotificationRepository.GetNotificationsByUserId(userId);

            if (notifications.Count == 0)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Notifications Not Found";
                return result;
            }

            _uow.NotificationRepository.Delete(notifications);

            await _uow.SaveAsync();

            _logger.LogInformation("Notifications Cleared for User : {userId}", userId);

            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Notifications Deleted";
            return result;
        }

        /// <summary>
        /// Deletes a notification for a given user.
        /// </summary>
        /// <param name="userId">The ID of the user who owns the notification.</param>
        /// <param name="notificationId">The ID of the notification to delete.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating the success or failure of the operation.</returns>
        public async Task<ServiceResult> DeleteNotification(int userId, int notificationId)
        {
            ServiceResult result = new();

            // Get the notification to be deleted by ID and user ID.
            Notification? notification = await _uow.NotificationRepository.GetNotificationByIdAndUserId(notificationId, userId);

            // If the notification does not exist, return a not found response.
            if (notification is null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Notification Not Found";
                return result;
            }

            // Log a message indicating that the notification has been cleared.
            _logger.LogInformation("Notifications : {notifcationId} Cleared for User : {userId}", notificationId, userId);

            // Delete the notification from the repository.
            _uow.NotificationRepository.Delete(notification);

            // Save changes to the repository.
            await _uow.SaveAsync();

            // Return a success response.
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Notification Deleted";
            return result;
        }

        /// <summary>
        /// Get all notifications for a specific user by user ID.
        /// </summary>
        /// <param name="userId">The ID of the user for whom to retrieve notifications.</param>
        /// <returns>A <see cref="ServiceResult"/> object containing a list of notifications, with the <see cref="ServiceResult.Data"/> property containing a collection of <see cref="NotificationView"/> objects.</returns>
        /// <remarks>This method retrieves all notifications for a user with the specified user ID, and converts each <see cref="Notification"/> object to a <see cref="NotificationView"/> object. The resulting list of <see cref="NotificationView"/> objects is returned as part of the <see cref="ServiceResult"/> object's <see cref="ServiceResult.Data"/> property.</remarks>
        public async Task<ServiceResult> GetNotificationsByUserId(int userId)
        {
            ServiceResult result = new();

            List<Notification> notifications = await _uow.NotificationRepository.GetNotificationsByUserId(userId);

            _logger.LogInformation("Notifications List Generated for User : {userId}", userId);

            result.ServiceStatus = ServiceStatus.Success;
            result.Data = notifications.Select(notification => new NotificationView(notification));
            result.Message = "Notifications";
            return result;
        }

        /// <summary>
        /// Gets the count of unread notifications for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A <see cref="ServiceResult"/> object containing the result of the operation.</returns>
        public async Task<ServiceResult> GetUnreadNotificationCount(int userId)
        {
            return new ServiceResult()
            {
                ServiceStatus = ServiceStatus.Success,
                Message = "Unread Notification Count",
                Data = await _uow.NotificationRepository.GetUnreadNotificationCount(userId),
            };
        }


        /// <summary>
        /// Marks all unread notifications for a given user as read.
        /// </summary>
        /// <param name="userId">The ID of the user whose notifications should be marked as read.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating the outcome of the operation.</returns>
        public async Task<ServiceResult> MarkAsRead(int userId)
        {
            ServiceResult result = new();

            // Get all unread notifications for the given user.
            List<Notification> notifications = await _uow.NotificationRepository.GetNotificationsByUserIdAndStatus(userId, Notification.NotificationStatus.UNREAD);

            // If there are no unread notifications, return a bad request response.
            if (notifications.Count == 0)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Already Read";
                return result;
            }

            // Mark all unread notifications as read and update the updated date.
            _uow.NotificationRepository.Update(
                notifications.Select(notification =>
                {
                    notification.Status = Notification.NotificationStatus.READ;
                    notification.UpdatedDate = DateTime.Now;
                    return notification;
                })
            );

            // Save the changes to the database.
            await _uow.SaveAsync();

            // Log the operation.
            _logger.LogInformation("Notifications Marked As Read for User : {userId}", userId);

            // Return a success response.
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "All Notifications Marked As Read";
            return result;
        }
    }
}