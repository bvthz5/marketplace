namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class OrderForm
    {
        public int DeliveryAddressId { get; set; }
        public int[] ProductIds { get; set; } = null!;
    }
}
