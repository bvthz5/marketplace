using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;

namespace MarketPlaceAdmin.Bussiness.Interfaces
{
    public interface ICategoryService
    {
        Task<ServiceResult> AddCategory(CategoryForm form);

        Task<ServiceResult> EditCategory(CategoryForm form, int categoryId);

        Task<ServiceResult> GetCategoryList();

        Task<ServiceResult> ChangeCategoryStatus(int categoryId, byte status);
    }
}
