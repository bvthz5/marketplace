using MarketPlaceAdmin.Bussiness.Services;
using MarketPlaceAdmin.Bussiness.Settings;
using MarketPlaceAdmin.Bussiness.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using System.Text;
using Xunit;

namespace MarketplaceAdminTest.Utils
{
    public class FileUtilTests
    {
        private readonly FileUtil _fileUtil;
        private readonly ImageSettings _imageSettings;
        private readonly ILogger<FileUtil> _logger;


        public FileUtilTests()
        {

            _imageSettings = new ImageSettings()
            {
                Path = "./",
                AgentImagePath = "Agent",
                ProductImagePath = "Product",
                UserImagePath = "User"
            };

            _logger = Substitute.For<ILogger<FileUtil>>();

            _fileUtil = new FileUtil(Options.Create(_imageSettings), _logger);
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
        public void GetAgentProfile_Fail()
        {
            // Arrange
            var fileName = "Invalid_Filename.jpg";

            // Act
            var result = _fileUtil.GetAgentProfile(fileName);


            // Assert
            Assert.Null(result);
            _logger.ReceivedWithAnyArgs().LogError("Error");

        }

        [Fact]
        public void GetAgentProfile_Success()
        {
            // Arrange
            var fileName = "Valid_Filename.jpg";
            var path = Path.Combine(_imageSettings.Path, _imageSettings.AgentImagePath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (FileStream fs = File.Create(path + "/" + fileName))
            {
                byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            // Act
            var result = _fileUtil.GetAgentProfile(fileName);


            // Assert
            Assert.NotNull(result);

        }



        [Fact]
        public void UploadAgentProfilePic_WhenExceptionIsThrown()
        {
            // Arrange

            var agentId = 10;
            IFormFile file = Substitute.For<IFormFile>();

            // Act

            var result = _fileUtil.UploadAgentProfilePic(agentId, file);

            // Assert

            Assert.Null(result);

            _logger.ReceivedWithAnyArgs().LogError("Error");
        }
        [Fact]
        public void UploadAgentProfilePic_WhenExceptionIsThrown2()
        {
            // Arrange

            var agentId = 10;
            IFormFile file = Substitute.For<IFormFile>();

            file.FileName.Returns("ValidFilename.jpg");

            var fileUtil = new FileUtil(Options.Create(new ImageSettings()
            {
                Path = "",
                UserImagePath = "//q//"
            }), _logger);

            // Act
            fileUtil.UploadAgentProfilePic(agentId,file);


            


        }

        [Fact]
        public void UploadImage_Success()
        {
            // Arrange

            var agentId = 10;
            IFormFile file = Substitute.For<IFormFile>();

            file.FileName.Returns("ValidFilename.jpg");

            // Act

            var result = _fileUtil.UploadAgentProfilePic(agentId, file);

            // Assert

            Assert.NotNull(result);
            Assert.StartsWith(agentId.ToString(), result);
        }

        [Fact]
        public void DeleteAgentProfilePic_Success()
        {
            // Arrange
            var fileName = "Agent_ProfilePic_for_Delete.jpg";
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
            _fileUtil.DeleteAgentProfilePic(fileName);


            // Assert
            Assert.Null(_fileUtil.GetAgentProfile(fileName));
        }

    }
}
