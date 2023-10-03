using MailKit.Net.Smtp;
using MailKit.Security;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Text;

namespace MarketPlaceUser.Bussiness.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;

        private readonly ILogger<EmailService> _logger;

        private readonly IUnitOfWork _uow;

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger, IUnitOfWork uow)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Sends an email to the specified recipient with the specified subject and body.
        /// </summary>
        /// <param name="to">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        /// <returns>Returns true if the email was sent successfully; otherwise, false.</returns>
        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // create a new MimeMessage
                var email = new MimeMessage();

                // set the from address to the mail settings display name and email address
                email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));

                // add the recipient's email address to the To field
                email.To.Add(MailboxAddress.Parse(to));

                // set the subject of the email
                email.Subject = subject;

                // set the body of the email as a TextPart with HTML format
                email.Body = new TextPart(TextFormat.Html) { Text = body };

                // create a new SmtpClient and connect to the email server
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);

                // authenticate with the email server using the mail settings email and password
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Pswd);

                // send the email
                await smtp.SendAsync(email);

                // disconnect from the email server
                smtp.Disconnect(true);

                // return true to indicate that the email was sent successfully
                return true;
            }
            catch (Exception e)
            {
                // log the error message and return false to indicate that the email was not sent
                _logger.LogError("Error : {e.Message} {to} {subject} {e}", e.Message, to, subject, e);

                return false;
            }
        }

        /// <summary>
        /// Sends an email to the specified recipient with the specified subject and body.
        /// </summary>
        /// <param name="to">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        /// <returns>Returns true if the email was sent successfully; otherwise, false.</returns>
        public async Task<bool> SendEmailInvoiceAsync(string to, string subject, MimePart body)
        {
            try
            {
                // create a new MimeMessage
                var email = new MimeMessage();

                // set the from address to the mail settings display name and email address
                email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));

                // add the recipient's email address to the To field
                email.To.Add(MailboxAddress.Parse(to));

                // set the subject of the email
                email.Subject = subject;

                //Add the attachment to the message
                email.Body = new Multipart("mixed")
                {
                    body
                };

                // create a new SmtpClient and connect to the email server
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);

                // authenticate with the email server using the mail settings email and password
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Pswd);

                // send the email
                await smtp.SendAsync(email);

                // disconnect from the email server
                smtp.Disconnect(true);

                // return true to indicate that the email was sent successfully
                return true;
            }
            catch (Exception e)
            {
                // log the error message and return false to indicate that the email was not sent
                _logger.LogError("Error : {e.Message} {to} {subject} {e}", e.Message, to, subject, e);

                return false;
            }
        }

        public async void SendEmailInvoice(string to, string subject, MimePart body)
        {
            try
            {
                await SendEmailInvoiceAsync(to, subject, body);

            }
            catch (Exception e)
            {
                _logger.LogError("Error : {e}", e.Message);
            }
        }


        /// <summary>
        /// Sends an email to a user for email verification.
        /// </summary>
        /// <param name="to">The recipient's email address.</param>
        /// <param name="token">The email verification token.</param>
        public async void VerifyUser(string to, string token)
        {
            try
            {
                // Define the email subject.
                var subject = "Email Verification";

                // Read the HTML email template from file.
                var htmlContent = new StringBuilder(await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + @"/wwwroot/Templates/EmailTemplate.txt"));

                // Replace the placeholders in the email template with the actual values.
                htmlContent.Replace("@#title#@", "Email Verification");
                htmlContent.Replace("@#message#@", @"Tap the button below to confirm your email address");

                htmlContent.Replace("@#token_url#@", $"{_mailSettings.ClientBaseUrl}verify?token={token}");
                htmlContent.Replace("@#base_url#@", _mailSettings.ClientBaseUrl);

                htmlContent.Replace("@#button#@", "Click here to Verify");

                htmlContent.Replace("@#name#@", _mailSettings.DisplayName);
                htmlContent.Replace("@#logo_url#@", @"Logo");

                // Send the email.
                await SendEmailAsync(to, subject, htmlContent.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError("Error : {e}", e.Message);
            }
        }


        /// <summary>
        /// Sends an email to notify a user that their product has been sold out.
        /// </summary>
        /// <param name="to">The email address of the user.</param>
        /// <param name="productName">The name of the product that was sold out.</param>
        /// <param name="userName">The name of the user who sold the product.</param>
        public async void ProductSold(string to, string productName, string userName)
        {
            try
            {
                var subject = "Product Sold Out";

                var htmlContent = new StringBuilder(await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + @"/wwwroot/Templates/EmailTemplateProductSold.txt"));

                htmlContent.Replace("@#title#@", "Product Sold Out");

                htmlContent.Replace("@#username#@", userName);
                htmlContent.Replace("@#productname#@", productName);

                htmlContent.Replace("@#name#@", _mailSettings.DisplayName);
                htmlContent.Replace("@#logo_url#@", @"Logo");

                await SendEmailAsync(to, subject, htmlContent.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError("Verify Email Template Error : {e} ", e.Message);
            }
        }

        /// <summary>
        /// Sends an email to notify a user that their order has been confirmed
        /// </summary>
        /// <param name="to">The email address of the recipient</param>
        /// <param name="userName">The username of the recipient</param>
        /// <param name="productList">A list of products purchased by the recipient</param>
        public async void ProductBought(string to, string userName, List<OrderDetails> productList)
        {
            try
            {
                var subject = "Order Confirmed";

                // Read the email template from a file
                var htmlContent = new StringBuilder(await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + @"/wwwroot/Templates/EmailTemplateProductBought.txt"));

                // Read the product list template from a file
                var tempHtmlContent = new StringBuilder(await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + @"/wwwroot/Templates/EmailTemplateProductList.txt"));

                StringBuilder tempHtmlContent2 = new();

                // Loop through the list of products and replace the placeholders in the product list template with the actual values
                foreach (var item in productList)
                {
                    // Get the thumbnail picture of the product
                    string image = "D://Marketplace_Phase2/Resources" + _uow.PhotoRepository.FindThumbnailPicture(item.ProductId)?.Photo;

                    // Replace the placeholders in the product list template with the actual values
                    tempHtmlContent.Replace("@#image#@", image);
                    tempHtmlContent.Replace("@#productname#@", item.Product.ProductName);
                    tempHtmlContent.Replace("@#price#@", item.Product.Price.ToString());
                    tempHtmlContent.Replace("@#productdescription#@", item.Product.ProductDescription);

                    // Append the product list item to the product list template
                    tempHtmlContent2.Append(tempHtmlContent);
                }

                // Replace the placeholders in the email template with the actual values
                htmlContent.Replace("@#title#@", "Order Confirmed");
                htmlContent.Replace("@#username#@", userName);
                htmlContent.Replace("@#productlist#@", tempHtmlContent2.ToString());
                htmlContent.Replace("@#name#@", _mailSettings.DisplayName);
                htmlContent.Replace("@#logo_url#@", @"Logo");

                // Send the email
                await SendEmailAsync(to, subject, htmlContent.ToString());
            }
            catch (Exception e)
            {
                // Log any errors that occur
                _logger.LogError("Verify Email Template Error : {e} ", e.Message);
            }
        }


        /// <summary>
        /// Sends a forgot password email to the specified email address with a password reset token.
        /// </summary>
        /// <param name="to">The email address to send the email to.</param>
        /// <param name="token">The password reset token.</param>
        public async void ForgotPassword(string to, string token)
        {
            try
            {
                // Set email subject
                var subject = "Reset Password";

                // Load email template
                var htmlContent = new StringBuilder(await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + @"/wwwroot/Templates/EmailTemplate.txt"));

                // Replace email template variables with appropriate values
                htmlContent.Replace("@#title#@", "Reset Password");
                htmlContent.Replace("@#message#@", "Tap the button below to reset your password.");

                // Add password reset token to email template
                htmlContent.Replace("@#token_url#@", $"{_mailSettings.ClientBaseUrl}forgot-password?token={token}");
                htmlContent.Replace("@#base_url#@", _mailSettings.ClientBaseUrl);

                // Set button text in email template
                htmlContent.Replace("@#button#@", "Reset Password");

                // Set display name in email template
                htmlContent.Replace("@#name#@", _mailSettings.DisplayName);

                // Send the email
                await SendEmailAsync(to, subject, htmlContent.ToString());
            }
            catch (Exception e)
            {
                // Log any errors
                _logger.LogError("Forgot Password Email Template Error ", e);
            }
        }


    }
}
