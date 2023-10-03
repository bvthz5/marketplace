using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;
using Xunit;

namespace MarketplaceAdminTest.Validation
{

    public class PasswordAttributeTests
    {
        [Fact]
        public void IsValid_ReturnsFalse_WhenValueIsNull()
        {
            // Arrange
            var attribute = new PasswordAttribute();

            // Act
            var result = attribute.IsValid(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenValueIsEmpty()
        {
            // Arrange
            var attribute = new PasswordAttribute();

            // Act
            var result = attribute.IsValid(string.Empty);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenValueLengthIsLessThan8()
        {
            // Arrange
            var attribute = new PasswordAttribute();

            // Act
            var result = attribute.IsValid("Abcdefg");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenValueLengthIsMoreThan16()
        {
            // Arrange
            var attribute = new PasswordAttribute();

            // Act
            var result = attribute.IsValid("Abcdefghijklmnopqr");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenValueDoesNotContainUppercaseLetter()
        {
            // Arrange
            var attribute = new PasswordAttribute();

            // Act
            var result = attribute.IsValid("abcdefg1$");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenValueDoesNotContainLowercaseLetter()
        {
            // Arrange
            var attribute = new PasswordAttribute();

            // Act
            var result = attribute.IsValid("ABCDEFG1$");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenValueDoesNotContainNumber()
        {
            // Arrange
            var attribute = new PasswordAttribute();

            // Act
            var result = attribute.IsValid("Abcdefgh$");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenValueDoesNotContainSpecialCharacter()
        {
            // Arrange
            var attribute = new PasswordAttribute();

            // Act
            var result = attribute.IsValid("Abcdefgh1");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ReturnsTrue_WhenValueIsValidPassword()
        {
            // Arrange
            var attribute = new PasswordAttribute();

            // Act
            var result = attribute.IsValid("Abcd1234$");

            // Assert
            Assert.True(result);
        }
    }

}
