using MarketPlaceAdmin.Api.Controllers;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Net;
using Xunit;

namespace MarketplaceAdminTest.Controller
{
    public class ProductControllerTests
    {
        private readonly IProductService _productService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            // Create a substitute for the product service dependency
            _productService = Substitute.For<IProductService>();
            _controller = new ProductController(_productService);
        }

        [Fact]
        public async Task GetProduct_ReturnsOk_WhenProductExists()
        {
            // Arrange
            int productId = 1;
            ServiceResult expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _productService.GetProduct(productId).Returns(expectedResult);

            // Act
            IActionResult result = await _controller.GetProduct(productId);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)expectedResult.ServiceStatus, ((ObjectResult)result).StatusCode);
            Assert.Equal(expectedResult, ((ObjectResult)result).Value);
        }

        [Fact]
        public async Task AdminUpdate_ReturnsOk_WhenProductNameIsValid()
        {
            // Arrange
            int productId = 1;
            string productName = "New Product Name";
            ServiceResult expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _productService.AdminEditProduct(productName, productId).Returns(expectedResult);

            // Act
            IActionResult result = await _controller.AdminUpdate(productId, productName);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)expectedResult.ServiceStatus, ((ObjectResult)result).StatusCode);
            Assert.Equal(expectedResult, ((ObjectResult)result).Value);
        }

        [Fact]
        public async Task OffsetPaginatedProductList_ReturnsOk_WhenParametersAreValid()
        {
            // Arrange
            ProductOffsetPaginationParams form = new ProductOffsetPaginationParams { Offset = 0, PageSize = 10 };
            ServiceResult expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _productService.ProductListOffset(form).Returns(expectedResult);

            // Act
            IActionResult result = await _controller.OffsetPaginatedProductList(form);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)expectedResult.ServiceStatus, ((ObjectResult)result).StatusCode);
            Assert.Equal(expectedResult, ((ObjectResult)result).Value);
        }

        [Fact]
        public async Task PaginatedProductList_ReturnsOk_WhenParametersAreValid()
        {
            // Arrange
            ProductPaginationParams form = new ProductPaginationParams { PageNumber = 1, PageSize = 10 };
            ServiceResult expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _productService.ProductList(form).Returns(expectedResult);

            // Act
            IActionResult result = await _controller.PaginatedProductList(form);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)expectedResult.ServiceStatus, ((ObjectResult)result).StatusCode);
            Assert.Equal(expectedResult, ((ObjectResult)result).Value);
        }

        [Fact]
        public async Task ChangeProductStatus_ValidData_ReturnsOk()
        {
            // Arrange
            int productId = 1;
            RequestForm form = new() { Approved = true };
            ServiceResult result = new();
            _productService.ChangeStatusAsync(productId, form).Returns(result);

            // Act
            var response = await _controller.ChangeProductStatus(productId, form);

            // Assert
            Assert.IsType<ObjectResult>(response);
            Assert.Equal((int)HttpStatusCode.OK, ((ObjectResult)response).StatusCode);
        }
    }
}
