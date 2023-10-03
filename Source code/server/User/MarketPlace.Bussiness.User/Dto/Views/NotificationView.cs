using MarketPlace.DataAccess.Model;

namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class NotificationView
    {
        public int NotificationId { get; set; }

        public byte Type { get; set; }

        public string Data { get; set; }

        public byte Status { get; set; }

        public DateTime UpdatedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public NotificationView(Notification notification)
        {
            NotificationId = notification.NotificationId;
            Type = (byte)notification.NotificationType;
            Data = notification.Data;
            Status = (byte)notification.Status;
            CreatedDate = notification.CreatedDate;
            UpdatedDate = notification.UpdatedDate;
        }

    }
}
