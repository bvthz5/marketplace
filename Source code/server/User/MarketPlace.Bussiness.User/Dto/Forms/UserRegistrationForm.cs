using MarketPlaceUser.Bussiness.Dto.Forms.Validations;
using System.ComponentModel.DataAnnotations;

namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class UserRegistrationForm
    {
        [Required(ErrorMessage = "First name is required", AllowEmptyStrings = false)]
        [StringLength(60)]
        public string FirstName { get; set; } = null!;

        [StringLength(60)]
        public string? LastName { get; set; }

        [Email]
        public string Email { get; set; } = string.Empty;

        [Password]
        public string Password { get; set; } = string.Empty;
    }
}
