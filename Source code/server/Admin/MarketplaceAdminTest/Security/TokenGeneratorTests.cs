using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Security;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using MarketPlaceAdmin.Bussiness.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace MarketplaceAdminTest.Security
{
    public class TokenGeneratorTests
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ITokenGenerator _tokenGenerator;
        private const string _seperator = "#.#";

        public TokenGeneratorTests()
        {
            _jwtSettings = new JwtSettings
            {
                Key = "my-secret-key_with_strong_complexity",
                Issuer = "my_issuer",
                Audience = "my_audience",
                RefreshTokenExpiry = 30
            };

            var options = Substitute.For<IOptions<JwtSettings>>();
            options.Value.Returns(_jwtSettings);

            _tokenGenerator = new TokenGenerator(options);
        }

        [Fact]
        public void GenerateAccessToken_ReturnsValidToken()
        {
            // Arrange
            var admin = new Admin { AdminId = 1 };

            // Act
            var result = _tokenGenerator.GenerateAccessToken(admin.AdminId, true);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Value);
            Assert.True(result.Expiry - DateTime.Now.AddMinutes(60) < TimeSpan.FromMilliseconds(100));
        }

        [Fact]
        public void GenerateAccessToken_ReturnsValidToken2()
        {
            // Arrange
            var agent = new Agent { AgentId = 1 };

            // Act
            var result = _tokenGenerator.GenerateAccessToken(agent.AgentId, false);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Value);
            Assert.True(result.Expiry - DateTime.Now.AddMinutes(60) < TimeSpan.FromMilliseconds(100));
        }

        [Fact]
        public void GenerateRefreshToken_ValidAdmin_ReturnsTokenWithCorrectProperties()
        {
            // Arrange
            var admin = new Admin
            {
                AdminId = 1,
                Email = "admin@example.com",
                Password = "admin_password"
            };

            // Act
            var token = _tokenGenerator.GenerateRefreshToken(admin.AdminId, admin.Email, admin.Password);

            // Assert
            Assert.NotNull(token);
            Assert.NotNull(token.Value);
            Assert.NotEmpty(token.Value);
            Assert.True(token.Expiry - DateTime.Now.AddMinutes(60) < TimeSpan.FromMilliseconds(100));
        }

        [Fact]
        public void GenerateRefreshToken_ValidAdmin_ReturnsTokenSignedWithCorrectKey()
        {
            // Arrange
            var admin = new Admin
            {
                AdminId = 1,
                Email = "admin@example.com",
                Password = "Admin@123"
            };

            // Act
            var token = _tokenGenerator.GenerateRefreshToken(admin.AdminId, admin.Email, admin.Password);

            // Assert
            var refreshTokenBytes = Convert.FromBase64String(token.Value);
            var refreshTokenString = Encoding.Unicode.GetString(refreshTokenBytes);
            var refreshTokenParts = refreshTokenString.Split(_seperator);

            Assert.Equal(admin.AdminId.ToString(), refreshTokenParts[0]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{admin.Password}")),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience
            };

            var validatedToken = tokenHandler.ValidateToken(refreshTokenParts[1], validationParameters, out var _);
            Assert.NotNull(validatedToken);
        }


        [Fact]
        public void VerifyRefreshToken_ValidToken_ReturnsNewToken()
        {
            var admin = new Admin
            {
                AdminId = 1,
                Email = "admin@example.com",
                Password = "encoded_admin_password"
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{admin.Password}"));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,admin.Email),
            };

            var expiry = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpiry);

            var token = new JwtSecurityToken(_jwtSettings.Issuer,
              _jwtSettings.Audience,
               claims,
               expires: expiry,
               signingCredentials: credentials);

            var tokenData = new JwtSecurityTokenHandler().WriteToken(token);

            // Act
            var result = _tokenGenerator.VerifyRefreshToken(tokenData, admin);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(admin.AdminId + "#.#" + tokenData, Encoding.Unicode.GetString(Convert.FromBase64String(result.Value)));
            Assert.Equal(token.ValidTo.ToLocalTime(), result.Expiry);
        }

        [Fact]
        public void VerifyRefreshToken_InvalidToken_ThrowsException()
        {
            // Arrange
            var admin = new Admin
            {
                AdminId = 123,
                Email = "admin@example.com",
                Password = "password"
            };

            var tokenData = "invalid_token_data";

            // Act + Assert
            Assert.ThrowsAny<Exception>(() => _tokenGenerator.VerifyRefreshToken(tokenData, admin));
        }

        [Fact]
        public void VerifyRefreshToken_InvalidToken_ClaimNotPresent_ThrowsException()
        {
            // Arrange
            var admin = new Admin
            {
                AdminId = 1,
                Email = "admin@example.com",
                Password = "encoded_admin_password"
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{admin.Password}"));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpiry);

            var token = new JwtSecurityToken(_jwtSettings.Issuer,
              _jwtSettings.Audience,
               expires: expiry,
               signingCredentials: credentials);

            var tokenData = new JwtSecurityTokenHandler().WriteToken(token);

            // Act + Assert
            Assert.ThrowsAny<Exception>(() => _tokenGenerator.VerifyRefreshToken(tokenData, admin));
        }

        [Fact]
        public void VerifyRefreshToken_InvalidNameIdentifier_ThrowsException()
        {
            // Arrange
            var admin = new Admin
            {
                AdminId = 1,
                Email = "admin@example.com",
                Password = "encoded_admin_password"
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{admin.Password}"));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,"non_matching_email"),
            };

            var expiry = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpiry);

            var token = new JwtSecurityToken(_jwtSettings.Issuer,
              _jwtSettings.Audience,
               claims,
               expires: expiry,
               signingCredentials: credentials);

            var tokenData = new JwtSecurityTokenHandler().WriteToken(token);

            // Act + Assert
            Assert.Throws<SecurityTokenException>(() => _tokenGenerator.VerifyRefreshToken(tokenData, admin));
        }


        [Fact]
        public void GetAdminIdAndTokenData_ReturnsExpectedValues()
        {
            // Arrange
            int adminId = 1;
            string tokenData = "token_data";
            string data = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{adminId}{_seperator}{tokenData}"));

            // Act
            var result = _tokenGenerator.GetAdminIdAndTokenData(data);

            // Assert
            Assert.IsType<string[]>(result);
            Assert.True(result.Length == 2);
            Assert.True(int.TryParse(result[0], out int outAdminId));
            Assert.Equal(adminId, outAdminId);
            Assert.Equal(tokenData, result[1]);
        }

        // Agent Verify


        [Fact]
        public void Agent_VerifyRefreshToken_ValidToken_ReturnsNewToken()
        {
            var agent = new Agent
            {
                AgentId = 1,
                Email = "admin@example.com",
                Password = "encoded_admin_password"
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{agent.Password}"));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,agent.Email),
            };

            var expiry = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpiry);

            var token = new JwtSecurityToken(_jwtSettings.Issuer,
              _jwtSettings.Audience,
               claims,
               expires: expiry,
               signingCredentials: credentials);

            var tokenData = new JwtSecurityTokenHandler().WriteToken(token);

            // Act
            var result = _tokenGenerator.VerifyRefreshTokenAgent(tokenData, agent);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(agent.AgentId + "#.#" + tokenData, Encoding.Unicode.GetString(Convert.FromBase64String(result.Value)));
            Assert.Equal(token.ValidTo.ToLocalTime(), result.Expiry);
        }

        [Fact]
        public void Agent_VerifyRefreshToken_InvalidToken_ThrowsException()
        {
            // Arrange
            var agent = new Agent
            {
                AgentId = 123,
                Email = "agent@example.com",
                Password = "password"
            };

            var tokenData = "invalid_token_data";

            // Act + Assert
            Assert.ThrowsAny<Exception>(() => _tokenGenerator.VerifyRefreshTokenAgent(tokenData, agent));
        }

        [Fact]
        public void Agent_VerifyRefreshToken_InvalidToken_ClaimNotPresent_ThrowsException()
        {
            // Arrange
            var agent = new Agent
            {
                AgentId = 1,
                Email = "agent@example.com",
                Password = "encoded_admin_password"
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{agent.Password}"));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpiry);

            var token = new JwtSecurityToken(_jwtSettings.Issuer,
              _jwtSettings.Audience,
               expires: expiry,
               signingCredentials: credentials);

            var tokenData = new JwtSecurityTokenHandler().WriteToken(token);

            // Act + Assert
            Assert.ThrowsAny<Exception>(() => _tokenGenerator.VerifyRefreshTokenAgent(tokenData, agent));
        }

        [Fact]
        public void Agent_VerifyRefreshToken_InvalidNameIdentifier_ThrowsException()
        {
            // Arrange
            var agent = new Agent
            {
                AgentId = 1,
                Email = "agent@example.com",
                Password = "encoded_admin_password"
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{agent.Password}"));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,"non_matching_email"),
            };

            var expiry = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpiry);

            var token = new JwtSecurityToken(_jwtSettings.Issuer,
              _jwtSettings.Audience,
               claims,
               expires: expiry,
               signingCredentials: credentials);

            var tokenData = new JwtSecurityTokenHandler().WriteToken(token);

            // Act + Assert
            Assert.Throws<SecurityTokenException>(() => _tokenGenerator.VerifyRefreshTokenAgent(tokenData, agent));
        }
    }
}