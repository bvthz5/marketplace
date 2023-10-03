using MarketPlaceUser.Bussiness.Dto.Forms.Validations;

namespace MarketPlaceUserTest.Validations
{
    public class LoginFieldAttributeTests
    {
        [Fact]
        public void LoginFieldAttribute_ValueIsNull_ReturnsFalseWithError()
        {
            // Arrange
            var fieldName = "Username";
            var loginFieldAttribute = new LoginFieldAttribute(fieldName);
            string value = null;

            // Act
            var result = loginFieldAttribute.IsValid(value);

            // Assert
            Assert.False(result);
            Assert.Equal($"Username is Required", loginFieldAttribute.ErrorMessage);
        }

        [Fact]
        public void LoginFieldAttribute_ValueIsEmpty_ReturnsFalseWithError()
        {
            // Arrange
            var fieldName = "Password";
            var loginFieldAttribute = new LoginFieldAttribute(fieldName);
            var value = "";

            // Act
            var result = loginFieldAttribute.IsValid(value);

            // Assert
            Assert.False(result);
            Assert.Equal($"Password is Required", loginFieldAttribute.ErrorMessage);
        }

        [Fact]
        public void LoginFieldAttribute_ValueIsValid_ReturnsTrue()
        {
            // Arrange
            var fieldName = "Username";
            var loginFieldAttribute = new LoginFieldAttribute(fieldName);
            var value = "john123";

            // Act
            var result = loginFieldAttribute.IsValid(value);

            // Assert
            Assert.True(result);
        }

    }
}
