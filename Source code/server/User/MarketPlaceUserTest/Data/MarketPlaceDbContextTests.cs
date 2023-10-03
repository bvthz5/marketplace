using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceUserTest.Db
{
    public class MarketPlaceDbContextTests
    {
        [Fact]
        public void EnsureDatabaseCreated()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "OnModelCreating_ShouldApplyUniqueConstraints")
                .Options;

            // Act
            var dbContext = new MarketPlaceDbContext(options);

            // Assert
            Assert.True(dbContext.Database.EnsureCreated());
        }

        [Fact]
        public void DbSetProperties_ShouldNotBeNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "DbSetProperties_ShouldNotBeNull")
                .Options;

            // Act
            var dbContext = new MarketPlaceDbContext(options);

            // Assert
            Assert.NotNull(dbContext.Admins);
            Assert.NotNull(dbContext.Users);
            Assert.NotNull(dbContext.Products);
            Assert.NotNull(dbContext.WishLists);
            Assert.NotNull(dbContext.Orders);
            Assert.NotNull(dbContext.Categories);
            Assert.NotNull(dbContext.OrderDetails);
            Assert.NotNull(dbContext.Photos);
            Assert.NotNull(dbContext.Cart);
            Assert.NotNull(dbContext.DeliveryAddresses);
            Assert.NotNull(dbContext.Notifications);
            Assert.NotNull(dbContext.Agents);
        }
    }
}