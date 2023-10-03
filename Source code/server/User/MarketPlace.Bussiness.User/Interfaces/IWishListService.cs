using MarketPlaceUser.Bussiness.Helper;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface IWishListService
    {
        Task<ServiceResult> AddToWishListAsync(int userId, int productId);

        Task<ServiceResult> RemoveFromWishListAsync(int userId, int productId);

        Task<ServiceResult> GetWishListAsync(int userId);
    }
}
