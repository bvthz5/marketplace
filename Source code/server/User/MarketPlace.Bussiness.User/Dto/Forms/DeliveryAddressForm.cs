using MarketPlaceUser.Bussiness.Dto.Forms.Validations;
using System.ComponentModel.DataAnnotations;

namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class DeliveryAddressForm
    {
        [StringLength(60)]
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(200)]
        [Address]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(300)]
        [Address]
        public string StreetAddress { get; set; } = null!;

        [StringLength(50)]
        [Required]
        [Place]
        public string City { get; set; } = null!;

        [StringLength(50)]
        [Required]
        [Place]
        public string State { get; set; } = null!;

        [Required]
        [ZipCode]
        public string ZipCode { get; set; } = null!;

        [PhoneNumber]
        [Required]
        public string Phone { get; set; } = null!;

    }
}
