using MarketPlace.DataAccess.Model;

namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class OrderDetailView : OrderView
    {
        public IEnumerable<OrderDetailsProductView> Items { get; set; } = null!;

        public OrderDetailView(Orders order) : base(order) { }

    }

    public class OrderDetailsProductView
    {

        public int OrderDetailsId { get; set; }

        public ProductDetailView Product { get; set; }

        public byte Status { get; set; }

        public string? Reason { get; set; }

        public OrderDetailsProductView(int orderDetailsId, ProductDetailView product, OrderHistory.HistoryStatus status, string? reason)
        {
            OrderDetailsId = orderDetailsId;
            Product = product;
            Status = (byte)status;
            Reason= reason;
        }
    }

}


