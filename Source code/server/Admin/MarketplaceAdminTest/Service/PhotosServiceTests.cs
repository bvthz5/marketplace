using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Services;
using MarketPlaceAdmin.Bussiness.Util.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace MarketplaceAdminTest.Service
{
    public class PhotosServiceTests
    {
        [Fact]
        public async Task GetPhotos_Should_Return_ServiceResult_With_PhotosView_List()
        {
            // Arrange
            var uow = Substitute.For<IUnitOfWork>();
            var fileUtil = Substitute.For<IFileUtil>();
            var logger = Substitute.For<ILogger<PhotosService>>();

            var service = new PhotosService(uow, fileUtil, logger);

            var productId = 1;

            var photos = new List<Photos>
            {
                new Photos { PhotosId = 1, Photo = "photo1.jpg", ProductId = 1 , Product = new Product() },
                new Photos { PhotosId = 2, Photo = "photo2.jpg", ProductId = 1 , Product = new Product() }
            };

            uow.PhotoRepository.FindByProductIdAsync(productId).Returns(Task.FromResult(photos));

            // Act
            var result = await service.GetPhotos(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Product Image List", result.Message);

            var photosViewList = Assert.IsType<List<PhotosView>>(result.Data);
            Assert.Equal(photos.Count, photosViewList.Count);
        }

        [Fact]
        public async Task GetPhotosByName_Should_Return_FileStream_If_File_Exists()
        {
            // Arrange
            var uow = Substitute.For<IUnitOfWork>();
            var fileUtil = Substitute.For<IFileUtil>();
            var logger = Substitute.For<ILogger<PhotosService>>();

            var service = new PhotosService(uow, fileUtil, logger);

            var fileName = "photo1.jpg";

            uow.PhotoRepository.IsPhotoExists(fileName).Returns(true);

            var fileStream = File.Create("productsimage.jpg");

            fileUtil.GetProductImages(fileName).Returns(fileStream);

            // Act
            var result = await service.GetPhotosByName(fileName);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<FileStream>(result);
            Assert.Equal(fileStream, result);
        }

        [Fact]
        public async Task GetPhotosByName_Should_Return_Null_If_File_Does_Not_Exist()
        {
            // Arrange
            var uow = Substitute.For<IUnitOfWork>();
            var fileUtil = Substitute.For<IFileUtil>();
            var logger = Substitute.For<ILogger<PhotosService>>();

            var service = new PhotosService(uow, fileUtil, logger);

            var fileName = "photo1.jpg";

            uow.PhotoRepository.IsPhotoExists(fileName).Returns(false);

            // Act
            var result = await service.GetPhotosByName(fileName);

            // Assert
            Assert.Null(result);
        }
    }
}
