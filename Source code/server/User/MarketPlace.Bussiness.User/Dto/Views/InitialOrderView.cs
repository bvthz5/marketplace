using MarketPlace.DataAccess.Model;

namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class InitialOrderView
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string Email { get; set; }

        public int UserId { get; set; }

        public double TotalPrice { get; set; }

        public byte PaymentStatus { get; set; }

        public InitialOrderView(Orders order)
        {
            OrderId = order.OrdersId;
            OrderNumber = order.OrderNumber;
            UserId = order.UserId;
            Email = order.User.Email;
            TotalPrice = order.TotalPrice;
            PaymentStatus = (byte)order.PaymentStatus;
        }
    }
}
