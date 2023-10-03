using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Helper;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface IOrderService
    {
        Task<ServiceResult> AddOrderAsync(int userId, int deliveryAddressId, int[] productIds);

        Task<ServiceResult> ConfirmPayment(ConfirmPaymentForm confirmPaymentForm, int userId);

        Task<ServiceResult> GetOrders(int userId);

        Task<ServiceResult> CancelOrder(string orderNumber);

        Task<object> DownloadInvoice(int orderDetailsId, int userId);

        Task<ServiceResult> EmailInvoice(int orderDetailsId, int userId);

        Task<ServiceResult> GetOrderHistory(int orderDetailsId, int userId);

    }
}

