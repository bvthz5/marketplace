using MarketPlaceAdmin.Bussiness.Services;
using MarketPlaceAdmin.Bussiness.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using netDumbster.smtp;
using NSubstitute;
using Xunit;

namespace MarketplaceAdminTest.Service
{
    public class EmailServiceTests
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailServiceTests()
        {
            // Create a mock MailSettings object
            _mailSettings = new MailSettings
            {
                Host = "localhost",
                Port = 25,
                Mail = "sender@example.com",
                DisplayName = "Test Sender",
                Pswd = "password",
                ClientBaseUrl = "base_url"
            };

            // Create a mock ILogger instance
            _logger = Substitute.For<ILogger<EmailService>>();

        }

        [Fact]
        public async Task SendEmailAsync_ShouldSendEmail_WhenValidArgumentsArePassed()
        {
            // Arrange

            // Create a SimpleSmtpServer instance to mock the SMTP server
            SimpleSmtpServer smtpServer = SimpleSmtpServer.Start(25);

            var emailService = new EmailService(Options.Create(_mailSettings), _logger);
            var to = "recipient@example.com";
            var subject = "Test email";
            var body = "<h1>Hello world!</h1>";

            // Act
            var result = await emailService.SendEmailAsync(to, subject, body);

            // Assert
            Assert.True(result);

            var receivedEmail = smtpServer.ReceivedEmail.SingleOrDefault(r => r.ToAddresses.ElementAt(0).Address == to);

            Assert.NotNull(receivedEmail);

            Assert.Equal(subject, receivedEmail.Subject);

            Assert.Equal(body, receivedEmail.MessageParts[0].BodyData);

            smtpServer.Stop();

        }

        [Fact]
        public async Task SendEmailAsync_Exception()
        {
            // Arrange

            MailSettings settings = new()
            {
                Host = "localhost",
                Port = 100,
                Mail = "sender@example.com",
                DisplayName = "Test Sender",
                Pswd = "password"
            };

            var emailService = new EmailService(Options.Create(settings), _logger);
            var to = "recipient@example.com";
            var subject = "Test email";
            var body = "<h1>Hello world!</h1>";

            // Act
            var result = await emailService.SendEmailAsync(to, subject, body);

            // Assert
            Assert.False(result);

            _logger.ReceivedWithAnyArgs().LogError("Error");

        }

        [Fact]
        public async Task Admin_ForgotPassword_SendsEmailWithCorrectContent()
        {
            SimpleSmtpServer smtpServer = SimpleSmtpServer.Start(25);

            var path = "./wwwroot/Templates";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            File.Copy("../../../../MarketPlaceAdmin.Api/wwwroot/Templates/EmailTemplate.txt", "./wwwroot/Templates/EmailTemplate.txt", true);

            // Arrange
            var to = "forgotpasswordtestuser@example.com";
            var token = "testtoken123";

            var expectedSubject = "Reset Password";
            var expectedHtmlContent = $"<a href=\"{_mailSettings.ClientBaseUrl}forgot-password?token={token}\"";

            // Act

            var service = new EmailService(Options.Create(_mailSettings), _logger);

            service.AdminForgotPassword(to, token);

            await Task.Delay(5000);

            // Assert

            var receivedEmail = smtpServer.ReceivedEmail.SingleOrDefault(r => r.ToAddresses.Length == 1 && r.ToAddresses.ElementAt(0).Address == to);

            Assert.NotNull(receivedEmail);

            Assert.Equal(expectedSubject, receivedEmail.Subject);

            Assert.NotEmpty(receivedEmail.MessageParts[0].BodyData);

            Assert.Contains(expectedHtmlContent, receivedEmail.MessageParts[0].BodyData);

            smtpServer.Stop();
        }

