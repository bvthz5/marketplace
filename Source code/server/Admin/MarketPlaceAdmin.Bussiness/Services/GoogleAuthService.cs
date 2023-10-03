using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using Microsoft.Extensions.Logging;

namespace MarketPlaceAdmin.Bussiness.Services
{
    /// <summary>
    /// This class provides authentication services using Google Sign-In API.
    /// </summary>
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IGoogleAuth _googleAuth;
        private readonly IUnitOfWork _uow;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger<GoogleAuthService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleAuthService"/> class.
        /// </summary>
        /// <param name="googleAuth">The GoogleAuth service used for verifying the ID token.</param>
        /// <param name="unitOfWork">The Unit of Work for database access.</param>
        /// <param name="tokenGenerator">The TokenGenerator service used for generating access and refresh tokens.</param>
        /// <param name="logger">The ILogger used for logging messages.</param>
        public GoogleAuthService(IGoogleAuth googleAuth, IUnitOfWork unitOfWork, ITokenGenerator tokenGenerator, ILogger<GoogleAuthService> logger)
        {
            _uow = unitOfWork;
            _googleAuth = googleAuth;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
        }



        /// <summary>
        /// Google Login Using IdToken
        /// </summary>
        /// <param name="idToken">The ID token provided by the Google Sign-In API.</param>
        /// <returns>A ServiceResult object containing a LoginView object (which contains information about the admin user, access and refresh tokens).</returns>
        public async Task<ServiceResult> Login(string idToken)
        {
            ServiceResult result = new();

            // Verify the ID token using the Google Auth service
            var info = await _googleAuth.VerifyGoogleToken(idToken);

            if (info is not null)
            {
                // Check if there is an admin user with the given email address
                Admin? admin = await _uow.AdminRepository.FindByEmail(info.Email);

                if (admin is not null)
                {
                    // Generate access and refresh tokens for the user
                    Token accessToken = _tokenGenerator.GenerateAccessToken(admin.AdminId, true);
                    Token refreshToken = _tokenGenerator.GenerateRefreshToken(admin.AdminId, admin.Email, admin.Password);

                    // Set the result properties and return the result
                    result.Message = "Success";
                    result.ServiceStatus = ServiceStatus.Success;
                    result.Data = new LoginView(admin, accessToken, refreshToken);

                    // Log the successful login
                    _logger.LogInformation("Google Login Success Email : '{email}'", info.Email);

                    return result;
                }

                // Log the failed login attempt (invalid email address)
                _logger.LogInformation("Invalid Google Login Email : '{email}'", info.Email);
            }
            else
            {
                // Log the failed login attempt (invalid ID token)
                _logger.LogInformation("Invalid Google Login Id Token : '{idToken}'", idToken);
            }

            // Set the result properties and return the result
            result.ServiceStatus = ServiceStatus.BadRequest;
            result.Message = "Invalid Credentials";

            return result;
        }

        public async Task<ServiceResult> AgentLogin(string idToken)
        {
            ServiceResult result = new();

            // Verify the ID token using the Google Auth service
            var info = await _googleAuth.VerifyGoogleToken(idToken);

            if (info is null)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Credentials";
                return result;
            }

            // Check if there is an agent user with the given email address
            Agent? agent = await _uow.AgentRepository.FindByEmailAsync(info.Email);

            if (agent is null)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Agent not found";
                return result;
            }

            if (agent.Status == Agent.DeliveryAgentStatus.BLOCKED)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"Agent {Enum.GetName(typeof(Agent.DeliveryAgentStatus), agent.Status)}";
                return result;
            }

            // Generate access and refresh tokens for the user
            Token accessToken = _tokenGenerator.GenerateAccessToken(agent.AgentId, false);
            Token refreshToken = _tokenGenerator.GenerateRefreshToken(agent.AgentId, agent.Email, agent.Password);

            // Set the result properties and return the result
            result.Message = "Success";
            result.ServiceStatus = ServiceStatus.Success;
            result.Data = new AgentLoginView(agent, accessToken, refreshToken);

            // Log the successful login
            _logger.LogInformation("Google Login Success Email : '{email}'", info.Email);

            return result;

        }
    }
}