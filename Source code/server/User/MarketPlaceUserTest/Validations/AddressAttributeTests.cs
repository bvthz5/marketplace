using MarketPlaceUser.Bussiness.Dto.Forms.Validations;

namespace MarketPlaceUserTest.Validations
{
    public class AddressAttributeTests
    {
        [Fact]
        public void IsValid_ValidAddress_ReturnsTrue()
        {
            // Arrange
            var attribute = new AddressAttribute();
            var validAddress = "123 Main St, Anytown USA";

            // Act
            var isValid = attribute.IsValid(validAddress);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsValid_NullValue_ReturnsFalse()
        {
            // Arrange
            var attribute = new AddressAttribute();
            string nullValue = null;

            // Act
            var isValid = attribute.IsValid(nullValue);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsValid_EmptyString_ReturnsFalse()
        {
            // Arrange
            var attribute = new AddressAttribute();
            var emptyString = "";

            // Act
            var isValid = attribute.IsValid(emptyString);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsValid_InvalidCharacters_ReturnsFalse()
        {
            // Arrange
            var attribute = new AddressAttribute();
            var invalidAddress = "123 Main St., *";

            // Act
            var isValid = attribute.IsValid(invalidAddress);

            // Assert
            Assert.False(isValid);
        }
    }

}
