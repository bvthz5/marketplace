using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using MarketPlaceAdmin.Bussiness.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MarketPlaceAdmin.Bussiness.Security
{
    /// <summary>
    /// The TokenGenerator class generates access and refresh tokens for a specific admin user, verifies the integrity of refresh tokens, and parses base64-encoded refresh tokens to return admin ID and JWT token.
    /// </summary>
    public class TokenGenerator : ITokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        private const string _seperator = "#.#";

        /// <summary>
        /// Initializes a new instance of the TokenGenerator class.
        /// </summary>
        /// <param name="jwtSettings">The JWT settings.</param>
        public TokenGenerator(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Generates an access token for a specific admin user.
        /// </summary>
        /// <param name="nameIdentifier"></param>
        /// <param name="isAdmin"></param>
        /// <returns>The access token as a Token object.</returns>
        public Token GenerateAccessToken(int nameIdentifier, bool isAdmin)
        {
            // Create a new security key from the JWT settings key and encode it with UTF-8.
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            // Create new signing credentials using the security key and the HmacSha256 algorithm.
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create an array of claims containing only the name identifier claim.
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,nameIdentifier.ToString()),
                new Claim(ClaimTypes.Role,isAdmin ? "Admin" : "Agent")
            };

            // Set the expiry time of the access token to the current time plus the access token expiry time from the JWT settings.
            var expiry = DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpiry);

            // Create a new JWT security token with the issuer, audience, claims, expiry, and signing credentials.
            var token = new JwtSecurityToken(_jwtSettings.Issuer,
               _jwtSettings.Audience,
                claims,
                expires: expiry,
                signingCredentials: credentials);

            // Return the access token as a Token object with its expiry time.
            return new Token(new JwtSecurityTokenHandler().WriteToken(token), expiry);
        }

        /// <summary>
        /// Generates a refresh token for a particular Admin.
        /// The token is signed using the key and the admin's password combination.
        /// The token expires after the refresh token expiry time in the appsettings.json file.
        /// If the admin changes their password, the token will also expire.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>The generated token.</returns>
        public Token GenerateRefreshToken(int id, string email, string password)
        {
            // Create a new SymmetricSecurityKey using the key and admin password combination.
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{password}"));

            // Create a new SigningCredentials using the SymmetricSecurityKey and the HmacSha256 algorithm.
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create an array of claims containing the admin's email.
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,email)
            };

            // Set the expiry time of the token to the current time plus the refresh token expiry time.
            var expiry = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpiry);

            // Create a new JwtSecurityToken using the appsettings.json values, the claims array,
            // the expiry time, and the SigningCredentials.
            var token = new JwtSecurityToken(_jwtSettings.Issuer,
               _jwtSettings.Audience,
                claims: claims,
                expires: expiry.ToUniversalTime(),
                signingCredentials: credentials);

            // Combine the admin ID and the token into a string and convert it to a Base64 string.
            string refreshToken = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{id}{_seperator}{new JwtSecurityTokenHandler().WriteToken(token)}"));

            // Return a new Token object containing the refresh token and the expiry time.
            return new Token(refreshToken, expiry);
        }

        /// <summary>
        /// Verify the integrity of a refresh token (JWT token) and return a new Token object.
        /// </summary>
        /// <param name="tokenData">The refresh token to verify.</param>
        /// <param name="admin">The admin whose token is being verified.</param>
        /// <returns>A new Token object containing the admin ID and refresh token, along with the expiration date.</returns>
        /// <exception cref="Exception">Thrown if the email claim is not present in the token.</exception>
        /// <exception cref="SecurityTokenException">Thrown if the refresh token is invalid.</exception>
        public Token VerifyRefreshToken(string tokenData, Admin admin)
        {
            // Set the token validation parameters.
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{admin.Password}"))
            };

            // Validate the token and get the claims principal and security token.
            var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(tokenData, tokenValidationParameters, out SecurityToken securityToken);

            // Get the email claim from the claims principal, or throw an exception if it is not present.
            var email = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier) ?? throw new Exception("Email not present in token.");

            // Check that the email claim in the token matches the email of the admin whose token is being verified.
            if (email.Value != admin.Email)
                throw new SecurityTokenException("Invalid token.");

            // Create a new Token object with the admin ID, refresh token, and expiration date.
            return new Token(Convert.ToBase64String(Encoding.Unicode.GetBytes($"{admin.AdminId}{_seperator}{tokenData}")), securityToken.ValidTo.ToLocalTime());
        }

        public Token VerifyRefreshTokenAgent(string tokenData, Agent agent)
        {
            // Set the token validation parameters.
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_jwtSettings.Key}{agent.Password}"))
            };

            // Validate the token and get the claims principal and security token.
            var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(tokenData, tokenValidationParameters, out SecurityToken securityToken);

            // Get the email claim from the claims principal, or throw an exception if it is not present.
            var email = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier) ?? throw new Exception("Email not present in token.");

            // Check that the email claim in the token matches the email of the admin whose token is being verified.
            if (email.Value != agent.Email)
                throw new SecurityTokenException("Invalid token.");

            // Create a new Token object with the admin ID, refresh token, and expiration date.
            return new Token(Convert.ToBase64String(Encoding.Unicode.GetBytes($"{agent.AgentId}{_seperator}{tokenData}")), securityToken.ValidTo.ToLocalTime());
        }

        /// <summary>
        /// Takes Base64Encoded Refresh Token <br/>
        /// Parse it and returns adminId and Jwt Token
        /// </summary>
        /// <param name="refreshToken">The Base64Encoded Refresh Token</param>
        /// <returns>
        /// An array of two strings:<br/>
        /// string[0] - adminId<br/>
        /// string[1] - Jwt Token
        /// </returns>
        public string[] GetAdminIdAndTokenData(string refreshToken)
        {
            // Convert the Base64Encoded Refresh Token to a Unicode string and split it on the _seperator character
            return Encoding.Unicode.GetString(Convert.FromBase64String(refreshToken)).Split(_seperator);
        }
    }
}
