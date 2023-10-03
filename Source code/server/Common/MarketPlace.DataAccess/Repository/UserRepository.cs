using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Extensions;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MarketPlace.DataAccess.Repository;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly MarketPlaceDbContext _dbContext;

    private static readonly string _primaryKey = "UserId";

    public Dictionary<string, Expression<Func<User, object>>> ColumnMapForSortBy { get; } = new()
    {
        [_primaryKey] = user => user.UserId,
        ["FirstName"] = user => user.FirstName,
        ["Email"] = user => user.Email,
        ["CreatedDate"] = user => user.CreatedDate
    };

    public UserRepository(MarketPlaceDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> FindByIdAndStatusAsync(int userId, User.UserStatus status)
    {
        return await _dbContext.Users

                                    .SingleOrDefaultAsync(user => user.UserId == userId && user.Status == status);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _dbContext.Users

                                    .SingleOrDefaultAsync(user => user.Email == email);
    }

    public async Task<User?> FindByEmailAndStatus(string email, User.UserStatus status)
    {
        return await _dbContext.Users

                                    .SingleOrDefaultAsync(user => user.Email == email && user.Status == status);
    }

    public async Task<User?> FindByEmailAndVerificationCode(string Email, string VerficationCode)
    {
        return await _dbContext.Users

                                    .FirstOrDefaultAsync(user => user.Email == Email && user.VerificationCode == VerficationCode);
    }

    public async Task<List<User>> FindAllByStatusAndNameOrEmailLikeAsync(User.UserStatus[]? status, User.UserRole[]? roles, string? Search, string? SortBy, bool SortByDesc)
    {

        return await _dbContext.Users
                                   .AsNoTracking()
                                   .Where(user => (roles == null || !roles.Any() || roles.Contains(user.Role)) &&
                                                  (status == null || !status.Any() || status.Contains(user.Status)) &&
                                                (string.IsNullOrWhiteSpace(Search) || new { Name = user.FirstName + " " + user.LastName }.Name.Contains(Search) ||
                                                user.Email.Contains(Search)))
                                   .ApplyOrdering(SortBy ?? _primaryKey, SortByDesc, ColumnMapForSortBy)
                                   .ToListAsync();
    }

    public async Task<User?> FindFirstByEmailLikeOrderByCreatedDateDesc(string email)
    {
        return await _dbContext.Users
                            .Where(user => user.Email.Contains(email))
                            .OrderByDescending(user => user.CreatedDate)
                            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsProfilePicExists(string fileName)
    {
        return (await _dbContext.Users.SingleOrDefaultAsync(user => user.ProfilePic == fileName)) != null;
    }

    public async Task<Dictionary<Product.ProductStatus, int>> GetSellerProductCountGroupByProductStatus(int userId)
    {
        return await _dbContext.Products
                     .Where(product => product.Status != Product.ProductStatus.DRAFT && product.CreatedUserId == userId)
                     .GroupBy(
                         product => product.Status,
                         product => product.ProductId)
                     .Select(g => new { status = g.Key, count = g.Count() })
                     .ToDictionaryAsync(obj => obj.status, obj => obj.count);
    }

    public async Task<Dictionary<int, int>> GetSellerProductCounts()
    {
        return await _dbContext.Products
                     .Where(product => product.Status != Product.ProductStatus.DRAFT)
                     .GroupBy(
                         product => product.CreatedUserId,
                         product => product.ProductId)
                     .Select(g => new { sellerId = g.Key, count = g.Count() })
                     .ToDictionaryAsync(g => g.sellerId, g => g.count);

    }
}