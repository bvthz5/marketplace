namespace MarketPlace.DataAccess.Model;

public class WishList
{
    public int WishListId { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;
}