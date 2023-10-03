using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.Extensions.Logging;

namespace MarketPlaceAdmin.Bussiness.Services
{
    /// <summary>
    /// Service class for managing products.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ProductService> _logger;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class with the specified unit of work.
        /// </summary>
        /// <param name="uow">The unit of work instance to use.</param>
        /// <param name="logger">The logger instance to use.</param>
        /// <param name="emailService">The email service instance to use.</param>
        public ProductService(IUnitOfWork uow, ILogger<ProductService> logger, IEmailService emailService)
        {
            _uow = uow;
            _logger = logger;
            _emailService = emailService;
        }

        /// <summary>
        /// Edit Product Name if Product status is in Active or Pending
        /// </summary>
        /// <param name="productName">The new name for the product.</param>
        /// <param name="productId">The ID of the product to be edited.</param>
        /// <returns>A <see cref="ServiceResult"/> object containing the result of the operation and the updated product details.</returns>
        public async Task<ServiceResult> AdminEditProduct(string productName, int productId)
        {
            _logger.LogInformation("AdminEditProduct method called with parameters {ProductName} and {ProductId}", productName, productId);

            ServiceResult result = new();

            // Find the product by ID
            Product? product = await _uow.ProductRepostory.FindById(productId);

            if (product is null)
            {
                _logger.LogWarning("Inavlid Product Id : {productId}", productId);

                // If the product is not found, return a "not found" error
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";
                return result;
            }

            if (product.Status != Product.ProductStatus.ACTIVE && product.Status != Product.ProductStatus.PENDING)
            {
                _logger.LogWarning("Product Status {status}", product.Status);

                // If the product status is not "Active" or "Pending", return a "bad request" error
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"Product Status : {product.Status}";
                return result;
            }

            // Update the product name and update date
            product.ProductName = productName.Trim();
            product.UpdatedDate = DateTime.Now;

            // Save the changes to the database
            product = _uow.ProductRepostory.Update(product);
            await _uow.SaveAsync();

            _logger.LogInformation("Product updated with new name {ProductName}", productName);

            // Return a success result with the updated product details
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Product Updated";
            result.Data = new ProductDetailView(product);
            return result;
        }

        /// <summary>
        /// Retrieves the details of a product, excluding products in Draft status.
        /// </summary>
        /// <param name="productId">The ID of the product to retrieve.</param>
        /// <returns>A ServiceResult object containing the retrieved ProductDetailView if successful, or an error message if not.</returns>
        public async Task<ServiceResult> GetProduct(int productId)
        {
            ServiceResult result = new();

            // Attempt to retrieve the product from the database
            Product? product = await _uow.ProductRepostory.FindById(productId);

            // If the product is null or in Draft status, return a not found error message
            if (product is null || product.Status == Product.ProductStatus.DRAFT)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";
                return result;
            }

            // If successful, set the ServiceResult properties and return the retrieved ProductDetailView
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Success";
            result.Data = new ProductDetailView(product);

