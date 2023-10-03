using MarketPlace.DataAccess.Model;

namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class AdminView
    {
        public int AdminId { get; }

        public string Name { get; }

        public string Email { get; }

        public string? ProfilePic { get; }

        public DateTime CreatedDate { get; }

        public DateTime UpdatedDate { get; }

        public AdminView(Admin admin)
        {
            AdminId = admin.AdminId;
            Name = admin.Name;
            Email = admin.Email;
            ProfilePic = admin.ProfilePic;
            CreatedDate = admin.CreatedDate;
            UpdatedDate = admin.UpdatedDate;
        }
    }
}
