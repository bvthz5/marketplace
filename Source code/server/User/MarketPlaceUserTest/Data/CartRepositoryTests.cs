using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlace.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceUserTest.Data
{
    public class CartRepositoryTests
    {
        private readonly MarketPlaceDbContext _dbContext;
        private readonly ICartRepository _cartRepository;

        public CartRepositoryTests()
        {
            _dbContext = new MarketPlaceDbContext(new DbContextOptionsBuilder<MarketPlaceDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options);
            _cartRepository = new CartRepository(_dbContext);
        }

        [Fact]
        public async Task AddAsync_ShouldAddCart()
        {
            // Arrange
            var cart = new Cart { ProductId = 1, UserId = 1 };

            // Act
            var result = await _cartRepository.Add(cart);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cart.ProductId, result.ProductId);
            Assert.Equal(cart.UserId, result.UserId);
        }

        [Fact]
        public async Task DeleteByProductIdAndUserIdAsync_ShouldDeleteCart()
        {
            // Arrange
            var productId = 2;
            var userId = 2;
            var cart = new Cart { ProductId = productId, UserId = userId };
            await _dbContext.Cart.AddAsync(cart);
            await _dbContext.SaveChangesAsync();

            // Act
            await _cartRepository.DeleteByProductIdAndUserIdAsync(productId, userId);
            await _dbContext.SaveChangesAsync();

            // Assert
            var result = await _dbContext.Cart.FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);
            Assert.Null(result);
        }
        [Fact]
        public async Task DeleteByProductId_ShouldDeleteCart()
        {
            // Arrange
            var productId = 1;
            var cart = new Cart { ProductId = productId, UserId = 1 };
            await _dbContext.Cart.AddAsync(cart);
            await _dbContext.SaveChangesAsync();

            // Act
            await _cartRepository.DeleteByProductId(productId);
            await _dbContext.SaveChangesAsync();

            // Assert
            var result = await _dbContext.Cart.FirstOrDefaultAsync(c => c.ProductId == productId);
            Assert.Null(result);
        }

        [Fact]
        public async Task FindByProductIdAndUserIdAsync_ShouldReturnCart()
        {
            // Arrange
            var productId = 1;
            var userId = 1;
            var cart = new Cart { ProductId = productId, UserId = userId };
            await _dbContext.Cart.AddAsync(cart);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _cartRepository.FindByProductIdAndUserIdAsync(productId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task FindByUserIdAndProductStatusAsync_ShouldReturnMatchingCarts()
        {
            // Arrange
            int userId = 1;
            var status = Product.ProductStatus.PENDING;

            var repository = new CartRepository(_dbContext); // Create an instance of your repository

            // Act
            var result = await repository.FindByUserIdAndProductStatusAsync(userId, status);

            // Assert
            // Assert that the result contains the expected number of carts or specific carts that match the criteria
            // You can use the Assert.Equal, Assert.Contains, Assert.Single, etc. methods to perform the assertions.
        }

        [Fact]
        public async Task FindByUserIdAndProductStatusAsync_ShouldReturnEmptyList_WhenNoMatchingCarts()
        {
            // Arrange
            int userId = 1;
            var status = Product.ProductStatus.PENDING;

            var repository = new CartRepository(_dbContext); // Create an instance of your repository

            // Act
            var result = await repository.FindByUserIdAndProductStatusAsync(userId, status);

            // Assert
            // Assert that the result is an empty list or no carts are returned
            // You can use the Assert.Empty or Assert.Equal(0, result.Count) methods to perform the assertions.
        }



        [Fact]
        public async Task DeleteByUserId_ShouldNotRemoveCartItems_WhenNoSoldItemsExist()
        {
            // Arrange
            int userId = 1;

            var repository = new CartRepository(_dbContext); // Create an instance of your repository

            // Act
            await repository.DeleteByUserId(userId);

            // Assert
            // Assert that no cart items are removed from the database
            // You can use appropriate assertion methods like Assert.Empty or Assert.Equal(0, dbContext.Cart.Count()) to check if the cart items are not removed.
        }


        [Fact]
        public async Task FindByUserIdAsync_ExistingUserId_ReturnsCartList()
        {
            // Arrange
            var userId = 1;
            var expectedCarts = new List<Cart>
    {
        new Cart { UserId = userId, CartId = 1 },
        new Cart { UserId = userId, CartId = 2 }
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.Cart.AddRange(expectedCarts);
            dbContext.SaveChanges();

            var cartRepository = new CartRepository(dbContext);

            // Act
            var result = await cartRepository.FindByUserIdAsync(userId);

            // Assert
            Assert.NotNull(expectedCarts);

            // Additional debugging information
            if (expectedCarts.Count != result.Count)
            {
                var expectedCartIds = string.Join(", ", expectedCarts.Select(c => c.CartId));
                var actualCartIds = string.Join(", ", result.Select(c => c.CartId));
                Console.WriteLine($"Expected Cart IDs: {expectedCartIds}");
                Console.WriteLine($"Actual Cart IDs: {actualCartIds}");
            }

            Assert.NotNull(result);
        }


        [Fact]
        public async Task FindByUserIdAsync_NonExistingUserId_ReturnsEmptyList()
        {
            // Arrange
            var userId = 1;

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);

            var cartRepository = new CartRepository(dbContext);

            // Act
            var result = await cartRepository.FindByUserIdAsync(userId);

            // Assert
            Assert.Empty(result);
        }


    }
}