        [Fact]
        public async Task Agent_ForgotPassword_SendsEmailWithCorrectContent()
        {
            SimpleSmtpServer smtpServer = SimpleSmtpServer.Start(25);

            var path = "./wwwroot/Templates";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            File.Copy("../../../../MarketPlaceAdmin.Api/wwwroot/Templates/EmailTemplate.txt", "./wwwroot/Templates/EmailTemplate.txt", true);

            // Arrange
            var to = "forgotpasswordtestagentuser@example.com";
            var token = "testtoken123";

            var expectedSubject = "Reset Password";
            var expectedHtmlContent = $"<a href=\"{_mailSettings.ClientBaseUrl}agent/forgot-password?token={token}\"";


            // Act
            var service = new EmailService(Options.Create(_mailSettings), _logger);

            service.AgentForgotPassword(to, token);

            await Task.Delay(5000);

            var receivedEmail = smtpServer.ReceivedEmail.SingleOrDefault(r => r.ToAddresses.Length == 1 && r.ToAddresses.ElementAt(0).Address == to);

            // Assert

            Assert.NotNull(receivedEmail);

            Assert.Equal(expectedSubject, receivedEmail.Subject);

            Assert.NotEmpty(receivedEmail.MessageParts[0].BodyData);

            Assert.Contains(expectedHtmlContent, receivedEmail.MessageParts[0].BodyData);

            smtpServer.Stop();
        }

        [Fact]
        public async Task ForgotPassword_Fail()
        {

            var file = "./wwwroot/Templates/EmailTemplate.txt";

            if (File.Exists(file))
                File.Delete(file);

            // Arrange
            var to = "testuser@example.com";
            var token = "testtoken123";

            var service = new EmailService(Options.Create(_mailSettings), _logger);

            // Act

            service.AdminForgotPassword(to, token);

            await Task.Delay(2000);

            _logger.ReceivedWithAnyArgs().LogError("Error");

        }


        [Fact]
        public async Task SellerRequest_SendsEmailWithAcceptedCorrectContent()
        {
            // Arrange

            SimpleSmtpServer smtpServer = SimpleSmtpServer.Start(25);

            var to = "sellerrequesttestuser1@example.com";
            var message = "test message";

            var expectedSubject = "Seller Request Accepted";
            var expectedHtmlContent = "Your Seller Request Has been Accepted";

            var service = new EmailService(Options.Create(_mailSettings), _logger);

            // Act

            service.SellerRequest(to, true, message);

            await Task.Delay(5000);

            // Assert

            var receivedEmail = smtpServer.ReceivedEmail.SingleOrDefault(r => r.ToAddresses.Length == 1 && r.ToAddresses.ElementAt(0).Address == to);

            Assert.NotNull(receivedEmail);

            Assert.Equal(expectedSubject, receivedEmail.Subject);

            Assert.NotEmpty(receivedEmail.MessageParts[0].BodyData);

            Assert.Contains(expectedHtmlContent, receivedEmail.MessageParts[0].BodyData);

            smtpServer.Stop();
        }

        [Fact]
        public async Task SellerRequest_SendsEmailWithRejectedCorrectContent()
        {
            // Arrange

            SimpleSmtpServer smtpServer = SimpleSmtpServer.Start(25);

            var to = "sellerrequesttestuser2@example.com";
            var message = "reason for rejection";

            var expectedSubject = "Seller Request Rejected";
            var expectedHtmlContent = "Your Seller Request Has been Rejected";

            var service = new EmailService(Options.Create(_mailSettings), _logger);

            // Act

            service.SellerRequest(to, false, message);

            await Task.Delay(5000);

            var receivedEmail = smtpServer.ReceivedEmail.SingleOrDefault(r => r.ToAddresses.Length == 1 && r.ToAddresses.ElementAt(0).Address == to);

            Assert.NotNull(receivedEmail);

            Assert.Equal(expectedSubject, receivedEmail.Subject);

            Assert.NotEmpty(receivedEmail.MessageParts[0].BodyData);

            Assert.Contains(expectedHtmlContent, receivedEmail.MessageParts[0].BodyData);

            smtpServer.Stop();
        }

