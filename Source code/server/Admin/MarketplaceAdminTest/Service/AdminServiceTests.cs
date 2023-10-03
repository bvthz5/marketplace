using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using MarketPlaceAdmin.Bussiness.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReceivedExtensions;
using NSubstitute.ReturnsExtensions;
using System.Text;
using Xunit;

namespace MarketplaceAdminTest.Service
{
    public class AdminServiceTests
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger<AdminService> _logger;
        private readonly IEmailService _emailService;

        private readonly AdminService _adminService;

        public AdminServiceTests()
        {
            _uow = Substitute.For<IUnitOfWork>();
            _tokenGenerator = Substitute.For<ITokenGenerator>();
            _logger = Substitute.For<ILogger<AdminService>>();
            _emailService = Substitute.For<IEmailService>();

            _adminService = new AdminService(_uow, _tokenGenerator, _logger, _emailService);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsSuccessResultWithLoginView()
        {
            // Arrange
            var form = new LoginForm
            {
                Email = "admin@example.com",
                Password = "Admin1234!"
            };

            var createdDate = DateTime.Now;
            var updatedDate = DateTime.Now;

            var admin = new Admin
            {
                AdminId = 1,
                Email = form.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(form.Password),
                ProfilePic = "abc.png",
                CreatedDate = createdDate,
                UpdatedDate = updatedDate,
                VerificationCode = "Test"
            };

            var accessExpiry = DateTime.Now;
            var refreshExpiry = DateTime.Now;

            _uow.AdminRepository.FindByEmail(form.Email).Returns(admin);

            var accessToken = new Token("accesToken", accessExpiry);
            var refreshToken = new Token("refresh_token", refreshExpiry);

            _tokenGenerator.GenerateAccessToken(admin.AdminId, true).Returns(accessToken);
            _tokenGenerator.GenerateRefreshToken(admin.AdminId, admin.Email, admin.Password).Returns(refreshToken);

            // Act
            var result = await _adminService.Login(form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Success", result.Message);

            var loginView = Assert.IsType<LoginView>(result.Data);
            Assert.Equal(admin.AdminId, loginView.AdminId);
            Assert.Equal(admin.Email, loginView.Email);
            Assert.Equal(accessExpiry, loginView.AccessToken.Expiry);
            Assert.Equal(refreshExpiry, loginView.RefreshToken.Expiry);
            Assert.Equal(accessToken.Value, loginView.AccessToken.Value);
            Assert.Equal(refreshToken.Value, loginView.RefreshToken.Value);
            Assert.Equal(admin.ProfilePic, loginView.ProfilePic);
            Assert.Equal(admin.CreatedDate, loginView.CreatedDate);
            Assert.Equal(admin.UpdatedDate, loginView.UpdatedDate);
        }

        [Fact]
        public async Task Login_InvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var form = new LoginForm { Email = "invalid_email", Password = "Admin123!@" };

            // Act
            var result = await _adminService.Login(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task Login_InvalidPasswordFormat_ReturnsBadRequest()
        {
            // Arrange
            var form = new LoginForm { Email = "admin@example.com", Password = "invalid_password" };
            var admin = new Admin { AdminId = 1, Email = form.Email, Password = BCrypt.Net.BCrypt.HashPassword("Admin123!@") };
            _uow.AdminRepository.FindByEmail(form.Email).Returns(admin);

            // Act
            var result = await _adminService.Login(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var form = new LoginForm { Email = "admin@example.com", Password = "Inval1dP@ssword" };
            var admin = new Admin { AdminId = 1, Email = form.Email, Password = BCrypt.Net.BCrypt.HashPassword("Admin123!@") };
            _uow.AdminRepository.FindByEmail(form.Email).Returns(admin);

            // Act
            var result = await _adminService.Login(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task Login_AdminNotFound_ReturnsBadRequest()
        {
            // Arrange
            var form = new LoginForm { Email = "admin@example.com", Password = "Admin123!@" };
            _uow.AdminRepository.FindByEmail(form.Email).Returns((Admin?)null);

            // Act
            var result = await _adminService.Login(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.Null(result.Data);
        }

        // Refresh

        [Fact]
        public async Task Refresh_ValidToken_ReturnsSuccess()
        {
            // Arrange
            var token = "valid_refreshToken";
            var admin = new Admin() { AdminId = 1, Name = "Admin" };
            var accessToken = new Token("accessToken", DateTime.Now);
            var refreshToken = new Token("refreshToken", DateTime.Now);
            var loginView = new LoginView(admin, accessToken, refreshToken);

            _tokenGenerator.GetAdminIdAndTokenData(token).Returns(new[] { "1", "refresh_token_data" });
            _uow.AdminRepository.FindById(1).Returns(admin);
            _tokenGenerator.VerifyRefreshToken("refresh_token_data", admin).Returns(refreshToken);
            _tokenGenerator.GenerateAccessToken(admin.AdminId, true).Returns(accessToken);

            // Act
            var result = await _adminService.Refresh(token);

            // Assert

            var loginViewresult = Assert.IsType<LoginView>(result.Data);

            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Success", result.Message);
            Assert.Equal(loginView.AdminId, loginViewresult.AdminId);
            Assert.Equal(loginView.Name, loginViewresult.Name);
            Assert.Equal(loginView.AccessToken, loginView.AccessToken);
            Assert.Equal(loginView.RefreshToken, loginView.RefreshToken);
        }

        [Fact]
        public async Task Refresh_ExpiredToken_ReturnsUnauthorized()
        {
            // Arrange
            var token = "expired_token";

            _tokenGenerator.GetAdminIdAndTokenData(token).Throws(new SecurityTokenExpiredException("Token expired"));

            // Act
            var result = await _adminService.Refresh(token);

            // Assert
            Assert.Equal(ServiceStatus.Unauthorized, result.ServiceStatus);
            Assert.Equal("Token Expired", result.Message);
        }

        [Fact]
        public async Task Refresh_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            var token = "invalid_token";

            _tokenGenerator.GetAdminIdAndTokenData(token).Throws(new Exception("Invalid token"));

            // Act
            var result = await _adminService.Refresh(token);

            // Assert
            Assert.Equal(ServiceStatus.Unauthorized, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);
        }

        [Fact]
        public async Task Refresh_InvalidToken2_ReturnsUnauthorized()
        {
            // Arrange
            var token = "modified_refreshToken_with_wrong_id";

            _tokenGenerator.GetAdminIdAndTokenData(token).Returns(new[] { "1", "refresh_token_data" });
            _uow.AdminRepository.FindById(1).ReturnsNull();

            // Act
            var result = await _adminService.Refresh(token);

            // Assert
            Assert.Equal(ServiceStatus.Unauthorized, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);
        }

        // ForgotPassword

        [Fact]
        public async Task ForgotPasswordRequest_SuccessfullySendsEmail()
        {
            // Arrange
            string email = "test@example.com";
            Admin admin = new Admin { AdminId = 1, Email = email };
            _uow.AdminRepository.FindByEmail(email).Returns(admin);

            // Act
            ServiceResult result = await _adminService.ForgotPasswordRequest(email);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Request Send Succesfully", result.Message);
            _emailService.Received().AdminForgotPassword(email, Arg.Any<string>());
            _uow.AdminRepository.Received(1).Update(Arg.Any<Admin>());
            await _uow.Received(1).SaveAsync();
        }

        [Fact]
        public async Task ForgotPasswordRequest_FailsToSendEmail()
        {
            // Arrange
            string email = "test@example.com";
            _uow.AdminRepository.FindByEmail(email).ReturnsNull();

            // Act
            ServiceResult result = await _adminService.ForgotPasswordRequest(email);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Not Found", result.Message);
        }

        // Reset Password

        [Fact]
        public async Task InvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var invalidToken = "invalid token";

            // Act
            var result = await _adminService.ResetPassword(new ForgotPasswordForm { Token = invalidToken });

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);
        }

        [Fact]
        public async Task InvalidToken2_ReturnsBadRequest()
        {
            // Arrange
            var invalidToken = Convert.ToBase64String(Encoding.Unicode.GetBytes("1#data"));

            // Act
            var result = await _adminService.ResetPassword(new ForgotPasswordForm { Token = invalidToken });

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);
        }

        [Fact]
        public async Task InvalidToken3_ReturnsBadRequest()
        {
            // Arrange
            var invalidToken = Convert.ToBase64String(Encoding.Unicode.GetBytes("1#data#data$2022-20-23"));

            // Act
            var result = await _adminService.ResetPassword(new ForgotPasswordForm { Token = invalidToken });

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);
        }

        [Fact]
        public async Task ExpiredToken_ReturnsBadRequest()
        {
            // Arrange
            var expiredToken = Convert.ToBase64String(Encoding.Unicode.GetBytes($"1#email#uuid${DateTime.Now - TimeSpan.FromMinutes(20)}"));

            _uow.AdminRepository.FindByEmailAndVerificationCode(Arg.Any<string>(), Arg.Any<string>())
                .ReturnsNull();

            // Act
            var result = await _adminService.ResetPassword(new ForgotPasswordForm { Token = expiredToken });

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Token Expired", result.Message);
        }

        [Fact]
        public async Task ExpiredToken2_ReturnsBadRequest()
        {
            // Arrange
            var expiredToken = Convert.ToBase64String(Encoding.Unicode.GetBytes($"1#email#uuid${DateTime.Now - TimeSpan.FromMinutes(20)}"));

            Admin admin = new() { AdminId = 1 };

            _uow.AdminRepository.FindByEmailAndVerificationCode(Arg.Any<string>(), Arg.Any<string>())
                .Returns(admin);

            // Act
            var result = await _adminService.ResetPassword(new ForgotPasswordForm { Token = expiredToken });

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);

        }

        [Fact]
        public async Task ValidToken_ChangesPassword()
        {
            // Arrange
            DateTime time = DateTime.Now;
            var validToken = Convert.ToBase64String(Encoding.Unicode.GetBytes($"1#email#uuid${time}"));

            var admin = new Admin
            {
                AdminId = 1,
                Email = "admin@example.com",
                VerificationCode = $"uuid#{time}",
                Password = "old password",
                UpdatedDate = DateTime.Now
            };

            _uow.AdminRepository.FindByEmailAndVerificationCode(Arg.Any<string>(), Arg.Any<string>())
                .Returns(admin);

            // Act
            var newPassword = "new password";
            var result = await _adminService.ResetPassword(new ForgotPasswordForm { Token = validToken, Password = newPassword });

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Password Changed", result.Message);
            Assert.Null(admin.VerificationCode);
            Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, admin.Password));
            _uow.AdminRepository.Received().Update(Arg.Any<Admin>());
            await _uow.Received().SaveAsync();
        }

        // Change Password

        [Fact]
        public async Task ChangePassword_Success()
        {
            // Arrange
            int adminId = 1;
            string currentPassword = "password1";
            string newPassword = "password2";

            Admin admin = new()
            {
                AdminId = adminId,
                Password = BCrypt.Net.BCrypt.HashPassword(currentPassword)
            };

            _uow.AdminRepository.FindById(adminId).Returns(admin);

            // Act
            var result = await _adminService.ChangePassword(new ChangePasswordForm
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            }, adminId);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Password changed", result.Message);
            Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, admin.Password));
            _uow.AdminRepository.Received().Update(admin);
            await _uow.Received().SaveAsync();
        }

        [Fact]
        public async Task ChangePassword_CurrentPasswordMatchesNewPassword()
        {
            // Arrange
            int adminId = 1;
            string currentPassword = "password1";
            string newPassword = "password1";

            // Act
            var result = await _adminService.ChangePassword(new ChangePasswordForm
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            }, adminId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("NewPassword should not be equal to CurrentPassword", result.Message);
            _uow.AdminRepository.DidNotReceive().Update(Arg.Any<Admin>());
            await _uow.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task ChangePassword_AdminNotFound()
        {
            // Arrange
            int adminId = 1;

            _uow.AdminRepository.FindById(adminId).ReturnsNull();

            // Act
            var result = await _adminService.ChangePassword(new ChangePasswordForm
            {
                CurrentPassword = "password1",
                NewPassword = "password2"
            }, adminId);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Not Found", result.Message);
            _uow.AdminRepository.DidNotReceive().Update(Arg.Any<Admin>());
            await _uow.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task ChangePassword_CurrentPasswordDoesNotMatch()
        {
            // Arrange
            int adminId = 1;
            string currentPassword = "password1";
            string newPassword = "password2";

            Admin admin = new()
            {
                AdminId = adminId,
                Password = BCrypt.Net.BCrypt.HashPassword("wrongpassword")
            };

            _uow.AdminRepository.FindById(adminId).Returns(admin);

            // Act
            var result = await _adminService.ChangePassword(new ChangePasswordForm
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            }, adminId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Password MissMatch", result.Message);
            Assert.False(BCrypt.Net.BCrypt.Verify(newPassword, admin.Password));
            _uow.AdminRepository.DidNotReceive().Update(Arg.Any<Admin>());
            await _uow.DidNotReceive().SaveAsync();
        }
    }
}