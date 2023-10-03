namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class LocationView
    {
        public string? Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public LocationView(string? addess, double latitude, double longitude)
        {
            Address = addess;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
