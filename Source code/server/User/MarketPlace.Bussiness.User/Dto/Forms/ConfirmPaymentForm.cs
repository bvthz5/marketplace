namespace MarketPlaceUser.Bussiness.Dto.Forms
{
    public class ConfirmPaymentForm
    {
        public string RazorpayPaymentId { get; set; } = null!;
        public string RazorpayOrderId { get; set; } = null!;
        public string RazorpaySignature { get; set; } = null!;
    }
}
