using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;

namespace MarketPlaceUser.Bussiness.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;

        private readonly ISecurityUtil _securityUtil;

        public ProductService(IUnitOfWork uow, ISecurityUtil securityUtil)
        {
            _uow = uow;
            _securityUtil = securityUtil;
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="form">The form containing information about the new product.</param>
        /// <param name="userId">The ID of the user creating the product.</param>
        /// <returns>A <see cref="ServiceResult"/>  containing information about the result of the operation.</returns>
        public async Task<ServiceResult> AddProductAsync(ProductForm form, int userId)
        {
            Product? product;

            ServiceResult result = new();

            // Check if the category ID is valid
            if (await _uow.CategoryRepository.FindByIdAndStatusAsync(form.CategoryId, Category.CategoryStatus.ACTIVE) == null)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Category Id";

                return result;
            }

            // Add the new product to the database
            product = await _uow.ProductRepostory.Add(new Product()
            {
                ProductName = form.ProductName.Trim(),
                ProductDescription = form.ProductDescription?.Trim(),
                CategoryId = form.CategoryId,
                CreatedUserId = userId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Price = form.Price,
                Address = form.Location.Address,
                Latitude = form.Location.Latitude,
                Longitude = form.Location.Longitude,
                Status = Product.ProductStatus.DRAFT
            });
            await _uow.SaveAsync();

            // Return a view of the newly created product
            result.Data = new ProductView(product, null);

            return result;
        }


        /// <summary>
        /// Deletes a product with the given ID, if it exists and is not already sold or deleted, and the authenticated user is the creator of the product.
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
        /// <param name="userId">The ID of the authenticated user.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating the status of the operation.</returns>
        public async Task<ServiceResult> DeleteProductAsync(int productId, int userId)
        {
            ServiceResult result = new();

            // Find the product with the given ID
            var product = await _uow.ProductRepostory.FindById(productId);

            // If the product doesn't exist, or if it's already sold or deleted, return a not found error
            if (product == null || product.Status == Product.ProductStatus.SOLD || product.Status == Product.ProductStatus.DELETED)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";

                return result;
            }

            // If the authenticated user is not the creator of the product, return an unauthorized error
            if (product.CreatedUserId != userId)
            {
                result.ServiceStatus = ServiceStatus.Unauthorized;
                result.Message = "Unauthorized Access";

                return result;
            }

            // Set the product's status to deleted and update its updated date
            product.Status = Product.ProductStatus.DELETED;
            product.UpdatedDate = DateTime.Now;
            _uow.ProductRepostory.Update(product);

            // Save changes to the database
            await _uow.SaveAsync();

            // Set the message in the result to indicate that the product was successfully deleted
            result.Message = "Product deleted";

            return result;
        }


        /// <summary>
        /// Edits a product and returns a service result.
        /// </summary>
        /// <param name="form">A product form containing updated information.</param>
        /// <param name="productId">The ID of the product to be updated.</param>
        /// <param name="userId">The ID of the user making the request.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServiceResult> EditProductAsync(ProductForm form, int productId, int userId)
        {
            ServiceResult result = new();

            // Find the product to be updated by ID
            Product? product = await _uow.ProductRepostory.FindById(productId);

            // If the product is not found, return a not found error
            if (product == null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";
                return result;
            }

            // If the user making the request is not the creator of the product or the product is not in a valid status, return a not found error
            if (product.CreatedUserId != userId || (product.Status != Product.ProductStatus.ACTIVE && product.Status != Product.ProductStatus.PENDING && product.Status != Product.ProductStatus.INACTIVE && product.Status != Product.ProductStatus.DRAFT))
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";
                return result;
            }

            // Find the category for the updated product by ID
            var category = await _uow.CategoryRepository.FindById(form.CategoryId);

            // If the category is not found, return a bad request error
            if (category == null)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Category Id";
                return result;
            }

            // Update the product with the new information
            product.ProductName = form.ProductName.Trim();
            product.ProductDescription = form.ProductDescription?.Trim();
            product.CategoryId = category.CategoryId;
            product.Category = category;
            product.UpdatedDate = DateTime.Now;
            product.Price = form.Price;
            product.Address = form.Location.Address;
            product.Latitude = form.Location.Latitude;
            product.Longitude = form.Location.Longitude;
            product.Status = Product.ProductStatus.PENDING;

            // If the product has no photos, set its status to draft
            if ((await _uow.PhotoRepository.FindByProductIdAsync(product.ProductId)).Count == 0)
                product.Status = Product.ProductStatus.DRAFT;

            // Save the changes to the product and return the updated product in a service result
            product = _uow.ProductRepostory.Update(product);
            await _uow.SaveAsync();

            result.Data = new ProductDetailView(product, product.Photos?.FirstOrDefault()?.Photo);
            return result;
        }


        /// <summary>
        /// Gets a detailed view of a product by ID and user ID.
        /// </summary>
        /// <param name="productId">The ID of the product to retrieve.</param>
        /// <param name="userId">The ID of the user making the request.</param>
        /// <returns>A <see cref="ServiceResult"/>  object containing the product details.</returns>
        public async Task<ServiceResult> GetProductAsync(int productId, int userId)
        {
            ServiceResult result = new();

            // Find the product with the given ID.
            Product? product = await _uow.ProductRepostory.FindById(productId);

            if (product == null)
            {
                // If the product is not found, return an error response.
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";
                return result;
            }

            if (product.Status == Product.ProductStatus.SOLD)
            {
                // If the product is sold, check if the user has made a purchase of the product.
                List<OrderDetails> orderDetailList = await _uow.OrderDetailsRepository.FindByBuyerIdAndProductId(userId, productId);

                if (orderDetailList.Count == 0)
                {
                    // If the user has not made a purchase of the product, return an error response.
                    result.ServiceStatus = ServiceStatus.NotFound;
                    result.Message = "Product not found";
                    return result;
                }

                // If the user has made a purchase of the product, return a detailed view of the product.
                result.Data = new ProductDetailView(product, product.Photos?.FirstOrDefault()?.Photo);
                return result;
            }

            // Check if the user has permission to view the product.
            if (product.CreatedUserId != _securityUtil.GetCurrentUserId() &&
                (product.Status == Product.ProductStatus.DRAFT ||
                product.Status != Product.ProductStatus.ACTIVE))
            {
                // If the user does not have permission to view the product, return an error response.
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";
                return result;
            }

            // If the user is not logged in and the product is not active, return an error response.
            if (_securityUtil.GetCurrentUserId() == 0 && product.Status != Product.ProductStatus.ACTIVE)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";
                return result;
            }

            // If the creator of the product is not active, return an error response.
            if (product.CreatedUser.Status != User.UserStatus.ACTIVE)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";
                return result;
            }

            // Return a detailed view of the product.
            result.Data = new ProductDetailView(product, product.Photos?.FirstOrDefault()?.Photo);
            return result;
        }
        /// <summary>
        /// Returns a paginated list of products based on the given search parameters.
        /// </summary>
        /// <param name="form">The pagination and search parameters for the products to be returned.</param>
        /// <param name="role">The role of the user performing the search.</param>
        /// <returns>A <see cref="ServiceResult"/> object containing the paginated list of products.</returns>
        public async Task<ServiceResult> ProductListAsync(ProductPaginationParams form, User.UserRole? role)
        {
            // Create a new ServiceResult object to hold the result of the operation
            ServiceResult result = new();

            // Parse the category IDs from the form parameter and filter out any invalid IDs

            // Check if the SortBy parameter is valid. If not, return a BadRequest error.
            if (form.SortBy != null && !_uow.ProductRepostory.ColumnMapForSortBy.ContainsKey(form.SortBy))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"SortBy : Accepts [{string.Join(", ", _uow.ProductRepostory.ColumnMapForSortBy.Keys)}] values only";

                return result;
            }


            // Create a dictionary of search parameters to pass to the repository method.

            Dictionary<string, string?> args = new()
            {
                {"Search",form.Search},
                {"Location",form.Location},
                {"SortBy",form.SortBy}
            };


            List<Product>? products = await _uow.ProductRepostory.FindAllByCategoryOrBrandLikeAndPriceAndLocationBetweenAsync(
                                                form.CategoryId?.Where(category => category.HasValue).Cast<int>().ToArray() ?? Array.Empty<int>(),
                                                form.StartPrice,
                                                form.EndPrice,
                                                form.SortByDesc,
                                                new Product.ProductStatus[] { Product.ProductStatus.ACTIVE },
                                                args);


            products = products.Where(product => product.CreatedUser.Status == User.UserStatus.ACTIVE).ToList();

            int currentUserId = _securityUtil.GetCurrentUserId();

            if (currentUserId != 0)
                products = products.Where(product => product.CreatedUserId != currentUserId).ToList();

            // Get the total count of products and create a PagerOffset object for pagination.
            int count = products.Count;

            var index = products.FindIndex(product => product.ProductId == form.Offset);

            List<ProductView> productViews = products.Skip(index + 1).Take(form.PageSize).ToList().ConvertAll(product => new ProductView(product, product.Photos?.FirstOrDefault()?.Photo));

            PagerOffset<ProductView> pager = new(index, form.PageSize, count, productViews);

            // Set the ServiceResult object to Success and return the paginated list of products.

            result.ServiceStatus = ServiceStatus.Success;
            result.Data = pager;

            return result;
        }


        /// <summary>
        /// Gets a list of products created by a user, along with their thumbnail images.
        /// </summary>
        /// <param name="userId">The ID of the user whose products to retrieve.</param>
        /// <returns>A <see cref="ServiceResult"/> object containing a list of ProductView objects representing the products, or an error message if no products are found.</returns>
        public async Task<ServiceResult> GetProductByUserIdAsync(int userId)
        {
            ServiceResult result = new();

            // Find all products created by the user with the given ID.
            var products = await _uow.ProductRepostory.FindByUserIAsync(userId);

            // Filter the products based on whether the current user is the same as the user whose products are being retrieved.
            if (_securityUtil.GetCurrentUserId() != userId)
            {
                // If the current user is different, only return products with status set to ACTIVE.
                products = products.Where(product => product.Status == Product.ProductStatus.ACTIVE).ToList();
            }
            else
            {
                // If the current user is the same, return all products except those with status set to DELETED.
                products = products.Where(product => product.Status != Product.ProductStatus.DELETED).ToList();
            }

            // Create a list of ProductView objects representing the products, along with their thumbnail images.
            List<ProductView> productslist = products.ConvertAll(product => new ProductView(product, product.Photos?.FirstOrDefault()?.Photo)).ToList();

            // If no products are found, return an error message.
            if (productslist == null || productslist.Count == 0)
            {
                result.Data = productslist;
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "No Products found";
                return result;
            }

            // Otherwise, return the list of ProductView objects.
            result.Data = productslist;
            return result;
        }


        /// <summary>
        /// Changes the status of a product in the database.
        /// </summary>
        /// <param name="product">The product to update.</param>
        /// <param name="status">The new status for the product.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ChangeStatusAsync(Product product, Product.ProductStatus status)
        {
            // If the product is not sold, update its status and the updated date
            if (product.Status != Product.ProductStatus.SOLD)
            {
                product.Status = status;
                product.UpdatedDate = DateTime.Now;

                // Update the product in the repository and save the changes to the database
                _uow.ProductRepostory.Update(product);
                await _uow.SaveAsync();
            }
        }
    }
}

