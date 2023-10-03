using MarketPlaceAdmin.Api.Controllers;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace MarketplaceAdminTest.Controller
{
    public class GoogleControllerTest
    {
        [Fact]
        public async Task GoogleLoginTest()
        {
            //Arrange

            IGoogleAuthService googleAuthService = Substitute.For<IGoogleAuthService>();

            string token = "Valid Google Id Token";

            googleAuthService.Login(token).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            GoogleController googleUserController = new(googleAuthService);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            //Act

            var actualResult = await googleUserController.Login(token) as ObjectResult;

            //Assert
            Assert.NotNull(actualResult);
            Assert.NotNull(actualResult.Value);
            Assert.Equal(actualResult.StatusCode, StatusCodes.Status200OK);
            Assert.Equal(((ServiceResult)actualResult.Value).ServiceStatus, expectedResult.ServiceStatus);
        }

        [Fact]
        public async Task GoogleLoginSuccessTest()
        {
            // Arrange
            string idToken = "valid_token";

            IGoogleAuthService googleAuthService = Substitute.For<IGoogleAuthService>();
            googleAuthService.AgentLogin(idToken).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            GoogleController googleUserController = new(googleAuthService);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            // Act
            var actualResult = await googleUserController.GoogleLogin(idToken) as ObjectResult;

            // Assert
            Assert.NotNull(actualResult);
            Assert.NotNull(actualResult.Value);
            Assert.Equal(actualResult.StatusCode, StatusCodes.Status200OK);
            Assert.Equal(((ServiceResult)actualResult.Value).ServiceStatus, expectedResult.ServiceStatus);
        }
    }
}
