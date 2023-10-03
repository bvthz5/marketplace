using System.ComponentModel.DataAnnotations;

namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class LocationForm
    {
        [StringLength(200)]
        public string Address { get; set; } = null!;

        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Range(-180, 180)]
        public double Longitude { get; set; }
    }
}
