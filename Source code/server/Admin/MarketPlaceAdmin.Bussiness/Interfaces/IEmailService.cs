namespace MarketPlaceAdmin.Bussiness.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);

        void AdminForgotPassword(string to, string token);

        void SellerRequest(string to, bool accepted, string? message);

        void ProductRequest(string to, int productId, string productName, bool accepted, string? message);

        void SendAgentEmailAsync(string email, string generatePasswords, string name);

        void AgentForgotPassword(string to, string token);

        void ForgotPassword(string to, string token, string route);

        void DeliveryOtp(string to, string name, string otp, string[] productNamesList);
    }
}
