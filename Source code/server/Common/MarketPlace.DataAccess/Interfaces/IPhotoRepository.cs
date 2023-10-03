using MarketPlace.DataAccess.Model;

namespace MarketPlace.DataAccess.Interfaces;

public interface IPhotoRepository : IRepository<Photos>
{
    Task<List<Photos>> FindByProductIdAsync(int productId);

    Photos? FindThumbnailPicture(int productId);

    Task<bool> IsPhotoExists(string fileName);
}