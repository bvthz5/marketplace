using MarketPlaceUser.Bussiness.Helper;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface ICategoryService
    {
        Task<ServiceResult> GetActiveCategoryList();
    }
}
