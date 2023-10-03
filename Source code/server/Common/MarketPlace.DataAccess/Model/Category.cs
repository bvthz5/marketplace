using System.ComponentModel.DataAnnotations;
using static MarketPlace.DataAccess.Model.DeliveryAddress;

namespace MarketPlace.DataAccess.Model;

public class Category
{
    public enum CategoryStatus : byte
    {
        INACTIVE = 0,
        ACTIVE = 1
    }

    public int CategoryId { get; set; }

    [StringLength(20)]
    public string CategoryName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public CategoryStatus Status { get; set; }
}