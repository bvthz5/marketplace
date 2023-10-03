using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;

namespace MarketPlaceAdmin.Bussiness.Interfaces
{
    public interface IAgentService
    {
        Task<ServiceResult> AddAgentAsync(AgentRegistrationForm form);

        Task<ServiceResult> AgentListAsync(AgentPaginationParams form);

        Task<ServiceResult> EditAgentAsync(int agentId, EditAgentForm form);

        Task<ServiceResult> ChangeAgentStatusAsync(int agentId, byte status);

        Task<ServiceResult> Login(string email, string password);

        Task<ServiceResult> AgentRefresh(string token);

        Task<ServiceResult> ChangePassword(ChangePasswordForm form, int agentId);

        Task<bool> IsValidAgent(int agentId, string urlPath);

        Task<ServiceResult> ForgotPassword(string email);

        Task<ServiceResult> ResetPassword(ForgotPasswordForm form);

        Task<ServiceResult> GetAgent(int agentId);

        Task<ServiceResult> SetProfilePic(int agentId, ImageForm form);

        Task<FileStream?> GetAgentProfilePic(string fileName);

    }
}


