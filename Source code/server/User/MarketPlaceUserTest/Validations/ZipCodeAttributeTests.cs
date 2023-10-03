using MarketPlaceUser.Bussiness.Dto.Forms.Validations;

namespace MarketPlaceUserTest.Validations
{
    public class ZipCodeAttributeTests
    {
        [Fact]
        public void IsValid_ValidZipCode_ReturnsTrue()
        {
            // Arrange
            var attribute = new ZipCodeAttribute();
            var zipCode = "123456";

            // Act
            var result = attribute.IsValid(zipCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsInvalid_ValidZipCode_ReturnsFalse()
        {
            // Arrange
            var attribute = new ZipCodeAttribute();
            var zipCode = "1234567";

            // Act
            var result = attribute.IsValid(zipCode);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void IsInvalid_NullZipCode_ReturnsFalse()
        {
            // Arrange
            var attribute = new ZipCodeAttribute();

            string? zipCode = null;

            // Act
            var result = attribute.IsValid(zipCode);

            // Assert
            Assert.False(result);
        }
    }
}
