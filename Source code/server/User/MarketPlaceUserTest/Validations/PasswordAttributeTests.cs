using MarketPlaceUser.Bussiness.Dto.Forms.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlaceUserTest.Validations
{
    public class PasswordAttributeTests
    {
        [Fact]
        public void PasswordAttribute_NullValue_ReturnsFalse()
        {
            // Arrange
            var passwordAttribute = new PasswordAttribute();

            // Act
            var result = passwordAttribute.IsValid(null);

            // Assert
            Assert.False(result);
            Assert.Equal("Password is Required", passwordAttribute.ErrorMessage);
        }

        [Fact]
        public void PasswordAttribute_EmptyValue_ReturnsFalse()
        {
            // Arrange
            var passwordAttribute = new PasswordAttribute();

            // Act
            var result = passwordAttribute.IsValid(string.Empty);

            // Assert
            Assert.False(result);
            Assert.Equal("Password is Required", passwordAttribute.ErrorMessage);
        }

        [Fact]
        public void PasswordAttribute_ShortValue_ReturnsFalse()
        {
            // Arrange
            var passwordAttribute = new PasswordAttribute();

            // Act
            var result = passwordAttribute.IsValid("pass");

            // Assert
            Assert.False(result);
            Assert.Equal("Password Length should be 8 - 16", passwordAttribute.ErrorMessage);
        }

        [Fact]
        public void PasswordAttribute_LongValue_ReturnsFalse()
        {
            // Arrange
            var passwordAttribute = new PasswordAttribute();
            var password = "password123456789"; // length 17

            // Act
            var result = passwordAttribute.IsValid(password);

            // Assert
            Assert.False(result);
            Assert.Equal("Password Length should be 8 - 16", passwordAttribute.ErrorMessage);
        }

        [Fact]
        public void PasswordAttribute_WithoutUppercase_ReturnsFalse()
        {
            // Arrange
            var passwordAttribute = new PasswordAttribute();
            var password = "password1!";

            // Act
            var result = passwordAttribute.IsValid(password);

            // Assert
            Assert.False(result);
            Assert.Equal("Must contain at least one uppercase letter, one lowercase letter, one number and one special character", passwordAttribute.ErrorMessage);
        }

        [Fact]
        public void PasswordAttribute_WithoutLowercase_ReturnsFalse()
        {
            // Arrange
            var passwordAttribute = new PasswordAttribute();
            var password = "PASSWORD1!";

            // Act
            var result = passwordAttribute.IsValid(password);

            // Assert
            Assert.False(result);
            Assert.Equal("Must contain at least one uppercase letter, one lowercase letter, one number and one special character", passwordAttribute.ErrorMessage);
        }

        [Fact]
        public void PasswordAttribute_WithoutNumber_ReturnsFalse()
        {
            // Arrange
            var passwordAttribute = new PasswordAttribute();
            var password = "Password!";

            // Act
            var result = passwordAttribute.IsValid(password);

            // Assert
            Assert.False(result);
            Assert.Equal("Must contain at least one uppercase letter, one lowercase letter, one number and one special character", passwordAttribute.ErrorMessage);
        }

        [Fact]
        public void PasswordAttribute_WithoutSpecialChar_ReturnsFalse()
        {
            // Arrange
            var passwordAttribute = new PasswordAttribute();
            var password = "Password1";

            // Act
            var result = passwordAttribute.IsValid(password);

            // Assert
            Assert.False(result);
            Assert.Equal("Must contain at least one uppercase letter, one lowercase letter, one number and one special character", passwordAttribute.ErrorMessage);
        }

        [Fact]
        public void PasswordAttribute_ValidValue_ReturnsTrue()
        {
            // Arrange
            var passwordAttribute = new PasswordAttribute();
            var password = "Passw0rd!";

            // Act
            var result = passwordAttribute.IsValid(password);

            // Assert
            Assert.True(result);
        }

    }
}
