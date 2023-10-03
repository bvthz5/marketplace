using MailKit.Net.Smtp;
using MailKit.Security;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Text;


namespace MarketPlaceAdmin.Bussiness.Services
{
    /// <summary>
    /// A service for sending email messages.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;

        private readonly ILogger<EmailService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class with the specified MailSettings and ILogger instance.
        /// </summary>
        /// <param name="mailSettings">The MailSettings object containing SMTP server configuration details.</param>
        /// <param name="logger">The ILogger instance for logging errors and messages.</param>
        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Sends an email message to the specified recipient.
        /// </summary>
        /// <param name="to">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the email was sent successfully.</returns>
        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = body };

                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTlsWhenAvailable);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Pswd);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while sending an email to {to}. Subject: {subject}. Body: {body}.{e} ", to, subject, body, e);
                return false;
            }
        }

        public async void ForgotPassword(string to, string token, string route)
        {
            try
            {
                var subject = "Reset Password";

                // Read the email template from file
                var htmlContent = new StringBuilder(await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + @"/wwwroot/Templates/EmailTemplate.txt"));

                // Replace placeholders in the email template with actual values
                htmlContent.Replace("@#title#@", "Reset Password");
                htmlContent.Replace("@#message#@", "Tap the button below to reset your password.");

                htmlContent.Replace("@#token_url#@", $"{_mailSettings.ClientBaseUrl}{route}?token={token}");
                htmlContent.Replace("@#base_url#@", _mailSettings.ClientBaseUrl);

                htmlContent.Replace("@#button#@", "Reset Password");

                htmlContent.Replace("@#name#@", _mailSettings.DisplayName);

                // Send the email using the generated HTML content
                await SendEmailAsync(to, subject, htmlContent.ToString());
            }
            catch (Exception e)
            {
                // Log any errors that occur during email sending
                _logger.LogError("Forgot Password Email Template Error ", e);
            }
        }

        /// <summary>
        /// Sends a password reset email to the specified email address with a unique token.
        /// </summary>
        /// <param name="to">The recipient email address.</param>
        /// <param name="token">The unique token generated for the password reset link.</param>
        public void AdminForgotPassword(string to, string token)
        {
            ForgotPassword(to, token, "forgot-password");
        }

        /// <summary>
        /// Sends a password reset email to the specified email address with a unique token.
        /// </summary>
        /// <param name="to">The recipient email address.</param>
        /// <param name="token">The unique token generated for the password reset link.</param>
        public void AgentForgotPassword(string to, string token)
        {
            ForgotPassword(to, token, "agent/forgot-password");
        }

        /// <summary>
        /// Sends an email to a seller to notify them whether their seller request has been accepted or rejected.
        /// </summary>
        /// <param name="to">The email address of the seller who requested to be a seller.</param>
        /// <param name="accepted">A boolean value indicating whether the request was accepted or rejected. If true, the email will inform the seller that their request has been accepted. If false, the email will inform the seller that their request has been rejected.</param>
        /// <param name="message">An optional string message that can be provided if the request was rejected. The message will be included in the email if the request was accepted, and will be preceded by a statement indicating that the request was rejected if the 'accepted' parameter is false and 'message' is not null or whitespace.</param>
        public async void SellerRequest(string to, bool accepted, string? message)
        {
            // Determine the request status (accepted or rejected) based on the 'accepted' parameter.
            string requestStatus = accepted ? "Accepted" : "Rejected";

            // Construct the subject line of the email.
            string subject = $"Seller Request {requestStatus}";

            // Construct the body of the email, including a greeting and a statement about the status of the request.
            string body = @$"
                Hi,
                <br>
                Your Seller Request Has been {requestStatus} <br>
                ";

            // If the request was rejected and a message was provided, include the reason for rejection in the body of the email.
            if (!string.IsNullOrWhiteSpace(message))
                body += @$" Reason for Rejection : {message}";

            // Send the email asynchronously using the 'SendEmailAsync' method.
            await SendEmailAsync(to, subject, body);

        }

        public async void ProductRequest(string to, int productId, string productName, bool accepted, string? message)
        {
            // Determine the request status (accepted or rejected) based on the 'accepted' parameter.
            string requestStatus = accepted ? "Accepted" : "Rejected";

            // Construct the subject line of the email.
            string subject = $"Product Request {requestStatus}";

            // Construct the body of the email, including a greeting and a statement about the status of the request.
            string body = @$"
                Hi,
                <br>
                Your Product Request for {productName} Has been {requestStatus} <br>
                ";

            // If the request was rejected and a message was provided, include the reason for rejection in the body of the email.
            if (!string.IsNullOrWhiteSpace(message))
                body += @$" Reason for Rejection : {message}";

            // Send the email asynchronously using the 'SendEmailAsync' method.
            await SendEmailAsync(to, subject, body);

        }

        public async void SendAgentEmailAsync(string email, string generatePasswords, string name)
        {
            string subject = "Agent Login Credentials";

            string body = $@"Dear {name},
                <br/><br/>
                Thank you for registering as an agent with MarketPlace.We are excited to have you on board.<br/><br/>

                Please find below your login credentials to access your account:<br/><br/>
                
                Email: {email} <br>
                Password: {generatePasswords}<br/><br/>

                To access your account, please click on the following link: <a href='{_mailSettings.ClientBaseUrl}agent/login'>Click here to Login?</a><br/><br/>

                If you have any questions or concerns, please do not hesitate to contact our support team at support@ invmarketplace4u@gmail.com.<br/><br/>

                Best regards,<br/>
                MarketPlace";

            await SendEmailAsync(email, subject, body);
        }

        public async void DeliveryOtp(string to, string name, string otp, string[] productNamesList)
        {
            string subject = "OTP for Delivery Verification";

            var productNames = string.Join(", <br/><br/>", productNamesList);

            string body = $@"Dear {name},
                <br/><br/>  
                Thank you for purchasing with Marketplace.<br/>

                Use the following OTP to complete the Delivery process. OTP is valid for 10 minutes.

                <h2 style='background: #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;'>{otp}</h2>
               

                Products :<br/> {productNames} <br/> <br/>

                If you have any questions or concerns, please do not hesitate to contact our support team at support@ invmarketplace4u@gmail.com.<br/><br/>

                Best regards,<br/>
                MarketPlace";

            await SendEmailAsync(to, subject, body);
        }
    }
}

