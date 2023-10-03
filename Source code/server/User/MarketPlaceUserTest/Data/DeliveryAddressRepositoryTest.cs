using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlace.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceUserTest.Data
{
    public class DeliveryAddressRepositoryTest
    {
        private readonly MarketPlaceDbContext _dbContext;
        public DeliveryAddressRepositoryTest()
        {
            _dbContext = new MarketPlaceDbContext(new DbContextOptionsBuilder<MarketPlaceDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options);
        }

        [Fact]
        public async Task AddAsync_ValidDeliveryAddress_ShouldAddDeliveryAddress()
        {
            // Arrange
            DeliveryAddress deliveryAddress = new()
            {
                DeliveryAddressId = 1,
                Name = "Binil",
                Address = "123 Street",
                StreetAddress = "City",
                City = "State",
                State = "krk",
                ZipCode = "656586",
                Phone = "6565856520",
                CreatedUserId = 1,
            };

            var repository = new DeliveryAddressRepository(_dbContext);

            // Act
            var result = await repository.Add(deliveryAddress);

            // Assert
            Assert.Equal(deliveryAddress, result);
            Assert.Contains(deliveryAddress, _dbContext.DeliveryAddresses);
        }

        [Fact]
        public async Task FindByAddressIdAsync_NonExistingDeliveryAddressId_ShouldReturnNull()
        {
            // Arrange
            int deliveryAddressId = 1;

            var repository = new DeliveryAddressRepository(_dbContext);

            // Act
            var result = await repository.FindByAddressIdAsync(deliveryAddressId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task FindByUserIdAndAddressIdAsync_ExistingUserIdAndAddressId_ShouldReturnDeliveryAddress()
        {
            // Arrange
            int userId = 1;
            int deliveryAddressId = 1;

            var deliveryAddress = new DeliveryAddress
            {
                DeliveryAddressId = 1,
                Name = "Binil",
                Address = "123 Street",
                StreetAddress = "City",
                City = "State",
                State = "krk",
                ZipCode = "656586",
                Phone = "6565856520",
                CreatedUserId = 1,
            };

            _dbContext.DeliveryAddresses.Add(deliveryAddress);
            await _dbContext.SaveChangesAsync();

            var repository = new DeliveryAddressRepository(_dbContext);

            // Act
            var result = await repository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId);

            // Assert
            Assert.Equal(deliveryAddress, result);
        }

        [Fact]
        public async Task FindByUserIdAndAddressIdAsync_NonExistingUserId_ShouldReturnNull()
        {
            // Arrange
            int userId = 1;
            int deliveryAddressId = 1;

            var repository = new DeliveryAddressRepository(_dbContext);

            // Act
            var result = await repository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void FindByUserIdAndStatusAsync_NonExistingUserId_ShouldReturnNull()
        {
            // Arrange
            int userId = 1;
            var status = DeliveryAddress.DeliveryAddressStatus.DEFAULT;

            var repository = new DeliveryAddressRepository(_dbContext);

            // Act
            var result = repository.FindByUserIdAndStatusAsync(userId, status);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindByUserIdAndStatusNotRemovedAsync_NonExistingUserId_ShouldReturnEmptyList()
        {
            // Arrange
            int userId = 1;

            var repository = new DeliveryAddressRepository(_dbContext);

            // Act
            var result = await repository.FindByUserIdAndStatusNotRemovedAsync(userId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Update_NonExistingDeliveryAddress_ShouldThrowException()
        {
            // Arrange
            var deliveryAddress = new DeliveryAddress
            {
                DeliveryAddressId = 1,
                Name = "John Doe",
                Address = "123 Street",
                CreatedUserId = 1,
            };

            var updatedDeliveryAddress = new DeliveryAddress
            {
                DeliveryAddressId = 2,
                Name = "Jane Smith",
                Address = "456 Avenue",
                CreatedUserId = 1,
            };

            var repository = new DeliveryAddressRepository(_dbContext);

            // Act and Assert
            Assert.NotNull(repository.Update(updatedDeliveryAddress));
        }

        [Fact]
        public async Task FindByUserIdAsync_ReturnsMatchingDeliveryAddresses()
        {
            // Arrange
            var userId = 1;
            var deliveryAddresses = new List<DeliveryAddress>
    {
        new DeliveryAddress
        {
            DeliveryAddressId = 2,
            Name = "Jane Smith",
            Address = "455 Avenue",
            City = "City 1",
            Phone = "1234567890",
            State = "State 1",
            StreetAddress = "Street 1",
            ZipCode = "12345",
            CreatedUserId = 1
        },
        new DeliveryAddress
        {
            DeliveryAddressId = 3,
            Name = "Jane Smith",
            Address = "456 Avenue",
            City = "City 2",
            Phone = "9876543210",
            State = "State 2",
            StreetAddress = "Street 2",
            ZipCode = "54321",
            CreatedUserId = 2
        },
        new DeliveryAddress
        {
            DeliveryAddressId = 4,
            Name = "Jane Smith",
            Address = "457 Avenue",
            City = "City 3",
            Phone = "1112223333",
            State = "State 3",
            StreetAddress = "Street 3",
            ZipCode = "67890",
            CreatedUserId = 3
        }
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            var repository = new DeliveryAddressRepository(dbContext);

            // Clear the database
            dbContext.DeliveryAddresses.RemoveRange(dbContext.DeliveryAddresses);
            await dbContext.SaveChangesAsync();

            // Add the delivery addresses
            dbContext.DeliveryAddresses.AddRange(deliveryAddresses);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.FindByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            Assert.All(result, deliveryAddress => Assert.Equal(userId, deliveryAddress.CreatedUserId));
        }

    }
}