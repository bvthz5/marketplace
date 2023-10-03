namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class DeliveryAddressOrderView
    {
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string StreetAddress { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DeliveryAddressOrderView(string deliveryAddress)
        {
            string[] addressComponents = deliveryAddress.Split('\b');

            if (addressComponents.Length == 7)
            {
                Name = addressComponents[0];
                Address = addressComponents[1];
                StreetAddress = addressComponents[2];
                City = addressComponents[3];
                ZipCode = addressComponents[4];
                State = addressComponents[5];
                PhoneNumber = addressComponents[6];
            }
        }
    }
}
