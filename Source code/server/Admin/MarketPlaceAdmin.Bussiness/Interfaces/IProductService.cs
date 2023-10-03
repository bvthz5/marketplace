using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Helper;

namespace MarketPlaceAdmin.Bussiness.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResult> AdminEditProduct(string productName, int productId);

        Task<ServiceResult> GetProduct(int productId);

        Task<ServiceResult> ProductListOffset(ProductOffsetPaginationParams form);

        Task<ServiceResult> ProductList(ProductPaginationParams form);

        Task<ServiceResult> ChangeStatusAsync(int productId, RequestForm form);
    }
}
