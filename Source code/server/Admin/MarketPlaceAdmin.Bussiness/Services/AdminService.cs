using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.RegularExpressions;

namespace MarketPlaceAdmin.Bussiness.Services
{
    /// <summary>
    /// Service class that provides functionality for admin login, token generation, and password reset.
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger<AdminService> _logger;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the AdminService class with the specified dependencies.
        /// </summary>
        /// <param name="uow">The IUnitOfWork instance used for database access.</param>
        /// <param name="tokenGenerator">The TokenGenerator instance used for generating access and refresh tokens.</param>
        /// <param name="logger">The ILogger instance used for logging.</param>
        /// <param name="emailService">The IEmailService instance used for sending email notifications.</param>
        public AdminService(IUnitOfWork uow, ITokenGenerator tokenGenerator, ILogger<AdminService> logger, IEmailService emailService)
        {
            _uow = uow;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
            _emailService = emailService;
        }

        /// <summary>
        /// Admin Login using the email and password provided through the LoginForm.
        /// </summary>
        /// <param name="form">LoginForm object containing email and password fields</param>
        /// <returns>
        /// ServiceResult object containing the LoginView object with the admin details, access token, and refresh token
        /// </returns>
        public async Task<ServiceResult> Login(LoginForm form)
        {
            // Initialize the ServiceResult object
            ServiceResult result = new();

            // Regular expression patterns for validating password and email
            string paswdPattern = PasswordAttribute.Pattern;
            string emailPattern = EmailAttribute.Pattern;

            // Check if the provided email and password are valid
            if (!Regex.IsMatch(form.Password, paswdPattern) || !Regex.IsMatch(form.Email, emailPattern, RegexOptions.IgnoreCase))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Credentials";
                return result;
            }

            // Find the admin object using the provided email
            Admin? admin = await _uow.AdminRepository.FindByEmail(form.Email);

            // Check if the admin object exists and if the provided password is correct
            if (admin is null || !BCrypt.Net.BCrypt.Verify(form.Password, admin.Password))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Credentials";

                // Log the failed login attempt
                _logger.LogInformation("Invalid Login");

