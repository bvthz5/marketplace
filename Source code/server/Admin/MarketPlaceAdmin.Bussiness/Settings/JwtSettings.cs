namespace MarketPlaceAdmin.Bussiness.Settings
{
    /// <summary>
    /// Represents JWT settings used for token creation and validation.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// The secret key used for signing and verifying JWT tokens.
        /// </summary>
        public string Key { get; set; } = null!;

        /// <summary>
        /// The issuer of JWT tokens.
        /// </summary>
        public string Issuer { get; set; } = null!;

        /// <summary>
        /// The audience of JWT tokens.
        /// </summary>
        public string Audience { get; set; } = null!;

        /// <summary>
        /// The expiry time of access tokens, in minutes.
        /// </summary>
        public int AccessTokenExpiry { get; set; } = 60;

        /// <summary>
        /// The expiry time of refresh tokens, in minutes.
        /// </summary>
        public int RefreshTokenExpiry { get; set; } = 60 * 24 * 7;
    }

}
