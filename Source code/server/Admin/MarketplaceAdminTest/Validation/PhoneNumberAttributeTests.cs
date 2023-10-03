using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;
using Xunit;

namespace MarketplaceAdminTest.Validation
{
    public class PhoneNumberAttributeTests
    {
        private readonly PhoneNumberAttribute _attribute = new PhoneNumberAttribute();

        [Fact]
        public void IsValid_WithNullValue_ReturnsFalse()
        {
            // Arrange
            object? value = null;

            // Act
            var result = _attribute.IsValid(value);

            // Assert
            Assert.False(result);
            Assert.Equal("Phone Number Required", _attribute.ErrorMessage);
        }

        [Fact]
        public void IsValid_WithEmptyString_ReturnsFalse()
        {
            // Arrange
            object value = "";

            // Act
            var result = _attribute.IsValid(value);

            // Assert
            Assert.False(result);
            Assert.Equal("Phone Number Required", _attribute.ErrorMessage);
        }

        [Fact]
        public void IsValid_WithInvalidPhoneNumber_ReturnsFalse()
        {
            // Arrange
            object value = "123";

            // Act
            var result = _attribute.IsValid(value);

            // Assert
            Assert.False(result);
            Assert.Equal("Not a valid Phone Number", _attribute.ErrorMessage);
        }
    }

}
