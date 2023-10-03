using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Security;

namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class LoginView : AdminView
    {
        public class TokenView
        {
            public string Value { get; }

            public DateTime Expiry { get; }

            public TokenView(Token token)
            {
                Value = token.Value;
                Expiry = token.Expiry;
            }
        }
        public TokenView AccessToken { get; }

        public TokenView RefreshToken { get; }

        public LoginView(Admin admin, Token accessToken, Token refreshToken) : base(admin)
        {
            AccessToken = new TokenView(accessToken);

            RefreshToken = new TokenView(refreshToken);
        }
    }
}
