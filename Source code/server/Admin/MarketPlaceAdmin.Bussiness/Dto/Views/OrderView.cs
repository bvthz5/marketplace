using MarketPlace.DataAccess.Model;

namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class OrderView
    {
        public int OrdersId { get; }

        public string OrderNumber { get; }

        public UserDetailView Buyer { get; }

        public DeliveryAddressView DeliveryAddress { get; }

        public double TotalPrice { get; }

        public byte OrderStatus { get; }

        public byte PaymentStatus { get; }

        public DateTime OrderDate { get; }

        public DateTime PaymentDate { get; }

        public OrderView(Orders orders)
        {
            OrdersId = orders.OrdersId;
            OrderNumber = orders.OrderNumber;
            Buyer = new UserDetailView(orders.User);
            DeliveryAddress = new DeliveryAddressView(orders.DeliveryAddress);
            TotalPrice = orders.TotalPrice;
            PaymentStatus = (byte)orders.PaymentStatus;
            OrderStatus = (byte)orders.OrderStatus;
            OrderDate = orders.OrderDate;
            PaymentDate = orders.PaymentDate;
        }
    }
}
