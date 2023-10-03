using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceAdmin.Api.Controllers
{
    /// <summary>
    /// This class represents a controller for handling photo-related HTTP requests for a web API.
    /// </summary>
    [Route("api/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IPhotosService _photosService;

        /// <summary>
        /// Initializes a new instance of the PhotosController class.
        /// </summary>
        /// <param name="photosService">An instance of IPhotosService to handle photo-related operations.</param>
        public PhotosController(IPhotosService photosService)
        {
            _photosService = photosService;
        }

        /// <summary>
        /// Retrieves user-added images for a particular product.
        /// </summary>
        /// <param name="ProductId">The ID of the product to retrieve images for.</param>
        /// <returns>An HTTP response containing the requested images, or an error message if the operation fails.</returns>
        [Authorize]
        [HttpGet("{ProductId:int:min(1)}", Name = "Product Images")]
        public async Task<IActionResult> GetPhotos(int ProductId)
        {
            ServiceResult result = await _photosService.GetPhotos(ProductId);
            // Return the result of the service method as an HTTP response.
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Retrieves an image file with the given filename.
        /// </summary>
        /// <param name="filePath">The name of the image file to retrieve.</param>
        /// <returns>An HTTP response containing the requested image file, or a 404 Not Found response if the file is not found.</returns>
        [AllowAnonymous]
        [HttpGet("path/{filePath}", Name = "Get Product Image")]
        public async Task<IActionResult> GetPhotosByPath(string filePath)
        {
            FileStream? fileStream = await _photosService.GetPhotosByName(filePath);

            // If the requested image file was not found, return a 404 Not Found HTTP response.
            if (fileStream is null)
                return NotFound();

            // Otherwise, return the requested image file as an HTTP response with content type "image/jpeg".
            return File(fileStream, "image/jpeg");
        }
    }
}
