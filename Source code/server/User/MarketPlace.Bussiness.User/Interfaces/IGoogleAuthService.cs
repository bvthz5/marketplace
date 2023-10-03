using MarketPlaceUser.Bussiness.Helper;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<ServiceResult> RegisterAndLogin(string idToken);
    }
}
