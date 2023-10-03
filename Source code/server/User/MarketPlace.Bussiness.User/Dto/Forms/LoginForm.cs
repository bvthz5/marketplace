using MarketPlaceUser.Bussiness.Dto.Forms.Validations;

namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class LoginForm
    {
        [LoginField("Email")]
        public string Email { get; set; } = null!;

        [LoginField("Password")]
        public string Password { get; set; } = null!;
    }
}
