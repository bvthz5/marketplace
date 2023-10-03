using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using MarketPlaceUser.Bussiness.Services;
using MarketPlaceUser.Bussiness.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using System.Text;

namespace MarketPlaceUserTest.Servicetest
{

    public class UserServiceTest
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _uow;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IEmailService _emailService;
        private readonly IFileUtil _fileUtil;
        private readonly ILogger<UserService> _logger;

        public UserServiceTest()
        {
            _uow = Substitute.For<IUnitOfWork>();
            _tokenGenerator = Substitute.For<ITokenGenerator>();
            _emailService = Substitute.For<IEmailService>();
            _fileUtil = Substitute.For<IFileUtil>();
            _logger = Substitute.For<ILogger<UserService>>();

            _userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsSuccessfulServiceResultWithTokensAndUserInfo()
        {
            // Arrange
            var form = new LoginForm
            {
                Email = "test@example.com",
                Password = "Abc123!@"
            };
            var user = new User
            {
                UserId = 1,
                Email = form.Email,
                FirstName = "Binil",
                LastName = "Vincent",
                Password = BCrypt.Net.BCrypt.HashPassword(form.Password),
                Status = User.UserStatus.ACTIVE
            };
            _uow.UserRepository.FindByEmailAsync(form.Email).Returns(user);
            Token accessToken = new("access_token", DateTime.Now);
            Token refreshToken = new("refresh_token", DateTime.Now);
            _tokenGenerator.GenerateAccessToken(user).Returns(accessToken);
            _tokenGenerator.GenerateRefreshToken(user).Returns(refreshToken);

            // Act
            var result = await _userService.Login(form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.NotNull(result.Data);
            var loginView = result.Data as LoginView;
            Assert.NotNull(loginView);
            Assert.Equal(user.UserId, loginView.UserId);
            Assert.Equal(user.Email, loginView.Email);
            Assert.Equal(user.FirstName, loginView.FirstName);
            Assert.Equal(user.LastName, loginView.LastName);
            Assert.Equal((byte)user.Role, loginView.Role);
            Assert.Equal(user.ProfilePic, loginView.ProfilePic);
            Assert.Equal(user.CreatedDate, loginView.CreatedDate);
            Assert.Equal(accessToken.Value, loginView.AccessToken.Value);
            Assert.Equal(refreshToken.Value, loginView.RefreshToken.Value);
            Assert.Equal(accessToken.Expiry, loginView.AccessToken.Expiry);
            Assert.Equal(refreshToken.Expiry, loginView.RefreshToken.Expiry);
            await _uow.UserRepository.Received(1).FindByEmailAsync(form.Email);
            _tokenGenerator.Received(1).GenerateAccessToken(user);
            _tokenGenerator.Received(1).GenerateRefreshToken(user);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var form = new LoginForm { Email = "test@example.com", Password = "invalid_password" };
            var user = new User { UserId = 1, Email = "test@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Abc123!@") };
            _uow.UserRepository.FindByEmailAsync(form.Email).Returns(user);
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.Login(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task Login_UserIsDeleted_ReturnsBadRequest()
        {
            // Arrange
            var form = new LoginForm { Email = "test@example.com", Password = "Abc123!@" };
            var user = new User { UserId = 1, Email = "test@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Abc123!@"), Status = User.UserStatus.DELETED };
            _uow.UserRepository.FindByEmailAsync(form.Email).Returns(user);
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.Login(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.False(result.Status);
        }


        [Fact]
        public async Task Login_UserIsInactive_ReturnsBadRequest()
        {
            // Arrange
            var form = new LoginForm { Email = "test@example.com", Password = "Abc123!@" };
            var user = new User { UserId = 1, Email = "test@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Abc123!@"), Status = User.UserStatus.INACTIVE };
            _uow.UserRepository.FindByEmailAsync(form.Email).Returns(user);
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.Login(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("User not verified", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task Login_UserIsBlocked_ReturnsBadRequest()
        {
            // Arrange
            var form = new LoginForm { Email = "test@example.com", Password = "Abc123!@" };
            var user = new User { UserId = 1, Email = "test@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Abc123!@"), Status = User.UserStatus.BLOCKED };
            _uow.UserRepository.FindByEmailAsync(form.Email).Returns(user);
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.Login(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("User Blocked", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task Login_PasswordNotSet_ReturnsBadRequest()
        {
            // Arrange
            var form = new LoginForm { Email = "test@example.com", Password = "Abc123!@" };
            var user = new User { UserId = 1, Email = "test@example.com", Password = "", Status = User.UserStatus.ACTIVE };
            _uow.UserRepository.FindByEmailAsync(form.Email).Returns(user);
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.Login(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Password not set", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task Login_PasswordNotMatch_ReturnsBadRequest()
        {
            // Arrange
            var form = new LoginForm { Email = "test@example.com", Password = "Abc1234!@" };
            var user = new User { UserId = 1, Email = "test@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Abc123!@"), Status = User.UserStatus.ACTIVE };
            _uow.UserRepository.FindByEmailAsync(form.Email).Returns(user);
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.Login(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task Login_UserIsNull_ReturnsBadRequest()
        {
            // Arrange
            var form = new LoginForm { Email = "test@example.com", Password = "Abc123!@" };
            _uow.UserRepository.FindByEmailAsync(form.Email).ReturnsNull();
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.Login(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task RefreshAsync_ValidToken_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            var tokenData = new string[] { userId.ToString(), "refresh_token_data" };
            var base64EncodedToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(string.Join(":", tokenData)));

            var user = new User { UserId = userId, Status = User.UserStatus.ACTIVE };
            _uow.UserRepository.FindByIdAndStatusAsync(userId, User.UserStatus.ACTIVE).Returns(user);

            Token refreshToken = new("refresh_token", DateTime.Now);
            _tokenGenerator.VerifyRefreshToken(tokenData[1], user).Returns(refreshToken);

            _tokenGenerator.GetUserIdAndTokenData(base64EncodedToken).Returns(tokenData);

            Token accessToken = new("access_token", DateTime.Now);
            _tokenGenerator.GenerateAccessToken(user).Returns(accessToken);

            // Act
            var result = await _userService.RefreshAsync(base64EncodedToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.NotNull(result.Data);
            Assert.IsType<LoginView>(result.Data);

            var loginView = (LoginView)result.Data;
            Assert.Equal(user.UserId, loginView.UserId);
            Assert.Equal(user.Email, loginView.Email);
            Assert.Equal(user.FirstName, loginView.FirstName);
            Assert.Equal(user.LastName, loginView.LastName);
            Assert.Equal((byte)user.Role, loginView.Role);
            Assert.Equal(user.ProfilePic, loginView.ProfilePic);
            Assert.Equal(user.CreatedDate, loginView.CreatedDate);
            Assert.Equal(accessToken.Value, loginView.AccessToken.Value);
            Assert.Equal(refreshToken.Value, loginView.RefreshToken.Value);
            Assert.Equal(accessToken.Expiry, loginView.AccessToken.Expiry);
            Assert.Equal(refreshToken.Expiry, loginView.RefreshToken.Expiry);

            await _uow.UserRepository.Received(1).FindByIdAndStatusAsync(userId, User.UserStatus.ACTIVE);
            _tokenGenerator.Received(1).VerifyRefreshToken(tokenData[1], user);
            _tokenGenerator.Received(1).GenerateAccessToken(user);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task RefreshAsync_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            var base64EncodedToken = "invalid_token";

            _tokenGenerator.GetUserIdAndTokenData(base64EncodedToken).Throws(new Exception("Invalid Token"));

            // Act
            var result = await _userService.RefreshAsync(base64EncodedToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Unauthorized, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);
            Assert.False(result.Status);

            _tokenGenerator.Received(1).GetUserIdAndTokenData(base64EncodedToken);
        }

        [Fact]
        public async Task AddUserAsync_WhenUserDoesNotExist_ShouldCreateUserAndSendVerificationEmail()
        {
            // Arrange
            var uow = Substitute.For<IUnitOfWork>();
            var tokenGenerator = Substitute.For<ITokenGenerator>();
            var emailService = Substitute.For<IEmailService>();
            var fileUtil = Substitute.For<IFileUtil>();
            var logger = Substitute.For<ILogger<UserService>>();
            var service = new UserService(uow, tokenGenerator, emailService, fileUtil, logger);
            uow.UserRepository.Add(Arg.Any<User>()).Returns(new User()
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password123",
            });

            var form = new UserRegistrationForm
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password123",
            };

            // Act
            var result = await service.AddUserAsync(form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("User Created", result.Message);
            Assert.NotNull(result.Data);
            UserView userView = (UserView)result.Data;
            Assert.NotNull(userView);
            Assert.Equal(form.FirstName, userView.FirstName);
            Assert.Equal(form.LastName, userView.LastName);
            Assert.Equal(form.Email, userView.Email);
            Assert.Equal((byte)User.UserRole.USER, userView.Role);
            Assert.True(result.Status);

            // Check that the user was added to the repository and saved
            await uow.UserRepository.Received(1).Add(Arg.Any<User>());
            await uow.Received(1).SaveAsync();

            // Check that an email was sent to the user for email verification
            emailService.Received(1).VerifyUser(form.Email, Arg.Any<string>());
        }



        [Fact]
        public async Task AddUserAsync_UserAlreadyExists_ReturnsAlreadyExistsServiceResult()
        {
            // Arrange
            var form = new UserRegistrationForm
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "password123"
            };

            var user = new User
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = User.UserRole.USER,
                Status = User.UserStatus.ACTIVE,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _uow.UserRepository.FindByEmailAsync(form.Email).Returns(user);

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.AddUserAsync(form);

            // Assert
            Assert.Equal(ServiceStatus.AlreadyExists, result.ServiceStatus);
            Assert.Equal("User Already Exists", result.Message);
            Assert.Null(result.Data);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task AddUserAsync_InactiveUser_ReturnsBadRequestServiceResult()
        {
            // Arrange
            var form = new UserRegistrationForm
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "password123"
            };

            var user = new User
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = User.UserRole.USER,
                Status = User.UserStatus.INACTIVE,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _uow.UserRepository.FindByEmailAsync(form.Email).Returns(user);

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.AddUserAsync(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Inactive User", result.Message);
            Assert.Null(result.Data);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task AddUserAsync_BlockedUser_ReturnsBadRequestServiceResult()
        {
            // Arrange
            var form = new UserRegistrationForm
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "password123"
            };

            var user = new User
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = User.UserRole.USER,
                Status = User.UserStatus.BLOCKED,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };


            _uow.UserRepository.FindByEmailAsync(form.Email).Returns(user);

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.AddUserAsync(form);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Blocked User", result.Message);
            Assert.Null(result.Data);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task AddUserAsync_UserCreated_ReturnsSuccessResult2()
        {
            // Arrange
            var form = new UserRegistrationForm
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "password123"
            };

            var user = new User
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = User.UserRole.USER,
                Status = User.UserStatus.DELETED,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _uow.UserRepository.FindByEmailAsync(form.Email).Returns(user);

            _uow.UserRepository.Add(Arg.Any<User>()).Returns(user);

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.AddUserAsync(form);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("User Created", result.Message);
            Assert.NotNull(result.Data);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task AddUserAsync_UserCreatedWithNull_ReturnsSuccessResult()
        {
            // Arrange
            var form = new UserRegistrationForm
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "password123"
            };

            var user = new User
            {
                UserId = 1,
                FirstName = "John",
                LastName = null,
                Email = "johndoe@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = User.UserRole.USER,
                Status = User.UserStatus.DELETED,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _uow.UserRepository.FindByEmailAsync(form.Email).Returns(user);

            _uow.UserRepository.Add(Arg.Any<User>()).Returns(user);

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.AddUserAsync(form);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("User Created", result.Message);
            Assert.NotNull(result.Data);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task GetUserAsync_WithValidId_ReturnsUserDetailView()
        {
            // Arrange
            int userId = 1;
            var user = new User
            {
                UserId = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@gmail.com",
                Address = "Address",
                City = "city",
                CreatedDate = DateTime.Now,
                District = "District",
                Password = "Password@123",
                PhoneNumber = "9874563210",
                ProfilePic = "picture.png",
                Role = 0,
                State = "State",
                Status = User.UserStatus.ACTIVE,
                UpdatedDate = DateTime.Now,
                VerificationCode = null
            };
            _uow.UserRepository.FindById(userId).Returns(user);

            // Act
            var result = await _userService.GetUserAsync(userId);

            // Assert
            Assert.NotNull(result.Data);
            Assert.IsType<UserDetailView>(result.Data);
            var userDetailView = (UserDetailView)result.Data;
            Assert.Equal(userId, userDetailView.UserId);
            Assert.Equal(user.FirstName, userDetailView.FirstName);
            Assert.Equal(user.State, userDetailView.State);
            Assert.Equal(user.LastName, userDetailView.LastName);
            Assert.Equal(user.Email, userDetailView.Email);
            Assert.Equal((byte)user.Status, userDetailView.Status);
            Assert.Equal(user.Address, userDetailView.Address);
            Assert.Equal(user.PhoneNumber, userDetailView.PhoneNumber);
            Assert.Equal(user.ProfilePic, userDetailView.ProfilePic);
            Assert.Equal(user.CreatedDate, userDetailView.CreatedDate);
            Assert.Equal(user.UpdatedDate, userDetailView.UpdatedDate);
            Assert.Equal((byte)user.Role, userDetailView.Role);
            Assert.Equal(user.City, userDetailView.City);
            Assert.Equal(user.District, userDetailView.District);
            await _uow.UserRepository.Received(1).FindById(userId);
        }

        [Fact]
        public async Task GetUserAsync_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;
            _uow.UserRepository.FindById(userId).ReturnsNull();

            // Act
            var result = await _userService.GetUserAsync(userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("User not found", result.Message);
            Assert.Null(result.Data);
            Assert.False(result.Status);
            await _uow.UserRepository.Received(1).FindById(userId);
        }

        [Fact]
        public async Task ResendVerificationMailAsync_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);
            string email = "test@example.com";
            _uow.UserRepository.FindByEmailAsync(email).ReturnsNull();

            // Act
            var result = await userService.ResendVerificationMailAsync(email);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("User Not Found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task ResendVerificationMailAsync_ReturnsBadRequest_WhenUserIsNotInactive()
        {
            // Arrange
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);
            string email = "test@example.com";
            User user = new User
            {
                UserId = 1,
                Email = email,
                Status = User.UserStatus.ACTIVE
            };
            _uow.UserRepository.FindByEmailAsync(email).Returns(user);

            // Act
            var result = await userService.ResendVerificationMailAsync(email);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid User", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task ResendVerificationMailAsync_SendsEmailAndReturnsSuccess_WhenUserIsInactive()
        {
            // Arrange
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);
            string email = "test@example.com";
            string verificationCode = Guid.NewGuid().ToString();
            User user = new()
            {
                UserId = 1,
                Email = email,
                Status = User.UserStatus.INACTIVE,
                VerificationCode = verificationCode
            };
            _uow.UserRepository.FindByEmailAsync(email).Returns(user);
            string token = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{user.UserId}#{user.Email}#{verificationCode}"));

            // Act
            var result = await userService.ResendVerificationMailAsync(email);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Email Sent Successfully", result.Message);
            _emailService.ReceivedWithAnyArgs(1).VerifyUser(email, token);
            _uow.UserRepository.Received(1).Update(user);
            Assert.True(result.Status);
            await _uow.Received(1).SaveAsync();
        }

        [Fact]
        public async Task VerifyUserAsync_WithValidToken_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            var email = "test@example.com";
            var verificationCode = "123456";
            var tokenData = $"{userId}#{email}#{verificationCode}";
            var token = Convert.ToBase64String(Encoding.Unicode.GetBytes(tokenData));
            var user = new User { UserId = userId, Email = email, VerificationCode = verificationCode, UpdatedDate = DateTime.Now };
            _uow.UserRepository.FindByEmailAndVerificationCode(email, verificationCode).Returns(user);
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.VerifyUserAsync(token);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("User verified", result.Message);
            _uow.UserRepository.Received(1).Update(user);
            Assert.True(result.Status);
            await _uow.Received(1).SaveAsync();
        }

        [Fact]
        public async Task VerifyUserAsync_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var token = "invalid_token";
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.VerifyUserAsync(token);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);
            Assert.False(result.Status);
            await _uow.UserRepository.DidNotReceiveWithAnyArgs().FindByEmailAndVerificationCode(Arg.Any<string>(), Arg.Any<string>());
            await _uow.DidNotReceiveWithAnyArgs().SaveAsync();
        }

        [Fact]
        public async Task VerifyUserAsync_WithExpiredToken_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var email = "test@example.com";
            var verificationCode = "123456";
            var tokenData = $"{userId}#{email}#{verificationCode}";
            var token = Convert.ToBase64String(Encoding.Unicode.GetBytes(tokenData));
            var user = new User { UserId = userId, Email = email, VerificationCode = verificationCode, UpdatedDate = DateTime.Now.AddMinutes(-11) };
            _uow.UserRepository.FindByEmailAndVerificationCode(email, verificationCode).Returns(user);
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.VerifyUserAsync(token);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Token Expired", result.Message);
            Assert.False(result.Status);
            await _uow.UserRepository.Received(1).FindByEmailAndVerificationCode(email, verificationCode);
            await _uow.DidNotReceiveWithAnyArgs().SaveAsync();
        }

        [Fact]
        public async Task VerifyUserAsync_WithNullUser_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var email = "test@example.com";
            var verificationCode = "123456";
            var tokenData = $"{userId}#{email}#{verificationCode}";
            var token = Convert.ToBase64String(Encoding.Unicode.GetBytes(tokenData));
            _uow.UserRepository.FindByEmailAndVerificationCode(email, verificationCode).ReturnsNull();
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.VerifyUserAsync(token);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);
            Assert.False(result.Status);
            await _uow.UserRepository.Received(1).FindByEmailAndVerificationCode(email, verificationCode);
            await _uow.DidNotReceiveWithAnyArgs().SaveAsync();
        }

        [Fact]
        public async Task EditAsync_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            int userId = 1;
            var form = new UserUpdateForm { FirstName = "John" };

            _uow.UserRepository.FindById(userId).ReturnsNull();

            // Act
            var result = await _userService.EditAsync(userId, form);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("User not found", result.Message);
            Assert.Null(result.Data);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task EditAsync_ValidUser_ReturnsUserDetailView()
        {
            // Arrange
            int userId = 1;
            var form = new UserUpdateForm { FirstName = "John", LastName = "Doe", Address = "123 Main St", City = "Anytown", District = "ABC", State = "NY", PhoneNumber = "555-1234" };
            var user = new User { UserId = userId, FirstName = "Jane", LastName = "Doe", Address = "456 Main St", City = "Othertown", District = "XYZ", State = "CA", PhoneNumber = "555-5678" };

            _uow.UserRepository.FindById(userId).Returns(user);

            // Act
            var result = await _userService.EditAsync(userId, form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Null(result.Message);
            Assert.NotNull(result.Data);
            Assert.IsType<UserDetailView>(result.Data);

            var userDetailView = (UserDetailView)result.Data;
            Assert.Equal(userId, userDetailView.UserId);
            Assert.Equal(form.FirstName, userDetailView.FirstName);
            Assert.Equal(form.LastName, userDetailView.LastName);
            Assert.Equal(form.Address, userDetailView.Address);
            Assert.Equal(form.City, userDetailView.City);
            Assert.Equal(form.District, userDetailView.District);
            Assert.Equal(form.State, userDetailView.State);
            Assert.Equal(form.PhoneNumber, userDetailView.PhoneNumber);
            Assert.True(result.Status);

            _uow.UserRepository.Received(1).Update(user);
            await _uow.Received(1).SaveAsync();
        }

        [Fact]
        public async Task EditAsync_ValidUser_ReturnsUserDetailView2()
        {
            // Arrange
            int userId = 1;
            var form = new UserUpdateForm { FirstName = "John", LastName = null, Address = null, City = null, District = null, State = null, PhoneNumber = "555-1234" };
            var user = new User { UserId = userId, FirstName = "Jane", LastName = "Doe", Address = "456 Main St", City = "Othertown", District = "XYZ", State = "CA", PhoneNumber = "555-5678" };

            _uow.UserRepository.FindById(userId).Returns(user);

            // Act
            var result = await _userService.EditAsync(userId, form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Null(result.Message);
            Assert.NotNull(result.Data);
            Assert.IsType<UserDetailView>(result.Data);

            var userDetailView = (UserDetailView)result.Data;
            Assert.Equal(userId, userDetailView.UserId);
            Assert.Equal(form.FirstName, userDetailView.FirstName);
            Assert.Equal(form.LastName, userDetailView.LastName);
            Assert.Equal(form.Address, userDetailView.Address);
            Assert.Equal(form.City, userDetailView.City);
            Assert.Equal(form.District, userDetailView.District);
            Assert.Equal(form.State, userDetailView.State);
            Assert.Equal(form.PhoneNumber, userDetailView.PhoneNumber);
            Assert.True(result.Status);

            _uow.UserRepository.Received(1).Update(user);
            await _uow.Received(1).SaveAsync();
        }

        [Fact]
        public async Task ForgotPasswordRequestAsync_ShouldReturnSuccessMessage_WhenUserFound()
        {
            // Arrange
            var user = new User { UserId = 1, Email = "test@example.com", VerificationCode = null };
            _uow.UserRepository.FindByEmailAndStatus(user.Email, User.UserStatus.ACTIVE).Returns(user);

            // Act
            var result = await _userService.ForgotPasswordRequestAsync(user.Email);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Request sent successfully", result.Message);
            Assert.True(result.Status);
            _uow.UserRepository.Received().Update(user);
            _emailService.Received().ForgotPassword(user.Email, Arg.Any<string>());
            await _uow.Received().SaveAsync();
        }

        [Fact]
        public async Task ForgotPasswordRequestAsync_ShouldReturnNotFound_WhenUserNotFound()
        {
            // Arrange
            _uow.UserRepository.FindByEmailAndStatus("test@example.com", User.UserStatus.ACTIVE).ReturnsNull();

            // Act
            var result = await _userService.ForgotPasswordRequestAsync("test@example.com");

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("User Not Found", result.Message);
            Assert.False(result.Status);
            _uow.UserRepository.DidNotReceive().Update(Arg.Any<User>());
            _emailService.DidNotReceive().ForgotPassword(Arg.Any<string>(), Arg.Any<string>());
            await _uow.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task ResetPasswordAsync_WithValidTokenAndForm_ShouldReturnSuccessMessage()
        {
            // Arrange

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            var userId = 1;
            var email = "test@example.com";
            var verificationCode = $"{Guid.NewGuid()}${DateTime.Now}";
            var token = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{userId}#{email}#{verificationCode}"));
            var password = "newpassword";

            var user = new User
            {
                UserId = userId,
                Email = email,
                VerificationCode = verificationCode,
                Status = User.UserStatus.ACTIVE
            };

            _uow.UserRepository.FindByEmailAndVerificationCode(email, verificationCode).Returns(user);

            // Act
            var result = await userService.ResetPasswordAsync(new ForgotPasswordForm
            {
                Token = token,
                Password = password
            });

            // Assert
            Assert.Equal("User Password Changed", result.Message);
            _uow.UserRepository.Received().Update(user);
            await _uow.Received().SaveAsync();
        }


        [Fact]
        public async Task ResetPasswordAsync_WithInValidTokenAndForm_ShouldReturnInvalidMessage()
        {
            // Arrange

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            var userId = 1;
            var email = "test@example.com";
            var verificationCode = $"{Guid.NewGuid()}${DateTime.Now}";
            var token = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{userId}{email}{verificationCode}"));
            var password = "newpassword";

            var user = new User
            {
                UserId = userId,
                Email = email,
                VerificationCode = verificationCode,
                Status = User.UserStatus.ACTIVE
            };

            _uow.UserRepository.FindByEmailAndVerificationCode(email, verificationCode).Returns(user);

            // Act
            var result = await userService.ResetPasswordAsync(new ForgotPasswordForm
            {
                Token = token,
                Password = password
            });

            // Assert
            Assert.Equal("Invalid Token", result.Message);
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task ResetPasswordAsync_WithexpiredTokenAndForm_ShouldReturnBadRequest()
        {
            // Arrange

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            var userId = 1;
            var email = "test@example.com";
            var verificationCode = $"{Guid.NewGuid()}${DateTime.Now}";
            var token = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{userId}#{email}#{verificationCode}"));
            var password = "newpassword";

            _uow.UserRepository.FindByEmailAndVerificationCode(email, verificationCode).ReturnsNull();

            // Act
            var result = await userService.ResetPasswordAsync(new ForgotPasswordForm
            {
                Token = token,
                Password = password
            });

            // Assert
            Assert.Equal("Token Expired", result.Message);
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task ResetPasswordAsync_IfUserNotActive_ShouldReturnBadRequest()
        {
            // Arrange

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            var userId = 1;
            var email = "test@example.com";
            var verificationCode = $"{Guid.NewGuid()}${DateTime.Now}";
            var token = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{userId}#{email}#{verificationCode}"));
            var password = "newpassword";

            var user = new User
            {
                UserId = userId,
                Email = email,
                VerificationCode = verificationCode,
                Status = User.UserStatus.INACTIVE
            };

            _uow.UserRepository.FindByEmailAndVerificationCode(email, verificationCode).Returns(user);

            // Act
            var result = await userService.ResetPasswordAsync(new ForgotPasswordForm
            {
                Token = token,
                Password = password
            });

            // Assert
            Assert.Equal("Invalid user", result.Message);
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task UploadImageAsync_ShouldReturnSuccess_WhenImageUploadSucceeds()
        {
            // Arrange
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);
            var userId = 1;
            var imageFile = new byte[] { 1, 2, 3 };
            var image = Substitute.For<IFormFile>();
            image.FileName.Returns("image.jpg");
            image.Length.Returns(imageFile.Length);
            image.OpenReadStream().Returns(new MemoryStream(imageFile));
            var user = new User { UserId = userId, Email = "testuser@gmail.com" };
            _uow.UserRepository.FindById(userId).Returns(user);
            _fileUtil.UploadUserProfilePic(user, image).Returns("image.jpg");

            // Act
            var result = await userService.UploadImageAsync(userId, new ImageForm { File = image });

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Success", result.Message);
            Assert.True(result.Status);
            _fileUtil.Received().UploadUserProfilePic(user, image);
            _uow.UserRepository.Received().Update(user);
            await _uow.Received().SaveAsync();
        }

        [Fact]
        public async Task UploadImageAsync_ShouldReturnBadRequest_WhenUserNotFound()
        {
            // Arrange
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);
            var userId = 1;
            var image = Substitute.For<IFormFile>();
            var user = default(User);
            _uow.UserRepository.FindById(userId).Returns(user);

            // Act
            var result = await userService.UploadImageAsync(userId, new ImageForm { File = image });

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("User not found", result.Message);
            Assert.False(result.Status);
            _fileUtil.DidNotReceiveWithAnyArgs().UploadUserProfilePic(Arg.Any<User>(), Arg.Any<IFormFile>());
            _uow.UserRepository.DidNotReceiveWithAnyArgs().Update(Arg.Any<User>());
            await _uow.DidNotReceiveWithAnyArgs().SaveAsync();
        }

        [Fact]
        public async Task UploadImageAsync_WhenFileIsEmpty_ReturnsBadRequest()
        {
            // Arrange
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);
            var userId = 1;
            var image = new ImageForm { };
            var user = new User { UserId = userId };
            _uow.UserRepository.FindById(userId).Returns(user);

            // Act
            var result = await userService.UploadImageAsync(userId, image);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Failed: Object reference not set to an instance of an object.", result.Message);
            Assert.False(result.Status);
            _uow.UserRepository.DidNotReceive().Update(Arg.Any<User>());
            await _uow.DidNotReceive().SaveAsync();
            _fileUtil.DidNotReceive().UploadUserProfilePic(Arg.Any<User>(), Arg.Any<IFormFile>());
        }

        [Fact]
        public async Task UploadImageAsync_WhenFileLengthIsZero_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var fileStream = new MemoryStream(Array.Empty<byte>());
            var image = new ImageForm
            {
                File = new FormFile(fileStream, 0, 0, "file", "file.jpg")
            };
            var user = new User { UserId = userId };
            _uow.UserRepository.FindById(userId).Returns(user);

            // Act
            var result = await _userService.UploadImageAsync(userId, image);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Failed", result.Message);
            Assert.False(result.Status);
            _uow.UserRepository.DidNotReceive().Update(Arg.Any<User>());
            await _uow.DidNotReceive().SaveAsync();
            _fileUtil.DidNotReceive().UploadUserProfilePic(Arg.Any<User>(), Arg.Any<IFormFile>());
        }

        [Fact]
        public async Task UploadImageAsync_ProfilePicNotNull_ReturnsSuccessResult()
        {
            // Arrange
            var userId = 1;
            var fileName = "test.jpg";
            var imageFile = new byte[] { 1, 2, 3 };
            var image = Substitute.For<IFormFile>();
            image.FileName.Returns("image.jpg");
            image.Length.Returns(imageFile.Length);
            image.OpenReadStream().Returns(new MemoryStream(imageFile));
            var user = new User { UserId = userId, ProfilePic = fileName };

            _uow.UserRepository.FindById(userId).Returns(user);

            _fileUtil.UploadUserProfilePic(Arg.Any<User>(), Arg.Any<IFormFile>()).Returns(fileName);

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.UploadImageAsync(userId, new ImageForm { File = image });

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Success", result.Message);
            Assert.True(result.Status);

            _fileUtil.Received(1).DeleteUserProfilePic(fileName);
            _fileUtil.Received(1).UploadUserProfilePic(user, Arg.Any<IFormFile>());
            _uow.UserRepository.Received(1).Update(user);
            await _uow.Received(1).SaveAsync();
        }

        [Fact]
        public async Task UploadImageAsync_FilenameNull_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var fileName = "test.jpg";
            var imageFile = new byte[] { 1, 2, 3 };
            var image = Substitute.For<IFormFile>();
            image.FileName.Returns("image.jpg");
            image.Length.Returns(imageFile.Length);
            image.OpenReadStream().Returns(new MemoryStream(imageFile));
            var user = new User { UserId = userId, ProfilePic = fileName };

            _uow.UserRepository.FindById(userId).Returns(user);

            _fileUtil.UploadUserProfilePic(Arg.Any<User>(), Arg.Any<IFormFile>()).ReturnsNull();

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.UploadImageAsync(userId, new ImageForm { File = image });

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Failed", result.Message);
            Assert.False(result.Status);

            _fileUtil.Received(1).DeleteUserProfilePic(fileName);
            _fileUtil.Received(1).UploadUserProfilePic(user, Arg.Any<IFormFile>());
            _uow.UserRepository.DidNotReceive().Update(Arg.Any<User>());
            await _uow.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task GetProfilePic_WithExistingProfilePic_ReturnsStream()
        {
            // Arrange
            var fileName = "profilepic.jpg";
            var fileStream = File.Create("test_porfilepic.png");
            _uow.UserRepository.IsProfilePicExists(fileName).Returns(true);
            _fileUtil.GetUserProfile(Arg.Any<string>()).Returns(fileStream);

            // Act
            var result = await _userService.GetProfilePic(fileName);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<FileStream>(result);

        }

        [Fact]
        public async Task GetProfilePic_FileDoesNotExist_ReturnsNull()
        {
            // Arrange
            string fileName = "test.jpg";
            _uow.UserRepository.IsProfilePicExists(fileName).Returns(false);

            // Act
            var result = await _userService.GetProfilePic(fileName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RequsetToSeller_WhenUserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);
            int userId = 1;
            _uow.UserRepository.FindById(userId).ReturnsNull();

            // Act
            var result = await userService.RequsetToSeller(userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("User not found", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task RequsetToSeller_WhenUserIsSeller_ReturnsBadRequestResult()
        {
            // Arrange
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);
            int userId = 1;
            var user = new User { UserId = userId, Role = User.UserRole.SELLER };
            _uow.UserRepository.FindById(userId).Returns(user);

            // Act
            var result = await userService.RequsetToSeller(userId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("User is already a Seller / Requested", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task RequsetToSeller_WhenUserHasRequested_ReturnsBadRequestResult()
        {
            // Arrange
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);
            int userId = 1;
            var user = new User { UserId = userId, Role = User.UserRole.REQUESTED };
            _uow.UserRepository.FindById(userId).Returns(user);

            // Act
            var result = await userService.RequsetToSeller(userId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("User is already a Seller / Requested", result.Message);
            Assert.False(result.Status);
        }

        [Fact]
        public async Task RequestToSeller_Success()
        {
            // Arrange
            int userId = 123;
            _uow.UserRepository.FindById(userId).Returns(new User { UserId = userId });
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.RequsetToSeller(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Request completed", result.Message);
            Assert.True(result.Status);
            _uow.UserRepository.Received().Update(Arg.Any<User>());
            await _uow.Received().SaveAsync();
            _logger.Received().LogInformation("User Role changed");
        }

        [Fact]
        public async Task ChangePasswordAsync_Should_Return_Success_Result()
        {
            // Arrange
            var userId = 1;
            var oldPassword = "oldpassword";
            var newPassword = "newpassword";
            var confirmNewPassword = "newpassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(oldPassword);

            var form = new ChangePasswordForm
            {
                CurrentPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmNewPassword = confirmNewPassword
            };

            _uow.UserRepository.FindById(userId).Returns(new User { UserId = userId, Password = hashedPassword });



            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.ChangePasswordAsync(form, userId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Password changed", result.Message);
            Assert.True(result.Status);
            await _uow.UserRepository.Received(1).FindById(userId);
        }

        [Fact]
        public async Task ChangePasswordAsync_NewPasswodAndConfirmpasswordMissmatch_ShouldReturnBadRequest()
        {
            // Arrange
            var userId = 1;
            var oldPassword = "oldpassword";
            var newPassword = "newpassword";
            var confirmNewPassword = "confirmnewpassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(oldPassword);


            var form = new ChangePasswordForm
            {
                CurrentPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmNewPassword = confirmNewPassword
            };

            _uow.UserRepository.FindById(userId).Returns(new User { UserId = userId, Password = hashedPassword });



            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.ChangePasswordAsync(form, userId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("ConfirmPassword does not maches NewPassword", result.Message);
            Assert.False(result.Status);
            await _uow.UserRepository.Received(1).FindById(userId);
        }

        [Fact]
        public async Task ChangePasswordAsync_NewPasswodAndOldPasswordMatch_ShouldReturnBadRequest()
        {
            // Arrange
            var userId = 1;
            var oldPassword = "oldpassword";
            var newPassword = "oldpassword";
            var confirmNewPassword = "oldpassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(oldPassword);


            var form = new ChangePasswordForm
            {
                CurrentPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmNewPassword = confirmNewPassword
            };

            _uow.UserRepository.FindById(userId).Returns(new User { UserId = userId, Password = hashedPassword });



            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.ChangePasswordAsync(form, userId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("NewPassword should not be equal to CurrentPassword", result.Message);
            Assert.False(result.Status);
            await _uow.UserRepository.Received(1).FindById(userId);
        }

        [Fact]
        public async Task ChangePasswordAsync_UserIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var userId = 1;
            var oldPassword = "oldpassword";
            var newPassword = "newpassword";
            var confirmNewPassword = "newpassword";

            var form = new ChangePasswordForm
            {
                CurrentPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmNewPassword = confirmNewPassword
            };

            _uow.UserRepository.FindById(userId).ReturnsNull();

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.ChangePasswordAsync(form, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("User not found", result.Message);
            Assert.False(result.Status);
            await _uow.UserRepository.Received(1).FindById(userId);
        }

        [Fact]
        public async Task ChangePasswordAsync_PasswordNotSet_ShouldReturnNotFound()
        {
            // Arrange
            var userId = 1;
            var oldPassword = "oldpassword";
            var newPassword = "newpassword";
            var confirmNewPassword = "newpassword";

            var form = new ChangePasswordForm
            {
                CurrentPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmNewPassword = confirmNewPassword
            };

            _uow.UserRepository.FindById(userId).Returns(new User { UserId = userId, Password = "" });

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.ChangePasswordAsync(form, userId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Password Not Set", result.Message);
            Assert.False(result.Status);
            await _uow.UserRepository.Received(1).FindById(userId);
        }

        [Fact]
        public async Task ChangePasswordAsync_PasswordMissMatch_ShouldReturnBadRequest()
        {
            // Arrange
            var userId = 1;
            var oldPassword = "wrongpassword";
            var newPassword = "newpassword";
            var confirmNewPassword = "newpassword";
            var currentPassword = "currentpassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(currentPassword);
            var form = new ChangePasswordForm
            {
                CurrentPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmNewPassword = confirmNewPassword
            };

            _uow.UserRepository.FindById(userId).Returns(new User { UserId = userId, Password = hashedPassword });

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.ChangePasswordAsync(form, userId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Password MissMatch", result.Message);
            Assert.False(result.Status);
            await _uow.UserRepository.Received(1).FindById(userId);
        }

        [Fact]
        public async Task IsValidActiveUser_Should_Return_True_When_User_Exists_And_Is_Active()
        {
            // Arrange
            int userId = 1;
            _uow.UserRepository.FindByIdAndStatusAsync(userId, User.UserStatus.ACTIVE).Returns(new User { UserId = userId });
            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.IsValidActiveUser(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsValidActiveUser_Should_Return_False_When_User_Does_Not_Exist()
        {
            // Arrange
            int userId = 1;
            _uow.UserRepository.FindByIdAndStatusAsync(userId, User.UserStatus.ACTIVE).ReturnsNull();

            var userService = new UserService(_uow, _tokenGenerator, _emailService, _fileUtil, _logger);

            // Act
            var result = await userService.IsValidActiveUser(userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ResendVerificationMailByToken_InvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var token = "invalid_token";

            // Act
            var result = await _userService.ResendVerificationMailByToken(token);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);
        }

        [Fact]
        public async Task ResendVerificationMailByToken_InactiveUser_ReturnsBadRequest()
        {
            // Arrange
            var token = "valid_token";
            var email = "testuser@example.com";
            var inactiveUser = new User
            {
                Email = email,
                Status = User.UserStatus.INACTIVE
            };

            _uow.UserRepository.FindByEmailAsync(email).Returns(inactiveUser);

            // Act
            var result = await _userService.ResendVerificationMailByToken(token);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);
        }

    }
}


