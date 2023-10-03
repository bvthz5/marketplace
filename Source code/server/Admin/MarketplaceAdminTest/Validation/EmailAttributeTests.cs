using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;
using Xunit;

namespace MarketplaceAdminTest.Validation
{
    public class EmailAttributeTests
    {
        [Fact]
        public void IsValid_ValidEmail_ReturnsTrue()
        {
            // Arrange
            var attribute = new EmailAttribute();
            var email = "johndoe@example.com";

            // Act
            var result = attribute.IsValid(email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_EmptyEmail_ReturnsFalse()
        {
            // Arrange
            var attribute = new EmailAttribute();
            var email = "";

            // Act
            var result = attribute.IsValid(email);

            // Assert
            Assert.False(result);
            Assert.Equal("Email is Required", attribute.ErrorMessage);
        }

        [Fact]
        public void IsValid_NullEmail_ReturnsFalse()
        {
            // Arrange
            var attribute = new EmailAttribute();
            string? email = null;

            // Act
            var result = attribute.IsValid(email);

            // Assert
            Assert.False(result);
            Assert.Equal("Email is Required", attribute.ErrorMessage);
        }

        [Fact]
        public void IsValid_InvalidEmail_ReturnsFalse()
        {
            // Arrange
            var attribute = new EmailAttribute();
            var email = "invalidemail";

            // Act
            var result = attribute.IsValid(email);

            // Assert
            Assert.False(result);
            Assert.Equal("Not a valid Email Address", attribute.ErrorMessage);
        }

        [Fact]
        public void IsValid_LongEmail_ReturnsFalse()
        {
            // Arrange
            var attribute = new EmailAttribute();
            var email = "a".PadRight(256, 'a') + "@example.com";

            // Act
            var result = attribute.IsValid(email);

            // Assert
            Assert.False(result);
            Assert.Equal("Email Length should be less than 255", attribute.ErrorMessage);
        }

    }
}
