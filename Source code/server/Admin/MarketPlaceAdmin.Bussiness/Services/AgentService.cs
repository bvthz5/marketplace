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
using MarketPlaceAdmin.Bussiness.Util.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MarketPlaceAdmin.Bussiness.Services
{
    public class AgentService : IAgentService
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IFileUtil _fileUtil;
        private readonly IEmailService _emailService;
        private readonly ILogger<AgentService> _logger;


        public AgentService(IUnitOfWork uow, IEmailService emailService, ILogger<AgentService> logger, ITokenGenerator tokenGenerator, IFileUtil fileUtil)
        {
            _uow = uow;
            _emailService = emailService;
            _logger = logger;
            _tokenGenerator = tokenGenerator;
            _fileUtil = fileUtil;
        }

        /// <summary>
        /// AddAgentAsync method adds a new Agent to the database with the given Agent Registration Form.
        /// </summary>
        /// <param name="form">A AgentRegistrationForm object representing the form data of the new Agent</param>
        /// <returns>A <see cref="ServiceResult"/>  object containing the status of the operation, a AgentDetailView object of the newly created Agent, and a message.</returns>
        public async Task<ServiceResult> AddAgentAsync(AgentRegistrationForm form)
        {
            ServiceResult result = new();

            Agent? agent = await _uow.AgentRepository.FindByEmailAsync(form.Email);

            if (agent != null)
            {
                result.ServiceStatus = ServiceStatus.AlreadyExists;
                result.Message = "Agent Already Exist";
                return result;
            }

            string password = GeneratePasswords().Trim();

            agent = await _uow.AgentRepository.Add(new Agent()
            {
                Name = form.Name,
                Email = form.Email,
                PhoneNumber = form.PhoneNumber,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            });
            await _uow.SaveAsync();

            _emailService.SendAgentEmailAsync(agent.Email, password, agent.Name);

            result.Data = new AgentDetailView(agent);
            result.Message = "Agent Created";

            return result;
        }
        public static string GeneratePasswords()
        {
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digitChars = "0123456789";
            const string symbolChars = "@$!%*?&";

            // Set the minimum and maximum length of the password
            const int minLength = 8;
            const int maxLength = 8;

            // Create a byte array to hold the password
            byte[] passwordBytes = new byte[maxLength];

            // Generate a cryptographically secure random number generator
            RandomNumberGenerator rng = RandomNumberGenerator.Create();

            // Add a random lowercase character
            byte[] randomBytes = new byte[1];
            rng.GetBytes(randomBytes);
            passwordBytes[0] = (byte)lowercaseChars[randomBytes[0] % lowercaseChars.Length];

            // Add a random uppercase character
            rng.GetBytes(randomBytes);
            passwordBytes[1] = (byte)uppercaseChars[randomBytes[0] % uppercaseChars.Length];

            // Add a random digit
            rng.GetBytes(randomBytes);
            passwordBytes[2] = (byte)digitChars[randomBytes[0] % digitChars.Length];

            // Add a random symbol
            rng.GetBytes(randomBytes);
            passwordBytes[3] = (byte)symbolChars[randomBytes[0] % symbolChars.Length];

            // Add random characters until the password meets the minimum length
            // Initialize i to 4
            int i = 4;

            // Loop while i is less than minLength
            while (i < minLength)
            {
                // Generate random bytes using the rng(Random Number Generator) object
                rng.GetBytes(randomBytes);

                // Convert the random byte to a character
                char randomChar = (char)randomBytes[0];

                // Check if the character is a lowercase letter
                if (lowercaseChars.Contains(randomChar))
                {
                    // Add the character to the passwordBytes array and increment i
                    passwordBytes[i++] = (byte)randomChar;
                }

                // Check if the character is an uppercase letter
                else if (uppercaseChars.Contains(randomChar))
                {
                    // Add the character to the passwordBytes array and increment i
                    passwordBytes[i++] = (byte)randomChar;
                }

                // Check if the character is a digit
                else if (digitChars.Contains(randomChar))
                {
                    // Add the character to the passwordBytes array and increment i
                    passwordBytes[i++] = (byte)randomChar;
                }

                // Check if the character is a symbol
                else if (symbolChars.Contains(randomChar))
                {
                    // Add the character to the passwordBytes array and increment i
                    passwordBytes[i++] = (byte)randomChar;
                }
            }

            // Convert the byte array to a string and truncate it if it exceeds the maximum length
            string password = Encoding.ASCII.GetString(passwordBytes);
            if (password.Length > maxLength)
            {
                password = password[..maxLength];
            }

            return password;
        }


        /// <summary>
        /// Retrieves a paginated list of Agents with search and filter functionality.
        /// </summary>
        /// <param name="form">An object containing the parameters for pagination, search, and filtering.</param>
        /// <returns>A ServiceResult object containing a PagerView of Agents that match the specified search and filter criteria.</returns>
        /// <remarks>
        /// <para>The AgentListAsync method retrieves a list of Agents with pagination, search, and filter functionality. It takes a AgentPaginationParams object as input, which contains the following parameters:</para>
        /// <list type="bullet">
        /// <item><description>PageNumber: The current page number of the Agent list.</description></item>
        /// <item><description>PageSize: The number of Agent to display per page.</description></item>
        /// <item><description>Search: A string to search for in the Agents's name or email.</description></item>
        /// <item><description>Status: An array of byte values that correspond to Agent status values. Only Agents with the specified status values will be returned.</description></item>
        /// <item><description>SortBy: A string representing the column to sort by. Must be one of the values defined in the AgentRepository's ColumnMapForSortBy dictionary.</description></item>
        /// <item><description>SortByDesc: A boolean value indicating whether to sort in descending order.</description></item>
        /// </list>
        /// <para>If any of the parameters are invalid, a ServiceResult object with a BadRequest status and an error message will be returned.</para>
        /// <para>The AgentListAsync method then applies the search and filter criteria to the agent data retrieved from the database and creates a PagerView of AgentDetailView objects to return. Each AgentDetailView object contains a subset of Agent data that is relevant to the Agent list view.</para>
        /// </remarks>
        public async Task<ServiceResult> AgentListAsync(AgentPaginationParams form)
        {
            ServiceResult result = new();

            // Validate Status

            byte[]? status = form.Status?.Where(status => status.HasValue).Cast<byte>().ToArray();

            if (status != null && !status.All(status => Enum.IsDefined(typeof(Agent.DeliveryAgentStatus), status)))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"Invalid Status Value";
                return result;
            }

            // Check if SortBy value if declared and acceptable

            if (form.SortBy != null && !_uow.AgentRepository.ColumnMapForSortBy.ContainsKey(form.SortBy))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"SortBy : Accepts [{string.Join(", ", _uow.AgentRepository.ColumnMapForSortBy.Keys)}] values only";
                return result;
            }
            // Applying Filter and fetching data
            List<Agent> agents = await _uow.AgentRepository.FindAllByStatusAndNameOrEmailLikeAsync(
                                                                                      status?.Cast<Agent.DeliveryAgentStatus>().ToArray(),
                                                                                      form.Search,
                                                                                      form.SortBy,
                                                                                      form.SortByDesc);
            Pager<AgentView> pager = new(form.PageNumber, form.PageSize, agents.Count);
            pager.SetResult(agents.Skip((form.PageNumber - 1) * form.PageSize)
                                  .Take(form.PageSize).ToList()
                                  .ConvertAll(agent => new AgentView(agent)));

            result.Data = pager;
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Agent List";
            return result;
        }

        ///<summary>
        /// Updates the agent's information in the database.
        ///</summary>
        /// <param name="agentId">The ID of the agent to update</param>
        /// <param name="form">The updated agent information</param>
        /// <returns>A <see cref="ServiceResult"/>  object wrapped in a Task</returns>
        public async Task<ServiceResult> EditAgentAsync(int agentId, EditAgentForm form)
        {
            ServiceResult result = new();

            // Find agent with matching ID
            Agent? agent = await _uow.AgentRepository.FindById(agentId);

            //agent not found
            if (agent == null || agent.Status == Agent.DeliveryAgentStatus.BLOCKED || agent.Status == Agent.DeliveryAgentStatus.DELETED)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Agent Not Found";
                return result;
            }

            // Update agent information
            agent.Name = form.Name.Trim();
            agent.PhoneNumber = form.PhoneNumber;

            // Save changes to the database
            _uow.AgentRepository.Update(agent);
            await _uow.SaveAsync();

            // Create and return a AgentDetailView object
            result.Data = new AgentDetailView(agent);
            return result;
        }

        /// <summary>
        /// Changes the status of a agent in the system.
        /// </summary>
        /// <param name="agentId">The ID of the agent whose status will be changed.</param>
        /// <param name="status">The new status code of the agent. Must be one of the following:  1 (Active), 2 (Blocked), or 3 (Deleted).</param>
        /// <returns>A ServiceResult object that includes a AgentDetailView object if the operation was successful.</returns>
        public async Task<ServiceResult> ChangeAgentStatusAsync(int agentId, byte status)
        {
            var result = new ServiceResult();

            // Validate the provided status code
            var validStatusCodes = new List<byte>
            {
                (byte)Agent.DeliveryAgentStatus.ACTIVE,
                (byte)Agent.DeliveryAgentStatus.BLOCKED,
                (byte) Agent.DeliveryAgentStatus.DELETED
            };
            if (!validStatusCodes.Contains(status))
            {

                _logger.LogInformation("Invalid status code {StatusCode} provided for Agent {AgentId}", status, agentId);

                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid status";
                return result;
            }

            // Find the agent with the provided ID
            var agent = await _uow.AgentRepository.FindById(agentId);

            // If the agent is not found, return a NotFound ServiceResult object with an error message
            if (agent == null)
            {

                _logger.LogWarning("Agent {AgentId} not found", agentId);

                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Agent Not Found";
                return result;
            }

            // If the agent is already deleted, return a BadRequest ServiceResult object with an error message
            if (agent.Status == Agent.DeliveryAgentStatus.DELETED)
            {
                _logger.LogWarning("Agent {AgentId} already deleted", agentId);

                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Agent Deleted";
                return result;
            }

            // Update the agent's status and set the updated date to the current date and time
            agent.Status = (Agent.DeliveryAgentStatus)status;
            agent.UpdatedDate = DateTime.Now;

            // If the new status is "DELETED", perform additional actions
            if (status == (byte)Agent.DeliveryAgentStatus.DELETED)
            {
                // Append the agent's email with the current date and time
                agent.Email += $"#{DateTime.Now}";

                agent.Status = Agent.DeliveryAgentStatus.DELETED;

                if (agent.ProfilePic is not null)
                {
                    _fileUtil.DeleteUserProfilePic(agent.ProfilePic);
                    agent.ProfilePic = null;
                }

                _logger.LogInformation("Agent '{agentId}' has been deleted at {deleteTime}.", agentId, DateTime.Now);
            }

            // Update the agent in the database
            _uow.AgentRepository.Update(agent);

            // Save the changes to the database
            await _uow.SaveAsync();

            // Log the status change
            _logger.LogInformation("Agent '{agentId}' status changed to '{status}'", agentId, agent.Status);

            // Return a Success ServiceResult object with a success message and the updated agent's details in a AgentDetailView object
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Agent status changed";
            result.Data = new AgentDetailView(agent);
            return result;
        }

        /// <summary>
        /// Agent Login using the email and password provided through the email.
        /// </summary>
        /// <param name="email">The email of the Agent</param>
        /// <param name="password">The password of the Agent </param>
        /// 
        /// <returns>
        /// ServiceResult object containing the AgentLoginView object with the agent details, access token, and refresh token
        /// </returns>
        public async Task<ServiceResult> Login(string email, string password)
        {
            // Initialize the ServiceResult object
            ServiceResult result = new();

            // Regular expression patterns for validating password and email
            string paswdPattern = PasswordAttribute.Pattern;
            string emailPattern = EmailAttribute.Pattern;

            // Check if the provided email and password are valid
            if (!Regex.IsMatch(password, paswdPattern) || !Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Credentials";
                return result;
            }

            // Find the agent object using the provided email
            Agent? agent = await _uow.AgentRepository.FindByEmailAsync(email);

            // Check if the admin object exists and if the provided password is correct
            if (agent is null || !BCrypt.Net.BCrypt.Verify(password, agent.Password))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Credentials";

                // Log the failed login attempt
                _logger.LogInformation("Invalid Login");

                return result;
            }
            if (agent.Status != Agent.DeliveryAgentStatus.PASSWORD_NOT_CHANGED && agent.Status != Agent.DeliveryAgentStatus.ACTIVE)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"Agent {Enum.GetName(typeof(Agent.DeliveryAgentStatus), agent.Status)}";
                return result;
            }

            // Generate an access token for the agent
            Token accessToken = _tokenGenerator.GenerateAccessToken(agent.AgentId, false);

            // Generate a refresh token for the agent
            Token refreshToken = _tokenGenerator.GenerateRefreshToken(agent.AgentId, agent.Email, agent.Password);

            // Set the ServiceResult object to Success and create a new AgentLoginView object with the agent, access token, and refresh token
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Success";
            result.Data = new AgentLoginView(agent, accessToken, refreshToken);
            return result;
        }

        public async Task<ServiceResult> ChangePassword(ChangePasswordForm form, int agentId)
        {
            // Create a new ServiceResult object to hold the result of the operation.
            ServiceResult result = new();

            // Find the agent with the given ID.
            var agent = await _uow.AgentRepository.FindById(agentId);

            // If no agent user is found with the specified ID, return an error message.
            if (agent is null)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Agent Not Found";
                return result;
            }

            if (form.CurrentPassword == form.NewPassword)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "NewPassword should not be equal to CurrentPassword";
                return result;
            }

            // Verify that the current password provided matches the password stored for the agent user. If not, return an error message.
            if (!BCrypt.Net.BCrypt.Verify(form.CurrentPassword, agent.Password))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Password MissMatch";
                return result;
            }

            // If the current password matches the stored password, hash the new password and update the agent user's password in the database.
            agent.Password = BCrypt.Net.BCrypt.HashPassword(form.NewPassword);

            // Set the status of the agent to ACTIVE
            agent.Status = Agent.DeliveryAgentStatus.ACTIVE;

            // Update the agent object in the database
            _uow.AgentRepository.Update(agent);
            await _uow.SaveAsync();

            // Return a success message.
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Password changed";
            return result;

        }

        public async Task<bool> IsValidAgent(int agentId, string urlPath)
        {
            // Retrieve the Agent object by its ID using the repository
            Agent? agent = await _uow.AgentRepository.FindById(agentId);

            // Check if the agent object is not null and has an Active status,
            // or if the urlPath is "/api/agent/change-password" and the agent has
            // either a Password_Not_Changed or Active status
            return agent is not null && (agent.Status == Agent.DeliveryAgentStatus.ACTIVE || (urlPath == "/api/agent/change-password"
                                     && (agent.Status == Agent.DeliveryAgentStatus.PASSWORD_NOT_CHANGED || agent.Status == Agent.DeliveryAgentStatus.ACTIVE)));
        }

        public async Task<ServiceResult> AgentRefresh(string token)
        {
            Agent agent;

            // Create a new ServiceResult object
            ServiceResult result = new();

            Token refreshToken;

            try
            {
                // Decode the refresh token to get the admin ID and token data
                var data = _tokenGenerator.GetAdminIdAndTokenData(token);

                var refreshTokenData = data[1];

                // Find the admin user by ID
                agent = await _uow.AgentRepository.FindById(Convert.ToInt32(data[0])) ?? throw new Exception("Invalid UserId");

                // Verify the refresh token and get the token object
                refreshToken = _tokenGenerator.VerifyRefreshTokenAgent(refreshTokenData, agent);

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
            var accessToken = _tokenGenerator.GenerateAccessToken(agent.AgentId, false);

            // Return a ServiceResult object with a LoginView object containing the refreshed login details (admin object, new access token, and same refresh token)
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Success";
            result.Data = new AgentLoginView(agent, accessToken, refreshToken);

            // Log the successful refresh and return the ServiceResult object
            _logger.LogInformation("Login token refreshed for agent ID {agentId}", agent.AgentId);

            return result;
        }

        public async Task<ServiceResult> ForgotPassword(string email)
        {
            ServiceResult result = new();

            // Find the agent account associated with the email address provided.
            Agent? agent = await _uow.AgentRepository.FindByEmailAsync(email);

            // If no account is found for the email address and invalid status, return a NotFound status.
            if (agent == null || agent.Status == Agent.DeliveryAgentStatus.BLOCKED || agent.Status == Agent.DeliveryAgentStatus.DELETED)
            {
                _logger.LogInformation("Not Found");

                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Agent not found";
                return result;
            }

            // Generate a unique verification code for password reset and store it in the agent's record.
            string verificationcode = $"{Guid.NewGuid()}${DateTime.Now}";
            agent.VerificationCode = verificationcode;
            _uow.AgentRepository.Update(agent);
            await _uow.SaveAsync();

            // Generate a forgot password token using the agents's ID, email, and verification code
            string token = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{agent.AgentId}#{agent.Email}#{verificationcode}"));

            // Send an email to the agents's email address with the forgot password token as a link.
            _emailService.AgentForgotPassword(agent.Email, token);

            _logger.LogInformation("Password reset link sent to email address '{email}'", email);

            // Set the ServiceResult object to indicate the request was successful.
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Request Send Succesfully";

            return result;
        }

        public async Task<ServiceResult> ResetPassword(ForgotPasswordForm form)
        {
            ServiceResult result = new();

            // Extracting data from the forgot password token
            int agentId;
            string email;
            string verificationCode;
            DateTime tokenGeneratedTime;

            try
            {
                string[] data = Encoding.Unicode.GetString(Convert.FromBase64String(form.Token)).Split('#');

                agentId = Convert.ToInt32(data[0]);
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
            Agent? agent = await _uow.AgentRepository.FindByEmailAndVerificationCode(email, verificationCode);

            if (agent == null || DateTime.Now - tokenGeneratedTime > TimeSpan.FromMinutes(15))
            {
                // Return an error response if the token is expired
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Token Expired";
                return result;

            }

            if (agent.Status == Agent.DeliveryAgentStatus.BLOCKED)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"Agent Blocked";
                return result;
            }

            if (agent.Status == Agent.DeliveryAgentStatus.PASSWORD_NOT_CHANGED)
            {
                agent.Status = Agent.DeliveryAgentStatus.ACTIVE;
            }

            // Update the agent's password and verification code, and save the changes to the database
            agent.Password = BCrypt.Net.BCrypt.HashPassword(form.Password);
            agent.VerificationCode = null;
            agent.UpdatedDate = DateTime.Now;
            _uow.AgentRepository.Update(agent);
            await _uow.SaveAsync();

            // Return a success response
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Password Changed";
            return result;

        }

        public async Task<ServiceResult> GetAgent(int agentId)
        {
            ServiceResult result = new();

            Agent? agent = await _uow.AgentRepository.FindById(agentId);

            if (agent is null)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Agent Not Found";
                return result;
            }

            result.Data = new AgentDetailView(agent);

            return result;
        }

        public async Task<ServiceResult> SetProfilePic(int agentId, ImageForm form)
        {
            ServiceResult result = new();

            Agent? agent = await _uow.AgentRepository.FindById(agentId);

            if (agent is null)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Agent Not Found";
                return result;
            }

            string? fileName = _fileUtil.UploadAgentProfilePic(agentId, form.File);

            if (fileName is null)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Failed";
                return result;
            }

            if (agent.ProfilePic is not null)
                _fileUtil.DeleteAgentProfilePic(agent.ProfilePic);

            agent.ProfilePic = fileName;

            _uow.AgentRepository.Update(agent);
            await _uow.SaveAsync();

            result.Message = "Profile Pic Uploaded";
            return result;
        }


        public async Task<FileStream?> GetAgentProfilePic(string fileName)
        {
            if (!await _uow.AgentRepository.IsProfilePicExists(fileName))
                return null;

            return _fileUtil.GetAgentProfile(fileName);
        }
    }
}
