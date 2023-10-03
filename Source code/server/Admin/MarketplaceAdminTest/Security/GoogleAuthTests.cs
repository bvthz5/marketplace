using Google.Apis.Auth;
using MarketPlaceAdmin.Bussiness.Security;
using MarketPlaceAdmin.Bussiness.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace MarketplaceAdminTest.Security
{
    public class GoogleAuthTests
    {
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly ILogger<GoogleAuth> _logger;
        private readonly IOptions<GoogleAuthSettings> _googleAuthSettingsOptions;
        private readonly GoogleAuth _googleAuth;

        public GoogleAuthTests()
        {
            _googleAuthSettings = new GoogleAuthSettings { ClientId = "1234567890.apps.googleusercontent.com" };
            _logger = Substitute.For<ILogger<GoogleAuth>>();
            _googleAuthSettingsOptions = Options.Create(_googleAuthSettings);
            _googleAuth = new GoogleAuth(_googleAuthSettingsOptions, _logger);
        }

        [Fact]
        public async Task VerifyGoogleToken_InvalidToken_ReturnsNull()
        {
            // Arrange
            var idToken = "invalid_token";
            GoogleJsonWebSignature.Payload? result = null;

            // Act
            result = await _googleAuth.VerifyGoogleToken(idToken);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task VerifyGoogleToken_ThrowsException_ReturnsNull()
        {
            // Arrange
            var idToken = "valid_token";
            GoogleJsonWebSignature.Payload? result = null;


            // Act
            result = await _googleAuth.VerifyGoogleToken(idToken);

            // Assert
            Assert.Null(result);
        }
    }
}
