using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;

namespace MarketPlaceAdmin.Bussiness.Interfaces
{
    public interface IAdminService
    {
        Task<ServiceResult> Login(LoginForm form);

        Task<ServiceResult> Refresh(string token);

        Task<ServiceResult> ForgotPasswordRequest(string email);

        Task<ServiceResult> ResetPassword(ForgotPasswordForm form);

        Task<ServiceResult> ChangePassword(ChangePasswordForm form, int adminId);
    }
}
