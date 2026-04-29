using GoalTracker.Data;
using GoalTracker.DTOs.GoalDTOs.Query;
using GoalTracker.Models;
using GoalTracker.Repositories.Extensions;
using GoalTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoalTracker.Repositories
{
    public class GoalRepository : IGoalRepository
    {
        private readonly AppDbContext _context;

        public GoalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Goal>> GetAllGoalsAsync(int userId)
        {
            return await _context.Goals
                .Include(g => g.GoalTasks)
                .Where(g => g.UserId == userId)
                .ToListAsync();
        }

        public Task<Goal?> GetGoalByIdAsync(int id, int userId)
        {
            return _context.Goals
                .Include(g => g.GoalTasks)
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
        }

        public async Task<(List<Goal> Items, int TotalCount)> GetGoalsAsync(int userId, GoalQueryParams queryParams)
        {
            var query = _context.Goals
                .Include(g => g.GoalTasks)
                .Where(g => g.UserId == userId);

            // Apply search filter
            query = query.ApplySearch(queryParams.Search);

            // Apply IsCompleted filter
            if (queryParams.IsCompleted.HasValue)
                query = query.Where(g => g.CompletedAt.HasValue == queryParams.IsCompleted);

            // Apply HasDeadline filter
            if (queryParams.HasDeadline.HasValue)
                query = query.Where(g => g.Deadline.HasValue == queryParams.HasDeadline);

            // Apply IsMissed filter (same logic as IsOverdue)
            if (queryParams.IsMissed.HasValue)
            {
                if (queryParams.IsMissed.Value)
                    query = query.Where(g => g.Deadline.HasValue && g.Deadline < DateTime.UtcNow && !g.CompletedAt.HasValue);
                else
                    query = query.Where(g => !g.Deadline.HasValue || g.Deadline >= DateTime.UtcNow || g.CompletedAt.HasValue);
            }

            // Apply UrgencyLevel filter
            if (queryParams.UrgencyLevel.HasValue)
                query = query.Where(g => g.UrgencyLevel == queryParams.UrgencyLevel);

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting and pagination
            var items = await query
                .ApplySorting(queryParams.SortBy)
                .ApplyPagination(queryParams.PageNumber, queryParams.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddGoalAsync(Goal goal)
        {
            await _context.Goals.AddAsync(goal);
        }

        public void UpdateGoal(Goal goal)
        {
            _context.Goals.Update(goal);
        }
        public void RemoveGoal(Goal goal)
        {
            _context.Goals.Remove(goal);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

