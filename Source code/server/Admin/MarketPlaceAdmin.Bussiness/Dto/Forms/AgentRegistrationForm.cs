using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;
using System.ComponentModel.DataAnnotations;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{
    public class AgentRegistrationForm
    {
        [Required(ErrorMessage = "Name is required", AllowEmptyStrings = false)]
        [StringLength(60)]
        public string Name { get; set; } = null!;

        [Email]
        public string Email { get; set; } = string.Empty;

        [PhoneNumber]
        public string PhoneNumber { get; set; } = null!;
    }

    public class EditAgentForm
    {
        [Required(ErrorMessage = "Name is required", AllowEmptyStrings = false)]
        [StringLength(60)]
        public string Name { get; set; } = null!;

        [PhoneNumber]
        public string PhoneNumber { get; set; } = null!;
    }
}

