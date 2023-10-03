using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Helper;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface IOrderDetailsService
    {
        Task AddOrderDetailsAsync(int productId, int orderId);

        Task<ServiceResult> GetAllOrders(int userId);

        Task ChangeStatus(OrderDetails order);

        void PaymentConfirmationJob(string orderNumber, int userId);

        Task<ServiceResult> GetOrderDetailsById(int userId, int orderDetailsId);

        void SendNotificationJob(int orderId);
    }
}
