using MarketPlaceUser.Bussiness.Security;
using MarketPlaceUser.Bussiness.Settings;
using Microsoft.Extensions.Options;

namespace MarketPlaceUserTest.Security
{
    public class GoogleAuthTest
    {
        private readonly GoogleAuthSettings _googleAuthSettings = new GoogleAuthSettings { ClientId = "test_client_id" };
        private readonly GoogleAuth _googleAuth;

        public GoogleAuthTest()
        {
            var options = Options.Create(_googleAuthSettings);
            _googleAuth = new GoogleAuth(options);
        }


        [Fact]
        public async Task VerifyGoogleToken_InvalidToken_ReturnsNull()
        {
            // Arrange
            var idToken = "invalid_token";

            // Act
            var result = await _googleAuth.VerifyGoogleToken(idToken);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task VerifyGoogleToken_NullToken_ReturnsNull()
        {
            // Arrange
            string idToken = null;

            // Act
            var result = await _googleAuth.VerifyGoogleToken(idToken);

            // Assert
            Assert.Null(result);
        }
    }
}