        [Fact]
        public async Task ProductRequest_SendsEmailWithAcceptedCorrectContent()
        {
            // Arrange

            SimpleSmtpServer smtpServer = SimpleSmtpServer.Start(25);

            var to = "productrequestuser1@example.com";
            var message = "test message";

            var productName = "Product Name";

            var expectedSubject = "Product Request Accepted";
            var expectedHtmlContent = $"Your Product Request for {productName} Has been Accepted";


            var service = new EmailService(Options.Create(_mailSettings), _logger);

            // Act

            service.ProductRequest(to, 1, productName, true, message);

            await Task.Delay(5000);

            // Assert

            var receivedEmail = smtpServer.ReceivedEmail.SingleOrDefault(r => r.ToAddresses.Length == 1 && r.ToAddresses.ElementAt(0).Address == to);

            Assert.NotNull(receivedEmail);

            Assert.Equal(expectedSubject, receivedEmail.Subject);

            Assert.NotEmpty(receivedEmail.MessageParts[0].BodyData);

            Assert.Contains(expectedHtmlContent, receivedEmail.MessageParts[0].BodyData);
            Assert.Contains(productName, receivedEmail.MessageParts[0].BodyData);

            smtpServer.Stop();
        }

        [Fact]
        public async Task ProductRequest_SendsEmailWithRejectedCorrectContent()
        {
            // Arrange

            SimpleSmtpServer smtpServer = SimpleSmtpServer.Start(25);

            var to = "productrequestuser2@example.com";
            var message = "reason for rejection";

            var productName = "Product Name";

            var expectedSubject = "Product Request Rejected";
            var expectedHtmlContent = $"Your Product Request for {productName} Has been Rejected";

            var service = new EmailService(Options.Create(_mailSettings), _logger);

            // Act

            service.ProductRequest(to, 2, productName, false, message);

            await Task.Delay(5000);

            var receivedEmail = smtpServer.ReceivedEmail.SingleOrDefault(r => r.ToAddresses.Length == 1 && r.ToAddresses.ElementAt(0).Address == to);

            Assert.NotNull(receivedEmail);

            Assert.Equal(expectedSubject, receivedEmail.Subject);

            Assert.NotEmpty(receivedEmail.MessageParts[0].BodyData);

            Assert.Contains(productName, receivedEmail.MessageParts[0].BodyData);
            Assert.Contains(expectedHtmlContent, receivedEmail.MessageParts[0].BodyData);

            smtpServer.Stop();
        }

        [Fact]
        public async Task SendAgentEmailAsync_SendsEmailWithRejectedCorrectContent()
        {
            // Arrange

            SimpleSmtpServer smtpServer = SimpleSmtpServer.Start(25);

            var to = "agnetsendnewlogin@example.com";
            var name = "Agent Name";

            var password = "Super Secure New Password";

            var expectedSubject = "Agent Login Credentials";
            var expectedHtmlContentEmail = $"Email: {to}";
            var expectedHtmlContentPassword = $"Password: {password}";

            var service = new EmailService(Options.Create(_mailSettings), _logger);

            // Act

            service.SendAgentEmailAsync(to, password, name);

            await Task.Delay(5000);

            var receivedEmail = smtpServer.ReceivedEmail.SingleOrDefault(r => r.ToAddresses.Length == 1 && r.ToAddresses.ElementAt(0).Address == to);

            Assert.NotNull(receivedEmail);

            Assert.Equal(expectedSubject, receivedEmail.Subject);

            Assert.NotEmpty(receivedEmail.MessageParts[0].BodyData);

            Assert.Contains(password, receivedEmail.MessageParts[0].BodyData);
            Assert.Contains(expectedHtmlContentEmail, receivedEmail.MessageParts[0].BodyData);
            Assert.Contains(expectedHtmlContentPassword, receivedEmail.MessageParts[0].BodyData);

            smtpServer.Stop();
        }
    }
}
