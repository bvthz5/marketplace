using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;

namespace MarketPlaceAdmin.Bussiness.Interfaces
{
    public interface IAgentOrderService
    {
        Task<ServiceResult> GetOrderList(AgentOrderPaginationParams form);

        Task<ServiceResult> AgentOrdersStatusCount(int agentId);

        Task<ServiceResult> GetOrderDetails(int orderId, int agentId);

        Task<ServiceResult> AssignOrder(int orderId);

        Task<ServiceResult> UnAssignOrder(int orderId);

        Task<ServiceResult> ChangeDeliveryStatus(int orderId, byte status, int agentId);

        Task<ServiceResult> GenerateOtp(int agentId, int orderId);

        Task<ServiceResult> VerifyOtp(int agentId, int orderId, string otp);
    }
}
