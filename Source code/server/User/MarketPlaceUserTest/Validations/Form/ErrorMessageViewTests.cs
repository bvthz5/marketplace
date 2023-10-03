using MarketPlaceUser.Bussiness.Dto.Views;

namespace MarketPlaceUserTest.Validations.Form
{
    public class ErrorMessageViewTests
    {
        [Fact]
        public void ErrorMessageView_SetErrorMessage_PropertyIsSet()
        {
            // Arrange
            string expectedErrorMessage = "Error!";

            // Act
            ErrorMessageView errorMessageView = new(expectedErrorMessage);

            // Assert
            Assert.Equal(expectedErrorMessage, errorMessageView.ErrorMessage);
        }

        [Fact]
        public void ErrorMessageView_NoErrorMessageProvided_PropertyIsEmpty()
        {
            // Arrange

            // Act
            ErrorMessageView errorMessageView = new("");

            // Assert
            Assert.Equal("", errorMessageView.ErrorMessage);
        }

        [Fact]
        public void ErrorMessageView_NoErrorMessageProvided_PropertyIsNotNull()
        {
            // Arrange

            // Act
            ErrorMessageView errorMessageView = new("");

            // Assert
            Assert.NotNull(errorMessageView.ErrorMessage);
        }

    }
}
