using MarketPlace.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MarketplaceAdminTest.Data
{
    public class UnitOfWorkTests
    {
        [Fact]
        public async Task SaveAsync_ShouldReturnFalse_WhenNoChanges()
        {
            // Arrange
            var dbContext = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "SaveAsync_ShouldReturnFalse_WhenNoChanges")
                .Options;
            var unitOfWork = new UnitOfWork(new MarketPlaceDbContext(dbContext));

            // Act
            var result = await unitOfWork.SaveAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Repository_ShouldNotBeNull()
        {
            // Arrange
            var dbContext = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "Repository_ShouldNotBeNull")
                .Options;
            var unitOfWork = new UnitOfWork(new MarketPlaceDbContext(dbContext));

            // Act & Assert
            Assert.NotNull(unitOfWork.AdminRepository);
            Assert.NotNull(unitOfWork.UserRepository);
            Assert.NotNull(unitOfWork.ProductRepostory);
            Assert.NotNull(unitOfWork.CategoryRepository);
            Assert.NotNull(unitOfWork.PhotoRepository);
            Assert.NotNull(unitOfWork.WishListRepository);
            Assert.NotNull(unitOfWork.OrderRepository);
            Assert.NotNull(unitOfWork.DeliveryAddressRepository);
            Assert.NotNull(unitOfWork.OrderDetailsRepository);
            Assert.NotNull(unitOfWork.NotificationRepository);
            Assert.NotNull(unitOfWork.CartRepository);
            Assert.NotNull(unitOfWork.AgentRepository);
            Assert.NotNull(unitOfWork.OrderHistoryRepository);
        }

    }
}
