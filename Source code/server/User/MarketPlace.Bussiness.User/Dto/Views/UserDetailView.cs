using MarketPlace.DataAccess.Model;

namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class UserDetailView : UserView
    {
        public string? Address { get; }

        public string? State { get; }

        public string? District { get; }

        public string? City { get; }

        public string? PhoneNumber { get; }

        public DateTime UpdatedDate { get; set; }

        public UserDetailView(User user) : base(user)
        {
            City = user.City;
            State = user.State;
            Address = user.Address;
            District = user.District;
            PhoneNumber = user.PhoneNumber;
            UpdatedDate = user.UpdatedDate;
        }
    }
}
