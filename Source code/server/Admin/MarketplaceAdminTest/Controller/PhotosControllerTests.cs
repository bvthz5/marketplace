using MarketPlaceAdmin.Api.Controllers;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace MarketplaceAdminTest.Controller
{
    public class PhotosControllerTests
    {
        private readonly IPhotosService _photosService;
        private readonly PhotosController _photosController;

        public PhotosControllerTests()
        {
            _photosService = Substitute.For<IPhotosService>();
            _photosController = new PhotosController(_photosService);
        }

        [Fact]
        public async Task GetPhotos_WithValidProductId_ReturnsOkResult()
        {
            // Arrange
            int productId = 1;
            ServiceResult serviceResult = new ServiceResult() { ServiceStatus = ServiceStatus.Success };
            _photosService.GetPhotos(productId).Returns(serviceResult);

            // Act
            IActionResult actionResult = await _photosController.GetPhotos(productId);

            // Assert
            Assert.IsType<ObjectResult>(actionResult);
        }


        [Fact]
        public async Task ProfilePicSuccessTest()
        {
            //Arrange
            string fileName = "Valid File Name with Extension";

            FileStream fs = File.Create("photostest");

            _photosService.GetPhotosByName(fileName).Returns(fs);

            //Act
            var actualResult = await _photosController.GetPhotosByPath(fileName) as FileStreamResult;

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

            _photosService.GetPhotosByName(fileName).ReturnsNull();

            //Act
            var actualResult = await _photosController.GetPhotosByPath(fileName) as NotFoundResult;

            //Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status404NotFound, actualResult.StatusCode);
        }
    }
}