using MarketPlace.DataAccess.Model;

namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class DeliveryAddressView
    {
        public int DeliveryAddressId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string PhoneNumber { get; set; }

        public byte Status { get; set; }

        public DeliveryAddressView(DeliveryAddress deliveryAddress)
        {
            DeliveryAddressId = deliveryAddress.DeliveryAddressId;
            Name = deliveryAddress.Name;
            Address = deliveryAddress.Address;
            StreetAddress = deliveryAddress.StreetAddress;
            PhoneNumber = deliveryAddress.Phone;
            City = deliveryAddress.City;
            State = deliveryAddress.State;
            ZipCode = deliveryAddress.ZipCode;
            Status = (byte)deliveryAddress.Status;
        }
    }
}
