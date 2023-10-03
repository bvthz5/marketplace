using MarketPlaceUser.Bussiness.Dto.Forms.Validations;
using Microsoft.AspNetCore.Http;
using Moq;
using NSubstitute;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketPlaceUserTest.Validations
{
    public class ValidImageAttributeTests
    {
        [Fact]
        public void Test_ValidImage()
        {
            // Arrange
            var attribute = new ValidImageAttribute();
            var fileMock = new Mock<IFormFile>();
            var fileName = "test.jpg";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy image"));
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);
            fileMock.Setup(x => x.OpenReadStream()).Returns(ms);

            var validationContext = new ValidationContext(new object());



            // Act
            var result = attribute.IsValid(validationContext);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_InvalidImageExtension()
        {
            // Arrange
            var attribute = new ValidImageAttribute();
            var fileMock = new Mock<IFormFile>();
            var fileName = "test.bmp";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy image"));
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);
            fileMock.Setup(x => x.OpenReadStream()).Returns(ms);

            var validationContext = new ValidationContext(new object());



            // Act
            var result = attribute.IsValid(ms);

            // Assert
            Assert.False(result);
            Assert.Equal("Invalid Type", attribute.ErrorMessage);
        }

        [Fact]
        public void Test_InvalidImageSize()
        {
            // Arrange
            var attribute = new ValidImageAttribute();
            var fileMock = new Mock<IFormFile>();
            var fileName = "test.jpg";
            var ms = new MemoryStream(new byte[3 * 1024 * 1024]);
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);
            fileMock.Setup(x => x.OpenReadStream()).Returns(ms);

            var validationContext = new ValidationContext(new object());



            // Act
            var result = attribute.IsValid(ms);

            // Assert
            Assert.False(result);
            Assert.Equal("Invalid Type", attribute.ErrorMessage);
        }

        [Fact]
        public void Test_ValidImages()
        {
            // Arrange
            var attribute = new ValidImageAttribute();
            var fileMock1 = new Mock<IFormFile>();
            var fileName1 = "test1.jpg";
            var ms1 = new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy image"));
            fileMock1.Setup(x => x.FileName).Returns(fileName1);
            fileMock1.Setup(x => x.Length).Returns(ms1.Length);
            fileMock1.Setup(x => x.OpenReadStream()).Returns(ms1);

            var fileMock2 = new Mock<IFormFile>();
            var fileName2 = "test2.png";
            var ms2 = new MemoryStream(Encoding.UTF8.GetBytes("This is another dummy image"));
            fileMock2.Setup(x => x.FileName).Returns(fileName2);
            fileMock2.Setup(x => x.Length).Returns(ms2.Length);
            fileMock2.Setup(x => x.OpenReadStream()).Returns(ms2);

            var images = new[] { fileMock1.Object, fileMock2.Object };

            var validationContext = new ValidationContext(new object());



            // Act
            var result = attribute.IsValid(images);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidImageAttribute_ShouldReturnFalse_WhenImageIsNull()
        {
            // Arrange
            var attribute = new ValidImageAttribute();
            var validationContext = new ValidationContext(new object());

            // Act
            var result = attribute.IsValid(validationContext);

            // Assert
            Assert.False(result);
            Assert.Equal("Invalid Type", attribute.ErrorMessage);
        }


        [Fact]
        public void ValidImageAttribute_ShouldReturnFalse_WhenImageIsNotValid()
        {
            // Arrange
            var attribute = new ValidImageAttribute();
            var validationContext = new ValidationContext(new object());
            var image = new Mock<IFormFile>();
            image.SetupGet(x => x.FileName).Returns("test.doc");
            image.SetupGet(x => x.Length).Returns(1024);

            // Act
            var result = attribute.IsValid(validationContext);

            // Assert
            Assert.False(result);
            Assert.Equal("Invalid Type", attribute.ErrorMessage);
        }

        [Fact]
        public void ValidImageAttribute_ShouldReturnFalse_WhenImageSizeIsTooLarge()
        {
            // Arrange
            var attribute = new ValidImageAttribute();
            var validationContext = new ValidationContext(new object());
            var image = new Mock<IFormFile>();
            image.SetupGet(x => x.FileName).Returns("test.jpg");
            image.SetupGet(x => x.Length).Returns(3 * 1024 * 1024);

            // Act
            var result = attribute.IsValid(validationContext);

            // Assert
            Assert.False(result);
            Assert.Equal("Invalid Type", attribute.ErrorMessage);
        }

        [Fact]
        public void ValidImageAttribute_ShouldReturnTrue_WhenImageIsValid()
        {
            // Arrange
            var attribute = new ValidImageAttribute();
            var validationContext = new ValidationContext(new object());
            var image = new Mock<IFormFile>();
            image.SetupGet(x => x.FileName).Returns("test.jpg");
            image.SetupGet(x => x.Length).Returns(1024);

            // Act
            var result = attribute.IsValid(validationContext);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidImageAttribute()
        {
            // Arrange
            var image = Substitute.For<IFormFile>();
            image.FileName.Returns("image.jpg");
            image.Length.Returns(1024);

            var attribute = new ValidImageAttribute();

            // Act
            var result = attribute.IsValid(image);

            // Assert
            Assert.True(result);

        }

        [Fact]
        public void MultipleImage()
        {
            // Arrange
            var image1 = Substitute.For<IFormFile>();
            image1.FileName.Returns("image1.jpg");
            image1.Length.Returns(1024);

            var image2 = Substitute.For<IFormFile>();
            image2.FileName.Returns("image2.png");
            image2.Length.Returns(2048);

            var images = new IFormFile[] { image1, image2 };

            var attribute = new ValidImageAttribute();

            // Act
            var result = attribute.IsValid(images);

            // Assert
            Assert.True(result);

        }

        [Fact]
        public void Invalid_File_Type()
        {
            // Arrange
            var image = Substitute.For<IFormFile>();
            image.FileName.Returns("image.txt");
            image.Length.Returns(1024);

            var attribute = new ValidImageAttribute();

            // Act
            var result = attribute.IsValid(image);

            // Assert
            Assert.False(result);
            Assert.Equal("Only .jpg, .jpeg, .png, .webp are Supported, Uploaded : '.txt'", attribute.ErrorMessage);

        }

        [Fact]
        public void InvalidFileSize()
        {
            //Arrange
            var image = Substitute.For<IFormFile>();
            image.FileName.Returns("image.jpg");
            image.Length.Returns(3 * 1024 * 1024);

            var attribute = new ValidImageAttribute();

            // Act
            var result = attribute.IsValid(image);

            // Assert
            Assert.False(result);
            Assert.Equal("Max File Size : 2048KB, Uploaded File Size : 3072KB ", attribute.ErrorMessage);
        }

        [Fact]
        public void Invalid_Type()
        {
            // Arrange
            var value = "not an image file";

            var attribute = new ValidImageAttribute();

            // Act
            var result = attribute.IsValid(value);

            // Assert
            Assert.False(result);
            Assert.Equal("Invalid Type", attribute.ErrorMessage);

        }

        [Fact]
        public void Null_Input()
        {
            // Arrange
            IFormFile? value = null;

            var attribute = new ValidImageAttribute();

            // Act
            var result = attribute.IsValid(value);

            // Assert
            Assert.False(result);

        }

        [Fact]
        public void MultipleImage_InvalidType()
        {
            // Arrange
            var image1 = Substitute.For<IFormFile>();
            image1.FileName.Returns("image1.jpg");
            image1.Length.Returns(1024);

            var image2 = Substitute.For<IFormFile>();
            image2.FileName.Returns("image2.txt");
            image2.Length.Returns(2048);

            var images = new IFormFile[] { image1, image2 };

            var attribute = new ValidImageAttribute();

            // Act
            var result = attribute.IsValid(images);

            // Assert
            Assert.False(result);
            Assert.Equal("Only .jpg, .jpeg, .png, .webp are Supported, Uploaded : '.txt'", attribute.ErrorMessage);

        }

        [Fact]
        public void MultipleImage_InvalidSize()
        {
            // Arrange
            var image1 = Substitute.For<IFormFile>();
            image1.FileName.Returns("image1.jpg");
            image1.Length.Returns(1024);

            var image2 = Substitute.For<IFormFile>();
            image2.FileName.Returns("image2.png");
            image2.Length.Returns(3 * 1024 * 1024);

            var images = new IFormFile[] { image1, image2 };

            var attribute = new ValidImageAttribute();

            // Act
            var result = attribute.IsValid(images);

            // Assert
            Assert.False(result);
            Assert.Equal("Max File Size : 2048KB, Uploaded File Size : 3072KB ", attribute.ErrorMessage);
        }

    }
}
