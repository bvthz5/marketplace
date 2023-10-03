using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Model;
using MarketPlace.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MarketplaceAdminTest.Data
{
    public class AdminRepositoryTests
    {
        private readonly DbContextOptions<MarketPlaceDbContext> _options;

        public AdminRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<MarketPlaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task FindByEmail_ShouldReturnAdminIfEmailExists()
        {
            // Arrange
            using var dbContext = new MarketPlaceDbContext(_options);
            var adminRepository = new AdminRepository(dbContext);
            var email = "test@example.com1";
            var expectedAdmin = new Admin { Name = "", Email = email, Password = "" };
            await dbContext.Admins.AddAsync(expectedAdmin);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await adminRepository.FindByEmail(email);

            // Assert
            Assert.Equal(expectedAdmin, result);
        }

        [Fact]
        public async Task FindByEmail_ShouldReturnNullIfEmailDoesNotExist()
        {
            // Arrange
            using var dbContext = new MarketPlaceDbContext(_options);
            var adminRepository = new AdminRepository(dbContext);
            var email = "test@example.com";

            // Act
            var result = await adminRepository.FindByEmail(email);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindByAdminId_ShouldReturnAdminIfAdminIdExists()
        {
            // Arrange
            using var dbContext = new MarketPlaceDbContext(_options);
            var adminRepository = new AdminRepository(dbContext);
            var adminId = 1;
            var expectedAdmin = new Admin { AdminId = adminId, Name = "", Password = "", Email = "2" };
            await dbContext.Admins.AddAsync(expectedAdmin);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await adminRepository.FindById(adminId);

            // Assert
            Assert.Equal(expectedAdmin, result);
        }

        [Fact]
        public async Task FindByAdminId_ShouldReturnNullIfAdminIdDoesNotExist()
        {
            // Arrange
            using var dbContext = new MarketPlaceDbContext(_options);
            var adminRepository = new AdminRepository(dbContext);
            var adminId = 100;

            // Act
            var result = await adminRepository.FindById(adminId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Update_ShouldUpdateAdmin()
        {
            // Arrange
            using var dbContext = new MarketPlaceDbContext(_options);
            var adminRepository = new AdminRepository(dbContext);
            var admin = new Admin { AdminId = 1, Name = "John", Email = "email", Password = "" };
            await dbContext.Admins.AddAsync(admin);
            await dbContext.SaveChangesAsync();

            // Act
            admin.Name = "Jane";
            var result = adminRepository.Update(admin);
            await dbContext.SaveChangesAsync();

            // Assert
            Assert.Equal("Jane", result.Name);
        }

        [Fact]
        public async Task FindByEmailAndVerificationCode_ShouldReturnAdminIfEmailAndVerificationCodeExist()
        {
            // Arrange
            using var dbContext = new MarketPlaceDbContext(_options);
            var adminRepository = new AdminRepository(dbContext);
            var email = "test@example.com4";
            var verificationCode = "1234";
            var expectedAdmin = new Admin { Name = "", Password = "", Email = email, VerificationCode = verificationCode, ProfilePic = "hh.png", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
            await dbContext.Admins.AddAsync(expectedAdmin);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await adminRepository.FindByEmailAndVerificationCode(email, verificationCode);

            // Assert
            Assert.Equal(expectedAdmin, result);
        }

    }
}
