using MarketPlaceAdmin.Api.Controllers;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace MarketplaceAdminTest.Controller
{
    public class UserControllerTests
    {

        private readonly IUserService _userService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userService = Substitute.For<IUserService>();
            _controller = new UserController(_userService);
        }

        [Fact]
        public async Task GetUser_Returns_SuccessResult()
        {
            // Arrange
            int userId = 1;
            var expectedResult = new ServiceResult();
            _userService.GetUser(userId).Returns(expectedResult);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var actualResult = Assert.IsType<ServiceResult>(okResult.Value);
            Assert.Equal(expectedResult.ServiceStatus, actualResult.ServiceStatus);
            Assert.Equal(expectedResult.Data, actualResult.Data);
        }


        [Fact]
        public async Task ProfilePicSuccessTest()
        {
            //Arrange
            string fileName = "Valid File Name with Extension";

            FileStream fs = File.Create("test");

            _userService.GetProfilePic(fileName).Returns(fs);

            //Act
            var actualResult = await _controller.GetProfile(fileName) as FileStreamResult;

            //Assert
            Assert.NotNull(actualResult);
            Assert.NotNull(actualResult.FileStream);
            Assert.True(actualResult.FileStream.CanRead);
            Assert.Equal("image/jpeg", actualResult.ContentType);
        }

        [Fact]
        public async Task ProfilePicFailTest()
        {
            //Arrange
            string fileName = "Valid File Name with Extension";

            _userService.GetProfilePic(fileName).ReturnsNull();

            //Act
            var actualResult = await _controller.GetProfile(fileName) as NotFoundResult;

            //Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status404NotFound, actualResult.StatusCode);
        }

        [Fact]
        public async Task ChangeUserStatus_Returns_SuccessResult()
        {
            // Arrange
            int userId = 1;
            byte status = 2;
            var expectedResult = new ServiceResult();
            _userService.ChangeStatusAsync(userId, status).Returns(expectedResult);

            // Act
            var result = await _controller.ChangeUserStatus(userId, status);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var actualResult = Assert.IsType<ServiceResult>(okResult.Value);
            Assert.Equal(expectedResult.ServiceStatus, actualResult.ServiceStatus);
            Assert.Null(actualResult.Data);
        }

        [Fact]
        public async Task PaginatedUserList_Returns_SuccessResult()
        {
            // Arrange
            var form = new UserPaginationParams();
            var expectedResult = new ServiceResult();
            _userService.UserListAsync(form).Returns(expectedResult);

            // Act
            var result = await _controller.PaginatedUserList(form);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var actualResult = Assert.IsType<ServiceResult>(okResult.Value);
            Assert.Equal(expectedResult.ServiceStatus, actualResult.ServiceStatus);
            Assert.Equal(expectedResult.Data, actualResult.Data);
        }

        [Fact]
        public async Task SellerRequest_Returns_SuccessResult()
        {
            // Arrange
            int userId = 1;
            var form = new RequestForm();
            var expectedResult = new ServiceResult();
            _userService.SellerRequest(userId, form).Returns(expectedResult);

            // Act
            var result = await _controller.SellerRequest(userId, form);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var actualResult = Assert.IsType<ServiceResult>(okResult.Value);
            Assert.Equal(expectedResult.ServiceStatus, actualResult.ServiceStatus);
            Assert.Equal(expectedResult.Data, actualResult.Data);
        }

        [Fact]
        public async Task SellerProductCount_Returns_Success()
        {
            // Arrange
            var expectedResult = new ServiceResult();
            _userService.SellerProductCount().Returns(expectedResult);

            // Act
            var result = await _controller.SellerProductCount();

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.NotNull(result);
            Assert.Equal((int)expectedResult.ServiceStatus, ((ObjectResult)result).StatusCode);
            Assert.Equal(expectedResult, ((ObjectResult)result).Value);
        }

        [Fact]
        public async Task SellerProductStatusCount_Returns_Success()
        {
            // Arrange
            int userId = 1;
            var expectedResult = new ServiceResult();
            _userService.SellerProductStatusCount(userId).Returns(expectedResult);

            // Act
            var result = await _controller.SellerProductCount(userId);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.NotNull(result);
            Assert.Equal((int)expectedResult.ServiceStatus, ((ObjectResult)result).StatusCode);
            Assert.Equal(expectedResult, ((ObjectResult)result).Value);
        }
    }
}