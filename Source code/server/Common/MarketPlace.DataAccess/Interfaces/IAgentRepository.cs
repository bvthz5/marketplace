using MarketPlace.DataAccess.Model;
using MarketPlace.DataAccess.Repository;
using System.Linq.Expressions;

namespace MarketPlace.DataAccess.Interfaces;

public interface IAgentRepository : IRepository<Agent>
{
    Task<Agent?> FindByEmailAsync(string email);

    Dictionary<string, Expression<Func<Agent, object>>> ColumnMapForSortBy { get; }

    Task<List<Agent>> FindAllByStatusAndNameOrEmailLikeAsync(Agent.DeliveryAgentStatus[]? status, string? Search, string? SortBy, bool SortByDesc);

    Task<Agent?> FindByEmailAndVerificationCode(string email, string verificationCode);

    Task<bool> IsProfilePicExists(string fileName);
}