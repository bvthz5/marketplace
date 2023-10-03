using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;

namespace MarketPlaceUser.Bussiness.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _uow;

        public CartService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        /// <summary>
        /// Adds a product to the cart of the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user to whose cart to add the product.</param>
        /// <param name="productId">The ID of the product to add to the cart.</param>
        /// <returns>A <see cref="ServiceResult"/> object containing a success message or an error message.</returns>
        public async Task<ServiceResult> AddToCartAsync(int userId, int productId)
        {
            // Create a new ServiceResult object to hold the result of the operation
            ServiceResult result = new();

            // Retrieve the product with the specified ID from the database
            var product = await _uow.ProductRepostory.FindById(productId);

            // If the product is not found, or is created by the same user, or is inactive, or created by an inactive user
            // Set the ServiceStatus of the result to NotFound and set the message to "Product Not Found"
            if (product == null || product.CreatedUserId == userId || product.Status != Product.ProductStatus.ACTIVE || product.CreatedUser.Status != User.UserStatus.ACTIVE)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";
                return result;
            }

            // Retrieve the cart items for the specified user from the database
            List<Cart> cartList = await _uow.CartRepository.FindByUserIdAsync(userId);

            // If the number of cart items is already at the maximum limit of 50
            // Set the ServiceStatus of the result to BadRequest and set the message to "Cart Max Limit (50) exceed"
            if (cartList.Count >= 50)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Cart Max Limit (50) exceed";
                return result;
            }

            // If the product is already in the user's cart
            // Set the ServiceStatus of the result to AlreadyExists and set the message to "Product Already Added"
            if (cartList.Any(cart => cart.ProductId == productId))
            {
                result.ServiceStatus = ServiceStatus.AlreadyExists;
                result.Message = "Product Already Added";
                return result;
            }

            // Create a new cart item for the user and the specified product
            var cart = new Cart()
            {
                UserId = userId,
                ProductId = productId
            };

            // Add the new cart item to the database using the Unit of Work
            await _uow.CartRepository.Add(cart);

            // Save the changes to the database using the Unit of Work
            await _uow.SaveAsync();

            // Set the message of the result to "Product Added"
            result.Message = "Product Added";

            // Return the ServiceResult object
            return result;
        }


        /// <summary>
        /// Retrieves the cart of a user and returns it as a list of ProductCartWishListView items.
        /// </summary>
        /// <param name="userId">The ID of the user whose cart to retrieve.</param>
        /// <returns>A <see cref="ServiceResult"/> object containing the cart data and/or an error message.</returns>
        public async Task<ServiceResult> GetCartAsync(int userId)
        {
            // Create a new ServiceResult object to hold the result of the operation
            ServiceResult result = new();

            // Retrieve the cart of the specified user from the database using the Unit of Work
            var cart = await _uow.CartRepository.FindByUserIdAsync(userId);

            // Convert each item in the cart to a ProductCartWishListView object and add it to the result data
            result.Data = cart.ConvertAll(w => new ProductCartWishListView(w.Product, _uow.PhotoRepository.FindThumbnailPicture(w.ProductId)?.Photo));

            // If the cart is empty, set the result message to "Cart is empty"
            if (cart.Count < 1)
                result.Message = "Cart is empty";

            // Return the ServiceResult object
            return result;
        }


        /// <summary>
        /// Removes a product from the cart of the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose cart should be updated.</param>
        /// <param name="productId">The ID of the product to remove from the cart.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The result is a <see cref="ServiceResult"/> object indicating whether the product was successfully removed from the cart.</returns>
        public async Task<ServiceResult> RemoveFromCartAsync(int userId, int productId)
        {
            ServiceResult result = new();

            // Check if the product is already in the cart
            if (await _uow.CartRepository.FindByProductIdAndUserIdAsync(productId, userId) == null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";

                return result;
            }

            // Delete the product from the cart
            await _uow.CartRepository.DeleteByProductIdAndUserIdAsync(productId, userId);
            await _uow.SaveAsync();

            result.Message = "Removed From Cart";

            return result;
        }

    }
}
