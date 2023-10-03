using MarketPlace.DataAccess.Model;

namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class UserView
    {
        public int UserId { get; }

        public string FirstName { get; }

        public string? LastName { get; }

        public string Email { get; }

        public byte Role { get; }

        public byte Status { get; }

        public DateTime CreatedDate { get; }

        public string? ProfilePic { get; set; }

        public UserView(User user)
        {
            UserId = user.UserId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Status = (byte)user.Status;
            Role = (byte)user.Role;
            ProfilePic = user.ProfilePic;
            CreatedDate = user.CreatedDate;
        }
    }
}
