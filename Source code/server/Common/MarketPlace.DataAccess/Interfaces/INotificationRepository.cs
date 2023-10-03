using MarketPlace.DataAccess.Model;

namespace MarketPlace.DataAccess.Interfaces;

public interface INotificationRepository : IRepository<Notification>
{
    Task<Notification?> GetNotificationByIdAndUserId(int notificationId, int userId);

    Task<List<Notification>> GetNotificationsByUserId(int userId);

    Task<List<Notification>> GetNotificationsByUserIdAndStatus(int userId, Notification.NotificationStatus status);

    Task<int> GetUnreadNotificationCount(int userId);
}