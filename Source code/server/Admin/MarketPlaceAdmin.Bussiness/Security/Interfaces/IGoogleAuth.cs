using Google.Apis.Auth;

namespace MarketPlaceAdmin.Bussiness.Security.Interfaces
{
    public interface IGoogleAuth
    {
        Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string idToken);
    }
}
