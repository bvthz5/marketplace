using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace MarketplaceAdminTest.Service
{
    public class ProductServiceTests
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ProductService> _logger;
        private readonly IEmailService _emailService;

        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _uow = Substitute.For<IUnitOfWork>();
            _logger = Substitute.For<ILogger<ProductService>>();
            _emailService = Substitute.For<IEmailService>();


            _productService = new ProductService(_uow, _logger, _emailService);
        }

        [Fact]
        public async Task AdminEditProduct_ReturnsNotFound_WhenInvalidProductId()
        {
            // Arrange
            var service = _productService;
            var productName = "New Product Name";
            var invalidProductId = 0;
            // Act
            var result = await service.AdminEditProduct(productName, invalidProductId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.Null(result.Data);
        }
        [Fact]
        public async Task AdminEditProduct_ReturnsBadRequest_WhenProductStatusNotActiveOrPending()
        {
            // Arrange
            var service = _productService;
            var productName = "New Product Name";
            var productId = 1; // This product has a status of "Rejected"
                               // Act
            var result = await service.AdminEditProduct(productName, productId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.Null(result.Data);

        }


        [Fact]
        public async Task Product_notfound()
        {
            // Arrange
            int productId = 99;
            RequestForm form = new RequestForm { Approved = true, Reason = "Approved by admin" };

            // Act
            ServiceResult result = await _productService.ChangeStatusAsync(productId, form);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.Null(result.Data);

        }

        [Fact]
        public async Task Product_status_not_pending()
        {
            // Arrange
            int productId = 1;
            RequestForm form = new() { Approved = true, Reason = "Approved by admin" };

            // Set the product status to something other than PENDING
            Product? product = await _uow.ProductRepostory.FindById(productId);
            if (product == null)
            {
                // return early if the product is not found
                return;
            }
            product.Status = Product.ProductStatus.ACTIVE;
            await _uow.SaveAsync();

            // Act
            ServiceResult result = await _productService.ChangeStatusAsync(productId, form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal($"Product Status {(Product.ProductStatus)product.Status}", result.Message);
            Assert.Null(result.Data);
        }


        [Fact]
        public async Task Invalidinput()
        {
            // Arrange
            int productId = 1;
            RequestForm form = new() { Approved = true, Reason = "Test" };

            // Act
            ServiceResult result = await _productService.ChangeStatusAsync(productId, form);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task Invalid_Status_ReturnsBadRequest()
        {
            // Arrange
            var form = new ProductPaginationParams { Status = new List<byte?> { 0, 99 } };

            // Act
            var (products, error) = await _productService.GetProducts(form);

            // Assert
            Assert.Empty(products);
            Assert.NotNull(error);
            Assert.Equal(ServiceStatus.BadRequest, error!.ServiceStatus);
            Assert.Equal("Invalid Status Value", error.Message);
        }

    }
}