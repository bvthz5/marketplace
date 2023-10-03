using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Security;
using MarketPlaceUser.Bussiness.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MarketPlaceUserTest.Security
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
            var user = new User { UserId = 1 };

            // Act
            var result = _tokenGenerator.GenerateAccessToken(user);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Value);
            Assert.True(result.Expiry - DateTime.Now.AddMinutes(60) < TimeSpan.FromMilliseconds(100));
        }

        [Fact]
        public void GenerateRefreshToken_ValidAdmin_ReturnsTokenWithCorrectProperties()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Email = "admin@example.com",
                Password = "admin_password"
            };

            // Act
            var token = _tokenGenerator.GenerateRefreshToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.NotNull(token.Value);
            Assert.NotEmpty(token.Value);
            Assert.True(token.Expiry - DateTime.Now.AddMinutes(60) < TimeSpan.FromMilliseconds(100));
        }

        [Fact]
        public void GenerateRefreshToken_ValidAdmin_ReturnsTokenSignedWithCorrectKey()
        {// Arrange
            var user = new User
            {
                UserId = 1,
                Email = "admin@example.com",
                Password = "admin_password"
            };

            // Act
            var token = _tokenGenerator.GenerateRefreshToken(user);

            // Assert
            var refreshTokenBytes = Convert.FromBase64String(token.Value);
            var refreshTokenString = Encoding.Unicode.GetString(refreshTokenBytes);
            var refreshTokenParts = refreshTokenString.Split(_seperator);

            Assert.Equal(user.UserId.ToString(), refreshTokenParts[0]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{user.Password}")),
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
            // Arrange
            var user = new User
            {
                UserId = 1,
                Email = "admin@example.com",
                Password = "admin_password"
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{user.Password}"));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Email),
            };

            var expiry = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpiry);

            var token = new JwtSecurityToken(_jwtSettings.Issuer,
              _jwtSettings.Audience,
               claims,
               expires: expiry,
               signingCredentials: credentials);

            var tokenData = new JwtSecurityTokenHandler().WriteToken(token);

            // Act
            var result = _tokenGenerator.VerifyRefreshToken(tokenData, user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.UserId+ "#.#" + tokenData, Encoding.Unicode.GetString(Convert.FromBase64String(result.Value)));
            Assert.Equal(token.ValidTo.ToLocalTime(), result.Expiry);
        }

        [Fact]
        public void VerifyRefreshToken_InvalidToken_ThrowsException()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Email = "admin@example.com",
                Password = "admin_password"
            };
            var tokenData = "invalid_token_data";

            // Act + Assert
            Assert.ThrowsAny<Exception>(() => _tokenGenerator.VerifyRefreshToken(tokenData, user));
        }

        [Fact]
        public void VerifyRefreshToken_InvalidToken_ClaimNotPresent_ThrowsException()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Email = "admin@example.com",
                Password = "admin_password"
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{user.Password}"));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpiry);

            var token = new JwtSecurityToken(_jwtSettings.Issuer,
              _jwtSettings.Audience,
               expires: expiry,
               signingCredentials: credentials);

            var tokenData = new JwtSecurityTokenHandler().WriteToken(token);

            // Act + Assert
            Assert.ThrowsAny<Exception>(() => _tokenGenerator.VerifyRefreshToken(tokenData, user));
        }

        [Fact]
        public void VerifyRefreshToken_InvalidNameIdentifier_ThrowsException()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Email = "admin@example.com",
                Password = "admin_password"
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{user.Password}"));

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
            Assert.Throws<SecurityTokenException>(() => _tokenGenerator.VerifyRefreshToken(tokenData, user));
        }


        [Fact]
        public void GetAdminIdAndTokenData_ReturnsExpectedValues()
        {
            // Arrange
            int userId = 1;
            string tokenData = "token_data";
            string data = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{userId}{_seperator}{tokenData}"));

            // Act
            var result = _tokenGenerator.GetUserIdAndTokenData(data);

            // Assert
            Assert.IsType<string[]>(result);
            Assert.True(result.Length == 2);
            Assert.True(int.TryParse(result[0], out int outAdminId));
            Assert.Equal(userId, outAdminId);
            Assert.Equal(tokenData, result[1]);
        }
    }
}