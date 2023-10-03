using MarketPlaceAdmin.Bussiness.Helper;

namespace MarketPlaceAdmin.Bussiness.Interfaces
{
    public interface IPhotosService
    {
        Task<ServiceResult> GetPhotos(int productId);

        Task<FileStream?> GetPhotosByName(string fileName);
    }
}
