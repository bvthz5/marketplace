using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Security;

namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class AgentLoginView : AgentDetailView
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

        public AgentLoginView(Agent agent, Token accessToken, Token refreshToken) : base(agent)
        {
            AccessToken = new TokenView(accessToken);

            RefreshToken = new TokenView(refreshToken);
        }
    }
}
