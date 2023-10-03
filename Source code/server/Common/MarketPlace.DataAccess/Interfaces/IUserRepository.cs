using MarketPlace.DataAccess.Model;
using System.Linq.Expressions;

namespace MarketPlace.DataAccess.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Dictionary<string, Expression<Func<User, object>>> ColumnMapForSortBy { get; }

    Task<User?> FindByIdAndStatusAsync(int userId, User.UserStatus status);

    Task<User?> FindByEmailAsync(string email);

    Task<User?> FindByEmailAndStatus(string email, User.UserStatus status);

    Task<User?> FindByEmailAndVerificationCode(string Email, string VerficationCode);

    Task<List<User>> FindAllByStatusAndNameOrEmailLikeAsync(User.UserStatus[]? status, User.UserRole[]? roles, string? Search, string? SortBy, bool SortByDesc);

    Task<User?> FindFirstByEmailLikeOrderByCreatedDateDesc(string email);

    Task<bool> IsProfilePicExists(string fileName);

    Task<Dictionary<Product.ProductStatus, int>> GetSellerProductCountGroupByProductStatus(int userId);

    Task<Dictionary<int, int>> GetSellerProductCounts();
}