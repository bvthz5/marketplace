using MarketPlaceAdmin.Bussiness.Helper;

namespace MarketPlaceAdmin.Bussiness.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<ServiceResult> Login(string idToken);
        Task<ServiceResult> AgentLogin(string idToken);


    }
}
