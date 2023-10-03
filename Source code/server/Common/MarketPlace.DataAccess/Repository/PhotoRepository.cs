using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.DataAccess.Repository;
public class PhotoRepository : Repository<Photos>, IPhotoRepository
{
    private readonly MarketPlaceDbContext _dbContext;

    public PhotoRepository(MarketPlaceDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public new async Task<Photos?> FindById(int id)
    {
        return await _dbContext.Photos.Include(photos => photos.Product).FirstOrDefaultAsync(photo => photo.PhotosId == id);
    }

    public async Task<List<Photos>> FindByProductIdAsync(int productId)
    {
        return await _dbContext.Photos.Where(photo => photo.ProductId == productId).ToListAsync();
    }

    public Photos? FindThumbnailPicture(int productId)
    {
        return _dbContext.Photos.FirstOrDefault(photo => photo.ProductId == productId);
    }

    public async Task<bool> IsPhotoExists(string fileName)
    {
        return (await _dbContext.Photos.SingleOrDefaultAsync(photo => photo.Photo == fileName)) != null;
    }
}
