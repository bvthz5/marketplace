using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlace.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MarketplaceAdminTest.Data
{
    public class PhotoRepositoryTests
    {
        private readonly MarketPlaceDbContext _dbContext;
        private readonly IPhotoRepository _repository;

        public PhotoRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;
            _dbContext = new MarketPlaceDbContext(options);
            _repository = new PhotoRepository(_dbContext);
        }

        [Fact]
        public async Task Add_ShouldAddNewPhotoToDatabase()
        {
            // Arrange
            var photo = new Photos { Photo = "testq.jpg" };

            // Act
            var result = await _repository.Add(photo);
            await _dbContext.SaveChangesAsync();

            // Assert
            Assert.Equal(photo, result);
            Assert.Equal(1, await _dbContext.Photos.Where(p => p.Photo == "testq.jpg").CountAsync());
        }

    }
}
