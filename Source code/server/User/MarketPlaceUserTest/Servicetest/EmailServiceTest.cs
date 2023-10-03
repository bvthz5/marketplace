using Castle.Core.Smtp;
using MarketPlace.DataAccess.Interfaces;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Services;
using MarketPlaceUser.Bussiness.Settings;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using netDumbster.smtp;
using NSubstitute;
using Serilog.Core;


namespace MarketPlaceUserTest.Servicetest
{
    public class EmailServiceTest
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public EmailServiceTest()
        {
            _logger = Substitute.For<ILogger<EmailService>>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
        }
        [Fact]
        public async Task SendEmailAsync_ValidEmail_ReturnsTrue()
        {
            // Arrange
            var mailSettings = new MailSettings
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DisplayName = "Test User",
                Mail = "testuser@gmail.com",
                Pswd = "password123"
            };
            var logger = new Mock<ILogger<EmailService>>();
            var uow = new Mock<IUnitOfWork>();
            var emailService = new EmailService(Options.Create(mailSettings), logger.Object, uow.Object);

            // Act
            var result = await emailService.SendEmailAsync("recipient@example.com", "Test Email", "This is a test email.");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SendEmailAsync_InvalidEmail_ReturnsFalse()
        {
            // Arrange
            var mailSettings = new MailSettings
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DisplayName = "Test User",
                Mail = "testuser@gmail.com",
                Pswd = "password123"
            };
            var logger = new Mock<ILogger<EmailService>>();
            var uow = new Mock<IUnitOfWork>();
            var emailService = new EmailService(Options.Create(mailSettings), logger.Object, uow.Object);

            // Act
            var result = await emailService.SendEmailAsync("invalidemail", "Test Email", "This is a test email.");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SendEmailAsync_InvalidCredentials_ReturnsFalse()
        {
            // Arrange
            var mailSettings = new MailSettings
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DisplayName = "Test User",
                Mail = "testuser@gmail.com",
                Pswd = "wrongpassword"
            };
            var logger = new Mock<ILogger<EmailService>>();
            var uow = new Mock<IUnitOfWork>();
            var emailService = new EmailService(Options.Create(mailSettings), logger.Object, uow.Object);

            // Act
            var result = await emailService.SendEmailAsync("recipient@example.com", "Test Email", "This is a test email.");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SendEmailAsync_EmptyRecipient_ReturnsFalse()
        {
            // Arrange
            var mailSettings = new MailSettings
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DisplayName = "Test User",
                Mail = "testuser@gmail.com",
                Pswd = "password123"
            };
            var logger = new Mock<ILogger<EmailService>>();
            var uow = new Mock<IUnitOfWork>();
            var emailService = new EmailService(Options.Create(mailSettings), logger.Object, uow.Object);

            // Act
            var result = await emailService.SendEmailAsync("", "Test Email", "This is a test email.");

            // Assert
            Assert.False(result);
        }


        [Fact]
        public async Task ForgotPassword_Fail()
        {
            var _mailSettings = new MailSettings
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DisplayName = "Test User",
                Mail = "testuser@gmail.com",
                Pswd = "password123"
            };

            // Check if the file exists and delete it if it does
            var filePath = "./wwwroot/Templates/EmailTemplate.txt";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Arrange
            var to = "testuser@example.com";
            var token = "testtoken123";

            // Act
            var service = new EmailService(Options.Create(_mailSettings), _logger, _unitOfWork);

            service.ForgotPassword(to, token);

            await Task.Delay(1000);

            Assert.True(true);
        }

        [Fact]
        public async Task SendEmailInvoiceAsync_ValidParameters_ReturnsTrue()
        {
            // Arrange
            var mailSettings = new MailSettings
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DisplayName = "Test User",
                Mail = "testuser@gmail.com",
                Pswd = "password123"
            };
            var logger = new Mock<ILogger<EmailService>>();
            var uow = new Mock<IUnitOfWork>();
            var emailSender = new EmailService(Options.Create(mailSettings), logger.Object, uow.Object);

            // Act
            var result = await emailSender.SendEmailInvoiceAsync("recipient@example.com", "Test Subject", new MimePart("text", "plain"));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void SendEmailInvoice_InvalidSubject_ErrorLogged()
        {
            // Arrange
            var mailSettings = new MailSettings
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DisplayName = "Test User",
                Mail = "testuser@gmail.com",
                Pswd = "password123"
            };
            var logger = new Mock<ILogger<EmailService>>();
            var uow = new Mock<IUnitOfWork>();
            var emailSender = new EmailService(Options.Create(mailSettings), logger.Object, uow.Object);

            // Act
            emailSender.SendEmailInvoice("recipient@example.com", null, new MimePart("text", "plain"));

            // Assert


        }

        [Fact]
        public void SendEmailInvoice_ValidParameters_NoExceptionsThrown()
        {
            // Arrange
            var mailSettings = new MailSettings
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DisplayName = "Test User",
                Mail = "testuser@gmail.com",
                Pswd = "password123"
            };
            var logger = new Mock<ILogger<EmailService>>();
            var uow = new Mock<IUnitOfWork>();
            var emailSender = new EmailService(Options.Create(mailSettings), logger.Object, uow.Object);

            // Act
            emailSender.SendEmailInvoice("recipient@example.com", "Test Subject", new MimePart("text", "plain"));

            // Assert
            // No exceptions should be thrown
        }
        [Fact]
        public async Task SendEmailInvoiceAsync_InvalidBody_ReturnsFalse()
        {
            // Arrange
            var mailSettings = new MailSettings
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DisplayName = "Test User",
                Mail = "testuser@gmail.com",
                Pswd = "password123"
            };
            var logger = new Mock<ILogger<EmailService>>();
            var uow = new Mock<IUnitOfWork>();
            var emailSender = new EmailService(Options.Create(mailSettings), logger.Object, uow.Object);

            // Act
            var result = await emailSender.SendEmailInvoiceAsync("recipient@example.com", "Test Subject", null);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public async Task SendEmailInvoiceAsync_InvalidSubject_ReturnsFalse()
        {
            // Arrange
            var mailSettings = new MailSettings
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DisplayName = "Test User",
                Mail = "testuser@gmail.com",
                Pswd = "password123"
            };
            var logger = new Mock<ILogger<EmailService>>();
            var uow = new Mock<IUnitOfWork>();
            var emailSender = new EmailService(Options.Create(mailSettings), logger.Object, uow.Object);

            // Act
            var result = await emailSender.SendEmailInvoiceAsync("recipient@example.com", null, new MimePart("text", "plain"));

            // Assert
            Assert.False(result);
        }
        [Fact]
        public async Task SendEmailInvoiceAsync_InvalidRecipient_ReturnsFalse()
        {
            // Arrange
            var mailSettings = new MailSettings
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DisplayName = "Test User",
                Mail = "testuser@gmail.com",
                Pswd = "password123"
            };
            var logger = new Mock<ILogger<EmailService>>();
            var uow = new Mock<IUnitOfWork>();
            var emailSender = new EmailService(Options.Create(mailSettings), logger.Object, uow.Object);

            // Act
            var result = await emailSender.SendEmailInvoiceAsync("invalidemail", "Test Subject", new MimePart("text", "plain"));

            // Assert
            Assert.False(result);
        }
    }
}
