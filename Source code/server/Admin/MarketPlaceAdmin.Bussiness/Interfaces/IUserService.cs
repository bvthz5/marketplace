using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;

namespace MarketPlaceAdmin.Bussiness.Interfaces
{
    public interface IUserService
    {

        Task<ServiceResult> GetUser(int userId);

        Task<ServiceResult> ChangeStatusAsync(int userId, byte status);

        Task<ServiceResult> UserListAsync(UserPaginationParams form);

        Task<FileStream?> GetProfilePic(string fileName);

        Task<ServiceResult> SellerRequest(int userId, RequestForm form);

        Task<ServiceResult> SellerProductCount();

        Task<ServiceResult> SellerProductStatusCount(int userId);
    }
}
