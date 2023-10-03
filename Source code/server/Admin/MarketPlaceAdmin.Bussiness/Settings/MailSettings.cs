namespace MarketPlaceAdmin.Bussiness.Settings
{
    /// <summary>
    /// Class representing the settings needed to send emails.
    /// </summary>
    public class MailSettings
    {
        /// <summary>
        /// The email address of the account used to send emails.
        /// </summary>
        public string Mail { get; set; } = null!;

        /// <summary>
        /// The display name used in the from field when sending emails.
        /// </summary>
        public string DisplayName { get; set; } = null!;

        /// <summary>
        /// The password of the account used to send emails.
        /// </summary>
        public string Pswd { get; set; } = null!;

        /// <summary>
        /// The SMTP server host name used to send emails.
        /// </summary>
        public string Host { get; set; } = null!;

        /// <summary>
        /// The SMTP server port number used to send emails.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The base URL of the client that will consume the emails.
        /// </summary>
        public string ClientBaseUrl { get; set; } = null!;
    }
}
