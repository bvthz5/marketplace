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
using MarketPlaceAdmin.Bussiness.Util.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using System.Linq.Expressions;
using Xunit;

namespace MarketplaceAdminTest.Service
{
    public class AgentServiceTests
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger<AgentService> _logger;
        private readonly IEmailService _emailService;
        private readonly IFileUtil _fileUtil;




        private readonly AgentService _agentService;

        public AgentServiceTests()
        {
            _uow = Substitute.For<IUnitOfWork>();
            _tokenGenerator = Substitute.For<ITokenGenerator>();
            _logger = Substitute.For<ILogger<AgentService>>();
            _emailService = Substitute.For<IEmailService>();
            _fileUtil = Substitute.For<IFileUtil>();


            _agentService = new AgentService(_uow, _emailService, _logger, _tokenGenerator, _fileUtil);
        }

        [Fact]
        public void GeneratePasswords_ShouldReturnValidPassword()
        {
            // Act
            var password = AgentService.GeneratePasswords();

            // Assert
            Assert.NotNull(password);
            Assert.Equal(8, password.Length);
            Assert.Matches(@"[a-z]", password);
            Assert.Matches(@"[A-Z]", password);
            Assert.Matches(@"[0-9]", password);
            Assert.Matches(@"[@$!%*?&#+-]", password);
        }

        [Fact]
        public void PasswordHasCorrectLength()
        {
            // Arrange
            const int minLength = 8;
            const int maxLength = 8;

            // Act
            string password = AgentService.GeneratePasswords();

            // Assert
            Assert.InRange(password.Length, minLength, maxLength);
        }

        [Fact]
        public void PasswordsWithSameLengthAreDifferent()
        {
            // Arrange

            // Act
            string password1 = AgentService.GeneratePasswords();
            string password2 = AgentService.GeneratePasswords();

            // Assert
            Assert.NotEqual(password1, password2);
        }


        [Fact]
        public async Task AddAgentAsync_WithAlreadyExistingEmail_ReturnsAlreadyExistsResult()
        {
            // Arrange
            var form = new AgentRegistrationForm
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                PhoneNumber = "1234567890",
            };

            var createdDate = DateTime.Now;
            var updatedDate = DateTime.Now;

            var existingAgent = new Agent
            {
                AgentId = 1,
                Name = "Existing Agent",
                Email = form.Email,
                PhoneNumber = "0987654321",
                Password = "TestPassword",
                CreatedDate = createdDate,
                UpdatedDate = updatedDate
            };

            _uow.AgentRepository.FindByEmailAsync(form.Email).Returns(existingAgent);

            // Act
            var result = await _agentService.AddAgentAsync(form);

            // Assert
            Assert.Equal(ServiceStatus.AlreadyExists, result.ServiceStatus);
            Assert.Equal("Agent Already Exist", result.Message);

