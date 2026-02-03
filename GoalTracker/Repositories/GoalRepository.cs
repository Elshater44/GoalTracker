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

        public async Task<List<Goal>> GetAllGoalsAsync()
        {
            return await _context.Goals.Include(g => g.GoalTasks).ToListAsync();
        }

        public Task<Goal?> GetGoalByIdAsync(int id)
        {
            return _context.Goals.Include(g => g.GoalTasks).FirstOrDefaultAsync(g => g.Id == id);
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
