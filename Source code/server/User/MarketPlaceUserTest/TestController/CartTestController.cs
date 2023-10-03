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
    public class CartTestController
    {
        private readonly ICartService _cartService;
        private readonly ISecurityUtil _securityUtil;
        private readonly CartController _cartController;

        public CartTestController()
        {
            _cartService = Substitute.For<ICartService>();
            _securityUtil = Substitute.For<ISecurityUtil>();
            _cartController = new CartController(_cartService, _securityUtil);
        }

        [Fact]
        public async Task Add_ReturnsOk_WhenProductAddedSuccessfullyAsync()
        {
            // Arrange
            int productId = 1;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _cartService.AddToCartAsync(_securityUtil.GetCurrentUserId(), productId).Returns(expectedResult);

            // Act
            var result = await _cartController.Add(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Add_ReturnsBadRequest_WhenProductIdIsInvalid()
        {
            // Arrange
            int productId = 0;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _cartService.AddToCartAsync(_securityUtil.GetCurrentUserId(), productId).Returns(expectedResult);

            // Act
            var result = await _cartController.Add(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            int productId = 1;
            // Mock GetCurrentUserId() to return null.

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _cartService.AddToCartAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(expectedResult);

            // Act
            var result = await _cartController.Add(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Get_ReturnsCart()
        {
            //Arrange


            var Items = new List<Cart>
            {
                new Cart { ProductId = 1, UserId = 2, CartId =1 },
                new Cart { ProductId = 3, UserId = 1, CartId = 2 }
            };


            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success, Data = Items };
            _cartService.GetCartAsync(_securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _cartController.Get() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
            Assert.Equal(expectedResult.Data, ((ServiceResult)result.Value).Data);
        }

        [Fact]
        public async Task UnauthorizedUser()
        {
            // Arrange
            var Items = new List<Cart>
            {
                new Cart { ProductId = 1, UserId = 0, CartId =1 },
                new Cart { ProductId = 3, UserId = 0, CartId = 2 }
            };

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _cartService.GetCartAsync(Arg.Any<int>()).Returns(expectedResult);

            // Act
            var result = await _cartController.Get() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Get_ReturnsBadrequest()
        {
            //Arrange


            var Items = new List<Cart>
            {
                new Cart { ProductId = 0, UserId = 1, CartId =1 }
            };


            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest, Data = Items };
            _cartService.GetCartAsync(_securityUtil.GetCurrentUserId()).Returns(expectedResult);

            // Act
            var result = await _cartController.Get() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
            Assert.Equal(expectedResult.Data, ((ServiceResult)result.Value).Data);
        }
        [Fact]
        public async Task Delete_ProductIdIsValid_ReturnsOkResult()
        {
            // Arrange
            int productId = 1;
            _cartService.RemoveFromCartAsync(_securityUtil.GetCurrentUserId(), productId)
                .Returns(new ServiceResult { ServiceStatus = ServiceStatus.Success });

            // Act
            var result = await _cartController.Delete(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<ServiceResult>(result.Value);
            Assert.Equal(ServiceStatus.Success, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Delete_ReturnsBadrequest()
        {
            //Arrange
            int productId = 0;
            _cartService.RemoveFromCartAsync(_securityUtil.GetCurrentUserId(), productId)
               .Returns(new ServiceResult { ServiceStatus = ServiceStatus.BadRequest });

            // Act
            var result = await _cartController.Delete(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<ServiceResult>(result.Value);
            Assert.Equal(ServiceStatus.BadRequest, ((ServiceResult)result.Value).ServiceStatus);

        }
    }

}
