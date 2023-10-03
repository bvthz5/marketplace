using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Dto.Forms.Validations;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using MarketPlaceUser.Bussiness.Util;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.RegularExpressions;

namespace MarketPlaceUser.Bussiness.Services
{
    public class UserService : IUserService
    {

        private readonly IUnitOfWork _uow; // UnitOfWork instance for database operations

        private readonly ITokenGenerator _tokenGenerator; // TokenGenerator instance for JWT token generation

        private readonly IEmailService _emailService; // EmailService instance for email sending

        private readonly IFileUtil _fileUtil; // FileUtil instance for file related operations

        private readonly ILogger<UserService> _logger; // ILogger instance for logging

        public UserService(IUnitOfWork uow, ITokenGenerator tokenGenerator, IEmailService emailService, IFileUtil fileUtil, ILogger<UserService> logger)
        {
            _uow = uow;
            _logger = logger;
            _emailService = emailService;
            _fileUtil = fileUtil;
            _tokenGenerator = tokenGenerator;
        }

        /// <summary>
        /// Authenticates a user and generates access and refresh tokens.
        /// </summary>
        /// <param name="form">The user's login credentials.</param>
        /// <returns>The generated tokens and user information.</returns>
        public async Task<ServiceResult> Login(LoginForm form)
        {
            ServiceResult result = new();

            // Check if email and password match the expected patterns
            string paswdPattern = PasswordAttribute.Pattern;
            string emailPattern = EmailAttribute.Pattern;

            if (!Regex.IsMatch(form.Password, paswdPattern) || !Regex.IsMatch(form.Email, emailPattern, RegexOptions.IgnoreCase))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Credentials";
                return result;
            }

            // Check if user exists and is active
            User? user = await _uow.UserRepository.FindByEmailAsync(form.Email);
            if (user == null || user.Status == User.UserStatus.DELETED)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Credentials";
                return result;
            }

