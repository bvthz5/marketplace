using MarketPlaceUser.Api.Controllers;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace MarketPlaceUserTest.TestController
{
    public class LoginTestController
    {
        [Fact]
        public async void LoginTest()
        {

            //Arrange

            IUserService userService = Substitute.For<IUserService>();

            LoginForm loginForm = new LoginForm()
            {
                Email = "anandhu@gmail.com",
                Password = "Password@123"
            };

            userService.Login(loginForm).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            LoginController loginController = new(userService);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };


            //Act

            var actualResult = await loginController.Login(loginForm) as ObjectResult;


            //Assert

            Assert.Equal(actualResult?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((actualResult?.Value as ServiceResult)?.ServiceStatus, expectedResult.ServiceStatus);

        }

        [Fact]
        public async Task RefreshSuccessTest()
        {
            //Arrange

            IUserService userService = Substitute.For<IUserService>();

            string token = "Valid JWT Token";

            userService.RefreshAsync(token).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            LoginController loginController = new(userService);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            //Act

            var actualResult = await loginController.Refresh(token) as ObjectResult;

            //Assert

            Assert.Equal(actualResult?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((actualResult?.Value as ServiceResult)?.ServiceStatus, expectedResult.ServiceStatus);

        }
    }
}