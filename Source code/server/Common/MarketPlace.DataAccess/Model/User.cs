using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataAccess.Model;


public class User
{
    public enum UserStatus : byte
    {
        INACTIVE = 0,
        ACTIVE = 1,
        BLOCKED = 2,
        DELETED = 3,
    }

    public enum UserRole : byte
    {
        USER = 0,
        REQUESTED = 1,
        SELLER = 2,
        ADMIN = 3
    }

    public int UserId { get; set; }

    [StringLength(60)]
    public string FirstName { get; set; } = null!;

    [StringLength(60)]
    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    [StringLength(255)]
    public string? Address { get; set; }

    [StringLength(255)]
    public string? State { get; set; }

    [StringLength(255)]
    public string? District { get; set; }

    [StringLength(255)]
    public string? City { get; set; }

    [StringLength(13)]
    public string? PhoneNumber { get; set; }

    public string? ProfilePic { get; set; }

    [StringLength(255)]
    public string? VerificationCode { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public UserStatus Status { get; set; }
}
