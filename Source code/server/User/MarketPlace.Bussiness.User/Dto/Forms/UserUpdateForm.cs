using MarketPlaceUser.Bussiness.Dto.Forms.Validations;
using System.ComponentModel.DataAnnotations;

namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class UserUpdateForm
    {
        [Required(ErrorMessage = "First Name is required", AllowEmptyStrings = false)]
        [StringLength(60)]
        public string FirstName { get; set; } = null!;

        [StringLength(60)]
        public string? LastName { get; set; }

        [StringLength(255)]
        [RegularExpression("^[\\d \\w \\s]+$", ErrorMessage = "Only Alphabets, Numbers and space allowed.")]
        public string? Address { get; set; }

        [StringLength(50)]
        [RegularExpression("^[\\d \\w \\s]+$", ErrorMessage = "Only Alphabets, Numbers and space allowed.")]
        public string? State { get; set; }

        [StringLength(50)]
        [RegularExpression("^[\\d \\w \\s]+$", ErrorMessage = "Only Alphabets, Numbers and space allowed.")]
        public string? District { get; set; }

        [StringLength(50)]
        [RegularExpression("^[\\d \\w \\s]+$", ErrorMessage = "Only Alphabets, Numbers and space allowed.")]
        public string? City { get; set; }

        [PhoneNumber(Nullable = true)]
        public string? PhoneNumber { get; set; }


    }
}
