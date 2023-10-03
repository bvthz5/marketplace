using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;

namespace MarketPlaceUser.Bussiness.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly GoogleAuth _googleAuth;
        private readonly IUnitOfWork _uow;
        private readonly ITokenGenerator _tokenGenerator;

        public GoogleAuthService(GoogleAuth googleAuth, IUnitOfWork unitOfWork, ITokenGenerator tokenGenerator)
        {
            _uow = unitOfWork;
            _googleAuth = googleAuth;
            _tokenGenerator = tokenGenerator;
        }


        /// <summary>
        /// Registers a new user or logs in an existing user with Google authentication.
        /// </summary>
        /// <param name="idToken">The Google ID token.</param>
        /// <param name="status">The status of the user.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<ServiceResult> RegisterAndLogin(string idToken)
        {
            // Create a new service result object
            ServiceResult result = new();

            // Verify the Google ID token
            var info = await _googleAuth.VerifyGoogleToken(idToken);

            // If the token is invalid, return a bad request response
            if (info == null)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Credentials";

                return result;
            }

            // Find the user in the database by email
            User? user = await _uow.UserRepository.FindByEmailAsync(info.Email);

            if (user == null)
            {
                // Create a new user and add them to the database
                user = await _uow.UserRepository.Add(new User()
                {
                    FirstName = info.GivenName,
                    LastName = info.FamilyName,
                    Email = info.Email,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Role = User.UserRole.USER,
                    Status = User.UserStatus.ACTIVE,

                });

                // Save the changes to the database
                await _uow.SaveAsync();
                result.ServiceStatus = ServiceStatus.Created;
                result.Message = "User Registered";
            }
            else if (user.Status != User.UserStatus.ACTIVE)
            {

                // If the user is inactive, return a bad request response
                if (user.Status == User.UserStatus.INACTIVE)
                {
                    user.Status = User.UserStatus.ACTIVE;
                    _uow.UserRepository.Update(user);
                    await _uow.SaveAsync();
                }
                else
                {
                    result.ServiceStatus = ServiceStatus.BadRequest;
                    result.Message = $"User {user.Status}";
                    return result;
                }
            }
            else
            {
                result.Message = "User LoggedIn";
            }

            // Return a success response with the new user's access and refresh tokens
            result.Data = new LoginView(user, _tokenGenerator.GenerateAccessToken(user), _tokenGenerator.GenerateRefreshToken(user));

            return result;
        }

    }
}
