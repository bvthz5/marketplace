using Google.Apis.Auth;
using MarketPlaceUser.Bussiness.Settings;
using Microsoft.Extensions.Options;

namespace MarketPlaceUser.Bussiness.Security
{
    public class GoogleAuth
    {
        private readonly GoogleAuthSettings _googleAuthSettings;

        public GoogleAuth(IOptions<GoogleAuthSettings> googleAuthSettings)
        {
            _googleAuthSettings = googleAuthSettings.Value;
        }

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
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                return null;
            }
        }

    }
}
