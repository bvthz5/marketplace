using System.ComponentModel.DataAnnotations;

namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class RefundOrderForm
    {
        [Required]
        [StringLength(255)]
        public string Reason { get; set; } = null!;

    }
}

