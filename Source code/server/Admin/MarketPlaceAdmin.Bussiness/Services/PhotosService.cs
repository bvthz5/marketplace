using MarketPlace.DataAccess.Interfaces;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Util.Interfaces;
using Microsoft.Extensions.Logging;

namespace MarketPlaceAdmin.Bussiness.Services
{
    /// <summary>
    /// Represents a service that provides functionality related to product photos.
    /// </summary>
    public class PhotosService : IPhotosService
    {
        private readonly IUnitOfWork _uow;
        private readonly IFileUtil _fileUtil;
        private readonly ILogger<PhotosService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotosService"/> class.
        /// </summary>
        /// <param name="uow">The unit of work used to access the data store.</param>
        /// <param name="fileUtil">The file utility used to manage files.</param>
        /// <param name="logger">The logger used to log messages.</param>
        public PhotosService(IUnitOfWork uow, IFileUtil fileUtil, ILogger<PhotosService> logger)
        {
            _uow = uow;
            _logger = logger;
            _fileUtil = fileUtil;
        }

        /// <summary>
        /// Gets the photos of a product.
        /// </summary>
        /// <param name="productId">The ID of the product to get photos for.</param>
        /// <returns>A <see cref="ServiceResult"/> object containing a list of <see cref="PhotosView"/> objects on success.</returns>
        public async Task<ServiceResult> GetPhotos(int productId)
        {
            _logger.LogInformation("Get Photo By Product Id : {productId}", productId);

            return new ServiceResult()
            {
                ServiceStatus = ServiceStatus.Success,
                Message = "Product Image List",
                Data = (await _uow.PhotoRepository.FindByProductIdAsync(productId)).ConvertAll(photo => new PhotosView(photo))
            };
        }

        /// <summary>
        /// Gets the product images from storage.
        /// </summary>
        /// <param name="fileName">The name of the file to get.</param>
        /// <returns>A <see cref="FileStream"/> object if the file exists, or null otherwise.</returns>
        public async Task<FileStream?> GetPhotosByName(string fileName)
        {
            if (!await _uow.PhotoRepository.IsPhotoExists(fileName))
                return null;

            return _fileUtil.GetProductImages(fileName);
        }
    }
}
