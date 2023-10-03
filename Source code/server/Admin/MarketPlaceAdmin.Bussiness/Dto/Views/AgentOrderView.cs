using MarketPlace.DataAccess.Model;

namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class AgentOrderView
    {
        public int OrdersId { get; }

        public string OrderNumber { get; }

        public DeliveryAddressView DeliveryAddress { get; }

        public double TotalPrice { get; }

        public byte OrderStatus { get; }

        public DateTime OrderDate { get; }

        public int? AgentId { get; }

        public string? AgentName { get; set; }

        public AgentOrderView(Orders orders)
        {
            OrdersId = orders.OrdersId;
            OrderNumber = orders.OrderNumber;
            DeliveryAddress = new DeliveryAddressView(orders.DeliveryAddress);
            TotalPrice = orders.TotalPrice;
            OrderStatus = (byte)orders.OrderStatus;
            OrderDate = orders.OrderDate;
            AgentId = orders.AgentId;
            AgentName = orders.Agent?.Name;
        }
    }
}
