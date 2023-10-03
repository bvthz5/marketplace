using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Api.Controllers;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using NSubstitute;
using System.Security.Claims;

namespace MarketPlaceUserTest.TestController
{
    public class PhotosTestController
    {


        [Fact]
        public async Task GetPhotos_Returns_Ok_When_ProductId_Is_Valid()
        {
            // Arrange
            int productId = 1;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };

            var mockPhotosService = new Mock<IPhotosService>();
            mockPhotosService.Setup(service => service.GetPhotosAsync(productId))
                             .ReturnsAsync(expectedResult);
            var photosController = new PhotosController(mockPhotosService.Object);

            // Act
            var result = await photosController.GetPhotos(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task GetPhotos_Returns_BadRequest_When_ProductId_Is_Zero()
        {
            // Arrange
            int productId = 0;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };

            var mockPhotosService = new Mock<IPhotosService>();
            mockPhotosService.Setup(service => service.GetPhotosAsync(productId))
                             .ReturnsAsync(expectedResult);
            var photosController = new PhotosController(mockPhotosService.Object);

            // Act
            var result = await photosController.GetPhotos(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task GetPhotos_Returns_NotFound_When_ProductId_Does_Not_Exist()
        {
            // Arrange
            int productId = 100;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.NotFound };

            var mockPhotosService = new Mock<IPhotosService>();
            mockPhotosService.Setup(service => service.GetPhotosAsync(productId))
                             .ReturnsAsync(expectedResult);
            var photosController = new PhotosController(mockPhotosService.Object);

            // Act
            var result = await photosController.GetPhotos(productId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task GetPhotosByPath_Returns_NotFound_When_File_Not_Found()
        {
            // Arrange
            string filePath = "invalid_file_path.jpg";
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.NotFound };
            var mockPhotosService = new Mock<IPhotosService>();
            mockPhotosService.Setup(service => service.GetPhotosByName(filePath));

            var photosController = new PhotosController(mockPhotosService.Object);

            // Act
            var result = await photosController.GetPhotosByPath(filePath);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Add_Returns_BadRequest_When_ProductId_Is_Invalid()
        {
            // Arrange
            int productId = 0;

            var image = new ProductImageForm { File = null };
            var mockPhotosService = new Mock<IPhotosService>();

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest, Message = "Invalid product ID" };
            mockPhotosService.Setup(service => service.AddPhotosAsync(productId, image))
                              .ReturnsAsync(expectedResult);
            var photosController = new PhotosController(mockPhotosService.Object);

            // Act
            var result = await photosController.Add(productId, image);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            var serviceResult = Assert.IsType<ServiceResult>(badRequestResult.Value);
            Assert.Equal(expectedResult.ServiceStatus, serviceResult.ServiceStatus);
            Assert.Equal(expectedResult.Message, serviceResult.Message);
        }

        [Fact]
        public async Task Add_Returns_Created_When_Photo_Is_Added_Successfully()
        {
            // Arrange
            int productId = 1;
            var image = new ProductImageForm { File = null };
            var mockPhotosService = new Mock<IPhotosService>();
            var expectedData = new
            {
                Id = 1,
                FilePath = "image.jpg"
            };
            var expectedResult = new ServiceResult
            {
                ServiceStatus = ServiceStatus.Created,
                Data = expectedData
            };
            mockPhotosService.Setup(service => service.AddPhotosAsync(productId, image))
                              .ReturnsAsync(expectedResult);
            var photosController = new PhotosController(mockPhotosService.Object);

            // Act
            var result = await photosController.Add(productId, image);

            // Assert
            var createdResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(expectedResult.ServiceStatus, (ServiceStatus)createdResult.StatusCode);
            Assert.Equal(expectedData, ((ServiceResult)createdResult.Value).Data);
        }

        [Fact]
        public async Task DeletePhotos_Returns_NoContent_When_Deleted_Successfully()
        {
            // Arrange
            int productId = 1;
            var mockPhotosService = new Mock<IPhotosService>();
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            mockPhotosService.Setup(service => service.DeletePhotosByProductIdAsync(productId))
                             .ReturnsAsync(expectedResult);
            var photosController = new PhotosController(mockPhotosService.Object);

            // Act
            var result = await photosController.DeletePhotos(productId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task DeletePhotos_Returns_NotFound_When_NoPhotos_Found()
        {
            // Arrange
            int productId = 1;
            var mockPhotosService = new Mock<IPhotosService>();
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.NotFound };
            mockPhotosService.Setup(service => service.DeletePhotosByProductIdAsync(productId))
                             .ReturnsAsync(expectedResult);
            var photosController = new PhotosController(mockPhotosService.Object);

            // Act
            var result = await photosController.DeletePhotos(productId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task DeletePhotos_Returns_BadRequest_When_ProductId_Is_Invalid()
        {
            // Arrange
            int productId = 0;
            var mockPhotosService = new Mock<IPhotosService>();
            mockPhotosService.Setup(service => service.DeletePhotosByProductIdAsync(productId))
                             .ReturnsAsync(new ServiceResult { ServiceStatus = ServiceStatus.BadRequest });
            var photosController = new PhotosController(mockPhotosService.Object);

            // Act
            var result = await photosController.DeletePhotos(productId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task DeletePhotosByPhotoId_Returns_NoContent_When_Photo_Is_Deleted_Successfully()
        {
            // Arrange
            int photoId = 1;
            var mockPhotosService = new Mock<IPhotosService>();
            var expectedResult = new ServiceResult
            {
                ServiceStatus = ServiceStatus.Success
            };
            mockPhotosService.Setup(service => service.DeletePhotosByPhotoIdAsync(photoId))
                             .ReturnsAsync(expectedResult);
            var photosController = new PhotosController(mockPhotosService.Object);

            // Act
            var result = await photosController.DeletePhotosByPhotoId(photoId);

            // Assert
            var noContentResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(expectedResult.ServiceStatus, (ServiceStatus)noContentResult.StatusCode);
        }

        [Fact]
        public async Task GetPhotosByPath_NonExistentFilePath_ReturnsNotFoundResult()
        {
            // Arrange
            string filePath = "path/to/nonexistent/file.jpg";
            var mockPhotosService = new Mock<IPhotosService>();
            mockPhotosService.Setup(s => s.GetPhotosByName(filePath)).ReturnsAsync((FileStream)null);
            var controller = new PhotosController(mockPhotosService.Object);

            // Act
            var result = await controller.GetPhotosByPath(filePath);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPhotosByPath_NullStream_ReturnsNotFoundResult()
        {
            // Arrange
            string filePath = "path/to/file.jpg";
            var mockPhotosService = new Mock<IPhotosService>();
            mockPhotosService.Setup(s => s.GetPhotosByName(filePath)).ReturnsAsync((FileStream)null);
            var controller = new PhotosController(mockPhotosService.Object);

            // Act
            var result = await controller.GetPhotosByPath(filePath);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPhotosByPath_ValidFile_ReturnsFileResult()
        {
            // Arrange
            var fileName = "validfile.jpg";
            var fileStream = new FileStream(fileName, FileMode.Create);
            var mockPhotosService = new Mock<IPhotosService>();
            mockPhotosService.Setup(s => s.GetPhotosByName(fileName)).ReturnsAsync(fileStream);
            var controller = new PhotosController(mockPhotosService.Object);

            // Act
            var result = await controller.GetPhotosByPath(fileName);

            // Assert
            Assert.IsType<FileStreamResult>(result);
            var fileResult = result as FileStreamResult;
            Assert.Equal("image/jpeg", fileResult.ContentType);
            // Clean up
            fileStream.Close();
            File.Delete(fileName);
        }

    }
}
