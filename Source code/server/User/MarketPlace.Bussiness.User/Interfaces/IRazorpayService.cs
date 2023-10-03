using MarketPlaceUser.Bussiness.Dto.Forms;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface IRazorpayService
    {
        bool IsOrderCreated(double amount, out string? orderId);

        bool IsPaymentConfirmed(ConfirmPaymentForm confirmPaymentForm);

        bool Refund(double amount, string orderNumber);
    }
}
