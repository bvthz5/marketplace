using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Api.Controllers;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace MarketPlaceUserTest.TestController
{
    public class WishListTestController
    {
        private readonly IWishListService _wishListService;
        private readonly ISecurityUtil _securityUtil;
        private readonly WishListController _wishListController;

        public WishListTestController()
        {
            _wishListService = Substitute.For<IWishListService>();
            _securityUtil = Substitute.For<ISecurityUtil>();
            _wishListController = new WishListController(_wishListService, _securityUtil);
        }

        [Fact]
        public async Task Add_Returns_CorrectStatusCode()
        {
            // Arrange
            int productId = 1;

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _wishListService.AddToWishListAsync(_securityUtil.GetCurrentUserId(), productId).Returns(expectedResult);

            // Act
            var result = await _wishListController.Add(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Add_Returns_badrequest()
        {
            // Arrange
            int productId = 0;

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _wishListService.AddToWishListAsync(_securityUtil.GetCurrentUserId(), productId).Returns(expectedResult);

            // Act
            var result = await _wishListController.Add(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Add_Returns_unauthorized()
        {
            // Arrange
            int productId = 0;
            int userId = 0;

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _wishListService.AddToWishListAsync(userId, productId).Returns(expectedResult);

            // Act
            var result = await _wishListController.Add(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Get_Returns_OK_With_WishList_When_User_Has_WishList()
        {
            // Arrange
            var userId = 1;
            var wishList = new List<Product>()
    {
        new Product
        {
            ProductId = 1,
            ProductName = "Product1",
            ProductDescription = "f",
            CategoryId = 1,
            CreatedUserId = 1,
            Price = 10,
            Longitude = 90,
            Latitude = 90,
            Address = "Thz",
            Photos = new List<Photos>()
        }
    };
            await _wishListService.AddToWishListAsync(userId, 1);

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _wishListService.GetWishListAsync(_securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _wishListController.Get() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

            var serviceResult = result.Value as ServiceResult;
            Assert.NotNull(serviceResult);
            Assert.Equal(expectedResult.ServiceStatus, serviceResult.ServiceStatus);
        }

        [Fact]
        public async Task Delete_Returns_Unauthorized_When_User_Is_Not_Authenticated()
        {
            // Arrange
            int productId = 1;
            int userId = 0;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _wishListService.RemoveFromWishListAsync(userId, productId).Returns(expectedResult);

            // Act
            var result = await _wishListController.Delete(productId) as ObjectResult;

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
            _wishListService.RemoveFromWishListAsync(_securityUtil.GetCurrentUserId(), productId).Returns(expectedResult);

            // Act
            var result = await _wishListController.Delete(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Delete_Returns_Correct_Status_Code_And_Service_Result()
        {
            // Arrange
            int productId = 1;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _wishListService.RemoveFromWishListAsync(_securityUtil.GetCurrentUserId(), productId).Returns(expectedResult);

            // Act
            var result = await _wishListController.Delete(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }
    }
}
