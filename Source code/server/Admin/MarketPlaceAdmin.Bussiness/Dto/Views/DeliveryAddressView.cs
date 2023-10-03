namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class DeliveryAddressView
    {
        public string Name { get; } = string.Empty;

        public string Address { get; } = string.Empty;

        public string StreetAddress { get; } = string.Empty;

        public string City { get; } = string.Empty;

        public string State { get; } = string.Empty;

        public string ZipCode { get; } = string.Empty;

        public string Phone { get; } = string.Empty;

        public DeliveryAddressView(string deliveryAddress)
        {
            string[] address = deliveryAddress.Split('\b');

            if (address.Length == 7)
            {
                Name = address[0];
                Address = address[1];
                StreetAddress = address[2];
                City = address[3];
                ZipCode = address[4];
                State = address[5];
                Phone = address[6];
            }
        }
    }
}