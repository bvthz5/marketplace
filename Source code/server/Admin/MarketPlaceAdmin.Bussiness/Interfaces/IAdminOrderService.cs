using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;

namespace MarketPlaceAdmin.Bussiness.Interfaces
{
    public interface IAdminOrderService
    {

        Task<ServiceResult> GetOrderList(OrderPaginationParams form);

        Task<ServiceResult> GetOrderDetails(int orderId);

        Task<ServiceResult> GetOrderHistory(int orderDetailsId);
    }
}
