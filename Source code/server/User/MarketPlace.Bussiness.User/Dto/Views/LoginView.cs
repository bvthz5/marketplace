using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Security;

namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class LoginView : UserView
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

        public LoginView(User user, Token AccessToken, Token RefreshToken) : base(user)
        {
            this.AccessToken = new TokenView(AccessToken);

            this.RefreshToken = new TokenView(RefreshToken);
        }
    }
}
