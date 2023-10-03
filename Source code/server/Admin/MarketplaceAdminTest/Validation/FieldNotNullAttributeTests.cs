using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MarketplaceAdminTest.Validation
{

    public class FieldNotNullAttributeTests
    {
        private class TestModel
        {
            [FieldNotNull("First Name")]
            public string? FirstName { get; set; }

            [FieldNotNull("Last Name")]
            public string? LastName { get; set; }
        }

        [Fact]
        public void IsValid_WithNullValue_ReturnsFalse()
        {
            // Arrange
            var attribute = new FieldNotNullAttribute("Test Field");

            // Act
            bool result = attribute.IsValid(null);

            // Assert
            Assert.False(result);
            Assert.Equal("Test Field is Required", attribute.ErrorMessage);
        }

        [Fact]
        public void IsValid_WithEmptyStringValue_ReturnsFalse()
        {
            // Arrange
            var attribute = new FieldNotNullAttribute("Test Field");

            // Act
            bool result = attribute.IsValid("");

            // Assert
            Assert.False(result);
            Assert.Equal("Test Field is Required", attribute.ErrorMessage);
        }

        [Fact]
        public void IsValid_WithNonEmptyStringValue_ReturnsTrue()
        {
            // Arrange
            var attribute = new FieldNotNullAttribute("Test Field");

            // Act
            bool result = attribute.IsValid("Test Value");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_WithModelContainingNullValues_ReturnsFalse()
        {
            // Arrange
            var model = new TestModel { FirstName = null, LastName = null };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            bool result = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(result);
            Assert.Equal(2, results.Count);
            Assert.Equal("First Name is Required", results[0].ErrorMessage);
            Assert.Equal("Last Name is Required", results[1].ErrorMessage);
        }

        [Fact]
        public void IsValid_WithModelContainingEmptyStringValues_ReturnsFalse()
        {
            // Arrange
            var model = new TestModel { FirstName = "", LastName = "" };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            bool result = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(result);
            Assert.Equal(2, results.Count);
            Assert.Equal("First Name is Required", results[0].ErrorMessage);
            Assert.Equal("Last Name is Required", results[1].ErrorMessage);
        }

        [Fact]
        public void IsValid_WithModelContainingNonEmptyStringValues_ReturnsTrue()
        {
            // Arrange
            var model = new TestModel { FirstName = "John", LastName = "Doe" };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            bool result = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.True(result);
        }
    }

}
