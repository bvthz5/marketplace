using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Model;
using MarketPlace.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MarketplaceAdminTest.Data
{
    public class WishListRepositoryTests
    {
        private readonly DbContextOptions<MarketPlaceDbContext> _options;
        private readonly MarketPlaceDbContext _dbContext;
        private readonly WishListRepository _repository;

        public WishListRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "WishListRepositoryTests")
                .Options;

            _dbContext = new MarketPlaceDbContext(_options);
            _repository = new WishListRepository(_dbContext);
        }

        [Fact]
        public async Task AddAsync_AddsWishListToDbContext()
        {
            // Arrange
            var wishList = new WishList();

            // Act
            var result = await _repository.Add(wishList);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(wishList, result);
        }

        [Fact]
        public async Task DeleteByProductId_RemovesMatchingWishListsFromDbContext()
        {
            // Arrange
            var productId = 1;
            var userId = 1;
            var wishList1 = new WishList { ProductId = productId, UserId = userId };
            var wishList2 = new WishList { ProductId = productId, UserId = userId + 1 };
            _dbContext.WishLists.AddRange(wishList1, wishList2);
            await _dbContext.SaveChangesAsync();

            // Act
            await _repository.DeleteByProductId(productId);
            await _dbContext.SaveChangesAsync();

            // Assert
            var remainingWishLists = await _dbContext.WishLists.Where(w => w.ProductId == productId).ToListAsync();
            Assert.Empty(remainingWishLists);
        }

        [Fact]
        public async Task DeleteByProductIdAndUserIdAsync_RemovesMatchingWishListsFromDbContext()
        {
            // Arrange
            var productId = 1;
            var userId = 1;
            var wishList1 = new WishList { ProductId = productId, UserId = userId };
            var wishList2 = new WishList { ProductId = productId, UserId = userId + 1 };
            _dbContext.WishLists.AddRange(wishList1, wishList2);
            await _dbContext.SaveChangesAsync();

            // Act
            await _repository.DeleteByProductIdAndUserIdAsync(productId, userId);
            await _dbContext.SaveChangesAsync();

            // Assert
            var remainingWishLists = await _dbContext.WishLists.Where(w => w.ProductId == productId && w.UserId == userId).ToListAsync();
            Assert.Empty(remainingWishLists);
        }

        [Fact]
        public async Task FindByProductIdAndUserIdAsync_ReturnsMatchingWishListFromDbContext()
        {
            // Arrange
            var productId = 10;
            var userId = 5;
            var wishList = new WishList { ProductId = productId, UserId = userId };
            _dbContext.WishLists.Add(wishList);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.FindByProductIdAndUserIdAsync(productId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(wishList.ProductId, result.ProductId);
            Assert.Equal(wishList.UserId, result.UserId);
        }

        [Fact]
        public async Task FindByUserIdAsync_UserIdDoesNotExist_ShouldReturnEmptyList()
        {
            // Arrange
            int userId = 1;
            var repository = new WishListRepository(_dbContext);

            // Act
            var result = await repository.FindByUserIdAsync(userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task FindUserIdsByProductIdAndNotUserId_ValidProductIdAndUserId_ReturnsUserIds()
        {
            // Arrange
            var productId = 1;
            var userId = 2;
            var expectedUserIds = new List<int> { 1, 3, 4, 5 };

            var wishLists = new List<WishList>
    {
        new WishList { ProductId = productId, UserId = 1 },
        new WishList { ProductId = productId, UserId = 3 },
        new WishList { ProductId = productId, UserId = 4 },
        new WishList { ProductId = productId, UserId = 5 },
        new WishList { ProductId = productId, UserId = 2 },
        new WishList { ProductId = 6, UserId = 2 } // Another product's wish list
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.WishLists.AddRange(wishLists);
            dbContext.SaveChanges();

            var repository = new WishListRepository(dbContext);

            // Act
            var result = await repository.FindUserIdsByProductIdAndNotUserId(productId, userId);

            // Assert
            Assert.Equal(expectedUserIds.Count, result.Count);
            Assert.Equal(expectedUserIds, result);

        }

        [Fact]
        public async Task FindUserIdsByProductIdAndNotUserId_NoMatchingWishLists_ReturnsEmptyList()
        {
            // Arrange
            var productId = 1;
            var userId = 2;

            var wishLists = new List<WishList>
    {
        new WishList { ProductId = 3, UserId = 1 }, // Another product's wish list
        new WishList { ProductId = 4, UserId = 2 }, // Another product's wish list
        new WishList { ProductId = 5, UserId = 3 } // Another product's wish list
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.WishLists.AddRange(wishLists);
            dbContext.SaveChanges();

            var repository = new WishListRepository(dbContext);

            // Act
            var result = await repository.FindUserIdsByProductIdAndNotUserId(productId, userId);

            // Assert
            Assert.NotNull(result);
        }

    }
}
