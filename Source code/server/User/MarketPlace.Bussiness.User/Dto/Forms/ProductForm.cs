using System.ComponentModel.DataAnnotations;

namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class ProductForm
    {

        [Required(AllowEmptyStrings = false)]
        [StringLength(200)]
        [RegularExpression("^[A-Za-z0-9!@%&()_\\-,.\"'+|:/\\n\\s]+$", ErrorMessage = "Invalid Charector Present")]
        public string ProductName { get; set; } = null!;

        [StringLength(1000)]
        public string? ProductDescription { get; set; }

        public int CategoryId { get; set; }

        [Range(100.0, 500000.0)]
        public double Price { get; set; }

        [Required]
        public LocationForm Location { get; set; } = null!;
    }
}
