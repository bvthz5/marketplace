using MarketPlaceUser.Api.Controllers;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace MarketPlaceUserTest.TestController
{
    public class GoogleUserTestController
    {
        [Fact]
        public async Task GoogleLoginTest()
        {
            //Arrange

            IGoogleAuthService googleAuthService = Substitute.For<IGoogleAuthService>();

            string token = "Valid Google Id Token";

            googleAuthService.RegisterAndLogin(token).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            GoogleUserController googleUserController = new(googleAuthService);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            //Act

            var actualResult = await googleUserController.Login(token) as ObjectResult;

            //Assert

            Assert.Equal(actualResult?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((actualResult?.Value as ServiceResult)?.ServiceStatus, expectedResult.ServiceStatus);

        }
    }
}
