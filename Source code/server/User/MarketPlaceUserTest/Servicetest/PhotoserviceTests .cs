using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using MarketPlaceUser.Bussiness.Services;
using MarketPlaceUser.Bussiness.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;

namespace MarketPlaceUserTest.Servicetest
{
    public class PhotoserviceTests
    {
        [Fact]
        public async Task AddPhotosAsync_ProductNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var productId = 1;
            var image = new ProductImageForm
            {
                File = new List<IFormFile> { Mock.Of<IFormFile>() }.ToArray()
            };

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(uow => uow.ProductRepostory.FindById(productId)).ReturnsAsync((Product)null);

            var mockSecurityUtil = new Mock<ISecurityUtil>();
            mockSecurityUtil.Setup(securityUtil => securityUtil.GetCurrentUserId()).Returns(1);

            var mockFileUtil = new Mock<IFileUtil>();
            var mockLogger = new Mock<ILogger<PhotosService>>();

            var service = new PhotosService(mockUow.Object, mockSecurityUtil.Object, mockFileUtil.Object, mockLogger.Object);

            // Act
            var result = await service.AddPhotosAsync(productId, image);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Product Not Found", result.Message);
            mockUow.Verify(uow => uow.PhotoRepository.Add(It.IsAny<Photos>()), Times.Never);
            mockUow.Verify(uow => uow.SaveAsync(), Times.Never);
        }



        [Fact]
        public async Task AddPhotosAsync_MaximumPhotoCountExceeded_ReturnsBadRequestResult()
        {
            // Arrange
            var productId = 1;
            var image = new ProductImageForm
            {
                File = new List<IFormFile>
        {
            GenerateMockFormFile(new byte[1]),
            GenerateMockFormFile(new byte[1]),
            GenerateMockFormFile(new byte[1])
        }.ToArray()
            };
            var product = new Product { CreatedUserId = 1, Status = Product.ProductStatus.ACTIVE };

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(uow => uow.ProductRepostory.FindById(productId)).ReturnsAsync(product);
            mockUow.Setup(uow => uow.PhotoRepository.FindByProductIdAsync(productId)).ReturnsAsync(new List<Photos> { new Photos(), new Photos(), new Photos() });

            var mockSecurityUtil = new Mock<ISecurityUtil>();
            mockSecurityUtil.Setup(securityUtil => securityUtil.GetCurrentUserId()).Returns(1);

            var mockFileUtil = new Mock<IFileUtil>();
            var mockLogger = new Mock<ILogger<PhotosService>>();

            var service = new PhotosService(mockUow.Object, mockSecurityUtil.Object, mockFileUtil.Object, mockLogger.Object);

            // Act
            var result = await service.AddPhotosAsync(productId, image);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Failed to add images", result.Message);
            mockUow.Verify(uow => uow.PhotoRepository.Add(It.IsAny<Photos>()), Times.Never);
            mockUow.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        private static IFormFile GenerateMockFormFile(byte[] fileBytes)
        {
            var fileMock = new Mock<IFormFile>();
            var fileName = "test.jpg";
            var content = new MemoryStream(fileBytes);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(content.Length);
            fileMock.Setup(_ => _.OpenReadStream()).Returns(content);
            return fileMock.Object;
        }

        [Fact]
        public async Task AddPhotosAsync_ValidInput_ReturnsSuccessResult()
        {
            // Arrange
            var _securityUtil = Substitute.For<ISecurityUtil>();
            var _uow = Substitute.For<IUnitOfWork>();
            var mockFileUtil = Substitute.For<IFileUtil>();
            var mockLogger = Substitute.For<ILogger<PhotosService>>();
            var productId = 1;
            var image = new ProductImageForm
            {
                File = new[] { new FormFile(Stream.Null, 0, 0, "testImage.jpg", "testImage.jpg") }
            };
            var product = new Product
            {
                ProductId = productId,
                CreatedUserId = _securityUtil.GetCurrentUserId(), // Set the current user ID
                Status = Product.ProductStatus.ACTIVE // Set the product status to ACTIVE
                                                            // Set other relevant properties of the product if needed
            };
            var _photosService = new PhotosService(_uow, _securityUtil, mockFileUtil, mockLogger);

            _uow.ProductRepostory.FindById(productId).Returns(product);
            _uow.PhotoRepository.FindByProductIdAsync(productId).Returns(new List<Photos>()); // No existing photos for the product

            // Act
            var result = await _photosService.AddPhotosAsync(productId, image);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Photos Added", result.Message);
             _uow.PhotoRepository.Received(image.File.Length).Add(Arg.Any<Photos>());
             _uow.ProductRepostory.Received(1).Update(product);
            await _uow.Received(1).SaveAsync();
        }



    }
}
