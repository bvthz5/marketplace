namespace MarketPlace.DataAccess.Model;

public class Orders
{
    public enum OrdersStatus : byte

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

    public enum PaymentsStatus : byte
    {
        UNPPAID = 0,
        PAID = 1,
        REFUNDED = 2,
    }

    public int OrdersId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int? AgentId { get; set; }

    public Agent? Agent { get; set; }

    public string DeliveryAddress { get; set; } = null!;

    public double TotalPrice { get; set; }

    public OrdersStatus OrderStatus { get; set; }

    public PaymentsStatus PaymentStatus { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? Otp { get; set; }

    public DateTime? OtpGeneratedTime { get; set; }

    public List<OrderDetails> OrderDetails { get; set; } = null!;
}
