using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Helper;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResult> AddProductAsync(ProductForm form, int userId);

        Task<ServiceResult> EditProductAsync(ProductForm form, int productId, int userId);

        Task<ServiceResult> DeleteProductAsync(int productId, int userId);

        Task<ServiceResult> GetProductAsync(int productId, int userId);

        Task<ServiceResult> ProductListAsync(ProductPaginationParams form, User.UserRole? role);

        Task<ServiceResult> GetProductByUserIdAsync(int userId);

        Task ChangeStatusAsync(Product product, Product.ProductStatus status);

    }
}