                return result;
            }

            // Generate an access token for the admin
            Token accessToken = _tokenGenerator.GenerateAccessToken(admin.AdminId, true);

            // Generate a refresh token for the admin
            Token refreshToken = _tokenGenerator.GenerateRefreshToken(admin.AdminId, admin.Email, admin.Password);

            // Set the ServiceResult object to Success and create a new LoginView object with the admin, access token, and refresh token
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Success";
            result.Data = new LoginView(admin, accessToken, refreshToken);
            return result;
        }

        /// <summary>
        /// Refreshes the user's login session by creating a new access token using a refresh token.
        /// </summary>
        /// <param name="token">The refresh token used to create a new access token.</param>
        /// <returns>A ServiceResult object that contains a LoginView object with details of the refreshed login.</returns>
        public async Task<ServiceResult> Refresh(string token)
        {
            Admin admin;

            // Create a new ServiceResult object
            ServiceResult result = new();

            Token refreshToken;

            try
            {
                // Decode the refresh token to get the admin ID and token data
                var data = _tokenGenerator.GetAdminIdAndTokenData(token);

                var refreshTokenData = data[1];

                // Find the admin user by ID
                admin = await _uow.AdminRepository.FindById(Convert.ToInt32(data[0])) ?? throw new Exception("Invalid UserId");

                // Verify the refresh token and get the token object
                refreshToken = _tokenGenerator.VerifyRefreshToken(refreshTokenData, admin);

            }
            catch (SecurityTokenExpiredException ex)
            {
                // If the refresh token is expired, return an unauthorized ServiceResult object
                result.ServiceStatus = ServiceStatus.Unauthorized;
                result.Message = "Token Expired";

                _logger.LogWarning("Token Expired {message}", ex.Message);

                return result;
            }
            catch (Exception ex)
            {
                // If the refresh token is invalid, return an unauthorized ServiceResult object
                result.ServiceStatus = ServiceStatus.Unauthorized;
                result.Message = "Invalid Token";

                _logger.LogWarning("Invalid Token {ex}", ex.Message);

                return result;
            }

            // Generate a new access token
            var accessToken = _tokenGenerator.GenerateAccessToken(admin.AdminId, true);

            // Return a ServiceResult object with a LoginView object containing the refreshed login details (admin object, new access token, and same refresh token)
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Success";
            result.Data = new LoginView(admin, accessToken, refreshToken);

            // Log the successful refresh and return the ServiceResult object
            _logger.LogInformation("Login token refreshed for admin ID {adminId}", admin.AdminId);

            return result;
        }

        /// <summary>
        /// Sends an email with a reset password link to the email address provided.
        /// </summary>
        /// <param name="email">The email address associated with the account for which the password reset is requested.</param>
        /// <returns>A <c>Task</c> object representing the asynchronous operation, containing a <c>ServiceResult</c> object which indicates the status of the operation.</returns>
        public async Task<ServiceResult> ForgotPasswordRequest(string email)
        {
            ServiceResult result = new();

            // Find the admin account associated with the email address provided.
            Admin? admin = await _uow.AdminRepository.FindByEmail(email);

            // If no account is found for the email address, return a NotFound status.
            if (admin is null)
            {
                _logger.LogInformation("Account not found");

                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Not Found";
                return result;
            }

            // Generate a unique verification code for password reset and store it in the admin's record.
            string verificationCode = $"{Guid.NewGuid()}${DateTime.Now}";
            admin.VerificationCode = verificationCode;
            _uow.AdminRepository.Update(admin);
            await _uow.SaveAsync();

            // Generate a forgot password token using the admin's ID, email, and verification code.
            string token = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{admin.AdminId}#{admin.Email}#{verificationCode}"));

            // Send an email to the admin's email address with the forgot password token as a link.
            _emailService.AdminForgotPassword(admin.Email, token);

            _logger.LogInformation("Password reset link sent to email address '{email}'", email);

            // Set the ServiceResult object to indicate the request was successful.
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Request Send Succesfully";

            return result;
        }

        /// <summary>
        /// Represents a service method for resetting an admin account's password using a token received through email.
        /// </summary>
        /// <param name="form">A <see cref="ForgotPasswordForm"/> object containing the token and the new password.</param>
        /// <returns>A <see cref="ServiceResult"/> object containing the result of the password reset operation.</returns>
        public async Task<ServiceResult> ResetPassword(ForgotPasswordForm form)
        {
            ServiceResult result = new();

            // Extracting data from the forgot password token
            int adminId;
            string email, verificationCode;
            DateTime tokenGeneratedTime;

            try
            {
                string[] data = Encoding.Unicode.GetString(Convert.FromBase64String(form.Token)).Split('#');

                adminId = Convert.ToInt32(data[0]);
                email = data[1];
                verificationCode = data[2];
                tokenGeneratedTime = DateTime.Parse(verificationCode.Split('$')[1]);
            }
            catch (Exception)
            {
                // Return an error response if the token is invalid
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Token";
                return result;
            }

            // Check the validity of the token
            Admin? admin = await _uow.AdminRepository.FindByEmailAndVerificationCode(email, verificationCode);

            if (admin == null || DateTime.Now - tokenGeneratedTime > TimeSpan.FromMinutes(15))
            {
                // Return an error response if the token is expired
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Token Expired";
                return result;
            }

            // Update the admin's password and verification code, and save the changes to the database
            admin.Password = BCrypt.Net.BCrypt.HashPassword(form.Password);
            admin.VerificationCode = null;
            admin.UpdatedDate = DateTime.Now;
            _uow.AdminRepository.Update(admin);
            await _uow.SaveAsync();

            // Return a success response
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Password Changed";
            return result;
        }

        /// <summary>
        /// Changes the password for a specified admin user.
        /// </summary>
        /// <param name="form">A <see cref="ChangePasswordForm"/> object that contains the current and new passwords.</param>
        /// <param name="adminId">An integer that identifies the admin user whose password should be changed.</param>
        /// <returns>A <see cref="ServiceResult"/> object that indicates whether the password was successfully changed or if there was an error.</returns>
        public async Task<ServiceResult> ChangePassword(ChangePasswordForm form, int adminId)
        {
            // Create a new ServiceResult object to hold the result of the password change operation.
            ServiceResult result = new();

            // Check if the new password is the same as the current password. If so, return an error message.
            if (form.CurrentPassword == form.NewPassword)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "NewPassword should not be equal to CurrentPassword";
                return result;
            }

            // Find the admin user with the specified ID.
            Admin? admin = await _uow.AdminRepository.FindById(adminId);

            // If no admin user is found with the specified ID, return an error message.
            if (admin is null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Not Found";
                return result;
            }

            // Verify that the current password provided matches the password stored for the admin user. If not, return an error message.
            if (!BCrypt.Net.BCrypt.Verify(form.CurrentPassword, admin.Password))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Password MissMatch";
                return result;
            }

            // If the current password matches the stored password, hash the new password and update the admin user's password in the database.
            admin.Password = BCrypt.Net.BCrypt.HashPassword(form.NewPassword);
            _uow.AdminRepository.Update(admin);
            await _uow.SaveAsync();

            // Return a success message.
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Password changed";
            return result;
        }
    }
}
