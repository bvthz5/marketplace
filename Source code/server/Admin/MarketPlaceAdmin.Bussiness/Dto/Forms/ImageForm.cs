using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;
using Microsoft.AspNetCore.Http;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{
    /// <summary>
    /// A class representing an image form data for uploading images.
    /// </summary>
    /// <remarks>
    /// The image file uploaded using this form data should meet the following requirements:
    /// - The file must not be null.
    /// - The file must be a valid image file.
    /// </remarks>
    public class ImageForm
    {
        /// <summary>
        /// Gets or sets the image file to be uploaded.
        /// </summary>
        [ValidImage]
        public IFormFile File { get; set; } = null!;
    }


}
