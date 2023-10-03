using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Api.Controllers;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using MarketPlaceUser.Bussiness.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NSubstitute;

namespace MarketPlaceUserTest.TestController
{
    public class ProductTestController
    {
        private readonly IProductService _productService;
        private readonly ISecurityUtil _securityUtil;
        private readonly ProductController _controller;

        public ProductTestController()
        {
            _productService = Substitute.For<IProductService>();
            _securityUtil = Substitute.For<ISecurityUtil>();
            _controller = new ProductController(_productService, _securityUtil);
        }


        [Fact]
        public async Task Add_Returns_Created_When_Product_Is_Added_Successfully()
        {
            // Arrange
            var form = new ProductForm { ProductName = "dd", ProductDescription = "c", CategoryId = 1, Price = 200, Location = new LocationForm { Address = "wev ", Latitude = 90, Longitude = 90 } };

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _productService.AddProductAsync(form, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.Add(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Add_Returns_BadRequest_When_Model_State_Is_Invalid()
        {
            // Arrange
            var form = new ProductForm { ProductName = "dd", ProductDescription = "c", CategoryId = 1, Price = 200, Location = new LocationForm { Address = "wev ", Latitude = 90, Longitude = 90 } };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _productService.AddProductAsync(form, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.Add(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Add_Returns_Unauthorized_When_User_Is_Not_Authorized()
        {
            // Arrange
            var form = new ProductForm { ProductName = "dd", ProductDescription = "c", CategoryId = 1, Price = 200, Location = new LocationForm { Address = "wev ", Latitude = 90, Longitude = 90 } };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _productService.AddProductAsync(form, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.Add(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task GetProduct_Returns_Ok_When_Product_Exists()
        {
            // Arrange
            int productId = 1;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _productService.GetProductAsync(productId, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.GetProduct(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task GetProduct_ReturnsBadRequest()
        {
            // Arrange
            int productId = 0;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _productService.GetProductAsync(productId, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.GetProduct(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }
        [Fact]
        public async Task GetProduct_ReturnsUnauthorized()
        {
            // Arrange
            int productId = 1;
            int userId = 0;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _productService.GetProductAsync(productId, userId).Returns(expectedResult);

            // Act
            var result = await _controller.GetProduct(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Update_Returns_Unauthorized_When_User_Is_Not_Admin()
        {
            // Arrange
            int productId = 1;
            int UserId = 0;
            var form = new ProductForm { ProductName = "dd", ProductDescription = "c", CategoryId = 1, Price = 200, Location = new LocationForm { Address = "wev ", Latitude = 90, Longitude = 90 } };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _productService.EditProductAsync(form, productId, UserId).Returns(expectedResult);

            // Act
            var result = await _controller.Update(productId, form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }
        [Fact]
        public async Task Update_Returns_Success()
        {
            // Arrange
            int productId = 1;
            var form = new ProductForm { ProductName = "dd", ProductDescription = "c", CategoryId = 1, Price = 200, Location = new LocationForm { Address = "wev ", Latitude = 90, Longitude = 90 } };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _productService.EditProductAsync(form, productId, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.Update(productId, form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Update_Returns_Badrequest()
        {
            // Arrange
            int productId = 0;
            var form = new ProductForm { ProductName = "dd", ProductDescription = "c", CategoryId = 1, Price = 200, Location = new LocationForm { Address = "wev ", Latitude = 90, Longitude = 90 } };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _productService.EditProductAsync(form, productId, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.Update(productId, form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Delete_Returns_Unauthorized_When_User_Is_Not_Authorized()
        {
            // Arrange
            int productId = 1;
            int userId = 0;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _productService.DeleteProductAsync(productId, userId).Returns(expectedResult);

            // Act
            var result = await _controller.Delete(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Delete_Returns_BadRequest_When_ProductId_Is_Invalid()
        {
            // Arrange
            int productId = 0;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _productService.DeleteProductAsync(productId, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.Delete(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Delete_Returns_NoContent_When_Product_Is_Successfully_Deleted()
        {
            // Arrange
            int productId = 1;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _productService.DeleteProductAsync(productId, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.Delete(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Delete_Returns_InternalServerError_When_Product_Cannot_Be_Deleted()
        {
            // Arrange
            int productId = 1;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.InternalServerError };
            _productService.DeleteProductAsync(productId, _securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _controller.Delete(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task PaginatedProductList_Returns_Ok_When_Parameters_Are_Valid()
        {
            // Arrange
            var form = new ProductPaginationParams
            {
                Offset = 1,
                PageSize = 10,
                CategoryId = new int?[1], // use int?[] instead of int[] 
                Search = "name",
                SortBy = "Date",
                SortByDesc = true,
                StartPrice = 200,
                EndPrice = 300,
                Location = "gf"
            };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _productService.ProductListAsync(form, _securityUtil.GetCurrentUserRole()).Returns(expectedResult);
            var controller = new ProductController(_productService, _securityUtil);

            // Act
            var result = await controller.PaginatedProductList(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);

        }

        [Fact]
        public async Task PaginatedProductList_Returns_BadRequest_When_Parameters_Are_Invalid()
        {
            // Arrange
            var form = new ProductPaginationParams
            {
                Offset = 1,
                PageSize = 10,
                CategoryId = new int?[1], // use int?[] instead of int[] 
                Search = "name",
                SortBy = "Date",
                SortByDesc = true,
                StartPrice = 200,
                EndPrice = 300,
                Location = "gf"
            };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _productService.ProductListAsync(form, _securityUtil.GetCurrentUserRole()).Returns(expectedResult);
            var controller = new ProductController(_productService, _securityUtil);

            // Act
            var result = await controller.PaginatedProductList(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task PaginatedProductList_Returns_UnAuthorized()
        {
            // Arrange

            var form = new ProductPaginationParams
            {
                Offset = 1,
                PageSize = 10,
                CategoryId = new int?[1], // use int?[] instead of int[] 
                Search = "name",
                SortBy = "Date",
                SortByDesc = true,
                StartPrice = 200,
                EndPrice = 300,
                Location = "gf"
            };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _productService.ProductListAsync(form, _securityUtil.GetCurrentUserRole()).Returns(expectedResult);
            var controller = new ProductController(_productService, _securityUtil);

            // Act
            var result = await controller.PaginatedProductList(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

       

        [Fact]
        public async Task GetProductByUserId_Returns_Ok_When_Products_Exist()
        {
            // Arrange
            int userId = 0;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _productService.GetProductByUserIdAsync(userId == 0 ? _securityUtil.GetCurrentUserId() : userId).Returns(expectedResult);

            // Act
            var result = await _controller.GetProductByUserId(userId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task GetProductByUserId_Returns_Unauthorized_When_User_Id_Is_Not_Zero_And_Current_User_Does_Not_Match()
        {
            // Arrange
            int userId = 2;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _productService.GetProductByUserIdAsync(userId == 0 ? _securityUtil.GetCurrentUserId() : userId).Returns(expectedResult);

            // Act
            var result = await _controller.GetProductByUserId(userId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

    }

}
