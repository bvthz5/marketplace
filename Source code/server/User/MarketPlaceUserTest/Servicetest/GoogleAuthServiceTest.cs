using Google.Apis.Auth;
using MarketPlace.DataAccess.Interfaces;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Security;
using MarketPlaceUser.Bussiness.Services;
using MarketPlaceUser.Bussiness.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MarketPlaceUserTest.Servicetest
{
    public class GoogleAuthServiceTest
    {
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly ILogger<GoogleAuth> _logger;
        private readonly IOptions<GoogleAuthSettings> _googleAuthSettingsOptions;
        private readonly GoogleAuth _googleAuth;
        private readonly GoogleAuthService _service;
        private readonly IUnitOfWork _uow;
        private readonly ITokenGenerator _token;



        public GoogleAuthServiceTest()
        {
            _googleAuthSettings = new GoogleAuthSettings { ClientId = "1234567890.apps.googleusercontent.com", ClientSecret = "bvgguud5xdhyu6vgcyf" };
            _logger = Substitute.For<ILogger<GoogleAuth>>();
            _googleAuthSettingsOptions = Options.Create(_googleAuthSettings);
            _googleAuth = new GoogleAuth(_googleAuthSettingsOptions);
            _uow = Substitute.For<IUnitOfWork>();
            _token = Substitute.For<ITokenGenerator>();
            _service = new GoogleAuthService(_googleAuth, _uow, _token);
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

        [Fact]
        public async Task RegisterAndLogin_InvalidToken_ReturnsBadRequestResult()
        {
            // Arrange
            var idToken = "invalid-token";
            await _googleAuth.VerifyGoogleToken(idToken).ConfigureAwait(false);

            // Act
            var result = await _service.RegisterAndLogin(idToken);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.Null(result.Data);
            Assert.False(result.Status);
        }


        [Fact]
        public async Task RegisterAndLogin_InvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var idToken = "invalid_id_token";

            await _googleAuth.VerifyGoogleToken(idToken);

            // Act
            var result = await _service.RegisterAndLogin(idToken);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            // Add more assertions as needed
        }

    }


}

