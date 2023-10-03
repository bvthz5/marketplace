using MarketPlaceAdmin.Api.Controllers;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Net;
using Xunit;

namespace MarketplaceAdminTest.Controller
{
    public class AgentControllerTests
    {

        private readonly IAgentService _agentService;
        private readonly ISecurityUtil _securityUtil;
        private readonly AgentController _agentController;

        public AgentControllerTests()
        {

            _agentService = Substitute.For<IAgentService>();
            _securityUtil = Substitute.For<ISecurityUtil>();
            _agentController = new AgentController(_agentService, _securityUtil);
        }

        [Fact]
        public async Task Add_WithValidForm_ReturnsSuccess()
        {
            // Arrange
            var form = new AgentRegistrationForm
            {
                Name = "Binil",
                Email = "binilvincent80@gmail.com",
                PhoneNumber = "1234567890"
            };

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _agentService.AddAgentAsync(form).Returns(expectedResult);

            // Act
            var result = await _agentController.Add(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task Add_WithInvalidForm_ReturnsBadRequest()
        {
            // Arrange
            var form = new AgentRegistrationForm
            {
                Name = "",
                Email = "invalid_email",
                PhoneNumber = "invalid_phone_number"
            };

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _agentService.AddAgentAsync(form).Returns(expectedResult);

            // Act
            var result = await _agentController.Add(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task PaginatedAgentList_WithValidParams_ReturnsSuccess()
        {
            // Arrange
            var form = new AgentPaginationParams
            {
                PageNumber = 1,
                PageSize = 10
            };

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _agentService.AgentListAsync(form).Returns(expectedResult);

            // Act
            var result = await _agentController.PaginatedAgentList(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task PaginatedAgentList_WithInvalidParams_ReturnsBadRequest()
        {
            // Arrange
            var form = new AgentPaginationParams
            {
                PageNumber = -1,
                PageSize = 0
            };

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _agentService.AgentListAsync(form).Returns(expectedResult);

            // Act
            var result = await _agentController.PaginatedAgentList(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task EditAgent_WithValidAgentIdAndForm_ReturnsSuccess()
        {
            // Arrange
            var agentId = 1;
            var form = new EditAgentForm
            {
                Name = "Binil",
                PhoneNumber = "1234567890"
            };

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _agentService.EditAgentAsync(agentId, form).Returns(expectedResult);

            // Act
            var result = await _agentController.EditAgent(agentId, form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task EditAgent_WithInvalidAgentId_ReturnsBadRequest()
        {
            // Arrange
            var agentId = -1;
            var form = new EditAgentForm
            {
                Name = "Binil",
                PhoneNumber = "1234567890"
            };

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _agentService.EditAgentAsync(agentId, form).Returns(expectedResult);

            // Act
            var result = await _agentController.EditAgent(agentId, form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ChangeAgentStatus_WithValidAgentIdAndStatus_ReturnsSuccess()
        {
            // Arrange
            int agentId = 1;
            byte status = 1;
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _agentService.ChangeAgentStatusAsync(agentId, status).Returns(expectedResult);

            // Act
            var result = await _agentController.ChangeAgentStatus(agentId, status) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult, result.Value);
        }



        [Fact]
        public async Task ChangePassword_WithValidForm_ReturnsSuccess()
        {
            // Arrange
            var form = new ChangePasswordForm
            {
                CurrentPassword = "cQ1?B&9E",
                NewPassword = "Binil@123"
            };
            int userId = 1;

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _agentService.ChangePassword(form, userId).Returns(expectedResult);
            _securityUtil.GetCurrentUserId().Returns(userId);

            // Act
            var result = await _agentController.ChangePassword(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ChangePassword_WithInvalidForm_ReturnsBadRequest()
        {
            // Arrange
            var form = new ChangePasswordForm
            {
                CurrentPassword = "old_password",
                NewPassword = "new_password",
            };
            int userId = 1;

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _agentService.ChangePassword(form, userId).Returns(expectedResult);
            _securityUtil.GetCurrentUserId().Returns(userId);

            // Act
            var result = await _agentController.ChangePassword(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ChangePassword_WithInvalidUser_ReturnsUnauthorized()
        {
            // Arrange
            var form = new ChangePasswordForm
            {
                CurrentPassword = "old_password",
                NewPassword = "new_password"
            };
            int userId = 1;

            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Unauthorized };
            _agentService.ChangePassword(form, userId).Returns(expectedResult);
            _securityUtil.GetCurrentUserId().Returns(userId);

            // Act
            var result = await _agentController.ChangePassword(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ForgotPassword_ValidEmail_ReturnsSuccess()
        {
            // Arrange
            string email = "test@example.com";
            var expectedServiceResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _agentService.ForgotPassword(email).Returns(expectedServiceResult);

            // Act
            var result = await _agentController.ForgotPassword(email) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedServiceResult, result.Value);
        }

        [Fact]
        public async Task ForgotPassword_InvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            string email = "invalidemail";
            var expectedServiceResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _agentService.ForgotPassword(email).Returns(expectedServiceResult);

            // Act
            var result = await _agentController.ForgotPassword(email) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedServiceResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ForgotPassword_NonexistentEmail_ReturnsNotFound()
        {
            // Arrange
            string nonexistentEmail = "nonexistent@example.com";
            var expectedServiceResult = new ServiceResult { ServiceStatus = ServiceStatus.NotFound };
            _agentService.ForgotPassword(nonexistentEmail).Returns(expectedServiceResult);

            // Act
            var result = await _agentController.ForgotPassword(nonexistentEmail) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(expectedServiceResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ResetPassword_ValidForm_ReturnsOk()
        {
            // Arrange
            var form = new ForgotPasswordForm { Token = "validtoken", Password = "New@password" };
            var expectedServiceResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _agentService.ResetPassword(form).Returns(expectedServiceResult);

            // Act
            var result = await _agentController.ResetPassword(form) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedServiceResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]

        public async Task AgentProfile_Get_Returns_Success()
        {
            //Arrange//
            int agentId = 1;
            var expectedServiceResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _agentService.GetAgent(agentId).Returns(expectedServiceResult);
            _securityUtil.GetCurrentUserId().Returns(agentId);

            //Act//
            var result = await _agentController.GetAgentProfile() as ObjectResult;

            //Assert//
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedServiceResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);

        }

        [Fact]

        public async Task AgentProfilePic_Put_Returns_Success()
        {
            // Arrange

            int agentId = 1;
            ImageForm form = new();
            _securityUtil.GetCurrentUserId().Returns(agentId);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            _agentService.SetProfilePic(agentId, form).Returns(expectedResult);

            // Act

            var result = await _agentController.PutAgentProfile(form) as ObjectResult;


            // Assert

            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]

        public async Task AgentProfilePic_Put_Returns_Success_With_Status_BadRequest()
        {
            // Arrange

            int agentId = 1;
            ImageForm form = new();
            _securityUtil.GetCurrentUserId().Returns(agentId);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.BadRequest };

            _agentService.SetProfilePic(agentId, form).Returns(expectedResult);

            // Act

            var result = await _agentController.PutAgentProfile(form) as ObjectResult;


            // Assert

            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }


        [Fact]

        public async Task AgentProfilePic_Put_Returns_NotFound()
        {
            // Arrange

            int agentId = 1;
            ImageForm form = new();
            _securityUtil.GetCurrentUserId().Returns(agentId);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.NotFound };

            _agentService.SetProfilePic(agentId, form).Returns(expectedResult);

            // Act

            var result = await _agentController.PutAgentProfile(form) as ObjectResult;


            // Assert

            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }


        [Theory]
        [InlineData(200)]
        [InlineData(400)]
        public async Task AgentProfile_Edit(int statusCode)
        {
            //Arrange

            int agentId = 1;
            EditAgentForm form = new();

            _securityUtil.GetCurrentUserId().Returns(agentId);

            ServiceResult expectedResult = new() { ServiceStatus = (ServiceStatus)statusCode };
            _agentService.EditAgentAsync(agentId, form).Returns(expectedResult);

            //Act

            var result = await _agentController.EditAgent(form) as ObjectResult;

            //Assert

            Assert.NotNull(result);

            Assert.Equal(statusCode, result.StatusCode);

            Assert.IsType<ServiceResult>(result.Value);

            Assert.Equal((ServiceStatus)statusCode, ((ServiceResult)result.Value).ServiceStatus);

        }

        [Fact]
        public async Task ProfilePicSuccessTest()
        {
            //Arrange
            string fileName = "Valid File Name with Extension";
            FileStream fs = File.Create("agent test");

            _agentService.GetAgentProfilePic(fileName).Returns(fs);

            //Act
            var actualResult = await _agentController.GetAgentProfilePic(fileName) as FileStreamResult;

            //Assert
            Assert.NotNull(actualResult);
            Assert.NotNull(actualResult.FileStream);
            Assert.True(actualResult.FileStream.CanRead);
            Assert.Equal("image/jpeg", actualResult.ContentType);
        }

        [Fact]
        public async Task ProfilePicFailTest()
        {
            //Arrange
            string fileName = "Valid File Name with Extension";

            _agentService.GetAgentProfilePic(fileName).Returns(Task.FromResult((FileStream?)null));

            //Act
            var actualResult = await _agentController.GetAgentProfilePic(fileName) as NotFoundResult;

            //Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status404NotFound, actualResult.StatusCode);
        }
    }
}



