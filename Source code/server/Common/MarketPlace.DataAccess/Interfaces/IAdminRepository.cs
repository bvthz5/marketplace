using MarketPlace.DataAccess.Model;

namespace MarketPlace.DataAccess.Interfaces;
public interface IAdminRepository : IRepository<Admin>
{
    Task<Admin?> FindByEmail(string email);

    Task<Admin?> FindByEmailAndVerificationCode(string email, string verificationCode);
}