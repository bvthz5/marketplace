using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Helper;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface IRefundService
    {
        Task<ServiceResult> Refund(RefundOrderForm form, int orderDetailsId, int userId);
    }
}