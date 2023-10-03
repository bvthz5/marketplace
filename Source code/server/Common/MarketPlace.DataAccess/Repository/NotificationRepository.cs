using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.DataAccess.Repository;
public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    private readonly MarketPlaceDbContext _dbContext;

    public NotificationRepository(MarketPlaceDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Notification?> GetNotificationByIdAndUserId(int notificationId, int userId)
    {
        return await _dbContext.Notifications
                                    .FirstOrDefaultAsync(notification => notification.NotificationId == notificationId && notification.UserId == userId);
    }

    public async Task<List<Notification>> GetNotificationsByUserId(int userId)
    {
        return await _dbContext.Notifications
                                    .Include(notification => notification.User)
                                    .Where(notification => notification.UserId == userId)
                                    .OrderByDescending(notification => notification.CreatedDate)
                                    .ToListAsync();
    }

    public async Task<List<Notification>> GetNotificationsByUserIdAndStatus(int userId, Notification.NotificationStatus status)
    {
        return await _dbContext.Notifications
                                    .Include(notification => notification.User)
                                    .Where(notification => notification.UserId == userId && notification.Status == status)
                                    .ToListAsync();
    }

    public async Task<int> GetUnreadNotificationCount(int userId)
    {
        return await _dbContext.Notifications.Where(notification => notification.UserId == userId && notification.Status == Notification.NotificationStatus.UNREAD).CountAsync();
    }
}