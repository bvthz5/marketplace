using MarketPlace.DataAccess.Model;
using MimeKit;

namespace MarketPlaceUser.Bussiness.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);

        Task<bool> SendEmailInvoiceAsync(string to, string subject, MimePart body);

        void VerifyUser(string to, string token);

        void ForgotPassword(string to, string token);

        void ProductSold(string to, string productName, string userName);

        void ProductBought(string to, string userName, List<OrderDetails> productList);

        void SendEmailInvoice(string to, string subject, MimePart body);

    }
}
