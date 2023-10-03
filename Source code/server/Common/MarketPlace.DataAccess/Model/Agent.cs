namespace MarketPlace.DataAccess.Model;

public class Agent
{
    public enum DeliveryAgentStatus : byte
    {
        PASSWORD_NOT_CHANGED = 0,
        ACTIVE = 1,
        BLOCKED = 2,
        DELETED = 3,
    }

    public int AgentId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? VerificationCode { get; set; }

    public string? ProfilePic { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public DeliveryAgentStatus Status { get; set; }
}