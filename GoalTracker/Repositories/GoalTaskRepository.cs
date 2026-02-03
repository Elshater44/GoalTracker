using GoalTracker.Data;
using GoalTracker.Models;
using GoalTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoalTracker.Repositories
{
    public class GoalTaskRepository : IGoalTaskRepository
    {
        private readonly AppDbContext _context;

        public GoalTaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GoalTask>> GetAllGoalTasksAsync(int goalId)
        {
            var goalTasks = await _context.GoalTasks.Where(gt => gt.GoalId == goalId).ToListAsync();
            return goalTasks;
        }

        public async Task<GoalTask?> GetGoalTaskAsync(int goalId, int goalTaskId)
        {
            var goalTask = await _context.GoalTasks.FirstOrDefaultAsync(gt => gt.GoalId == goalId && gt.Id == goalTaskId);
            return goalTask;
        }

        public async Task AddGoalTaskAsync(GoalTask goalTask)
        {
            await _context.GoalTasks.AddAsync(goalTask);
        }
        public void DeleteGoalTask(GoalTask goalTask)
        {
            _context.GoalTasks.Remove(goalTask);
        }

        public void UpdateGoalTask(GoalTask goalTask)
        {
            _context.GoalTasks.Update(goalTask);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
