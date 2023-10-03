using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Razorpay.Api;

namespace MarketPlaceUser.Bussiness.Services
{
    public class RazorpayService : IRazorpayService
    {

        private readonly ILogger<OrderService> _logger; // Logger for logging errors
        private readonly RazorpayClient _razorpayClient; // Client for accessing Razorpay payment gateway

        public RazorpayService(ILogger<OrderService> logger, IOptions<RazorPaySettings> razorOptions)
        {
            _logger = logger;
            _razorpayClient = new RazorpayClient(razorOptions.Value.Key, razorOptions.Value.Secret);

        }

        public bool IsOrderCreated(double amount, out string? orderId)
        {
            var options = new Dictionary<string, object>
            {
                { "amount", amount*100 },
                { "currency", "INR" },
                { "receipt", "recipt_1" },
                // auto capture payments rather than manual capture
                // razor pay recommended option
                { "payment_capture", true }
            };
            Order order;
            try
            {
                order = _razorpayClient.Order.Create(options);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error :{ex}", ex.Message);
                orderId = null;
                return false;
            }
            orderId = order["id"];
            return true;
        }

        public bool IsPaymentConfirmed(ConfirmPaymentForm confirmPaymentForm)
        {
            // Verify payment signature using Razorpay utility method
            var attributes = new Dictionary<string, string>
            {
                { "razorpay_payment_id", confirmPaymentForm.RazorpayPaymentId },
                { "razorpay_order_id", confirmPaymentForm.RazorpayOrderId },
                { "razorpay_signature", confirmPaymentForm.RazorpaySignature }
            };

            try
            {
                Utils.verifyPaymentSignature(attributes);

                // Fetch payment details from Razorpay
                var payment = _razorpayClient.Payment.Fetch(confirmPaymentForm.RazorpayPaymentId);

                // If payment status is captured,return true else false
                return (payment["status"] == "captured");

            }
            catch (Exception ex)
            {
                _logger.LogError("Error : {ex}", ex.Message);
                return false;
            }

        }

        public bool Refund(double amount, string orderNumber)
        {
            try
            {
                var refundRequest = new Dictionary<string, object>
                {
                    { "amount",  amount* 100 }, // Razorpay uses paise as the smallest currency unit
                    { "payment_id", orderNumber},
                    { "speedy_refund", true } // Enable speedy refunds to ensure faster processing
                };

                _razorpayClient.Refund.Create(refundRequest);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund");
                return false;
            }
        }
    }
}
