namespace MarketPlaceUser.Bussiness.Settings
{
    public class JwtSettings
    {
        public string Key { get; set; } = null!;

        public string Issuer { get; set; } = null!;

        public string Audience { get; set; } = null!;

        public int AccessTokenExpiry { get; set; } = 60;

        public int RefreshTokenExpiry { get; set; } = 60 * 24 * 7;
    }
}
