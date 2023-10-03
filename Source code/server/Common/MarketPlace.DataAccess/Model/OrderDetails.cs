namespace MarketPlace.DataAccess.Model;

public class OrderDetails
{
    public int OrderDetailsId { get; set; }

    public int OrderId { get; set; }

    public Orders Order { get; set; } = null!;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public List<OrderHistory> Histories { get; set; } = null!;
}