            return result;
        }

        /// <summary>
        /// Approve or reject a product.
        /// </summary>
        /// <param name="productId">The ID of the product to approve/reject.</param>
        /// <param name="form">The status to set on the product.</param>
        /// <returns>
        /// A <see cref="ServiceResult"/> object indicating the result of the operation, with a message describing the outcome and, if the operation was successful, a <see cref="ProductView"/> object representing the updated product.
        /// </returns>
        /// <remarks>
        /// This method changes the status of the product with the given <paramref name="productId"/> with respect to <paramref name="form"/>. The operation can only be performed on products with a current status of <see cref="Product.ProductStatus.PENDING"/> value. 
        /// </remarks>
        public async Task<ServiceResult> ChangeStatusAsync(int productId, RequestForm form)
        {
            _logger.LogInformation("ChangeStatusAsync method called with parameters {ProductId} and {@Form}", productId, form);

            ServiceResult result = new();

            // Attempt to retrieve the product from the database
            Product? product = await _uow.ProductRepostory.FindById(productId);

            // If the product is not found, return an error
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", productId);

                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Product Not Found";
                return result;
            }

            // If the product is not in the PENDING status, return an error
            if (product.Status != Product.ProductStatus.PENDING)
            {
                _logger.LogWarning("Product with ID {ProductId} has status {(ProductStatus)} which is not PENDING", productId, product.Status);

                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"Product Status {product.Status}";
                return result;
            }

            // Update the product status and last updated date
            product.Status = form.Approved ? Product.ProductStatus.ACTIVE : Product.ProductStatus.INACTIVE;
            product.UpdatedDate = DateTime.Now;

            // Email user about the product status
            _emailService.ProductRequest(product.CreatedUser.Email, productId, product.ProductName, form.Approved, form.Reason);

            // Save the updated product to the database
            _uow.ProductRepostory.Update(product);
            await _uow.SaveAsync();

            _logger.LogInformation("Product with ID {ProductId} status updated to {(ProductStatus)}", productId, product.Status);

            // Return a success response with updated product data
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Product Status Updated";
            result.Data = new ProductView(product, product.Photos?.FirstOrDefault()?.Photo);
            return result;
        }

        /// <summary>
        /// Retrieves a paginated list of products with sorting, searching, and filtering capabilities.
        /// </summary>
        /// <param name="form">A <see cref="ProductPaginationParams"/> object containing pagination parameters for the product list.</param>
        ///<remarks>
        /// <list type="bullet">
        /// <item><description>UserId: The ID of the user who posted the products to retrieve.</description></item>
        /// <item><description>CategoryId: The category IDs of the products to retrieve. Only products with the specified category IDs will be retrieved.</description></item>
        /// <item><description>StartPrice: The minimum price of the products to retrieve. Only products with a price greater than or equal to the specified value will be retrieved.</description></item>
        /// <item><description>EndPrice: The maximum price of the products to retrieve. Only products with a price less than or equal to the specified value will be retrieved.</description></item>
        /// <item><description>SortBy: The column name to sort the products by. Only products that match the search criteria will be sorted.</description></item>
        /// <item><description>SortByDesc: Determines whether the products will be sorted in descending order. If set to true, the products will be sorted in descending order. If set to false, the products will be sorted in ascending order. Default is false.</description></item>
        /// <item><description>Status: The status of the products to retrieve. Valid values are defined in the <see cref="Product.ProductStatus"/> enum. Only products with the specified status will be retrieved.</description></item>
        /// <item><description>Search: The keyword to search for in the product's name or description. Only products that match the search criteria will be retrieved.</description></item>
        /// <item><description>Location: The location of the products to retrieve. Only products with the specified location will be retrieved.</description></item>
        /// <item><description>PageNumber: The page number for the pagination.</description></item>
        /// <item><description>PageSize: The number of products to retrieve per page.</description></item>
        /// </list>
        ///</remarks>
        /// <returns>A <see cref="ServiceResult"/> object containing the paginated list of products. The <see cref="ServiceResult.ServiceStatus"/> property indicates the success or failure of the operation. The <see cref="ServiceResult.Message"/> property contains a message indicating the status of the operation. The <see cref="ServiceResult.Data"/> property contains a <see cref="Pager{T}"/> object that represents the paginated list of products. The parameter of the <see cref="Pager{T}"/> object is <see cref="ProductListView"/>, which is a view model representing a single product.</returns>

        public async Task<ServiceResult> ProductList(ProductPaginationParams form)
        {
            // Create a new ServiceResult object to hold the result of the operation.
            ServiceResult result = new();

            // Call the GetProducts method asynchronously with the specified ProductPaginationParams object and retrieve a tuple containing a list of products and an optional ServiceResult error object.
            (List<Product> products, ServiceResult? error) = await GetProducts(form);

            // Check if there was an error retrieving the products.
            if (error is not null)
                return error;

            // Create a PagerOffset object to hold the paginated list of products.
            Pager<ProductListView> pager = new(form.PageNumber, form.PageSize, products.Count);

            // Extract the specified page of products from the list and convert them to ProductView objects.
            List<ProductListView> productViews = products.Skip((form.PageNumber - 1) * form.PageSize)
                                                         .Take(form.PageSize).ToList()
                                                         .ConvertAll(product => new ProductListView(product, product.Photos?.FirstOrDefault()?.Photo));
            // Set extracted data into pager
            pager.SetResult(productViews);

            // Set the result status, message, and data properties and return the ServiceResult object.
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Paginated Product List";
            result.Data = pager;
            return result;
        }

        /// <summary>
        /// Retrieves a paginated list of products with sorting, searching, and filtering capabilities.
        /// </summary>
        /// <param name="form">A <see cref="ProductOffsetPaginationParams"/> object containing pagination parameters for the product list.</param>
        ///<remarks>
        /// <list type="bullet">
        /// <item><description>UserId: The ID of the user who posted the products to retrieve.</description></item>
        /// <item><description>CategoryId: The category IDs of the products to retrieve. Only products with the specified category IDs will be retrieved.</description></item>
        /// <item><description>StartPrice: The minimum price of the products to retrieve. Only products with a price greater than or equal to the specified value will be retrieved.</description></item>
        /// <item><description>EndPrice: The maximum price of the products to retrieve. Only products with a price less than or equal to the specified value will be retrieved.</description></item>
        /// <item><description>SortBy: The column name to sort the products by. Only products that match the search criteria will be sorted.</description></item>
        /// <item><description>SortByDesc: Determines whether the products will be sorted in descending order. If set to true, the products will be sorted in descending order. If set to false, the products will be sorted in ascending order. Default is false.</description></item>
        /// <item><description>Status: The status of the products to retrieve. Valid values are defined in the <see cref="Product.ProductStatus"/> enum. Only products with the specified status will be retrieved.</description></item>
        /// <item><description>Search: The keyword to search for in the product's name or description. Only products that match the search criteria will be retrieved.</description></item>
        /// <item><description>Location: The location of the products to retrieve. Only products with the specified location will be retrieved.</description></item>
        /// <item><description>Offset: The ID of the product to use as the starting point for the pagination. Only products with a greater ID value than the specified value will be retrieved.</description></item>
        /// <item><description>PageSize: The number of products to retrieve per page.</description></item>
        /// </list>
        ///</remarks>
        /// <returns>A <see cref="ServiceResult"/> object containing the paginated list of products. The <see cref="ServiceResult.ServiceStatus"/> property indicates the success or failure of the operation. The <see cref="ServiceResult.Message"/> property contains a message indicating the status of the operation. The <see cref="ServiceResult.Data"/> property contains a <see cref="PagerOffset{T}"/> object that represents the paginated list of products. The parameter of the <see cref="PagerOffset{T}"/> object is <see cref="ProductView"/>, which is a view model representing a single product.</returns>
        public async Task<ServiceResult> ProductListOffset(ProductOffsetPaginationParams form)
        {
            // Create a new ServiceResult object to hold the result of the operation.
            ServiceResult result = new();

            // Call the GetProducts method asynchronously with the specified ProductPaginationParams object and retrieve a tuple containing a list of products and an optional ServiceResult error object.
            (List<Product> products, ServiceResult? error) = await GetProducts(form);

            // Check if there was an error retrieving the products.
            if (error is not null)
                return error;

            // Count the total number of products returned by the query.
            int count = products.Count;

            // Find the index of the product with the specified offset value.
            var index = products.FindIndex(product => product.ProductId == form.Offset);

            // Extract the specified page of products from the list and convert them to ProductView objects.
            List<ProductListView> productViews = products.Skip(index + 1).Take(form.PageSize).ToList().ConvertAll(product => new ProductListView(product, product.Photos?.FirstOrDefault()?.Photo));

            // Create a PagerOffset object to hold the paginated list of products.
            PagerOffset<ProductListView> pager = new(index, form.PageSize, count, productViews);

            // Set the result status, message, and data properties and return the ServiceResult object.
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Paginated Product List";
            result.Data = pager;
            return result;
        }

        /// <summary>
        /// Retrieves a list of products that match the specified search criteria.
        /// </summary>
        /// <param name="form">A <see cref="ProductPaginationParams"/> object containing the search criteria.</param>
        /// <returns>A tuple containing a list of products and a nullable ServiceResult object.</returns>

        public async Task<(List<Product> productList, ServiceResult? error)> GetProducts(ProductPaginationParams form)
        {
            // Declare a nullable list of products.
            List<Product>? products = new();

            // Declare a nullable ServiceResult object.
            ServiceResult? error = null;

            // Filter the "form.Status" parameter to exclude null and undefined values, and convert it to a byte array.
            byte[]? status = form.Status?.Where(status => status.HasValue).Cast<byte>().ToArray();

            // Check if the "Status" parameter is valid and set the appropriate error message if not.
            if (status != null && !status.All(status => Enum.IsDefined(typeof(Product.ProductStatus), status)))
            {
                // Set the error object with BadRequest and error message.
                error = new()
                {
                    ServiceStatus = ServiceStatus.BadRequest,
                    Message = $"Invalid Status Value"
                };

                // Return tuple with empty product list and error object.
                return (products, error);
            }

            // Check if the "SortBy" parameter is valid and set the appropriate error message if not.
            if (form.SortBy != null && !_uow.ProductRepostory.ColumnMapForSortBy.ContainsKey(form.SortBy))
            {
                // Set the error object with BadRequest and error message.
                error = new()
                {
                    ServiceStatus = ServiceStatus.BadRequest,
                    Message = $"SortBy: Accepts [{string.Join(", ", _uow.ProductRepostory.ColumnMapForSortBy.Keys)}] values only"
                };

                // Return tuple with empty product list and error object
                return (products, error);
            }

            // Create a dictionary to hold the search parameters.
            Dictionary<string, string?> searchParams = new()
            {
                { "Search", form.Search },
                { "Location", form.Location },
                { "SortBy", form.SortBy }
            };

            // Retrieve the list of products that match the specified search criteria.
            products = await _uow.ProductRepostory.FindAllByUserIdAndCategoryAndLocationAndPriceBetweenAsync(
                                                form.UserId,
                                                form.CategoryId?.Where(id => id.HasValue).Cast<int>().ToArray(),
                                                form.StartPrice, form.EndPrice,
                                                form.SortByDesc,
                                                status?.Cast<Product.ProductStatus>().ToArray(),
                                                searchParams);

            // Return tuple with list of products and null error object.
            return (products, error);
        }
    }
}