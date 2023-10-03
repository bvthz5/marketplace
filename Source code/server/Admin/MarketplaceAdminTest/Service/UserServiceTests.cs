using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Services;
using MarketPlaceAdmin.Bussiness.Util.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Data;
using System.Linq.Expressions;
using Xunit;

namespace MarketplaceAdminTest.Service
{
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _uow;
        private readonly IEmailService _emailService;
        private readonly IFileUtil _fileUtil;
        private readonly ILogger<UserService> _logger;

        public UserServiceTests()
        {
            // create mock objects using NSubstitute
            _uow = Substitute.For<IUnitOfWork>();
            _emailService = Substitute.For<IEmailService>();
            _fileUtil = Substitute.For<IFileUtil>();
            _logger = Substitute.For<ILogger<UserService>>();

            // create UserService instance using mock objects
            _userService = new UserService(_uow, _emailService, _fileUtil, _logger);
        }

        [Fact]
        public async Task SellerProductCount_ReturnsSuccessResult()
        {
            // Arrange           
            var productCounts = new Dictionary<int, int>
            {
                {1, 5},
                {2, 3},
                {3, 2}
            };
            _uow.UserRepository.GetSellerProductCounts().Returns(productCounts);

            // Act
            var result = await _userService.SellerProductCount();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Seller Product Count", result.Message);
            Assert.NotNull(result.Data);

            IEnumerable<CountView> expectedCounts = productCounts
                .Select(kv => new CountView(kv.Key.ToString(), kv.Value));

            Assert.IsAssignableFrom<IEnumerable<CountView>>(result.Data);

            var actualCounts = (IEnumerable<CountView>)result.Data;

            foreach (var kv in actualCounts)
            {
                Assert.Equal(expectedCounts.Single(a => a.Property == kv.Property).Count, actualCounts.Single(a => a.Property == kv.Property).Count);
            }

        }

