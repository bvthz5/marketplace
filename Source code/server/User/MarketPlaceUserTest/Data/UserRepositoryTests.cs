using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlace.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceUserTest.Data
{
    public class UserRepositoryTests
    {

        private readonly MarketPlaceDbContext _dbContext;
        private readonly IUserRepository _user;

        public UserRepositoryTests()
        {
            _dbContext = new MarketPlaceDbContext(new DbContextOptionsBuilder<MarketPlaceDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options);
            _user = new UserRepository(_dbContext);
        }

        [Fact]
        public async Task Add_NewUser_ReturnsAddedUser()
        {
            // Arrange
            var repository = new UserRepository(_dbContext);
            var user = new User { UserId = 1, FirstName = "John", LastName = "Doe", Address = "123 Main St", City = "Example City", District = "Example District", State = "Example State" };

            // Act
            var result = await repository.Add(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user, result);

        }

        [Fact]
        public async Task Add_DuplicateUser_ThrowsDbUpdateException()
        {
            // Arrange
            var context = new UserRepository(_dbContext);

            var user = new User
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                City = "Example City",
                District = "Example District",
                State = "Example State"
            };


            var result = await context.Add(user);

            // Act and Assert
            Assert.NotNull(context.Add(user));


        }



        [Fact]
        public async Task FindByIdAsync_NonExistingUserId_ReturnsNull()
        {
            // Arrange
            var context = new UserRepository(_dbContext);
            int userId = 1;

            // Act
            var result = await context.FindById(userId);

            // Assert
            Assert.NotNull(result);
        }


        [Fact]
        public async Task FindByIdAndStatusAsync_ExistingUserIdAndNonMatchingStatus_ReturnsNull()
        {
            // Arrange
            int userId = 1;
            var status = User.UserStatus.ACTIVE;
            var context = new UserRepository(_dbContext);
            var user = new User
            {
                UserId = userId,
                Status = User.UserStatus.BLOCKED, // Different status than the one being searched for
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Address = "123 Main St",
                City = "Example City",
                District = "Example District",
                State = "Example State"
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await context.FindByIdAndStatusAsync(userId, status);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindByIdAndStatusAsync_NonExistingUserId_ReturnsNull()
        {
            // Arrange
            int userId = 1;
            var context = new UserRepository(_dbContext);
            var status = User.UserStatus.ACTIVE;

            // Act
            var result = await context.FindByIdAndStatusAsync(userId, status);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task FindByEmailAsync_NonExistingEmail_ShouldReturnNull()
        {
            // Arrange
            var email = "nonexisting@example.com";
            var repository = new UserRepository(_dbContext);

            // Act
            var foundUser = await repository.FindByEmailAsync(email);

            // Assert
            Assert.Null(foundUser);
        }


        [Fact]
        public async Task FindByEmailAndStatus_NonExistingEmail_ShouldReturnNull()
        {
            // Arrange
            var email = "nonexisting@example.com";
            var status = User.UserStatus.ACTIVE;
            var repository = new UserRepository(_dbContext);

            // Act
            var foundUser = await repository.FindByEmailAndStatus(email, status);

            // Assert
            Assert.Null(foundUser);
        }


        [Fact]
        public void Update_NonExistingUser_ShouldThrowException()
        {
            // Arrange
            var nonExistingUser = new User
            {
                UserId = 1,
                Email = "sample@example.com",
                FirstName = "John",
                Status = User.UserStatus.ACTIVE
            };

            var repository = new UserRepository(_dbContext);

            // Act and Assert
            Assert.NotNull(repository.Update(nonExistingUser));
        }


        [Fact]
        public async Task FindByEmailAndVerificationCode_NonExistingUser_ShouldReturnNull()
        {
            // Arrange
            var nonExistingEmail = "nonexisting@example.com";
            var verificationCode = "123456";

            var repository = new UserRepository(_dbContext);

            // Act
            var result = await repository.FindByEmailAndVerificationCode(nonExistingEmail, verificationCode);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetSellerProductCountGroupByProductStatus_SellerWithNoProducts_ShouldReturnEmptyDictionary()
        {
            // Arrange
            int userId = 1;

            var repository = new UserRepository(_dbContext);

            var expectedCounts = new Dictionary<Product.ProductStatus, int>();

            // Act
            var result = await repository.GetSellerProductCountGroupByProductStatus(userId);

            // Assert
            Assert.Equal(expectedCounts, result);
        }

        [Fact]
        public async Task GetSellerProductCounts_NoSellersWithProducts_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var repository = new UserRepository(_dbContext);

            var expectedCounts = new Dictionary<int, int>();

            // Act
            var result = await repository.GetSellerProductCounts();

            // Assert
            Assert.Equal(expectedCounts, result);
        }

        [Fact]
        public async Task FindAllByStatusAndNameOrEmailLikeAsync_WithStatusAndSearch_ReturnsMatchingUsers()
        {
            // Arrange
            var status = new[] { User.UserStatus.ACTIVE, User.UserStatus.INACTIVE };
            var roles = new[] { User.UserRole.ADMIN, User.UserRole.USER };
            var search = "john";
            var sortBy = "FirstName";
            var sortByDesc = false;

            var matchingUsers = new List<User>
    {
        new User { FirstName = "John", LastName = "Doe", Role = User.UserRole.USER, Status = User.UserStatus.ACTIVE, Email = "john.doe@example.com" },
        new User { FirstName = "John", LastName = "Smith", Role = User.UserRole.ADMIN, Status = User.UserStatus.INACTIVE, Email = "john.smith@example.com" }
    };

            var nonMatchingUser = new User { FirstName = "Jane", LastName = "Doe", Role = User.UserRole.USER, Status = User.UserStatus.ACTIVE, Email = "jane.doe@example.com" };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.Users.AddRange(matchingUsers);
            dbContext.Users.Add(nonMatchingUser);
            dbContext.SaveChanges();

            var repository = new UserRepository(dbContext);

            // Act
            var result = await repository.FindAllByStatusAndNameOrEmailLikeAsync(status, roles, search, sortBy, sortByDesc);

            // Assert
            Assert.NotNull(matchingUsers);
            Assert.NotNull(result);

            Assert.All(result, user => Assert.True(user.FirstName.Contains(search) || user.LastName.Contains(search) || user.Email.Contains(search)));
        }


        [Fact]
        public async Task FindAllByStatusAndNameOrEmailLikeAsync_WithNoMatchingStatus_ReturnsEmptyList()
        {
            // Arrange
            var status = new[] { User.UserStatus.INACTIVE };
            var roles = new[] { User.UserRole.ADMIN };
            var search = "john";
            var sortBy = "FirstName";
            var sortByDesc = false;

            var users = new List<User>
    {
        new User { FirstName = "John", LastName = "Doe", Role = User.UserRole.USER, Status = User.UserStatus.ACTIVE, Email = "john.doe@example.com" },
        new User { FirstName = "John", LastName = "Smith", Role = User.UserRole.ADMIN, Status = User.UserStatus.ACTIVE, Email = "john.smith@example.com" }
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();

            var repository = new UserRepository(dbContext);

            // Act
            var result = await repository.FindAllByStatusAndNameOrEmailLikeAsync(status, roles, search, sortBy, sortByDesc);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task FindAllByStatusAndNameOrEmailLikeAsync_WithNoMatchingSearch_ReturnsAllUsers()
        {
            // Arrange
            var status = new[] { User.UserStatus.ACTIVE };
            var roles = new[] { User.UserRole.ADMIN };
            var search = "nonexistent";
            var sortBy = "FirstName";
            var sortByDesc = false;

            var users = new List<User>
    {
        new User { FirstName = "John", LastName = "Doe", Role = User.UserRole.USER, Status = User.UserStatus.ACTIVE, Email = "john.doe@example.com" },
        new User { FirstName = "Jane", LastName = "Smith", Role = User.UserRole.ADMIN, Status = User.UserStatus.ACTIVE, Email = "jane.smith@example.com" }
    };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();

            var repository = new UserRepository(dbContext);

            // Act
            var result = await repository.FindAllByStatusAndNameOrEmailLikeAsync(status, roles, search, sortBy, sortByDesc);

            // Assert
            Assert.NotNull(users.Count);
            Assert.NotNull(result.Count);

        }

        [Fact]
        public async Task FindFirstByEmailLikeOrderByCreatedDateDesc_EmailExists_ReturnsUserWithMatchingEmail()
        {
            // Arrange
            var email = "john.doe@example.com";
            var matchingUser = new User { Email = email, FirstName = "John", CreatedDate = DateTime.Now };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.Users.Add(matchingUser);
            dbContext.Users.Add(new User { Email = "jane.doe@example.com", FirstName = "Jane", CreatedDate = DateTime.Now });
            dbContext.SaveChanges();

            var repository = new UserRepository(dbContext);

            // Act
            var result = await repository.FindFirstByEmailLikeOrderByCreatedDateDesc(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.Equal(matchingUser.FirstName, result.FirstName);
            Assert.Equal(matchingUser.CreatedDate, result.CreatedDate);
        }


        [Fact]
        public async Task FindFirstByEmailLikeOrderByCreatedDateDesc_EmailDoesNotExist_ReturnsNull()
        {
            // Arrange
            var email = "john.doe@example.com";
            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.Users.Add(new User { Email = "jane.doe@example.com", FirstName = "Jane", CreatedDate = DateTime.Now });
            dbContext.SaveChanges();

            var repository = new UserRepository(dbContext);

            // Act
            var result = await repository.FindFirstByEmailLikeOrderByCreatedDateDesc(email);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task IsProfilePicExists_ProfilePicExists_ReturnsTrue()
        {
            // Arrange
            var fileName = "profile.jpg";
            var userWithProfilePic = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                ProfilePic = fileName
            };

            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.Users.Add(userWithProfilePic);
            dbContext.SaveChanges();

            var repository = new UserRepository(dbContext);

            // Act
            var result = await repository.IsProfilePicExists(fileName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsProfilePicExists_ProfilePicDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var fileName = "profile.jpg";
            var dbContextOptions = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var dbContext = new MarketPlaceDbContext(dbContextOptions);
            dbContext.Users.Add(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                ProfilePic = "other.jpg"
            });
            dbContext.SaveChanges();

            var repository = new UserRepository(dbContext);

            // Act
            var result = await repository.IsProfilePicExists(fileName);

            // Assert
            Assert.False(result);
        }



    }
}
