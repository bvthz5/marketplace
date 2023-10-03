using MarketPlaceUser.Bussiness.Dto.Forms.Validations;
using Microsoft.AspNetCore.Http;

namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class ProductImageForm
    {
        [ValidImage]
        public IFormFile[] File { get; set; } = null!;
    }
}
