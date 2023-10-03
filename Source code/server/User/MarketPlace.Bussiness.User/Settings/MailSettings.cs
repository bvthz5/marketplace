namespace MarketPlaceUser.Bussiness.Settings
{
    public class MailSettings
    {
        public string Mail { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Pswd { get; set; } = null!;
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string ClientBaseUrl { get; set; } = null!;
    }
}
