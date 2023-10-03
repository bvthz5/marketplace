using static MarketPlace.DataAccess.Model.DeliveryAddress;

namespace MarketPlace.DataAccess.Model;

public class Notification
{
    public enum NotificationStatus : byte
    {
        UNREAD = 0,
        READ = 1,
    }

    public enum NotificationTypes : byte
    {
        MY_PRODUCT_SOLD = 1,
        WISHLIST_PRODUCT_SOLD = 2,
    }

    public int NotificationId { get; set; }

    public NotificationTypes NotificationType { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public string Data { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public NotificationStatus Status { get; set; }
}