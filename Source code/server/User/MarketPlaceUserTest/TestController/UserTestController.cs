using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Api.Controllers;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Security;
using MarketPlaceUser.Bussiness.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NSubstitute;
using System.Text;

namespace MarketPlaceUserTest.TestController
{
    public class UserTestController
    {
        private readonly IUserService _userService;
        private readonly ISecurityUtil _securityUtil;
        private readonly UserController _userController;
        public UserTestController()
        {
            _userService = Substitute.For<IUserService>();
            _securityUtil = Substitute.For<ISecurityUtil>();
            _userController = new UserController(_userService, _securityUtil);
        }
        [Fact]
        public async Task AddUserTest()
        {
            //Arrange

            IUserService userService = Substitute.For<IUserService>();

            ISecurityUtil securityUtil = Substitute.For<ISecurityUtil>();

            UserRegistrationForm form = new UserRegistrationForm()
            {
                FirstName = "Anandhu",
                LastName = "Girish",
                Email = "anandhu@gmail.com",
                Password = "Anandhu@123"
            };

            userService.AddUserAsync(form).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            UserController userController = new(userService, securityUtil);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            //Act

            var actualResult = await userController.Add(form) as ObjectResult;

            //Assert

            Assert.Equal(actualResult?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((actualResult?.Value as ServiceResult)?.ServiceStatus, expectedResult.ServiceStatus);

        }
        [Fact]
        public async Task ResendVerifyMailTest()
        {
            //Arrange

            IUserService userService = Substitute.For<IUserService>();

            ISecurityUtil securityUtil = Substitute.For<ISecurityUtil>();

            string email = "anandhu@gmail.com";

            userService.ResendVerificationMailAsync(email).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            UserController userController = new(userService, securityUtil);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            //Act

            var actualResult = await userController.ResendVerificationMail(email) as ObjectResult;

            //Assert

            Assert.Equal(actualResult?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((actualResult?.Value as ServiceResult)?.ServiceStatus, expectedResult.ServiceStatus);

        }
        [Fact]
        public async Task RequestToSellerTest()
        {
            //Arrange

            IUserService userService = Substitute.For<IUserService>();

            ISecurityUtil securityUtil = Substitute.For<ISecurityUtil>();

            securityUtil.GetCurrentUserId().Returns(1);

            userService.RequsetToSeller(1).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            UserController userController = new(userService, securityUtil);

            ServiceResult expectedResult = new() { ServiceStatus = ServiceStatus.Success };

            //Act

            var actualResult = await userController.RequsetToSeller() as ObjectResult;

            //Assert

            Assert.Equal(actualResult?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((actualResult?.Value as ServiceResult)?.ServiceStatus, expectedResult.ServiceStatus);

        }

        [Fact]
        public async Task GetUser()
        {
            // Arrange
            int userId = 10;
            var expectedServiceResult = new ServiceResult()
            {
                ServiceStatus = ServiceStatus.Success,
                Message = "User found"
            };

            IUserService userService = Substitute.For<IUserService>();

            ISecurityUtil securityUtil = Substitute.For<ISecurityUtil>();

            userService.GetUserAsync(userId).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            UserController userController = new(userService, securityUtil);



            // Act
            var result = await userController.GetUser(userId) as ObjectResult;

            // Assert
            Assert.Equal(result?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((result?.Value as ServiceResult)?.ServiceStatus, expectedServiceResult.ServiceStatus);
        }

        [Fact]
        public async Task VerifyTest()
        {
            //Arrange
            var token = "token";

            var expectedServiceResult = new ServiceResult()
            {
                ServiceStatus = ServiceStatus.Success,
                Message = "User verified"
            };

            _userService.VerifyUserAsync(token).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });


            // Act
            var result = await _userController.Verify(token) as ObjectResult;

            // Assert
            Assert.Equal(result?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((result?.Value as ServiceResult)?.ServiceStatus, expectedServiceResult.ServiceStatus);
        }

        [Fact]
        public async Task GetCurrentUserTest()
        {
            //Arrange

            var user = new User
            {
                UserId = 1,
                Email = "test@gmail.com"
            };

            _securityUtil.GetCurrentUserId().Returns(1);
            _userService.GetUserAsync(Arg.Any<int>()).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success, Data = new UserView(user) });

            //Act
            var result = await _userController.GetCurrentUser() as ObjectResult;

            //Assert

            Assert.Equal(result?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((result?.Value as ServiceResult)?.ServiceStatus, ServiceStatus.Success);
            Assert.IsType<UserView>((result?.Value as ServiceResult)?.Data);
        }

        [Fact]
        public async Task UpdateTest()
        {
            //Arrange
            var form = new UserUpdateForm
            {
                Address = "Adrs",
                City = "cty",
                District = "dstct",
                FirstName = "Test",
                LastName = "Test",
                PhoneNumber = "Test",
                State = "Test",
            };

            _securityUtil.GetCurrentUserId().Returns(1);
            _userService.EditAsync(Arg.Any<int>(), form).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            //Act
            var result = await _userController.Update(form) as ObjectResult;

            //Assert
            Assert.Equal(result?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((result?.Value as ServiceResult)?.ServiceStatus, ServiceStatus.Success);
        }

        [Fact]
        public async Task ForgotPasswordTest()
        {
            //Arrange
            var email = "test@gmail.com";

            _userService.ForgotPasswordRequestAsync(email).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            //Act
            var result = await _userController.ForgotPassword(email) as ObjectResult;

            //Assert
            Assert.Equal(result?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((result?.Value as ServiceResult)?.ServiceStatus, ServiceStatus.Success);
        }

        [Fact]
        public async Task ResetPasswordTest()
        {
            //Arrange
            var form = new ForgotPasswordForm
            {
                Password = "Test",
                Token = "token",
            };
            _userService.ResetPasswordAsync(form).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            //Act
            var result = await _userController.ResetPassword(form) as ObjectResult;

            //Assert
            Assert.Equal(result?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((result?.Value as ServiceResult)?.ServiceStatus, ServiceStatus.Success);
        }

        [Fact]
        public async Task ResendVerificationMailByToken_Returns_BadRequest_When_ServiceResult_Has_Errors()
        {
            // Arrange
            string token = "1234";
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _userService.ResendVerificationMailByToken(token).Returns(expectedResult);

            // Act
            var result = await _userController.ResendVerificationMailByToken(token) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ResendVerificationMailByToken_Returns_Ok_When_ServiceResult_Is_Success()
        {
            // Arrange
            string token = "1234";
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _userService.ResendVerificationMailByToken(token).Returns(expectedResult);

            // Act
            var result = await _userController.ResendVerificationMailByToken(token) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }


        [Fact]
        public async Task SetProfilePic_Returns_BadRequest_When_Image_Is_Null()
        {
            // Arrange
            var _userServiceMock = new Mock<IUserService>();
            var _securityUtilMock = new Mock<ISecurityUtil>();
            var _controller = new UserController(_userServiceMock.Object, _securityUtilMock.Object);
            _securityUtilMock.Setup(x => x.GetCurrentUserId()).Returns(1);
            var formFile = new Mock<IFormFile>();
            formFile.Setup(x => x.Length).Returns(10);
            var imageForm = new ImageForm { File = null };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _userServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<int>(), It.IsAny<ImageForm>())).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.SetProfilePic(imageForm) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }


        [Fact]
        public async Task SetProfilePic_Returns_BadRequest_When_Image_Is_Invalid()
        {
            // Arrange
            var _userServiceMock = new Mock<IUserService>();
            var _securityUtilMock = new Mock<ISecurityUtil>();
            var _controller = new UserController(_userServiceMock.Object, _securityUtilMock.Object);
            _securityUtilMock.Setup(x => x.GetCurrentUserId()).Returns(1);
            var formFile = new Mock<IFormFile>();
            formFile.Setup(x => x.Length).Returns(10);
            var imageForm = new ImageForm { File = formFile.Object };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.BadRequest };
            _userServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<int>(), It.IsAny<ImageForm>())).ReturnsAsync(expectedResult);



            // Act
            var result = await _controller.SetProfilePic(imageForm) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task SetProfilePic_Returns_Ok_When_Image_Is_Valid()
        {
            // Arrange
            var _userServiceMock = new Mock<IUserService>();
            var _securityUtilMock = new Mock<ISecurityUtil>();
            var _controller = new UserController(_userServiceMock.Object, _securityUtilMock.Object);
            _securityUtilMock.Setup(x => x.GetCurrentUserId()).Returns(1);
            var formFile = new Mock<IFormFile>();
            formFile.Setup(x => x.Length).Returns(10);
            var imageForm = new ImageForm { File = formFile.Object };
            var expectedResult = new ServiceResult { ServiceStatus = ServiceStatus.Success };
            _userServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<int>(), It.IsAny<ImageForm>())).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.SetProfilePic(imageForm) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedResult.ServiceStatus, ((ServiceResult)result.Value).ServiceStatus);
        }

