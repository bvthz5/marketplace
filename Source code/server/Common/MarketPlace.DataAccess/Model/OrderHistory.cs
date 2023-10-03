using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataAccess.Model;

public class OrderHistory
{
    public enum HistoryStatus : byte
    {
        CREATED = 0,
        FAILED = 1,
        CONFIRMED = 2,
        WAITING_FOR_PICKUP = 3,
        INTRANSIT = 4,
        OUTFORDELIVERY = 5,
        CANCELLED = 6,
        DELIVERED = 7,
    }

    public int OrderHistoryId { get; set; }

    public int OrderDetailsId { get; set; }

    public OrderDetails OrderDetails { get; set; } = null!;

    public HistoryStatus Status { get; set; }

    public DateTime Date { get; set; }

    [StringLength(255)]
    public string? Remark { get; set; }
}
