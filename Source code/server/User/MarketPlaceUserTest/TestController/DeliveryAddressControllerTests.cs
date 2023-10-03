using MarketPlaceUser.Api.Controllers;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace MarketPlaceUserTest.TestController
{
    public class DeliveryAddressControllerTests
    {
        private readonly IDeliveryAddressService _deliveryAddressService;
        private readonly ISecurityUtil _securityUtil;
        private readonly DeliveryAddressController _controller;

        public DeliveryAddressControllerTests()
        {
            _deliveryAddressService = Substitute.For<IDeliveryAddressService>();
            _securityUtil = Substitute.For<ISecurityUtil>();
            _controller = new DeliveryAddressController(_deliveryAddressService, _securityUtil);
        }

        [Fact]
        public async Task AddAddress_ValidDeliveryAddress_ReturnsOk()
        {
            // Arrange
            var deliveryAddressForm = new DeliveryAddressForm { /* Valid delivery address form data */ };
            var userId = 123;
            _securityUtil.GetCurrentUserId().Returns(userId);
            _deliveryAddressService.AddAddressAsync(deliveryAddressForm, userId).Returns(new ServiceResult { ServiceStatus = ServiceStatus.Success });

            // Act
            var result = await _controller.AddAddress(deliveryAddressForm) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)ServiceStatus.Success, result.StatusCode);
        }

        [Fact]
        public async Task GetAddresses_AuthenticatedUser_ReturnsOk()
        {
            // Arrange
            var userId = 123;
            _securityUtil.GetCurrentUserId().Returns(userId);
            _deliveryAddressService.GetAddressAsync(userId).Returns(new ServiceResult { ServiceStatus = ServiceStatus.Success });

            // Act
            var result = await _controller.GetAdresses() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)ServiceStatus.Success, result.StatusCode);
        }
        [Fact]
        public async Task GetAddressById_ValidDeliveryAddressId_ReturnsOk()
        {
            // Arrange
            var deliveryAddressId = 456;
            var userId = 123;
            _securityUtil.GetCurrentUserId().Returns(userId);
            _deliveryAddressService.GetAddressById(deliveryAddressId, userId).Returns(new ServiceResult { ServiceStatus = ServiceStatus.Success });

            // Act
            var result = await _controller.GetAddressById(deliveryAddressId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)ServiceStatus.Success, result.StatusCode);
        }

        [Fact]
        public async Task UpdateStatus_ValidDeliveryAddressId_ReturnsOk()
        {
            // Arrange
            var deliveryAddressId = 456;
            var userId = 123;
            _securityUtil.GetCurrentUserId().Returns(userId);
            _deliveryAddressService.SetAddressDefault(userId, deliveryAddressId).Returns(new ServiceResult { ServiceStatus = ServiceStatus.Success });

            // Act
            var result = await _controller.UpdateStatus(deliveryAddressId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)ServiceStatus.Success, result.StatusCode);
        }

        [Fact]
        public async Task UpdateAddress_ValidDeliveryAddress_ReturnsOk()
        {
            // Arrange
            var deliveryAddressId = 456;
            var deliveryAddressForm = new DeliveryAddressForm { /* Valid delivery address form data */ };
            var userId = 123;
            _securityUtil.GetCurrentUserId().Returns(userId);
            _deliveryAddressService.EditAddressAsync(deliveryAddressForm, deliveryAddressId, userId).Returns(new ServiceResult { ServiceStatus = ServiceStatus.Success });

            // Act
            var result = await _controller.UpdateAddress(deliveryAddressId, deliveryAddressForm) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)ServiceStatus.Success, result.StatusCode);
        }
        [Fact]
        public async Task DeleteAddress_WithValidDeliveryAddressId_ShouldReturnSuccessResult()
        {
            // Arrange
            int deliveryAddressId = 1;
            int userId = 123;
            _securityUtil.GetCurrentUserId().Returns(userId);
            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            _deliveryAddressService.DeleteAddress(userId, deliveryAddressId)
                .Returns(expectedResult);

            // Act
            var result = await _controller.DeleteAddress(deliveryAddressId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)expectedResult.ServiceStatus, objectResult.StatusCode);
            Assert.Equal(expectedResult, objectResult.Value);
        }

    }
}
