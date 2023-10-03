using MarketPlaceUser.Bussiness.Helper;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface ICartService
    {
        Task<ServiceResult> AddToCartAsync(int userId, int productId);

        Task<ServiceResult> GetCartAsync(int userId);

        Task<ServiceResult> RemoveFromCartAsync(int userId, int productId);
    }
}
