using GoalTracker.Data;
using GoalTracker.Models;
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
