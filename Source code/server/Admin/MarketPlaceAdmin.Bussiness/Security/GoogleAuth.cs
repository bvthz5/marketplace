using Google.Apis.Auth;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using MarketPlaceAdmin.Bussiness.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarketPlaceAdmin.Bussiness.Security
{
    /// <summary>
    /// Provides methods to verify Google authentication tokens.
    /// </summary>
    public class GoogleAuth : IGoogleAuth
    {
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly ILogger<GoogleAuth> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleAuth"/> class with the specified Google authentication settings and logger.
        /// </summary>
        /// <param name="googleAuthSettings">The Google authentication settings. This includes the client ID for verifying tokens.</param>
        /// <param name="logger">The logger for writing log messages during token verification.</param>
        public GoogleAuth(IOptions<GoogleAuthSettings> googleAuthSettings, ILogger<GoogleAuth> logger)
        {
            _googleAuthSettings = googleAuthSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Verifies the specified Google ID token and returns the payload if the verification is successful.
        /// </summary>
        /// <param name="idToken">The Google ID token to verify.</param>
        /// <returns>The <see cref="Google.Apis.Auth.GoogleJsonWebSignature.Payload"/> containing information about the authenticated user if the verification is successful; otherwise, <c>null</c>.</returns>
        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _googleAuthSettings.ClientId }
                };
                return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Google Auth Error => Id Token : '{idToken}' => {message}, {ex}", idToken, ex.Message, ex);
                return null;
            }
        }
    }
}
