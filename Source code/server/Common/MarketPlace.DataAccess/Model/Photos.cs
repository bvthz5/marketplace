namespace MarketPlace.DataAccess.Model;

public class Photos
{
    public int PhotosId { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string Photo { get; set; } = null!;
}