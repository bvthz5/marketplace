using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Helper;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult> Login(LoginForm form);

        Task<ServiceResult> RefreshAsync(string token);

        Task<ServiceResult> AddUserAsync(UserRegistrationForm form);

        Task<bool> IsValidActiveUser(int userId);

        Task<ServiceResult> ResendVerificationMailAsync(string email);

        Task<ServiceResult> VerifyUserAsync(string token);

        Task<ServiceResult> GetUserAsync(int userId);

        Task<ServiceResult> EditAsync(int userId, UserUpdateForm form);

        Task<ServiceResult> ForgotPasswordRequestAsync(string email);

        Task<ServiceResult> ResetPasswordAsync(ForgotPasswordForm form);

        Task<ServiceResult> RequsetToSeller(int userId);

        Task<ServiceResult> UploadImageAsync(int userId, ImageForm image);

        Task<ServiceResult> ChangePasswordAsync(ChangePasswordForm form, int userId);

        Task<FileStream?> GetProfilePic(string fileName);

        Task<ServiceResult> ResendVerificationMailByToken(string token);
    }
}
