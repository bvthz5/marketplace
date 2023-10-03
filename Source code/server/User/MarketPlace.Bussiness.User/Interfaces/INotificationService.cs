using MarketPlaceUser.Bussiness.Helper;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface INotificationService
    {
        Task<ServiceResult> GetNotificationsByUserId(int userId);

        Task<ServiceResult> MarkAsRead(int userId);

        Task<ServiceResult> DeleteNotification(int userId, int notificationId);

        Task<ServiceResult> DeleteAllNotificationsByUserId(int userId);

        Task<ServiceResult> GetUnreadNotificationCount(int userId);
    }
}