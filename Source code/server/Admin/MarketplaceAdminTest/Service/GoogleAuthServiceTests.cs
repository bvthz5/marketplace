using Google.Apis.Auth;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Security;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using MarketPlaceAdmin.Bussiness.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace MarketplaceAdminTest.Service
{
    public class GoogleAuthServiceTests
    {
        private readonly IGoogleAuth _googleAuth;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger<GoogleAuthService> _logger;

        private readonly GoogleAuthService _googleAuthService;

        public GoogleAuthServiceTests()
        {
            _googleAuth = Substitute.For<IGoogleAuth>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _tokenGenerator = Substitute.For<ITokenGenerator>();
            _logger = Substitute.For<ILogger<GoogleAuthService>>();

            _googleAuthService = new GoogleAuthService(_googleAuth, _unitOfWork, _tokenGenerator, _logger);
        }

        [Fact]
        public async Task Login_ValidIdToken_ReturnsSuccessResult()
        {
            // Arrange
            var idToken = "valid_id_token";
            var email = "test@example.com";
            var admin = new Admin { Email = email };
            var accessToken = new Token("access_token", DateTime.Now);
            var refreshToken = new Token("refresh_token", DateTime.Now);
            _googleAuth.VerifyGoogleToken(idToken).Returns(new GoogleJsonWebSignature.Payload() { Email = email });
            _unitOfWork.AdminRepository.FindByEmail(email).Returns(admin);
            _tokenGenerator.GenerateAccessToken(admin.AdminId, true).Returns(accessToken);
            _tokenGenerator.GenerateRefreshToken(admin.AdminId, admin.Email, admin.Password).Returns(refreshToken);


            // Act
            var result = await _googleAuthService.Login(idToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Success", result.Message);
            Assert.NotNull(result.Data);
            Assert.IsType<LoginView>(result.Data);
            Assert.Equal(admin.Email, ((LoginView)result.Data).Email);
            Assert.Equal(accessToken.Value, ((LoginView)result.Data).AccessToken.Value);
            Assert.Equal(refreshToken.Value, ((LoginView)result.Data).RefreshToken.Value);
        }

        [Fact]
        public async Task Login_InvalidIdToken_ReturnsBadRequestResult()
        {
            // Arrange
            var idToken = "invalid_id_token";
            _googleAuth.VerifyGoogleToken(idToken).ReturnsNull();

            // Act
            var result = await _googleAuthService.Login(idToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task Login_AdminNotFound_ReturnsBadRequestResult()
        {
            // Arrange
            var idToken = "valid_id_token";
            var email = "test@example.com";
            _googleAuth.VerifyGoogleToken(idToken).Returns(new GoogleJsonWebSignature.Payload { Email = email });
            _unitOfWork.AdminRepository.FindByEmail(email).ReturnsNull();

            // Act
            var result = await _googleAuthService.Login(idToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.Null(result.Data);
        }

        // Agent Login

        [Fact]
        public async Task AgentLogin_InvalidIdToken_ReturnsBadRequestResult()
        {
            // Arrange
            var idToken = "invalid_id_token";
            _googleAuth.VerifyGoogleToken(idToken).ReturnsNull();

            // Act
            var result = await _googleAuthService.AgentLogin(idToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task ALogin_AdminNotFound_ReturnsBadRequestResult()
        {
            // Arrange
            var idToken = "valid_id_token";
            var email = "test@example.com";
            _googleAuth.VerifyGoogleToken(idToken).Returns(new GoogleJsonWebSignature.Payload { Email = email });
            _unitOfWork.AgentRepository.FindByEmailAsync(email).ReturnsNull();

            // Act
            var result = await _googleAuthService.AgentLogin(idToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Agent not found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task AgentLogin_BlockedAgent_ReturnsBadRequestResult()
        {
            // Arrange
            var idToken = "valid_token";

            Agent agent = new()
            {
                AgentId = 1,
                Name = "John Doe",
                Email = "johndoe@example.com",
                Password = "password",
                Status = Agent.DeliveryAgentStatus.BLOCKED
            };

            _googleAuth.VerifyGoogleToken(idToken).Returns(new GoogleJsonWebSignature.Payload
            {
                Email = "johndoe@example.com"
            });

            _unitOfWork.AgentRepository.FindByEmailAsync("johndoe@example.com").Returns(agent);


            // Act
            var result = await _googleAuthService.AgentLogin(idToken);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Agent BLOCKED", result.Message);
        }

        [Fact]
        public async Task AgentLogin_ValidIdToken_ReturnsSuccessResult()
        {
            // Arrange
            var idToken = "valid_id_token";
            var email = "test@example.com";
            var agent = new Agent { AgentId = 10, Email = email };
            var accessToken = new Token("access_token", DateTime.Now);
            var refreshToken = new Token("refresh_token", DateTime.Now);
            _googleAuth.VerifyGoogleToken(idToken).Returns(new GoogleJsonWebSignature.Payload() { Email = email });
            _unitOfWork.AgentRepository.FindByEmailAsync(email).Returns(agent);
            _tokenGenerator.GenerateAccessToken(agent.AgentId, false).Returns(accessToken);
            _tokenGenerator.GenerateRefreshToken(agent.AgentId, agent.Email, agent.Password).Returns(refreshToken);


            // Act
            var result = await _googleAuthService.AgentLogin(idToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Success", result.Message);
            Assert.NotNull(result.Data);
            Assert.IsType<AgentLoginView>(result.Data);
            Assert.Equal(agent.Email, ((AgentLoginView)result.Data).Email);
            Assert.Equal(accessToken.Value, ((AgentLoginView)result.Data).AccessToken.Value);
            Assert.Equal(refreshToken.Value, ((AgentLoginView)result.Data).RefreshToken.Value);
        }
    }
}