            // Check if user is verified and not blocked
            if (user.Status == User.UserStatus.INACTIVE)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "User not verified";
                return result;
            }
            if (user.Status == User.UserStatus.BLOCKED)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "User Blocked";
                return result;
            }

            // Check if user's password matches the provided password
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Password not set";
                return result;
            }
            if (!BCrypt.Net.BCrypt.Verify(form.Password, user.Password))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Credentials";
                return result;
            }

            // Generate tokens for the authenticated user
            Token accessToken = _tokenGenerator.GenerateAccessToken(user);
            Token refreshToken = _tokenGenerator.GenerateRefreshToken(user);
            result.Data = new LoginView(user, accessToken, refreshToken);

            return result;
        }


        /// <summary>
        /// Refreshes an expired access token using a refresh token.
        /// </summary>
        /// <param name="token">The base64 encoded refresh token.</param>
        /// <returns>A <see cref="ServiceResult"/>  containing the refreshed access token and the corresponding user information.</returns>
        public async Task<ServiceResult> RefreshAsync(string token)
        {
            ServiceResult result = new();

            User user;
            Token refreshToken;

            try
            {
                // Get Jwt Token And User Id From Base64 Encoded Refresh Token
                var data = _tokenGenerator.GetUserIdAndTokenData(token);

                var refreshTokenData = data[1];

                // Get User Object Form Refresh Token Data Which is a jwt Token
                user = await _uow.UserRepository.FindByIdAndStatusAsync(Convert.ToInt32(data[0]), User.UserStatus.ACTIVE) ?? throw new Exception("Invalid UserId");

                // Get Token Object from Jwt Token For Returning the Token 
                refreshToken = _tokenGenerator.VerifyRefreshToken(refreshTokenData, user);

            }
            catch (SecurityTokenExpiredException ex)
            {
                Console.WriteLine(ex);
                result.ServiceStatus = ServiceStatus.Unauthorized;
                result.Message = "Token Expired";

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error : {ex}", ex.Message);
                result.ServiceStatus = ServiceStatus.Unauthorized;
                result.Message = "Invalid Token";

                return result;
            }

            // Generate New AccessToken
            var accessToken = _tokenGenerator.GenerateAccessToken(user);

            // Same RefreshToken and New AccessToken is Returened
            result.Data = new LoginView(user, accessToken, refreshToken);

            return result;
        }


        /// <summary>
        /// AddUserAsync method adds a new user to the database with the given user registration form.
        /// </summary>
        /// <param name="form">A UserRegistrationForm object representing the form data of the new user</param>
        /// <returns>A <see cref="ServiceResult"/>  object containing the status of the operation, a UserView object of the newly created user, and a message.</returns>
        public async Task<ServiceResult> AddUserAsync(UserRegistrationForm form)
        {
            ServiceResult result = new();

            // Check If User Already Registered
            User? user = await _uow.UserRepository.FindByEmailAsync(form.Email);

            if (user != null)
            {
                if (user.Status == User.UserStatus.ACTIVE)
                {
                    result.ServiceStatus = ServiceStatus.AlreadyExists;
                    result.Message = "User Already Exists";

                    return result;
                }

                if (user.Status == User.UserStatus.INACTIVE)
                {
                    result.ServiceStatus = ServiceStatus.BadRequest;
                    result.Message = "Inactive User";

                    return result;
                }

                if (user.Status == User.UserStatus.BLOCKED)
                {
                    result.ServiceStatus = ServiceStatus.BadRequest;
                    result.Message = "Blocked User";

                    return result;
                }
            }

            // Guid Id Saved in the database for Email Verification Purpose
            Guid verificationCode = Guid.NewGuid();

            user = await _uow.UserRepository.Add(new User()
            {
                FirstName = form.FirstName.Trim(),
                LastName = form.LastName?.Trim(),
                Email = form.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(form.Password),
                Role = User.UserRole.USER,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                VerificationCode = verificationCode.ToString(),
            });

            await _uow.SaveAsync();

            // Generating Email Verification Token
            string token = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{user.UserId}#{user.Email}#{verificationCode}"));

            _emailService.VerifyUser(form.Email, token);


            result.Data = new UserView(user);
            result.Message = "User Created";

            return result;
        }


        /// <summary>
        /// Resends the verification email to a user with the specified email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>A Task object representing the asynchronous operation that returns A <see cref="ServiceResult"/>  object.</returns>
        public async Task<ServiceResult> ResendVerificationMailAsync(string email)
        {
            // Initialize a new ServiceResult object to hold the result of the method.
            ServiceResult result = new();

            // Find the user with the specified email address using a user repository object from the unit of work.
            User? user = await _uow.UserRepository.FindByEmailAsync(email);

            // If the user is not found, return a not found error.
            if (user == null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "User Not Found";

                return result;
            }

            // If the user's status is not "INACTIVE", return a bad request error.
            if (user.Status != User.UserStatus.INACTIVE)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid User";

                return result;
            }

            // Generate a unique verification code to use in the email verification process.
            Guid verificationCode = Guid.NewGuid();

            // Generate an email verification token by encoding the user ID, email address, and verification code in a Base64 string.
            string token = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{user.UserId}#{user.Email}#{verificationCode}"));

            // Use the email service to send the verification email to the user.
            _emailService.VerifyUser(user.Email, token);

            // Update the user's verification code and updated date in the user repository.
            user.VerificationCode = verificationCode.ToString();
            user.UpdatedDate = DateTime.Now;
            _uow.UserRepository.Update(user);

            // Save the changes to the database.
            await _uow.SaveAsync();

            // Set the message property of the ServiceResult object to indicate that the email was sent successfully.
            result.Message = "Email Sent Successfully";

            // Return the ServiceResult object.
            return result;
        }


        /// <summary>
        /// Verifies a user's email using a verification token.
        /// </summary>
        /// <param name="token">The verification token to use.</param>
        /// <returns>A task that represents the asynchronous verification operation. The task result contains a <see cref="ServiceResult"/> object.</returns>
        public async Task<ServiceResult> VerifyUserAsync(string token)
        {
            // Initialize a new ServiceResult object to return the result of the verification operation.
            ServiceResult result = new();

            // Declare variables to store user data extracted from the verification token.
            int userId;
            string email, verificationCode;

            // Get Data from Email Verification Token
            try
            {
                // Decode the verification token from base64 and split the data into its components.
                var data = Encoding.Unicode.GetString(Convert.FromBase64String(token)).Split("#");

                // Convert the user ID from string to integer and assign it to the userId variable.
                userId = Convert.ToInt32(data[0]);

                // Assign the email address to the email variable.
                email = data[1];

                // Assign the verification code to the verificationCode variable.
                verificationCode = data[2];
            }
            catch (Exception)
            {
                // If there was an error decoding the verification token or parsing its data, return a bad request error.
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Token";

                return result;
            }

            // Check if Token id Valid
            // Find the user with the specified email and verification code.
            var user = await _uow.UserRepository.FindByEmailAndVerificationCode(email, verificationCode);

            if (user == null)
            {
                // If there is no matching user, return a bad request error.
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Token";

                return result;
            }

            if (DateTime.Now - user.UpdatedDate > TimeSpan.FromMinutes(10))
            {
                // If the verification token has expired, return a bad request error.
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Token Expired";

                return result;
            }

            // Update the user's verification code, status, and updated date in the database.
            user.VerificationCode = null;
            user.Status = User.UserStatus.ACTIVE;
            user.UpdatedDate = DateTime.Now;

            _uow.UserRepository.Update(user);

            await _uow.SaveAsync();

            // Return a success message.
            result.Message = "User verified";

            return result;
        }

        public async Task<ServiceResult> ResendVerificationMailByToken(string token)
        {
            // Initialize a new ServiceResult object to return the result of the verification operation.
            ServiceResult result = new();

            // Declare variables to store user data extracted from the verification token.
            int userId;
            string email, verificationCode;

            // Get Data from Email Verification Token
            try
            {
                // Decode the verification token from base64 and split the data into its components.
                var data = Encoding.Unicode.GetString(Convert.FromBase64String(token)).Split("#");

                // Convert the user ID from string to integer and assign it to the userId variable.
                userId = Convert.ToInt32(data[0]);

                // Assign the email address to the email variable.
                email = data[1];

                // Assign the verification code to the verificationCode variable.
                verificationCode = data[2];
            }
            catch (Exception)
            {
                // If there was an error decoding the verification token or parsing its data, return a bad request error.
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Token";

                return result;
            }

            // Check if Token id Valid
            // Find the user with the specified email.
            var user = await _uow.UserRepository.FindByEmailAsync(email);

            if (user == null || user.Status != User.UserStatus.INACTIVE)
            {
                // If there is no matching user, return a bad request error.
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Token";

                return result;
            }

            // Generate a unique verification code to use in the email verification process.
            Guid newVerificationCode = Guid.NewGuid();

            // Generate an email verification token by encoding the user ID, email address, and verification code in a Base64 string.
            string newToken = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{user.UserId}#{user.Email}#{newVerificationCode}"));

            // Use the email service to send the verification email to the user.
            _emailService.VerifyUser(user.Email, newToken);

            // Update the user's verification code and updated date in the user repository.
            user.VerificationCode = newVerificationCode.ToString();
            user.UpdatedDate = DateTime.Now;
            _uow.UserRepository.Update(user);

            // Save the changes to the database.
            await _uow.SaveAsync();

            // Set the message property of the ServiceResult object to indicate that the email was sent successfully.
            result.Message = "Email Sent Successfully";

            // Return the ServiceResult object.
            return result;
        }

        ///<summary>
        /// Retrieves the user's information from the database.
        ///</summary>
        /// <param name="userId">The ID of the user to retrieve</param>
        /// <returns>A <see cref="ServiceResult"/>  object wrapped in a Task</returns>
        public async Task<ServiceResult> GetUserAsync(int userId)
        {
            ServiceResult result = new();

            // Find user with matching ID
            var user = await _uow.UserRepository.FindById(userId);

            if (user == null)
            {
                // User not found
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "User not found";
                return result;
            }

            // Create and return a UserDetailView object
            result.Data = new UserDetailView(user);
            return result;
        }


        ///<summary>
        /// Updates the user's information in the database.
        ///</summary>
        /// <param name="userId">The ID of the user to update</param>
        /// <param name="form">The updated user information</param>
        /// <returns>A <see cref="ServiceResult"/>  object wrapped in a Task</returns>
        public async Task<ServiceResult> EditAsync(int userId, UserUpdateForm form)
        {
            ServiceResult result = new();

            // Find user with matching ID
            var user = await _uow.UserRepository.FindById(userId);

            if (user == null)
            {
                // User not found
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "User not found";
                return result;
            }

            // Update user information
            user.FirstName = form.FirstName.Trim();
            user.LastName = form.LastName?.Trim();
            user.Address = form.Address?.Trim();
            user.City = form.City?.Trim();
            user.District = form.District?.Trim();
            user.State = form.State?.Trim();
            user.PhoneNumber = form.PhoneNumber;

            // Save changes to the database
            _uow.UserRepository.Update(user);
            await _uow.SaveAsync();

            // Create and return a UserDetailView object
            result.Data = new UserDetailView(user);
            return result;
        }


        ///<summary>
        /// Generates a forgot password token and sends an email to the user with the token.
        ///</summary>
        /// <param name="email">The email address of the user requesting the password reset</param>
        /// <returns>A <see cref="ServiceResult"/>  object wrapped in a Task</returns>
        public async Task<ServiceResult> ForgotPasswordRequestAsync(string email)
        {
            ServiceResult result = new();

            // Find user with matching email and active status
            var user = await _uow.UserRepository.FindByEmailAndStatus(email, User.UserStatus.ACTIVE);

            if (user == null)
            {
                // User not found
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "User Not Found";
                return result;
            }

            // Generate verification code for forgot password token
            string verificationCode = $"{Guid.NewGuid()}${DateTime.Now}";
            user.VerificationCode = verificationCode;
            _uow.UserRepository.Update(user);

            // Save changes to the database
            await _uow.SaveAsync();

            // Generate forgot password token
            string token = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{user.UserId}#{user.Email}#{verificationCode}"));

            // Send forgot password email with token
            _emailService.ForgotPassword(user.Email, token);

            // Return success message
            result.Message = "Request sent successfully";
            return result;
        }


        ///<summary>
        /// Resets the password of a user based on a forgot password token.
        ///</summary>
        /// <param name="form">A ForgotPasswordForm object containing the token and new password</param>
        /// <returns>A <see cref="ServiceResult"/>  object wrapped in a Task</returns>
        public async Task<ServiceResult> ResetPasswordAsync(ForgotPasswordForm form)
        {
            ServiceResult result = new();

            // Declare variables for data from the forgot password token
            int userId;
            string email, verificationCode;
            DateTime tokenGeneratedTime;

            // Getting Data From Forgot Password Token
            try
            {
                // Decode and parse data from the token
                var data = Encoding.Unicode.GetString(Convert.FromBase64String(form.Token)).Split(new char[] { '#' });

                userId = Convert.ToInt32(data[0]);
                email = data[1];
                verificationCode = data[2];
                tokenGeneratedTime = DateTime.Parse(verificationCode.Split('$')[1]);
            }
            catch (Exception)
            {
                // Token is invalid
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Token";
                return result;
            }

            // Check for Token Status
            var user = await _uow.UserRepository.FindByEmailAndVerificationCode(email, verificationCode);

            // Token has expired or user is not found
            if (user == null || DateTime.Now - tokenGeneratedTime > TimeSpan.FromMinutes(15))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Token Expired";
                return result;
            }

            // User is not active
            if (user.Status != User.UserStatus.ACTIVE)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid user";
                return result;
            }

            // Update user password and remove verification code
            user.Password = BCrypt.Net.BCrypt.HashPassword(form.Password);
            user.VerificationCode = null;
            user.UpdatedDate = DateTime.Now;

            _uow.UserRepository.Update(user);

            // Save changes to the database
            await _uow.SaveAsync();

            // Return success message
            result.Message = "User Password Changed";
            return result;
        }


        /// <summary>
        /// Uploads a user's profile picture to a specific location and updates the user's record in the database.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile picture is being uploaded.</param>
        /// <param name="image">An ImageForm object containing the image file to upload.</param>
        /// <returns>A <see cref="ServiceResult"/>  object indicating the status of the operation.</returns>
        public async Task<ServiceResult> UploadImageAsync(int userId, ImageForm image)
        {
            ServiceResult result = new();

            try
            {
                // Find the user by ID
                var user = await _uow.UserRepository.FindById(userId);

                if (user == null)
                {
                    // If the user is not found, return an error result
                    result.ServiceStatus = ServiceStatus.NotFound;
                    result.Message = "User not found";

                    return result;
                }

                if (image.File.Length > 0)
                {
                    // Delete any existing profile picture
                    if (user.ProfilePic != null)
                        _fileUtil.DeleteUserProfilePic(user.ProfilePic);

                    // Upload the new profile picture to the appropriate location
                    var fileName = _fileUtil.UploadUserProfilePic(user, image.File);

                    if (fileName == null)
                    {
                        // If the upload fails, return an error result
                        result.ServiceStatus = ServiceStatus.BadRequest;
                        result.Message = "Failed";

                        return result;
                    }

                    // Update the user's record with the new profile picture filename
                    user.ProfilePic = fileName;
                    _uow.UserRepository.Update(user);
                    await _uow.SaveAsync();

                    result.Message = "Success";

                    return result;
                }
                else
                {
                    // If the file is empty, return an error result
                    result.ServiceStatus = ServiceStatus.BadRequest;
                    result.Message = "Failed";

                    return result;
                }
            }
            catch (Exception e)
            {
                // If an exception occurs, return an error result with the exception message
                Console.WriteLine(e);
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Failed: " + e.Message;

                return result;
            }
        }


        /// <summary>
        /// Retrieves a user's profile picture with the given file name, if it exists.
        /// </summary>
        /// <param name="fileName">The file name of the profile picture to retrieve.</param>
        /// <returns>A FileStream object representing the user's profile picture, or null if the file doesn't exist.</returns>
        public async Task<FileStream?> GetProfilePic(string fileName)
        {
            // Check if the profile picture exists in the file system.
            if (!await _uow.UserRepository.IsProfilePicExists(fileName))
                return null;

            // If the profile picture exists, retrieve it using the file utility.
            return _fileUtil.GetUserProfile(fileName);
        }

        /// <summary>
        /// Sends a request for the user to become a seller.
        /// </summary>
        /// <param name="userId">The ID of the user making the request.</param>
        /// <returns>A <see cref="ServiceResult"/>  object indicating the success or failure of the operation.</returns>
        public async Task<ServiceResult> RequsetToSeller(int userId)
        {
            // Create a new ServiceResult object to hold the result of the operation.
            ServiceResult result = new();

            // Find the user with the given ID.
            var user = await _uow.UserRepository.FindById(userId);

            // Check if the user exists.
            if (user == null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "User not found";
                return result;
            }

            // Check if the user is already a seller or has already requested to become a seller.
            if (user.Role == User.UserRole.SELLER || user.Role == User.UserRole.REQUESTED)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "User is already a Seller / Requested";
                return result;
            }

            // Set the user's role to requested and update the updated date.
            user.Role = User.UserRole.REQUESTED;
            user.UpdatedDate = DateTime.Now;
            _uow.UserRepository.Update(user);
            await _uow.SaveAsync();

            // Log the role change.
            _logger.LogInformation("User Role changed");

            // Set the success message for the operation.
            result.Message = "Request completed";
            return result;
        }


        /// <summary>
        /// Allows a user to change their password.
        /// </summary>
        /// <param name="form">A ChangePasswordForm object containing the user's old and new passwords.</param>
        /// <param name="userId">The ID of the user whose password is being changed.</param>
        /// <returns>A <see cref="ServiceResult"/>  object indicating the success or failure of the operation.</returns>
        public async Task<ServiceResult> ChangePasswordAsync(ChangePasswordForm form, int userId)
        {
            // Create a new ServiceResult object to hold the result of the operation.
            ServiceResult result = new();

            // Find the user with the given ID.
            var user = await _uow.UserRepository.FindById(userId);

            // Check if the ConfirmNewPassword matches the NewPassword.
            if (form.ConfirmNewPassword != form.NewPassword)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "ConfirmPassword does not maches NewPassword";
                return result;
            }

            // Check if the NewPassword is the same as the CurrentPassword.
            if (form.CurrentPassword == form.NewPassword)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "NewPassword should not be equal to CurrentPassword";
                return result;
            }

            // Check if the user exists.
            if (user == null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "User not found";
                return result;
            }

            // Check if the user's password is set.
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Password Not Set";
                return result;
            }

            // Check if the user's current password is correct.
            if (!BCrypt.Net.BCrypt.Verify(form.CurrentPassword, user.Password))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Password MissMatch";
                return result;
            }

            // Hash the new password and update the user's password.
            user.Password = BCrypt.Net.BCrypt.HashPassword(form.NewPassword);
            _uow.UserRepository.Update(user);
            await _uow.SaveAsync();

            // Set the success message for the operation.
            result.Message = "Password changed";
            return result;
        }


        /// <summary>
        /// Checks if a given user ID is valid and active.
        /// </summary>
        /// <param name="userId">The ID of the user to check.</param>
        /// <returns>
        /// True if the user with the given ID is valid and active; false otherwise.
        /// </returns>
        public async Task<bool> IsValidActiveUser(int userId)
        {
            // Check if the user with the given ID and ACTIVE status exists in the database.
            if (await _uow.UserRepository.FindByIdAndStatusAsync(userId, User.UserStatus.ACTIVE) != null)
            {
                // If the user exists and is active, return true.
                return true;
            }

            // If the user does not exist or is not active, return false.
            return false;
        }


    }

}
