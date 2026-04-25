using GoalTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;
namespace GoalTracker.Repositories.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            return query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy) where T : class
        {
            if (string.IsNullOrEmpty(sortBy))
                return query;

            var allowedProperties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name)
                .ToHashSet();
            var sortExpression = new List<string>();
            var sortFields = sortBy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var part in sortFields)
            {
                var tokens = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0 || !allowedProperties.Contains(tokens[0]))
                    continue;
                var direction = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase) ? "descending" : "ascending";
                sortExpression.Add($"{tokens[0]} {direction}");
            }
            return sortExpression.Count > 0 ? query.OrderBy(string.Join(", ", sortExpression)) : query;
        }

        public static IQueryable<Goal> ApplySearch(this IQueryable<Goal> query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            return query.Where(g =>
                EF.Functions.Like(g.Name, $"%{search}%") ||
                EF.Functions.Like(g.Description, $"%{search}%") ||
                EF.Functions.Like(g.UrgencyLevel.ToString(), $"%{search}%"));
        }
    }
}