        [Fact]
        public async Task SellerProductStatusCount_ReturnsSuccessResult()
        {
            // Arrange
            int userId = 1;

            var user = new User { UserId = userId, Role = User.UserRole.SELLER };

            _uow.UserRepository.FindById(userId).Returns(user);

            var productStatusCounts = new Dictionary<Product.ProductStatus, int>
            {
                {Product.ProductStatus.ACTIVE, 5},
                {Product.ProductStatus.INACTIVE, 3},
                {Product.ProductStatus.SOLD, 2}
            };
            _uow.UserRepository.GetSellerProductCountGroupByProductStatus(userId).Returns(productStatusCounts);
            var userService = new UserService(_uow, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.SellerProductStatusCount(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Seller Product Status Count", result.Message);
            Assert.NotNull(result.Data);

            var expectedData = new List<string> { "INACTIVE", "ACTIVE", "PENDING", "SOLD", "DELETED", "ONPROCESS" };

            Assert.True(((IEnumerable<CountView>?)result.Data)?.Any(count => expectedData.Contains(count.Property)));
        }
        [Fact]
        public async Task SellerProductStatusCount2_ReturnsSuccessResult()
        {
            // Arrange
            int userId = 1;

            var user = new User { UserId = userId, Role = User.UserRole.SELLER };

            _uow.UserRepository.FindById(userId).Returns(user);

            var productStatusCounts = new Dictionary<Product.ProductStatus, int> { };
            _uow.UserRepository.GetSellerProductCountGroupByProductStatus(userId).Returns(productStatusCounts);
            var userService = new UserService(_uow, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.SellerProductStatusCount(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Seller Product Status Count", result.Message);
            Assert.NotNull(result.Data);

            var expectedData = new List<string> { "INACTIVE", "ACTIVE", "PENDING", "SOLD", "DELETED", "ONPROCESS" };

            Assert.True(((IEnumerable<CountView>?)result.Data)?.Any(count => expectedData.Contains(count.Property)));
        }

        [Fact]
        public async Task SellerProductStatusCount_Returns_NotFound_With_User_UserId()
        {
            // arrange
            int userId = 2;
            var user = new User { UserId = 2, Role = User.UserRole.USER };

            _uow.UserRepository.FindById(userId).Returns(user);

            // act
            var result = await _userService.SellerProductStatusCount(userId);

            // assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Seller Not Found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task SellerProductStatusCount_Returns_NotFound_With_Invalid_UserId()
        {
            // arrange
            int userId = 2;

            _uow.UserRepository.FindById(userId).ReturnsNull();

            // act
            var result = await _userService.SellerProductStatusCount(userId);

            // assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Seller Not Found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task SellerRequest_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            int userId = 1;
            RequestForm form = new() { Approved = true, Reason = "Approved" };
            _uow.UserRepository.FindById(userId).ReturnsNull();

            // Act
            ServiceResult result = await _userService.SellerRequest(userId, form);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("User not found", result.Message);
        }

        [Fact]
        public async Task SellerRequest_UserNotActive_ReturnsNotFoundResult()
        {
            // Arrange
            int userId = 1;
            RequestForm form = new() { Approved = true, Reason = "Approved" };
            User user = new() { UserId = userId, Status = User.UserStatus.INACTIVE };
            _uow.UserRepository.FindById(userId).Returns(user);

            // Act
            ServiceResult result = await _userService.SellerRequest(userId, form);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("User not found", result.Message);
        }

        [Fact]
        public async Task SellerRequest_UserRoleNotRequested_ReturnsBadRequestResult()
        {
            // Arrange
            int userId = 1;
            RequestForm form = new() { Approved = true, Reason = "Approved" };
            User user = new() { UserId = userId, Status = User.UserStatus.ACTIVE, Role = User.UserRole.SELLER };
            _uow.UserRepository.FindById(userId).Returns(user);

            // Act
            ServiceResult result = await _userService.SellerRequest(userId, form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal($"User role: {User.UserRole.SELLER}", result.Message);
        }

        [Fact]
        public async Task SellerRequest_Approved_Succeeds()
        {   // Arrange
            int userId = 1;
            RequestForm form = new() { Approved = true, Reason = "Approved" };
            User user = new() { UserId = userId, Email = "test@email.com", Status = User.UserStatus.ACTIVE, Role = User.UserRole.REQUESTED };

            User updatedUser = new() { UserId = userId, Email = "test@email.com", Status = User.UserStatus.ACTIVE, Role = User.UserRole.SELLER };

            _uow.UserRepository.FindById(userId).Returns(user);

            _uow.UserRepository.Update(user).Returns(updatedUser);

            // Act
            ServiceResult result = await _userService.SellerRequest(userId, form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Seller request accepted", result.Message);
            Assert.NotNull(result.Data);
            Assert.IsType<UserDetailView>(result.Data);
            Assert.Equal(user.UserId, ((UserDetailView)result.Data).UserId);
            Assert.Equal(user.FirstName, ((UserDetailView)result.Data).FirstName);
            Assert.Equal(user.LastName, ((UserDetailView)result.Data).LastName);
            Assert.Equal(user.Email, ((UserDetailView)result.Data).Email);
            Assert.Equal(user.Role, (User.UserRole)((UserDetailView)result.Data).Role);
            Assert.Equal(user.Status, (User.UserStatus)((UserDetailView)result.Data).Status);
            Assert.Equal(user.UserId, ((UserDetailView)result.Data).UserId);
            Assert.Equal(user.CreatedDate, ((UserDetailView)result.Data).CreatedDate);
            Assert.Equal(user.ProfilePic, ((UserDetailView)result.Data).ProfilePic);
            Assert.Equal(user.Address, ((UserDetailView)result.Data).Address);
            Assert.Equal(user.State, ((UserDetailView)result.Data).State);
            Assert.Equal(user.District, ((UserDetailView)result.Data).District);
            Assert.Equal(user.City, ((UserDetailView)result.Data).City);
            Assert.Equal(user.PhoneNumber, ((UserDetailView)result.Data).PhoneNumber);
            Assert.Equal(user.UpdatedDate, ((UserDetailView)result.Data).UpdatedDate);
            Assert.NotNull(user.Password);
            Assert.Null(user.VerificationCode);

            _uow.UserRepository.Received().Update(Arg.Any<User>());
            _emailService.Received().SellerRequest(user.Email, true, form.Reason);
        }

        [Fact]
        public async Task SellerRequest_Rejected_Succeeds()
        {   // Arrange
            int userId = 1;
            RequestForm form = new RequestForm { Approved = false, Reason = "Approved" };
            User user = new() { UserId = userId, Email = "test@email.com", Status = User.UserStatus.ACTIVE, Role = User.UserRole.REQUESTED };

            User updatedUser = new() { UserId = userId, Email = "test@email.com", Status = User.UserStatus.ACTIVE, Role = User.UserRole.USER };

            _uow.UserRepository.FindById(userId).Returns(user);

            _uow.UserRepository.Update(user).Returns(updatedUser);

            // Act
            ServiceResult result = await _userService.SellerRequest(userId, form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Seller request rejected", result.Message);
            Assert.NotNull(result.Data);
            Assert.IsType<UserDetailView>(result.Data);
            Assert.Equal(user.UserId, ((UserDetailView)result.Data).UserId);
            Assert.Equal(user.FirstName, ((UserDetailView)result.Data).FirstName);
            Assert.Equal(user.LastName, ((UserDetailView)result.Data).LastName);
            Assert.Equal(user.Email, ((UserDetailView)result.Data).Email);
            Assert.Equal(user.Role, (User.UserRole)((UserDetailView)result.Data).Role);
            Assert.Equal((byte)user.Status, ((UserDetailView)result.Data).Status);
            Assert.Equal(user.UserId, ((UserDetailView)result.Data).UserId);
            Assert.Equal(user.CreatedDate, ((UserDetailView)result.Data).CreatedDate);
            Assert.Equal(user.ProfilePic, ((UserDetailView)result.Data).ProfilePic);
            Assert.Equal(user.Address, ((UserDetailView)result.Data).Address);
            Assert.Equal(user.State, ((UserDetailView)result.Data).State);
            Assert.Equal(user.District, ((UserDetailView)result.Data).District);
            Assert.Equal(user.City, ((UserDetailView)result.Data).City);
            Assert.Equal(user.PhoneNumber, ((UserDetailView)result.Data).PhoneNumber);
            Assert.Equal(user.UpdatedDate, ((UserDetailView)result.Data).UpdatedDate);
            Assert.NotNull(user.Password);
            Assert.Null(user.VerificationCode);


            _uow.UserRepository.Received().Update(Arg.Any<User>());
            _emailService.Received().SellerRequest(user.Email, false, form.Reason);
        }

        [Fact]
        public async Task SellerProductCount_ReturnsNoData()
        {
            // Arrange
            _uow.UserRepository.GetSellerProductCounts().Returns(new Dictionary<int, int>());

            // Act
            var result = await _userService.SellerProductCount();

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Seller Product Count", result.Message);
            Assert.NotNull(result.Data);

            var data = (IEnumerable<CountView>)result.Data;
            Assert.Empty(data);
        }

        // GetUser

        [Fact]
        public async Task GetUser_UserNotFoundError()
        {
            // Arrange

            int userId = 1;

            _uow.UserRepository.FindById(userId).ReturnsNull();

            // Act

            var result = await _userService.GetUser(userId);

            // Assert

            Assert.NotNull(result);
            Assert.IsType<ServiceResult>(result);

            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal($"User Not Found for Id : {userId}", result.Message);
        }

        [Fact]
        public async Task GetUser_Success()
        {
            // Arrange

            int userId = 1;

            User? user = new()
            {
                UserId = userId,
                FirstName = "Test",
                LastName = "Q",
            };

            _uow.UserRepository.FindById(userId).Returns(user);

            // Act

            var result = await _userService.GetUser(userId);

            // Assert

            Assert.NotNull(result);
            Assert.IsType<ServiceResult>(result);

            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Success", result.Message);
        }

        // ChangeStatusAsync


        [Fact]
        public async Task ChangeStatusAsync_InvalidStatus_ReturnsBadRequest()
        {
            // Arrange
            var user = new User { UserId = 1, Status = User.UserStatus.ACTIVE };
            await _uow.UserRepository.Add(user);
            await _uow.SaveAsync();

            var service = _userService;

            // Act
            var result = await service.ChangeStatusAsync(user.UserId, 5);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Status", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task ChangeStatusAsync_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var service = _userService;

            // Act
            var result = await service.ChangeStatusAsync(999, (byte)User.UserStatus.ACTIVE);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("User Not Found for Id : 999", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task ChangeStatusAsync_UserAlreadyDeleted_ReturnsBadRequest()
        {
            // Arrange
            User user = new()
            {
                UserId = 1,
                Status = User.UserStatus.DELETED
            };

            _uow.UserRepository.FindById(user.UserId).Returns(user);

            // Act
            var result = await _userService.ChangeStatusAsync(user.UserId, (byte)User.UserStatus.ACTIVE);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Deleted User", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task ChangeStatusAsync_Success()
        {
            // Arrange
            User user = new()
            {
                UserId = 1,
                Status = User.UserStatus.ACTIVE
            };

            _uow.UserRepository.FindById(user.UserId).Returns(user);

            // Act
            var result = await _userService.ChangeStatusAsync(user.UserId, (byte)User.UserStatus.BLOCKED);

            // Assert

            _uow.UserRepository.ReceivedWithAnyArgs().Update(Arg.Any<User>());
            await _uow.ReceivedWithAnyArgs().SaveAsync();

            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("User Status Changed", result.Message);
            Assert.NotNull(result.Data);
            Assert.IsType<UserDetailView>(result.Data);
            Assert.Equal((byte)User.UserStatus.BLOCKED, ((UserDetailView)result.Data).Status);
        }


        [Fact]
        public async Task ChangeStatusAsync_Delete_Success()
        {
            // Arrange
            User user = new()
            {
                UserId = 1,
                Status = User.UserStatus.ACTIVE
            };

            _uow.UserRepository.FindById(user.UserId).Returns(user);

            // Act
            var result = await _userService.ChangeStatusAsync(user.UserId, (byte)User.UserStatus.DELETED);

            // Assert

            _uow.UserRepository.ReceivedWithAnyArgs().Update(Arg.Any<User>());
            await _uow.ProductRepostory.Received().DeleteProductAsync(user.UserId);
            await _uow.ReceivedWithAnyArgs().SaveAsync();

            _fileUtil.DidNotReceiveWithAnyArgs().DeleteUserProfilePic(Arg.Any<string>());

            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("User Status Changed", result.Message);
            Assert.NotNull(result.Data);
            Assert.IsType<UserDetailView>(result.Data);
            Assert.Equal((byte)User.UserStatus.DELETED, ((UserDetailView)result.Data).Status);

            Assert.Contains("#", ((UserDetailView)result.Data).Email);
        }


        [Fact]
        public async Task ChangeStatusAsync_Delete_WithProfilePic_Success()
        {
            // Arrange
            User user = new()
            {
                UserId = 1,
                Status = User.UserStatus.ACTIVE,
                ProfilePic = "fileName"
            };

            _uow.UserRepository.FindById(user.UserId).Returns(user);

            // Act
            var result = await _userService.ChangeStatusAsync(user.UserId, (byte)User.UserStatus.DELETED);

            // Assert

            _uow.UserRepository.ReceivedWithAnyArgs().Update(Arg.Any<User>());
            await _uow.ProductRepostory.Received().DeleteProductAsync(user.UserId);
            await _uow.ReceivedWithAnyArgs().SaveAsync();


            _fileUtil.ReceivedWithAnyArgs().DeleteUserProfilePic(user.ProfilePic);

            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("User Status Changed", result.Message);
            Assert.NotNull(result.Data);
            Assert.IsType<UserDetailView>(result.Data);
            Assert.Equal((byte)User.UserStatus.DELETED, ((UserDetailView)result.Data).Status);

            Assert.Contains("#", ((UserDetailView)result.Data).Email);
        }

        // GetProfilePic

        [Fact]
        public async Task GetProfilePic_ReturnsNull()
        {
            // Arrange
            var fileName = "Invalid File Name";

            _uow.UserRepository.IsProfilePicExists(fileName).Returns(false);

            // Act
            var result = await _userService.GetProfilePic(fileName);

            // Assert
            Assert.Null(result);

        }

        [Fact]
        public async Task GetProfilePic_ReturnsFile()
        {
            // Arrange
            var fileName = "Valid File Name";

            _uow.UserRepository.IsProfilePicExists(fileName).Returns(true);

            _fileUtil.GetUserProfile(fileName).Returns(File.Create("For Profile Test"));

            // Act
            var result = await _userService.GetProfilePic(fileName);


            // Assert
            Assert.NotNull(result);

        }


        // UserListAsync

        [Fact]
        public async Task UserListAsync_Invalid_Role()
        {
            // Arrange
            var paginationParams = new UserPaginationParams
            {
                Role = new List<byte?> { 10 }
            };

            // Act
            var result = await _userService.UserListAsync(paginationParams);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Role Value", result.Message);
            Assert.Null(result.Data);

        }

        [Fact]
        public async Task UserListAsync_Invalid_Status()
        {
            // Arrange
            var paginationParams = new UserPaginationParams
            {
                Status = new List<byte?> { 10 }
            };

            // Act
            var result = await _userService.UserListAsync(paginationParams);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Status Value", result.Message);
            Assert.Null(result.Data);

        }

        [Fact]
        public async Task UserListAsync_Invalid_SortBy()
        {
            // Arrange

            var paginationParams = new UserPaginationParams
            {
                Status = new List<byte?> { (byte)User.UserStatus.ACTIVE },
                SortBy = "Invalid Value"
            };

            _uow.UserRepository.ColumnMapForSortBy.Returns(new Dictionary<string, Expression<Func<User, object>>>
            {
                ["UserId"] = user => user.UserId,
                ["FirstName"] = user => user.FirstName,
                ["Email"] = user => user.Email,
                ["CreatedDate"] = user => user.CreatedDate
            });

            // Act 

            var result = await _userService.UserListAsync(paginationParams);


            // Assert

            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal($"SortBy : Accepts [{string.Join(", ", _uow.UserRepository.ColumnMapForSortBy.Keys)}] values only", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task UserListAsync_Success()
        {
            // Arrange

            var paginationParams = new UserPaginationParams { PageSize = 1, PageNumber = 2 };

            _uow.UserRepository.ColumnMapForSortBy.Returns(new Dictionary<string, Expression<Func<User, object>>>
            {
                ["UserId"] = user => user.UserId,
                ["FirstName"] = user => user.FirstName,
                ["Email"] = user => user.Email,
                ["CreatedDate"] = user => user.CreatedDate
            });

            List<User> expectedResult = new()
            {
                new User(),
                new User()
            };

            _uow.UserRepository.FindAllByStatusAndNameOrEmailLikeAsync(null, null, null, null, false).Returns(expectedResult);

            // Act 

            var result = await _userService.UserListAsync(paginationParams);


            // Assert

            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.IsType<Pager<UserView>>(result.Data);

            Assert.Equal(paginationParams.PageNumber, ((Pager<UserView>)result.Data).CurrentPage);
            Assert.Equal(paginationParams.PageSize, ((Pager<UserView>)result.Data).PageSize);
            Assert.Single(((Pager<UserView>)result.Data).Result);

        }

    }
}