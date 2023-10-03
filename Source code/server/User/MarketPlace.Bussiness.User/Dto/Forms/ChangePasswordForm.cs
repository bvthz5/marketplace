using MarketPlaceUser.Bussiness.Dto.Forms.Validations;

namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class ChangePasswordForm
    {

        [Password]
        public string CurrentPassword { get; set; } = string.Empty;

        [Password]
        public string NewPassword { get; set; } = string.Empty;

        [Password]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
