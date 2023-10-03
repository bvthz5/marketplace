using MarketPlaceUser.Bussiness.Dto.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlaceUserTest.Validations.Form
{
    public class SuccessMessageViewTests
    {
        [Fact]
        public void Constructor_WithMessage_SetsMessageProperty()
        {
            // Arrange
            string expectedMessage = "Test message";

            // Act
            SuccessMessageView view = new SuccessMessageView(expectedMessage);

            // Assert
            Assert.Equal(expectedMessage, view.Message);
        }
    }
}
