using MarketPlaceAdmin.Bussiness.Settings;
using Xunit;

namespace MarketplaceAdminTest.Extra
{
    public class GoogleAuthSettingsTests
    {
        [Fact]
        public void Test_ClientId_Property_Set_Correctly()
        {
            // Arrange
            var authSettings = new GoogleAuthSettings();
            var clientId = "myclientid";

            // Act
            authSettings.ClientId = clientId;

            // Assert
            Assert.Equal(clientId, authSettings.ClientId);
        }
        [Fact]
        public void Test_ClientSecret_Property_Set_Correctly()
        {
            // Arrange
            var authSettings = new GoogleAuthSettings();
            var clientSecret = "myclientsecret";

            // Act
            authSettings.ClientSecret = clientSecret;

            // Assert
            Assert.Equal(clientSecret, authSettings.ClientSecret);
        }

        [Fact]

        public void Test_ClientId_Property_Throws_Exception_If_Not_Set()
        {
            // Arrange
            var authSettings = new GoogleAuthSettings();

            // Act
            var clientId = authSettings.ClientId;

            // Assert
            // Expect exception to be thrown
        }

        [Fact]

        public void Test_ClientSecret_Property_Throws_Exception_If_Not_Set()
        {
            // Arrange
            var authSettings = new GoogleAuthSettings();

            // Act
            var clientSecret = authSettings.ClientSecret;

            // Assert
            // Expect exception to be thrown
        }

    }
}
