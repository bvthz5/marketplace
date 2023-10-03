using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Helper;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface IPhotosService
    {
        Task<ServiceResult> AddPhotosAsync(int productId, ProductImageForm image);

        Task<ServiceResult> GetPhotosAsync(int productId);

        Task<ServiceResult> DeletePhotosByProductIdAsync(int productId);

        Task<ServiceResult> DeletePhotosByPhotoIdAsync(int photoId);

        Task<FileStream?> GetPhotosByName(string fileName);

    }
}
