using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using MarketPlaceUser.Bussiness.Util;
using Microsoft.Extensions.Logging;

namespace MarketPlaceUser.Bussiness.Services
{
    public class PhotosService : IPhotosService
    {
        private readonly IUnitOfWork _uow;
        private readonly ISecurityUtil _securityUtil;
        private readonly IFileUtil _fileUtil;
        private readonly ILogger<PhotosService> _logger;

        public PhotosService(IUnitOfWork uow, ISecurityUtil securityUtil, IFileUtil fileUtil, ILogger<PhotosService> logger)
        {
            _uow = uow;
            _logger = logger;
            _securityUtil = securityUtil;
            _fileUtil = fileUtil;
        }

        /// <summary>
        /// Adds one or more photos to the specified product, if the user has the necessary permissions and the product exists.
        /// </summary>
        /// <param name="productId">The ID of the product to which the photos should be added.</param>
        /// <param name="image">The image(s) to be added to the product.</param>
        /// <returns>A <see cref="ServiceResult"/>  indicating whether the photos were added successfully or not.</returns>
        public async Task<ServiceResult> AddPhotosAsync(int productId, ProductImageForm image)
        {
            // Create a new ServiceResult object to store the result of the method.
            ServiceResult result = new();

            // Find the product with the specified ID.
            Product? product = await _uow.ProductRepostory.FindById(productId);

            // Check that the product exists and the user has the necessary permissions to add photos to it.
            if (product == null || _securityUtil.GetCurrentUserId() != product.CreatedUserId || product.Status == Product.ProductStatus.DELETED || product.Status == Product.ProductStatus.SOLD || product.Status == Product.ProductStatus.ONPROCESS)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";

                return result;
            }

            // Check that the total number of photos for the product does not exceed the maximum allowed (12).
            if ((await _uow.PhotoRepository.FindByProductIdAsync(productId)).Count + image.File.Length > 12)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Image count 12 exceeded";

                return result;
            }

            try
            {
                if (image.File.Length > 0)
                {
                    // For each image in the input, upload the image file to the server and save its filename in the database.
                    foreach (var imageItem in image.File)
                    {
                        var fileName = _fileUtil.UploadProductImage(product, imageItem) ?? throw new Exception("Image Not Uploaded");

                        await _uow.PhotoRepository.Add(new Photos()
                        {
                            Product = product,
                            Photo = fileName
                        });
                    }

                    // Update the status of the product to PENDING, indicating that it requires review by a moderator.
                    product.Status = Product.ProductStatus.PENDING;

                    // Save the changes to the database.
                    product = _uow.ProductRepostory.Update(product);
                    await _uow.SaveAsync();

                    // Set the result message to indicate that the photos were added successfully.
                    result.Message = "Photos Added";

                    return result;
                }

                // If no photos were provided in the input, return a BadRequest error.
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Photos were absent";

                return result;
            }
            catch (Exception e)
            {
                // If an error occurs while adding the photos, log the error and return a BadRequest error.
                _logger.LogError("Error : {e}", e.Message);

                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Failed to add images";
                return result;
            }
        }



        /// <summary>
        /// Retrieves all photos associated with the given product ID.
        /// </summary>
        /// <param name="productId">The ID of the product to retrieve photos for.</param>
        /// <returns>A <see cref="ServiceResult"/>  object containing the photos associated with the given product ID.</returns>
        public async Task<ServiceResult> GetPhotosAsync(int productId)
        {
            ServiceResult result = new()
            {
                ServiceStatus = ServiceStatus.Success,
                Data = await _uow.PhotoRepository.FindByProductIdAsync(productId)
            };

            return result;
        }


        /// <summary>
        /// Deletes all photos associated with the given product ID, if the current user has permission to do so.
        /// </summary>
        /// <param name="productId">The ID of the product whose photos should be deleted.</param>
        /// <returns>A <see cref="ServiceResult"/>  indicating whether the operation was successful.</returns>
        public async Task<ServiceResult> DeletePhotosByProductIdAsync(int productId)
        {
            ServiceResult result = new();

            // Find all photos associated with the given product ID.
            var photos = await _uow.PhotoRepository.FindByProductIdAsync(productId);

            // Find the product with the given ID.
            Product? product = await _uow.ProductRepostory.FindById(productId);

            // Check whether the photos exist and the current user has permission to delete them.
            if (photos.Count == 0 || _securityUtil.GetCurrentUserId() != product?.CreatedUserId || product.Status == Product.ProductStatus.DELETED || product.Status == Product.ProductStatus.SOLD || product.Status == Product.ProductStatus.ONPROCESS)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Photos Not Found";
                return result;
            }

            // Delete all photo files associated with the product.
            foreach (var photo in photos)
                _fileUtil.DeleteProductImages(photo.Photo);

            // Update the status of the product to draft.
            product.Status = Product.ProductStatus.DRAFT;
            _uow.ProductRepostory.Update(product);

            // Delete all photos associated with the product from the database.
            _uow.PhotoRepository.Delete(photos);

            // Save changes to the database.
            await _uow.SaveAsync();

            // Return a success message.
            result.Message = "Photos Deleted";
            return result;
        }



        /// <summary>
        /// Deletes the photo with the specified ID from the product it belongs to.
        /// </summary>
        /// <param name="photoId">The ID of the photo to delete.</param>
        /// <returns>A <see cref="ServiceResult"/>  object indicating the result of the operation.</returns>
        public async Task<ServiceResult> DeletePhotosByPhotoIdAsync(int photoId)
        {
            ServiceResult result = new();

            // Find the photo with the specified ID
            var photo = await _uow.PhotoRepository.FindById(photoId);

            // Check if the photo exists and is associated with a product owned by the current user, and if the product has not been deleted or sold
            if (photo == null ||
                photo.Product.CreatedUserId != _securityUtil.GetCurrentUserId() ||
                photo.Product.Status == Product.ProductStatus.DELETED ||
                photo.Product.Status == Product.ProductStatus.SOLD || photo.Product.Status == Product.ProductStatus.ONPROCESS)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Photos Not Found";

                return result;
            }

            // Find the product associated with the photo
            Product? product = await _uow.ProductRepostory.FindById(photo.ProductId);

            // Find all photos associated with the product
            var photos = await _uow.PhotoRepository.FindByProductIdAsync(photo.ProductId);

            // If the product exists and has only one photo associated with it, change its status to Draft
            if (product != null && photos.Count <= 1)
            {
                product.Status = Product.ProductStatus.DRAFT;
                _uow.ProductRepostory.Update(product);
            }

            // Delete the photo from the database and the file system
            _uow.PhotoRepository.Delete(photo);
            _fileUtil.DeleteProductImages(photo.Photo);

            // Save changes to the database
            await _uow.SaveAsync();

            result.Message = "Photos Deleted";

            return result;
        }



        /// <summary>
        /// Returns a file stream of the photo with the given file name, if it exists.
        /// </summary>
        /// <param name="fileName">The name of the photo file to retrieve.</param>
        /// <returns>A file stream of the photo with the given file name, if it exists; otherwise, null.</returns>
        public async Task<FileStream?> GetPhotosByName(string fileName)
        {
            // Check if the photo exists in the repository
            if (!await _uow.PhotoRepository.IsPhotoExists(fileName))
                return null;

            // Return a file stream of the photo using the file util
            return _fileUtil.GetProductImages(fileName);
        }
    }
}
