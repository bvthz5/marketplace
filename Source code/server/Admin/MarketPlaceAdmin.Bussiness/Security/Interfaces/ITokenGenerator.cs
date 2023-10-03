using MarketPlace.DataAccess.Model;

namespace MarketPlaceAdmin.Bussiness.Security.Interfaces
{

    public interface ITokenGenerator
    {
        Token GenerateAccessToken(int nameIdentifier, bool isAdmin);

        Token GenerateRefreshToken(int id, string email, string password);

        string[] GetAdminIdAndTokenData(string refreshToken);

        Token VerifyRefreshToken(string tokenData, Admin admin);

        Token VerifyRefreshTokenAgent(string tokenData, Agent agent);

    }
}
