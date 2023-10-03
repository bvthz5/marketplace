using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace MarketplaceAdminTest.Validation
{
    public class ValidImageAttributeTests
    {
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
