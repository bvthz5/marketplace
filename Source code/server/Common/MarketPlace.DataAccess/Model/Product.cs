using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataAccess.Model;

public class Product
{
    public enum ProductStatus : byte
    {
        INACTIVE = 0,
        ACTIVE = 1,
        PENDING = 2,
        SOLD = 3,
        DELETED = 4,
        DRAFT = 5,
        ONPROCESS = 6,
    }

    public int ProductId { get; set; }

    [StringLength(200)]
    public string ProductName { get; set; } = null!;

    [StringLength(1000)]
    public string? ProductDescription { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public int CreatedUserId { get; set; }

    public User CreatedUser { get; set; } = null!;

    public double Price { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }

    [StringLength(200)]
    public string Address { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public ProductStatus Status { get; set; }

    public IList<Photos>? Photos { get; set; }
    
}