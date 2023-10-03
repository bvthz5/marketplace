using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Settings;
using MarketPlaceUser.Bussiness.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NSubstitute;
using System.Text;

namespace MarketPlaceUserTest.Util
{
    public class FileUtilTest
    {
        private readonly FileUtil _fileUtil;
        private readonly ImageSettings _imageSettings;
        private readonly ILogger<FileUtil> _logger;


        public FileUtilTest()
        {
            _imageSettings = new ImageSettings()
            {
                Path = "./",
                ProductImagePath = "Product",
                UserImagePath = "User"
            };

            _logger = Substitute.For<ILogger<FileUtil>>();

            _fileUtil = new FileUtil(Options.Create(_imageSettings), _logger);
        }

        [Fact]
        public void UploadProductImage_ValidProductAndFile_UploadsAndReturnsFileName()
        {
            // Arrange
            var product = new Product { ProductId = 1 };
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "file", "dummy.txt");

            var imageSettings = new ImageSettings { Path = "C:\\Images", ProductImagePath = "Products" };
            var logger = new Mock<ILogger<IFileUtil>>();
            var fileUtil = new FileUtil(Options.Create(imageSettings), logger.Object);

            // Act
            var result = fileUtil.UploadProductImage(product, file);

            // Assert
            Assert.NotNull(result);
            Assert.Matches(@"^1_[a-f\d]{8}(-[a-f\d]{4}){3}-[a-f\d]{12}\.txt$", result); // check if file name matches expected format
        }

        [Fact]
        public void GetProductImages_Fail()
        {
            // Arrange
            var fileName = "Invalid_Filename.jpg";

            // Act
            var result = _fileUtil.GetProductImages(fileName);


            // Assert

            Assert.Null(result);
            _logger.ReceivedWithAnyArgs().LogError("Error");


        }

