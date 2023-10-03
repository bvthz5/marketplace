namespace MarketPlaceAdmin.Bussiness.Settings
{
    /// <summary>
    /// Represents the Google OAuth 2.0 client ID and client secret settings.
    /// </summary>
    public class GoogleAuthSettings
    {
        /// <summary>
        /// Gets or sets the client ID for Google OAuth 2.0.
        /// </summary>
        /// <remarks>
        /// This value can be obtained by creating a Google Cloud Console project.
        /// </remarks>
        public string ClientId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the client secret for Google OAuth 2.0.
        /// </summary>
        /// <remarks>
        /// This value can be obtained by creating a Google Cloud Console project.
        /// </remarks>
        public string ClientSecret { get; set; } = null!;
    }
}
