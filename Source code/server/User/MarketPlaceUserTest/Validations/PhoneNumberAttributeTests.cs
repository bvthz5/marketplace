using MarketPlaceUser.Bussiness.Dto.Forms.Validations;

namespace MarketPlaceUserTest.Validations
{
    public class PhoneNumberAttributeTests
    {
        [Fact]
        public void PhoneNumberAttribute_ValidPhoneNumber_ReturnsTrue()
        {
            // Arrange
            var phoneNumberAttribute = new PhoneNumberAttribute();
            var phoneNumber = "1234567890";
            // Act
            var result = phoneNumberAttribute.IsValid(phoneNumber);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void PhoneNumberAttribute_NullablePhoneNumber_ReturnsTrue()
        {
            // Arrange
            var phoneNumberAttribute = new PhoneNumberAttribute() { Nullable = true };
            string phoneNumber = null;

            // Act
            var result = phoneNumberAttribute.IsValid(phoneNumber);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void PhoneNumberAttribute_EmptyPhoneNumber_ReturnsFalse()
        {
            // Arrange
            var phoneNumberAttribute = new PhoneNumberAttribute();
            var phoneNumber = string.Empty;

            // Act
            var result = phoneNumberAttribute.IsValid(phoneNumber);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void PhoneNumberAttribute_InvalidPhoneNumber_ReturnsFalse()
        {
            // Arrange
            var phoneNumberAttribute = new PhoneNumberAttribute();
            var phoneNumber = "12345";

            // Act
            var result = phoneNumberAttribute.IsValid(phoneNumber);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void PhoneNumberAttribute_NullValue_ReturnsFalseWithCorrectErrorMessage()
        {
            // Arrange
            var attribute = new PhoneNumberAttribute();

            // Act
            var result = attribute.IsValid(null);

            // Assert
            Assert.False(result);
            Assert.Equal("Phone Number Required", attribute.ErrorMessage);
        }


    }
}
