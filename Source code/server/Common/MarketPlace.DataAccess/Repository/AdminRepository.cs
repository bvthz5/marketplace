using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.DataAccess.Repository
{
    public class AdminRepository : Repository<Admin>, IAdminRepository
    {
        private readonly MarketPlaceDbContext _dbContext;

        public AdminRepository(MarketPlaceDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Admin?> FindByEmail(string email)
        {
            return await _dbContext.Admins.SingleOrDefaultAsync(admin => admin.Email == email);
        }

        public async Task<Admin?> FindByEmailAndVerificationCode(string email, string verificationCode)
        {
            return await _dbContext.Admins.SingleOrDefaultAsync(admin => admin.Email == email && admin.VerificationCode == verificationCode);
        }
    }
}