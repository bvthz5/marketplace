using MarketPlaceUser.Bussiness.Dto.Forms.Validations;

namespace MarketPlaceUserTest.Validations
{
    public class PlaceAttributeTests
    {

        [Fact]
        public void PlaceAttribute_ValidPlace_ReturnsTrue()
        {
            // Arrange
            var placeAttribute = new PlaceAttribute();
            var place = "123 Market Street, San Francisco, CA";

            // Act
            var result = placeAttribute.IsValid(place);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void PlaceAttribute_PlaceContainsSpecialCharacter_ReturnsFalse()
        {
            // Arrange
            var placeAttribute = new PlaceAttribute();
            var place = "123 Market Street, San Francisco, CA !";

            // Act
            var result = placeAttribute.IsValid(place);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void PlaceAttribute_NullValue_ReturnsFalse()
        {
            // Arrange
            var placeAttribute = new PlaceAttribute();
            string place = null;

            // Act
            var result = placeAttribute.IsValid(place);

            // Assert
            Assert.False(result);
        }
    }

}

