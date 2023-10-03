using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;

namespace MarketPlaceUser.Bussiness.Services
{
    public class WishListService : IWishListService
    {
        private readonly IUnitOfWork _uow;

        public WishListService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// Adds a product to the user's wishlist.
        /// </summary>
        /// <param name="userId">The ID of the user who is adding the product to their wishlist.</param>
        /// <param name="productId">The ID of the product being added to the wishlist.</param>
        /// <returns>A <see cref="ServiceResult"/>  object with a success message if the product was added successfully, 
        /// or an error message if the product was not found or has already been added to the user's wishlist.</returns>
        public async Task<ServiceResult> AddToWishListAsync(int userId, int productId)
        {
            ServiceResult result = new();

            // Find the product with the given ID.
            var product = await _uow.ProductRepostory.FindById(productId);

            // If the product does not exist or is inactive, return an error message.
            if (product == null || product.CreatedUserId == userId || product.Status != Product.ProductStatus.ACTIVE || product.CreatedUser.Status != User.UserStatus.ACTIVE)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";

                return result;
            }

            // Check if the product has already been added to the user's wishlist.
            if (await _uow.WishListRepository.FindByProductIdAndUserIdAsync(productId, userId) != null)
            {
                result.ServiceStatus = ServiceStatus.AlreadyExists;
                result.Message = "Product Already Added";

                return result;
            }

            // Add the product to the user's wishlist.
            var wishList = new WishList()
            {
                UserId = userId,
                ProductId = productId
            };

            await _uow.WishListRepository.Add(wishList);

            await _uow.SaveAsync();

            // Return a success message.
            result.Message = "Product Added";

            return result;
        }


        /// <summary>
        /// Retrieves the wishlist of a user identified by their <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The identifier of the user whose wishlist to retrieve.</param>
        /// <returns>A service result object containing the wishlist as its data.</returns>
        public async Task<ServiceResult> GetWishListAsync(int userId)
        {
            ServiceResult result = new();

            var wishList = await _uow.WishListRepository.FindByUserIdAsync(userId);


            result.Data = wishList.ConvertAll(w => new ProductCartWishListView(w.Product, _uow.PhotoRepository.FindThumbnailPicture(w.ProductId)?.Photo));
            if (wishList.Count < 1)
                result.Message = "Wishlist is empty";

            return result;

        }

        /// <summary>
        /// Removes a product from the user's wishlist
        /// </summary>
        /// <param name="userId">The ID of the user whose wishlist the product will be removed from</param>
        /// <param name="productId">The ID of the product to remove</param>
        /// <returns>A <see cref="ServiceResult"/>  object indicating the outcome of the operation</returns>
        public async Task<ServiceResult> RemoveFromWishListAsync(int userId, int productId)
        {
            ServiceResult result = new();

            // Check if the product is in the user's wishlist
            if (await _uow.WishListRepository.FindByProductIdAndUserIdAsync(productId, userId) == null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";

                return result;
            }

            // Remove the product from the user's wishlist
            await _uow.WishListRepository.DeleteByProductIdAndUserIdAsync(productId, userId);

            // Save changes to the database
            await _uow.SaveAsync();

            // Set the message in the result object
            result.Message = "Removed From WishList";

            return result;
        }

    }
}
