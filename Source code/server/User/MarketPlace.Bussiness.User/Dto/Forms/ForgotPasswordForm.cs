using MarketPlaceUser.Bussiness.Dto.Forms.Validations;
using System.ComponentModel.DataAnnotations;

namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class ForgotPasswordForm
    {

        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; } = null!;

        [Password]
        public string Password { get; set; } = string.Empty;
    }
}
