using MarketPlace.DataAccess.Model;
using static MarketPlace.DataAccess.Model.OrderHistory;

namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class OrderHistoryView

    {
        public int OrderHistoryId { get; set; }

        public int OrderDetailsId { get; set; }

        public HistoryStatus Status { get; set; }

        public DateTime Date { get; set; }

        public string? Remark { get; set; }

        public OrderHistoryView(OrderHistory orderHistory)
        {
            OrderHistoryId = orderHistory.OrderHistoryId;
            OrderDetailsId = orderHistory.OrderDetailsId;
            Status = orderHistory.Status;
            Date = orderHistory.Date;
            Remark = orderHistory.Remark;
        }
    }
}