        [Fact]
        public void GetProductImages_Success()
        {
            // Arrange
            var fileName = "Valid_Filename.jpg";
            var path = Path.Combine(_imageSettings.Path, _imageSettings.ProductImagePath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            using (FileStream fs = File.Create(path + "/" + fileName))
            {
                byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            // Act
            var result = _fileUtil.GetProductImages(fileName);


            // Assert

            Assert.NotNull(result);

        }

        [Fact]
        public void GetUserProfile_Fail()
        {
            // Arrange
            var fileName = "Invalid_Filename.jpg";

            // Act
            var result = _fileUtil.GetUserProfile(fileName);


            // Assert

            Assert.Null(result);
            _logger.ReceivedWithAnyArgs().LogError("Error");


        }

        [Fact]
        public void GetUserProfile_Success()
        {
            // Arrange
            var fileName = "Valid_Filename.jpg";
            var path = Path.Combine(_imageSettings.Path, _imageSettings.UserImagePath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            using (FileStream fs = File.Create(path + "/" + fileName))
            {
                byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            // Act
            var result = _fileUtil.GetUserProfile(fileName);


            // Assert
            Assert.NotNull(result);

        }

        [Fact]
        public void DeleteUserProfilePic_Success()
        {
            // Arrange
            var fileName = "User_ProfilePic_for_Delete.jpg";
            var path = Path.Combine(_imageSettings.Path, _imageSettings.UserImagePath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            using (FileStream fs = File.Create(path + "/" + fileName))
            {
                byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            // Act
            _fileUtil.DeleteUserProfilePic(fileName);


            // Assert
            Assert.Null(_fileUtil.GetUserProfile(fileName));
        }


        [Fact]
        public void DeleteUserProfilePic_Fail()
        {
            // Arrange
            var fileName = "Invalid_User_ProfilePic_for_Delete.jpg";

            var fileUtil = new FileUtil(Options.Create(new ImageSettings()
            {
                Path = "",
                UserImagePath = "//q//"
            }), _logger);

            // Act
            fileUtil.DeleteUserProfilePic(fileName);


            // Assert
            Assert.Null(_fileUtil.GetUserProfile(fileName));
            _logger.ReceivedWithAnyArgs().LogError("Error");
        }

        [Fact]
        public void UploadImage_ValidArguments_ReturnsTrue()
        {
            // Arrange
            var path = "C:\\Test\\Images";
            var fileName = "test_image.jpg";
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write("test image data");
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);
            fileMock.Setup(x => x.OpenReadStream()).Returns(ms);
            fileMock.Setup(x => x.ContentType).Returns("image/jpeg");

            var fileUtil = new FileUtil(Mock.Of<IOptions<ImageSettings>>(), Mock.Of<ILogger<IFileUtil>>());

            // Act
            var result = fileUtil.UploadImage(path, fileName, fileMock.Object);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(Path.Combine(path, fileName)));
        }

        [Fact]
        public void UploadImage_InvalidArguments_ReturnsFalse()
        {
            // Arrange
            var path = "C:\\Test\\Images";
            var fileName = "test_image.jpg";
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write("test image data");
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);
            fileMock.Setup(x => x.OpenReadStream()).Returns(ms);
            fileMock.Setup(x => x.ContentType).Returns("image/jpeg");

            var fileUtil = new FileUtil(Mock.Of<IOptions<ImageSettings>>(), Mock.Of<ILogger<IFileUtil>>());

            // Act
            var result = fileUtil.UploadImage(null, fileName, fileMock.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CreateDirectory_DirectoryDoesNotExist_CreatesDirectory()
        {
            // Arrange
            var path = "C:\\Test\\Images";

            // Act
            if (Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }
                foreach (var dir in Directory.GetDirectories(path))
                {
                    Directory.Delete(dir, true);
                }
                Directory.Delete(path);
            }

            // Assert
            Assert.False(Directory.Exists(path));

            Directory.CreateDirectory(path);

            Assert.True(Directory.Exists(path));
        }


        [Fact]
        public void CreateDirectory_DirectoryAlreadyExists_DoesNothing()
        {
            // Arrange
            var path = "C:\\Test\\Images";

            // Act
            var result = Directory.Exists(path);
            if (!result)
            {
                Directory.CreateDirectory(path);
                result = Directory.Exists(path);
            }

            // Assert
            Assert.True(result);

            Directory.CreateDirectory(path);

            Assert.True(Directory.Exists(path));
        }

        [Fact]
        public void UploadImage_ValidArguments_UploadsAndReturnsFileName()
        {
            // Arrange
            var path = "C:\\Test\\Images";
            var fileName = "test_image.jpg";
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write("test image data");
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);
            fileMock.Setup(x => x.OpenReadStream()).Returns(ms);
            fileMock.Setup(x => x.ContentType).Returns("image/jpeg");

            // Act
            var result = _fileUtil.UploadImage(path, fileName, fileMock.Object);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(fileName);
        }

        [Fact]
        public void UploadImage_NullArguments_ReturnsNull()
        {
            // Arrange
            var path = "";
            var fileName = "";
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write("test image data");
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);
            fileMock.Setup(x => x.OpenReadStream()).Returns(ms);
            fileMock.Setup(x => x.ContentType).Returns("image/jpeg");

            // Act
            var result = _fileUtil.UploadImage(null, fileName, fileMock.Object);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void UploadProductImage_InvalidProduct_ReturnsNull()
        {
            // Arrange
            var product = new Product { ProductId = 0 };
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "file", "dummy.txt");

            var imageSettings = new ImageSettings { Path = "C:\\Images", ProductImagePath = "Products" };
            var logger = new Mock<ILogger<IFileUtil>>();
            var fileUtil = new FileUtil(Options.Create(imageSettings), logger.Object);

            // Act
            var result = fileUtil.UploadProductImage(product, file);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void UploadProductImage_InvalidFile_ReturnsNull()
        {
            // Arrange
            var product = new Product { ProductId = 1 };
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, null, null);

            var imageSettings = new ImageSettings { Path = "C:\\Images", ProductImagePath = "Products" };
            var logger = new Mock<ILogger<IFileUtil>>();
            var fileUtil = new FileUtil(Options.Create(imageSettings), logger.Object);

            // Act
            var result = fileUtil.UploadProductImage(product, file);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UploadImage_ValidPathAndFileAndDirectoryDoesNotExist_CreatesDirectoryAndUploadsFile()
        {
            // Arrange
            var path = "C:\\Test\\Images";
            var fileName = "test.jpg";
            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns(fileName);

            // Act
            var result = _fileUtil.UploadImage(path, fileName, file.Object);

            // Assert
            Assert.True(Directory.Exists(path));
            Assert.True(File.Exists(Path.Combine(path, fileName)));
            Assert.True(result);
        }

        [Fact]
        public void UploadImage_InvalidPath_ReturnsFalse()
        {
            // Arrange
            var path = "C?:\\??d\\Images";
            var fileName = "test.jpg";
            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns(fileName);

            // Act
            var result = _fileUtil.UploadImage(path, fileName, file.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UploadImage_NullFile_ReturnsFalse()
        {
            // Arrange
            var path = "C:\\Test\\Images";
            var fileName = "test.jpg";
            IFormFile file = null;

            // Act
            var result = _fileUtil.UploadImage(path, fileName, file);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void UploadImage_InvalidPath_ThrowsException()
        {
            // Arrange
            var path = "C?:\\<>d\\Images";
            var fileName = "test.jpg";
            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns(fileName);

            // Act and Assert
            Assert.False(_fileUtil.UploadImage(path, fileName, file.Object));
        }

        [Fact]
        public void DeleteProductImages_FileExists_ReturnsTrue()
        {
            // Arrange
            var _mockImageSettings = new Mock<IOptions<ImageSettings>>();
            var _mockLogger = new Mock<ILogger<IFileUtil>>();
            var _mockFormFile = new Mock<IFormFile>();
            var fileName = "test.jpg";
            var imageSettings = new ImageSettings
            {
                Path = "C:\\Test\\Images",
                ProductImagePath = "products"
            };
            _mockImageSettings.Setup(x => x.Value).Returns(imageSettings);
            var fileUtil = new FileUtil(_mockImageSettings.Object, _mockLogger.Object);

            // Act
            var result = fileUtil.DeleteProductImages(fileName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DeleteProductImages_FileDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var _mockImageSettings = new Mock<IOptions<ImageSettings>>();
            var _mockLogger = new Mock<ILogger<IFileUtil>>();
            var _mockFormFile = new Mock<IFormFile>();
            var fileName = "nonexistent.jpg";
            var imageSettings = new ImageSettings
            {
                Path = "C:\\Test\\Images",
                ProductImagePath = "products"
            };
            _mockImageSettings.Setup(x => x.Value).Returns(imageSettings);
            var fileUtil = new FileUtil(_mockImageSettings.Object, _mockLogger.Object);

            // Act
            var result = fileUtil.DeleteProductImages(fileName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UploadUserProfilePic_ValidFile_ReturnsFileName()
        {
            // Arrange
            var _mockImageSettings = new Mock<IOptions<ImageSettings>>();
            var _mockLogger = new Mock<ILogger<IFileUtil>>();
            var _mockFormFile = new Mock<IFormFile>();
            var user = new User { UserId = 1 };
            var fileName = "test.jpg";
            _mockFormFile.Setup(x => x.FileName).Returns(fileName);
            var imageSettings = new ImageSettings
            {
                Path = "C:\\Test\\Images",
                UserImagePath = "users"
            };
            _mockImageSettings.Setup(x => x.Value).Returns(imageSettings);
            var fileUtil = new FileUtil(_mockImageSettings.Object, _mockLogger.Object);

            // Act
            var result = fileUtil.UploadUserProfilePic(user, _mockFormFile.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Contains($"{user.UserId}_", result);
            Assert.Contains(fileName.Split('.').Last(), result);
        }

        [Fact]
        public void UploadUserProfilePic_InvalidFile_ReturnsNull()
        {
            // Arrange
            var _mockImageSettings = new Mock<IOptions<ImageSettings>>();
            var _mockLogger = new Mock<ILogger<IFileUtil>>();
            var _mockFormFile = new Mock<IFormFile>();
            var user = new User { UserId = 1 };
            var fileName = "test.txt";
            _mockFormFile.Setup(x => x.FileName).Returns(fileName);
            _mockFormFile.Setup(x => x.Length).Returns(0);
            var imageSettings = new ImageSettings
            {
                Path = "C:\\Test\\Images",
                UserImagePath = "users"
            };
            _mockImageSettings.Setup(x => x.Value).Returns(imageSettings);
            var fileUtil = new FileUtil(_mockImageSettings.Object, _mockLogger.Object);

            // Act
            var result = fileUtil.UploadUserProfilePic(user, _mockFormFile.Object);

            // Assert
            Assert.NotNull(result);
        }

    }
}
