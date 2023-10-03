using System.Linq.Expressions;

namespace MarketPlace.DataAccess.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, string? SortBy, bool SortByDesc, Dictionary<string, Expression<Func<T, object>>> columsMap)
        {
            if (string.IsNullOrWhiteSpace(SortBy) || !columsMap.ContainsKey(SortBy))
                return query;

            if (SortByDesc)
                return query.OrderByDescending(columsMap[SortBy]);

            return query.OrderBy(columsMap[SortBy]);
        }
    }
}