            await _uow.AgentRepository.DidNotReceive().Add(Arg.Any<Agent>());
            await _uow.DidNotReceive().SaveAsync();
            _emailService.DidNotReceive().SendAgentEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task EditAgentAsync_NonExistentAgentId_ReturnsNotFound()
        {
            // Arrange
            int agentId = 1;
            var form = new EditAgentForm { Name = "John Doe", PhoneNumber = "1234567890" };
            Agent? agent = null;
            var uow = Substitute.For<IUnitOfWork>();
            uow.AgentRepository.FindById(agentId).Returns(agent);
            var agentService = new AgentService(uow, _emailService, _logger, _tokenGenerator, _fileUtil);

            // Act
            var result = await agentService.EditAgentAsync(agentId, form);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Agent Not Found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task EditAgentAsync_DeletedAgent_ReturnsNotFound()
        {
            // Arrange
            int agentId = 1;
            var form = new EditAgentForm { Name = "John Doe", PhoneNumber = "1234567890" };
            var agent = new Agent { AgentId = agentId, Name = "Jane Doe", PhoneNumber = "0987654321", Status = Agent.DeliveryAgentStatus.DELETED };
            var uow = Substitute.For<IUnitOfWork>();
            uow.AgentRepository.FindById(agentId).Returns(agent);
            var agentService = new AgentService(uow, _emailService, _logger, _tokenGenerator, _fileUtil);

            // Act
            var result = await agentService.EditAgentAsync(agentId, form);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Agent Not Found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task EditAgentAsync_BlockedAgent_ReturnsNotFound()
        {
            // Arrange
            int agentId = 1;
            var form = new EditAgentForm { Name = "John Doe", PhoneNumber = "1234567890" };
            var agent = new Agent { AgentId = agentId, Name = "Jane Doe", PhoneNumber = "0987654321", Status = Agent.DeliveryAgentStatus.BLOCKED };
            var uow = Substitute.For<IUnitOfWork>();
            uow.AgentRepository.FindById(agentId).Returns(agent);
            var agentService = new AgentService(uow, _emailService, _logger, _tokenGenerator, _fileUtil);
            // Act
            var result = await agentService.EditAgentAsync(agentId, form);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Agent Not Found", result.Message);
            Assert.Null(result.Data);

        }

        [Fact]
        public async Task ChangeAgentStatusAsync_InvalidStatus_ReturnsBadRequest()
        {
            // Arrange
            int agentId = 1;
            byte status = 5;
            var uow = Substitute.For<IUnitOfWork>();
            var agentService = new AgentService(uow, _emailService, _logger, _tokenGenerator, _fileUtil);

            // Act
            var result = await agentService.ChangeAgentStatusAsync(agentId, status);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid status", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task ChangeAgentStatusAsync_AgentNotFound_ReturnsNotFound()
        {
            // Arrange
            int agentId = 1;
            byte status = (byte)Agent.DeliveryAgentStatus.ACTIVE;
            var uow = Substitute.For<IUnitOfWork>();
            uow.AgentRepository.FindById(agentId).Returns((Agent?)null);
            var agentService = new AgentService(uow, _emailService, _logger, _tokenGenerator, _fileUtil);

            // Act
            var result = await agentService.ChangeAgentStatusAsync(agentId, status);

            // Assert
            Assert.Equal(ServiceStatus.NotFound, result.ServiceStatus);
            Assert.Equal("Agent Not Found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task ChangeAgentStatusAsync_AgentAlreadyDeleted_ReturnsBadRequest()
        {
            // Arrange
            int agentId = 1;
            byte status = (byte)Agent.DeliveryAgentStatus.DELETED;
            var agent = new Agent { AgentId = agentId, Name = "John Doe", Email = "johndoe@example.com", Status = Agent.DeliveryAgentStatus.DELETED };
            var uow = Substitute.For<IUnitOfWork>();
            uow.AgentRepository.FindById(agentId).Returns(agent);
            var agentService = new AgentService(uow, _emailService, _logger, _tokenGenerator, _fileUtil);

            // Act
            var result = await agentService.ChangeAgentStatusAsync(agentId, status);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Agent Deleted", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            string email = "johndoe@example.com";
            string password = "invalidPassword";
            var uow = Substitute.For<IUnitOfWork>();
            var logger = Substitute.For<ILogger<AgentService>>();
            var tokenGenerator = Substitute.For<ITokenGenerator>();
            var service = new AgentService(uow, _emailService, logger, tokenGenerator, _fileUtil);

            // Act
            var result = await service.Login(email, password);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task Login_AgentNotFound_ReturnsBadRequest()
        {
            // Arrange
            string email = "johndoe@example.com";
            string password = "validPassword";
            var uow = Substitute.For<IUnitOfWork>();
            uow.AgentRepository.FindByEmailAsync(email).Returns((Agent?)null);
            var logger = Substitute.For<ILogger<AgentService>>();
            var tokenGenerator = Substitute.For<ITokenGenerator>();
            var service = new AgentService(uow, _emailService, logger, tokenGenerator, _fileUtil);

            // Act
            var result = await service.Login(email, password);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Credentials", result.Message);
            Assert.Null(result.Data);
        }


        [Fact]
        public async Task InvalidAgentId()
        {
            // Arrange
            var form = new ChangePasswordForm { CurrentPassword = "oldpassword", NewPassword = "Newp@ssword" };
            int agentId = 1;
            var uow = Substitute.For<IUnitOfWork>();
            uow.AgentRepository.FindById(agentId).Returns((Agent?)null);

            // Act
            var result = await _agentService.ChangePassword(form, agentId);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Agent Not Found", result.Message);

        }
        [Fact]
        public async Task Valid_Id_with_Active_Status()
        {
            // Arrange
            int agentId = 1;
            string urlPath = "/api/agent";
            var agent = new Agent { AgentId = agentId, Name = "John Doe", Email = "johndoe@example.com", Status = Agent.DeliveryAgentStatus.ACTIVE };
            _uow.AgentRepository.FindById(agentId).Returns(Task.FromResult<Agent?>(agent));


            // Act
            bool result = await _agentService.IsValidAgent(agentId, urlPath);

            // Assert
            Assert.True(result);

        }

        [Fact]
        public async Task Invalid_Agent_Notfound()
        {
            // Arrange
            int agentId = 2;
            string urlPath = "/api/agent";
            Agent? agent = null;
            var uow = Substitute.For<IUnitOfWork>();
            uow.AgentRepository.FindById(agentId).Returns(Task.FromResult(agent));


            // Act
            bool result = await _agentService.IsValidAgent(agentId, urlPath);

            // Assert
            Assert.False(result);

        }

        [Fact]
        public async Task Invalid_Agent_Invalid_Status()
        {
            // Arrange
            int agentId = 3;
            string urlPath = "/api/agent";
            var agent = new Agent { AgentId = agentId, Name = "John Doe", Email = "johndoe@example.com", Status = Agent.DeliveryAgentStatus.PASSWORD_NOT_CHANGED };
            var uow = Substitute.For<IUnitOfWork>();
            uow.AgentRepository.FindById(agentId).Returns(Task.FromResult<Agent?>(agent));


            // Act
            bool result = await _agentService.IsValidAgent(agentId, urlPath);

            // Assert
            Assert.False(result);

        }

        [Fact]
        public async Task Invalid_Agent_Status_InvalidUrl()
        {
            // Arrange
            int agentId = 5;
            string urlPath = "/api/agent";
            var agent = new Agent { AgentId = agentId, Name = "John Doe", Email = "johndoe@example.com", Status = Agent.DeliveryAgentStatus.PASSWORD_NOT_CHANGED };
            var uow = Substitute.For<IUnitOfWork>();
            uow.AgentRepository.FindById(agentId).Returns(Task.FromResult<Agent?>(agent));


            // Act
            bool result = await _agentService.IsValidAgent(agentId, urlPath);

            // Assert
            Assert.False(result);

        }

        [Fact]
        public async Task Refresh_ExpiredToken_ReturnsUnauthorized()
        {
            // Arrange
            var token = "expired_token";

            _tokenGenerator.GetAdminIdAndTokenData(token).Throws(new SecurityTokenExpiredException("Token expired"));

            // Act
            var result = await _agentService.AgentRefresh(token);

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
            var result = await _agentService.AgentRefresh(token);

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
            _uow.AgentRepository.FindById(1).ReturnsNull();

            // Act
            var result = await _agentService.AgentRefresh(token);

            // Assert
            Assert.Equal(ServiceStatus.Unauthorized, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);
        }

        [Fact]
        public async Task Invalid_token_results_in_a_bad_request_response()
        {
            // Arrange
            var form = new ForgotPasswordForm() { Token = "invalid_token" };
            _uow.AgentRepository.FindByEmailAndVerificationCode(Arg.Any<string>(), Arg.Any<string>()).Returns((Agent?)null);

            // Act
            var result = await _agentService.ResetPassword(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);
        }

        [Fact]
        public async Task Blocked_agent_results_in_a_bad_request_response()
        {
            // Arrange
            var form = new ForgotPasswordForm() { Token = "valid_token" };
            var agent = new Agent() { AgentId = 1, Email = "agent@test.com", VerificationCode = "valid_verification_code", Status = Agent.DeliveryAgentStatus.BLOCKED };
            _uow.AgentRepository.FindByEmailAndVerificationCode("agent@test.com", "valid_verification_code").Returns(agent);

            // Act
            var result = await _agentService.ResetPassword(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);

        }

        [Fact]
        public async Task Expired_token_results_in_a_bad_request_response()
        {
            // Arrange
            var form = new ForgotPasswordForm() { Token = "expired_token" };
            var agent = new Agent() { AgentId = 1, Email = "agent@test.com", VerificationCode = "expired_verification_code", Status = Agent.DeliveryAgentStatus.PASSWORD_NOT_CHANGED, UpdatedDate = DateTime.Now.AddMinutes(-20) };
            _uow.AgentRepository.FindByEmailAndVerificationCode("agent@test.com", "expired_verification_code").Returns(agent);

            // Act
            var result = await _agentService.ResetPassword(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Token", result.Message);

        }

        // Agent List 
        [Fact]
        public async Task AgentListAsync_WithValidParams_ReturnsPaginatedAgentList()
        {
            // Arrange
            var form = new AgentPaginationParams
            {
                Status = new List<byte?>() { (byte)Agent.DeliveryAgentStatus.ACTIVE },
                SortBy = "Name",
                SortByDesc = true,
                PageNumber = 1,
                PageSize = 10,
                Search = "test"
            };

            _uow.AgentRepository.ColumnMapForSortBy.Returns(new Dictionary<string, Expression<Func<Agent, object>>>()
            {
                ["Id"] = agent => agent.AgentId,
                ["Name"] = agent => agent.Name,
                ["Email"] = agent => agent.Email,
                ["Status"] = agent => agent.Status
            });

            var agents = new List<Agent>
            {
               new Agent { AgentId = 1, Name = "John Doe", Email = "john.doe@example.com", Status = Agent.DeliveryAgentStatus.ACTIVE},
               new Agent { AgentId = 2, Name = "Jane Smith", Email = "jane.smith@example.com", Status =  Agent.DeliveryAgentStatus.ACTIVE }
            };

            _uow.AgentRepository.FindAllByStatusAndNameOrEmailLikeAsync(
                Arg.Any<Agent.DeliveryAgentStatus[]>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<bool>()
            ).Returns(Task.FromResult(agents));

            // Act
            var result = await _agentService.AgentListAsync(form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Agent List", result.Message);
            Assert.IsType<Pager<AgentView>>(result.Data);

            var paginatedAgents = (Pager<AgentView>)result.Data;
            Assert.Equal(2, paginatedAgents.TotalItems);
            Assert.Equal(1, paginatedAgents.TotalPages);
            Assert.Equal(10, paginatedAgents.PageSize);
            Assert.Equal(2, paginatedAgents.Result.Count());
        }

        [Fact]
        public async Task AgentListAsync_ReturnsBadRequest()
        {
            // Arrange
            var form = new AgentPaginationParams
            {
                Status = new List<byte?>() { (byte)Agent.DeliveryAgentStatus.ACTIVE, 99 },
                SortBy = "Name",
                SortByDesc = true,
                PageNumber = 1,
                PageSize = 10,
                Search = "test"
            };

            // Act
            var result = await _agentService.AgentListAsync(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Status Value", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task AgentListAsync_WithInvalidStatus_ReturnsBadRequest()
        {
            // Arrange
            var form = new AgentPaginationParams
            {
                Status = new List<byte?> { 10 }
            };
            var expectedMessage = "Invalid Status Value";

            // Act
            var result = await _agentService.AgentListAsync(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal(expectedMessage, result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task AgentListAsync_WithInvalidSortBy_ReturnsBadRequest()
        {
            // Arrange
            var form = new AgentPaginationParams
            {
                SortBy = "InvalidSortBy"
            };

            _uow.AgentRepository.ColumnMapForSortBy.Returns(new Dictionary<string, Expression<Func<Agent, object>>>()
            {
                ["AgentId"] = agent => agent.AgentId,
                ["Name"] = agent => agent.Name,
                ["Email"] = agent => agent.Email,
                ["Status"] = agent => agent.Status
            });

            // Act
            var result = await _agentService.AgentListAsync(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("SortBy : Accepts [AgentId, Name, Email, Status] values only", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task AgentListAsync_WithNullSortBy_ReturnsSuccess()
        {
            // Arrange
            var form = new AgentPaginationParams
            {
                SortBy = null
            };

            _uow.AgentRepository.ColumnMapForSortBy.Returns(new Dictionary<string, Expression<Func<Agent, object>>>()
            {
                ["Name"] = agent => agent.Name,
                ["Email"] = agent => agent.Email,
                ["Status"] = agent => agent.Status
            });

            var agents = new List<Agent>
            {
                new Agent { AgentId = 1, Name = "Agent 1", Email = "agent1@example.com", Status =  Agent.DeliveryAgentStatus.ACTIVE },
                new Agent { AgentId = 2, Name = "Agent 2", Email = "agent2@example.com", Status =  Agent.DeliveryAgentStatus.PASSWORD_NOT_CHANGED }
            };

            _uow.AgentRepository.FindAllByStatusAndNameOrEmailLikeAsync(Arg.Any<Agent.DeliveryAgentStatus[]>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>())
                .Returns(Task.FromResult(agents));

            // Act
            var result = await _agentService.AgentListAsync(form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Agent List", result.Message);
            Assert.NotNull(result.Data);
        }

        // AgentListAsync_WithEmptySearch_ReturnsSuccess
        [Fact]
        public async Task AgentListAsync_WithEmptySearch_ReturnsSuccess()
        {
            // Arrange
            var form = new AgentPaginationParams
            {
                Search = "",
                PageSize = 10,
                PageNumber = 1
            };

            _uow.AgentRepository.ColumnMapForSortBy.Returns(new Dictionary<string, Expression<Func<Agent, object>>>()
            {
                ["Name"] = agent => agent.Name,
                ["Email"] = agent => agent.Email,
                ["PhoneNumber"] = agent => agent.PhoneNumber,
                ["Status"] = agent => agent.Status
            });

            var agents = new List<Agent>
            {
                new Agent { AgentId = 1, Name = "Agent 1", Email = "agent1@example.com", PhoneNumber = "1234567890", Status =  Agent.DeliveryAgentStatus.ACTIVE },
                new Agent { AgentId = 2, Name = "Agent 2", Email = "agent2@example.com", PhoneNumber = "1234567890", Status = Agent.DeliveryAgentStatus.PASSWORD_NOT_CHANGED }
            };

            _uow.AgentRepository.FindAllByStatusAndNameOrEmailLikeAsync(Arg.Any<Agent.DeliveryAgentStatus[]>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>())
                .Returns(Task.FromResult(agents));

            // Act
            var result = await _agentService.AgentListAsync(form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Agent List", result.Message);
            Assert.NotNull(result.Data);
            var pager = (Pager<AgentView>)result.Data;
            Assert.Equal(2, pager.TotalItems);
            Assert.Equal(2, pager.Result.Count());
        }

        // AgentListAsync_WithInvalidStatusType_ReturnsBadRequest
        [Fact]
        public async Task AgentListAsync_WithInvalidStatusType_ReturnsBadRequest()
        {
            // Arrange
            var form = new AgentPaginationParams
            {
                Status = new List<byte?> { 10 },
                PageSize = 10,
                PageNumber = 1
            };

            // Act
            var result = await _agentService.AgentListAsync(form);

            // Assert
            Assert.Equal(ServiceStatus.BadRequest, result.ServiceStatus);
            Assert.Equal("Invalid Status Value", result.Message);
            Assert.Null(result.Data);
        }

        // Forgot Password
        [Fact]
        public async Task ForgotPasswordRequest_SuccessfullySendsEmail()
        {
            // Arrange
            string email = "test@example.com";
            Agent agent = new() { AgentId = 1, Email = email };
            _uow.AgentRepository.FindByEmailAsync(email).Returns(agent);

            // Act
            ServiceResult result = await _agentService.ForgotPassword(email);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Request Send Succesfully", result.Message);
            _emailService.Received().AgentForgotPassword(email, Arg.Any<string>());
            _uow.AgentRepository.Received(1).Update(Arg.Any<Agent>());
            await _uow.Received(1).SaveAsync();
        }

        [Fact]
        public async Task ChangeAgentStatusAsync_ActiveAgent_DoesNotCallDeleteAgentAsync()
        {
            // Arrange
            int agentId = 1;
            var status = Agent.DeliveryAgentStatus.ACTIVE;
            var agent = new Agent() { AgentId = agentId, Status = status };
            _uow.AgentRepository.FindById(agentId).Returns(agent);
            var agentService = new AgentService(_uow, _emailService, _logger, _tokenGenerator, _fileUtil);

            // Act
            var result = await agentService.ChangeAgentStatusAsync(agentId, (byte)status);

            // Assert
            _fileUtil.DidNotReceiveWithAnyArgs().DeleteAgentProfilePic(Arg.Any<string>());
        }

        [Fact]
        public async Task AddAgentAsync_WithValidForm_ReturnsAgentDetailView()
        {
            // Arrange
            var form = new AgentRegistrationForm
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890"
            };
            _uow.AgentRepository.FindByEmailAsync(form.Email).Returns(Task.FromResult<Agent?>(null));
            _uow.AgentRepository.Add(Arg.Any<Agent>()).Returns(Task.FromResult(new Agent
            {
                AgentId = 1,
                Name = form.Name,
                Email = form.Email,
                PhoneNumber = form.PhoneNumber,
                Password = "hashed_password",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            }));
            _emailService.SendAgentEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());


            // Act
            var result = await _agentService.AddAgentAsync(form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Agent Created", result.Message);
            Assert.IsType<AgentDetailView>(result.Data);

            var agentDetail = (AgentDetailView)result.Data;
            Assert.Equal(1, agentDetail.AgentId);
            Assert.Equal(form.Name, agentDetail.Name);
            Assert.Equal(form.Email, agentDetail.Email);
            Assert.Equal(form.PhoneNumber, agentDetail.PhoneNumber);
            Assert.NotNull(agentDetail?.Status);
            Assert.NotNull(agentDetail?.CreatedDate);
            Assert.NotNull(agentDetail?.UpdateDate);
        }

        [Fact]
        public async Task EditAgentAsync_WithValidParams_ReturnsAgentDetailView()
        {
            // Arrange
            var form = new EditAgentForm
            {
                Name = "John Doe",
                PhoneNumber = "1234567890"
            };

            var agent = new Agent { AgentId = 1, Name = "Jane Smith", PhoneNumber = "0987654321", Status = Agent.DeliveryAgentStatus.ACTIVE, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };

            _uow.AgentRepository.FindById(Arg.Any<int>()).Returns(agent);

            // Act
            var result = await _agentService.EditAgentAsync(1, form);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.IsType<AgentDetailView>(result.Data);

            var agentDetailView = (AgentDetailView)result.Data;
            Assert.Equal(agent.AgentId, agentDetailView.AgentId);
            Assert.Equal(form.Name.Trim(), agentDetailView.Name);
            Assert.Equal(form.PhoneNumber, agentDetailView.PhoneNumber);
            Assert.Equal((byte)agent.Status, agentDetailView.Status);
            Assert.NotNull(agentDetailView?.CreatedDate);
            Assert.NotNull(agentDetailView?.UpdateDate);
        }

        [Fact]
        public async Task ChangeAgentStatusAsync()
        {
            // Arrange
            int agentId = 1;
            byte newStatus = (byte)Agent.DeliveryAgentStatus.BLOCKED;
            Agent agent = new()
            {
                AgentId = agentId,
                Status = Agent.DeliveryAgentStatus.ACTIVE,
                Name = "John",
                PhoneNumber = "1234567890",
                Email = "john@example.com",
                CreatedDate = DateTime.Now.AddDays(-1),
                UpdatedDate = DateTime.Now.AddDays(-1)
            };
            _uow.AgentRepository.FindById(Arg.Any<int>()).Returns(agent);

            // Act
            ServiceResult result = await _agentService.ChangeAgentStatusAsync(agentId, newStatus);

            // Assert
            Assert.Equal(ServiceStatus.Success, result.ServiceStatus);
            Assert.Equal("Agent status changed", result.Message);
            Assert.NotNull(result.Data);
            Assert.IsType<AgentDetailView>(result.Data);

            AgentDetailView agentDetailView = (AgentDetailView)result.Data;
            Assert.Equal(agentId, agentDetailView.AgentId);
            Assert.Equal(newStatus, agentDetailView.Status);
            Assert.Equal(agent.Name, agentDetailView.Name);
            Assert.Equal(agent.PhoneNumber, agentDetailView.PhoneNumber);
            Assert.Equal(agent.Email, agentDetailView.Email);
            Assert.Equal(agent.CreatedDate, agentDetailView.CreatedDate);
            Assert.Equal(agent.UpdatedDate, agentDetailView.UpdateDate); // check that UpdatedDate was updated
        }
        [Fact]
        public void Constructor_InitializesAccessTokenAndRefreshTokenProperties()
        {
            // Arrange
            var agent = new Agent { AgentId = 1, Email = "test@example.com", Password = "testpassword" };
            var accessToken = new Token("access_token_value", DateTime.UtcNow.AddHours(1));
            var refreshToken = new Token("refresh_token_value", DateTime.UtcNow.AddMonths(1));

            // Act
            var loginView = new AgentLoginView(agent, accessToken, refreshToken);

            // Assert
            Assert.NotNull(loginView.AccessToken);
            Assert.Equal(accessToken.Value, loginView.AccessToken.Value);
            Assert.Equal(accessToken.Expiry, loginView.AccessToken.Expiry);

            Assert.NotNull(loginView.RefreshToken);
            Assert.Equal(refreshToken.Value, loginView.RefreshToken.Value);
            Assert.Equal(refreshToken.Expiry, loginView.RefreshToken.Expiry);
        }
        [Fact]
        public void Constructor_InitializesAgentDetailViewProperties()
        {
            // Arrange
            var agent = new Agent { AgentId = 1, Email = "test@example.com", Password = "testpassword" };
            var accessToken = new Token("access_token_value", DateTime.UtcNow.AddHours(1));
            var refreshToken = new Token("refresh_token_value", DateTime.UtcNow.AddMonths(1));

            // Act
            var loginView = new AgentLoginView(agent, accessToken, refreshToken);

            // Assert

            Assert.Equal(agent.AgentId, loginView.AgentId);
            Assert.Equal(agent.Name, loginView.Name);
            Assert.Equal(agent.Email, loginView.Email);
            Assert.Equal(agent.PhoneNumber, loginView.PhoneNumber);
            Assert.Equal((byte)agent.Status, loginView.Status);
            Assert.Equal(agent.CreatedDate, loginView.CreatedDate);
            Assert.Equal(agent.UpdatedDate, loginView.UpdateDate);

        }

        [Fact]
        public void TokenViewConstructor_InitializesValueAndExpiryProperties()
        {
            // Arrange
            var token = new Token("test_token_value", DateTime.UtcNow.AddHours(1));

            // Act
            var tokenView = new AgentLoginView.TokenView(token);

            // Assert
            Assert.Equal(token.Value, tokenView.Value);
            Assert.Equal(token.Expiry, tokenView.Expiry);
        }

        [Fact]
        public void Constructor_SetsPropertiesToEmptyStrings_WhenEmptyDeliveryAddressStringIsPassed()
        {
            // Arrange
            string deliveryAddressString = string.Empty;

            // Act
            var deliveryAddress = new DeliveryAddressView(deliveryAddressString);

            // Assert
            Assert.Equal(string.Empty, deliveryAddress.Name);
            Assert.Equal(string.Empty, deliveryAddress.Address);
            Assert.Equal(string.Empty, deliveryAddress.StreetAddress);
            Assert.Equal(string.Empty, deliveryAddress.City);
            Assert.Equal(string.Empty, deliveryAddress.State);
            Assert.Equal(string.Empty, deliveryAddress.ZipCode);
            Assert.Equal(string.Empty, deliveryAddress.Phone);
        }
        [Fact]
        public void Constructor_SetsNameProperty_WhenDeliveryAddressStringHasOnlyName()
        {
            // Arrange
            string deliveryAddressString = "John Smith\b\b\b\b\b\b";

            // Act
            var deliveryAddress = new DeliveryAddressView(deliveryAddressString);

            // Assert
            Assert.Equal("John Smith", deliveryAddress.Name);
            Assert.Equal(string.Empty, deliveryAddress.Address);
            Assert.Equal(string.Empty, deliveryAddress.StreetAddress);
            Assert.Equal(string.Empty, deliveryAddress.City);
            Assert.Equal(string.Empty, deliveryAddress.State);
            Assert.Equal(string.Empty, deliveryAddress.ZipCode);
            Assert.Equal(string.Empty, deliveryAddress.Phone);
        }

    }
}
