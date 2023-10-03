namespace MarketPlace.DataAccess.Model;

public class DeliveryAddress
{
    public enum DeliveryAddressStatus : byte
    {
        ACTIVE = 0,
        DEFAULT = 1,
        REMOVED = 2,
    }

    public int DeliveryAddressId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string StreetAddress { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int CreatedUserId { get; set; }

    public User CreatedUser { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public DeliveryAddressStatus Status { get; set; }
}