        [Fact]
        public async Task ChangePassword_Returns_Unauthorized_When_User_Is_Not_Authenticated()
        {
            // Arrange
            var controller = new UserController(_userService, _securityUtil);
            var form = new ChangePasswordForm { CurrentPassword = "Binil@123", NewPassword = "Thz@123", ConfirmNewPassword = "Thz@123" };
            _userService.ChangePasswordAsync(form, _securityUtil.GetCurrentUserId()).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.Success });

            // Act
            var result = await controller.ChangePassword(form) as ObjectResult;

            // Assert

            Assert.Equal(result?.StatusCode, StatusCodes.Status200OK);
            Assert.Equal((result?.Value as ServiceResult)?.ServiceStatus, ServiceStatus.Success);
        }

        [Fact]
        public async Task ChangePassword_Returns_BadRequest_When_ChangePasswordForm_Is_Null()
        {
            // Arrange
            var controller = new UserController(_userService, _securityUtil);
            _userService.ChangePasswordAsync(null, _securityUtil.GetCurrentUserId()).Returns(new ServiceResult() { ServiceStatus = ServiceStatus.BadRequest });

            // Act
            var result = await controller.ChangePassword(null) as ObjectResult;

            // Assert
            Assert.Equal(result?.StatusCode, StatusCodes.Status400BadRequest);
            Assert.Equal((result?.Value as ServiceResult)?.ServiceStatus, ServiceStatus.BadRequest);
        }

        [Fact]
        public async Task ChangePassword_Returns_OK_When_Password_Change_Is_Successful()
        {
            // Arrange
            var controller = new UserController(_userService, _securityUtil);
            var form = new ChangePasswordForm { CurrentPassword = "Binil@123", NewPassword = "Thz@123", ConfirmNewPassword = "Thz@123" };
            var _userServiceMock = new Mock<IUserService>();
            var _securityUtilMock = new Mock<ISecurityUtil>();
            var _controller = new UserController(_userServiceMock.Object, _securityUtilMock.Object);
            _securityUtilMock.Setup(x => x.GetCurrentUserId()).Returns(1);
            _userServiceMock.Setup(x => x.ChangePasswordAsync(It.IsAny<ChangePasswordForm>(), It.IsAny<int>()))
                        .ReturnsAsync(new ServiceResult { ServiceStatus = ServiceStatus.Success });

            // Act
            var result = await _controller.ChangePassword(form);

            // Assert
            Assert.IsType<ObjectResult>(result);
            var serviceResult = ((ObjectResult)result).Value as ServiceResult;
            Assert.NotNull(serviceResult);
            Assert.Equal(ServiceStatus.Success, serviceResult.ServiceStatus);
        }

        [Fact]
        public async Task GetProfile_Returns_NotFound_When_FileStream_Is_Null()
        {
            // Arrange
            var _userServiceMock = new Mock<IUserService>();
            var _controller = new UserController(_userServiceMock.Object, _securityUtil);
            var fileName = "test.jpg";
            _userServiceMock.Setup(x => x.GetProfilePic(fileName)).ReturnsAsync((FileStream)null);

            // Act
            var result = await _controller.GetProfile(fileName) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetProfile_ValidFile_ReturnsFileResult()
        {
            // Arrange
            var fileName = "validfile.jpg";
            var fileStream = new FileStream(fileName, FileMode.Create);
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(s => s.GetProfilePic(fileName)).ReturnsAsync(fileStream);
            var controller = new UserController(mockUserService.Object, _securityUtil);

            // Act
            var result = await controller.GetProfile(fileName);

            // Assert
            Assert.IsType<FileStreamResult>(result);
            var fileResult = result as FileStreamResult;
            Assert.Equal("image/jpeg", fileResult.ContentType);
            // Clean up
            fileStream.Close();
            File.Delete(fileName);
        }


        [Fact]
        public async Task GetProfile_InvalidFile_ReturnsNotFoundResult()
        {
            // Arrange
            var fileName = "invalidfile.jpg";
            FileStream? fileStream = null;
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(s => s.GetProfilePic(fileName)).ReturnsAsync(fileStream);
            var controller = new UserController(mockUserService.Object, _securityUtil);

            // Act
            var result = await controller.GetProfile(fileName);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetProfile_NullFileStream_ReturnsNotFoundResult()
        {
            // Arrange
            var fileName = "nullfilestream.jpg";
            FileStream? fileStream = null;
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(s => s.GetProfilePic(fileName)).ReturnsAsync(fileStream);
            var controller = new UserController(mockUserService.Object, _securityUtil);

            // Act
            var result = await controller.GetProfile(fileName);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }


    }
}
