using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Extensions;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MarketPlace.DataAccess.Repository
{
    public class AgentRepository : Repository<Agent>, IAgentRepository
    {
        private readonly MarketPlaceDbContext _dbContext;

        public AgentRepository(MarketPlaceDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        private static readonly string _primaryKey = "AgentId";

        public Dictionary<string, Expression<Func<Agent, object>>> ColumnMapForSortBy { get; } = new()
        {
            [_primaryKey] = agent => agent.AgentId,
            ["Name"] = agent => agent.Name,
            ["Email"] = agent => agent.Email,
            ["CreatedDate"] = agent => agent.CreatedDate
        };

        public async Task<Agent?> FindByEmailAsync(string email)
        {
            return await _dbContext.Agents

                                        .SingleOrDefaultAsync(agent => agent.Email == email);
        }

        public async Task<List<Agent>> FindAllByStatusAndNameOrEmailLikeAsync(Agent.DeliveryAgentStatus[]? status, string? Search, string? SortBy, bool SortByDesc)
        {

            return await _dbContext.Agents
                                       .AsNoTracking()
                                       .Where(agent =>
                                                      (status == null || !status.Any() || status.Contains(agent.Status)) &&
                                                    (string.IsNullOrWhiteSpace(Search) || agent.Name.Contains(Search) ||
                                                    agent.Email.Contains(Search)))
                                       .ApplyOrdering(SortBy ?? _primaryKey, SortByDesc, ColumnMapForSortBy)
                                       .ToListAsync();
        }

        public async Task<Agent?> FindByEmailAndVerificationCode(string email, string verificationCode)
        {
            return await _dbContext.Agents.SingleOrDefaultAsync(agent => agent.Email == email && agent.VerificationCode == verificationCode);
        }
        public async Task<bool> IsProfilePicExists(string fileName)
        {
            return (await _dbContext.Agents.SingleOrDefaultAsync(agent => agent.ProfilePic == fileName)) != null;
        }

    }
}

