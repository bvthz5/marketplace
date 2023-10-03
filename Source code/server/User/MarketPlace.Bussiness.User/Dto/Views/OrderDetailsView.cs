using MarketPlace.DataAccess.Model;

namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class OrderDetailsView : InitialOrderView
    {
        public int OrderDetailsId { get; set; }

        public DateTime CreatedDate { get; set; }

        public byte OrderStatus { get; set; }

        public ProductView ProductView { get; set; }

        public UserView SellerView { get; set; }

        public DeliveryAddressOrderView DeliveryAddressOrderView { get; set; }

        public OrderDetailsView(OrderDetails orderDetails, string? thumbnail) : base(orderDetails.Order)
        {
            OrderDetailsId = orderDetails.OrderDetailsId;
            ProductView = new ProductView(orderDetails.Product, thumbnail);
            SellerView = new UserView(orderDetails.Product.CreatedUser);
            DeliveryAddressOrderView = new DeliveryAddressOrderView(orderDetails.Order.DeliveryAddress);
            CreatedDate = orderDetails.CreatedDate;
            UpdatedDate = orderDetails.UpdatedDate;
            OrderStatus = (byte)orderDetails.Histories.Last().Status;
        }
    }
}
