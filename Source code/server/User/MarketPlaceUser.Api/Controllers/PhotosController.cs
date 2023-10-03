using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceUser.Api.Controllers
{
    [Route("api/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IPhotosService _photosService;
        public PhotosController(IPhotosService photosService)
        {
            _photosService = photosService;
        }

        /// <summary>
        /// Adds photos for the specified product.
        /// </summary>
        /// <param name="productId">The ID of the product to add photos for.</param>
        /// <param name="image">The form data containing the image(s) to upload.</param>
        /// <returns>The status code and service result indicating the success or failure of the operation.</returns>
        [Authorize(Roles = "2")]
        [HttpPost("{productId:int:min(1)}")]
        public async Task<IActionResult> Add(int productId, [FromForm] ProductImageForm image)
        {
            ServiceResult result = await _photosService.AddPhotosAsync(productId, image);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Gets the photos for the specified product.
        /// </summary>
        /// <param name="productId">The ID of the product to get photos for.</param>
        /// <returns>The status code and service result containing the photo data.</returns>
        [HttpGet("{productId:int:min(1)}")]
        public async Task<IActionResult> GetPhotos(int productId)
        {
            ServiceResult result = await _photosService.GetPhotosAsync(productId);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Gets the photo(s) for the specified file path.
        /// </summary>
        /// <param name="filePath">The file path of the photo(s) to get.</param>
        /// <returns>The file stream of the photo(s) with the specified file path.</returns>
        [HttpGet("path/{filePath}")]
        public async Task<IActionResult> GetPhotosByPath(string filePath)
        {
            FileStream? fileStream = await _photosService.GetPhotosByName(filePath);
            if (fileStream == null)
                return new NotFoundResult();

            return File(fileStream, "image/jpeg");
        }

        /// <summary>
        /// Deletes all photos related to the specified product.
        /// </summary>
        /// <param name="productId">The ID of the product to delete photos for.</param>
        /// <returns>The status code and service result indicating the success or failure of the operation.</returns>
        [Authorize(Roles = "2")]
        [HttpDelete("{productId:int:min(1)}")]
        public async Task<IActionResult> DeletePhotos(int productId)
        {
            ServiceResult result = await _photosService.DeletePhotosByProductIdAsync(productId);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Deletes the photo with the specified ID.
        /// </summary>
        /// <param name="photoId">The ID of the photo to delete.</param>
        /// <returns>The status code and service result indicating the success or failure of the operation.</returns>
        [Authorize(Roles = "2")]
        [HttpDelete("by-photo/{photoId:int:min(1)}")]
        public async Task<IActionResult> DeletePhotosByPhotoId(int photoId)
        {
            ServiceResult result = await _photosService.DeletePhotosByPhotoIdAsync(photoId);
            return StatusCode((int)result.ServiceStatus, result);
        }
    }
}
