using GoalTracker.Models;

namespace GoalTracker.Repositories.Interfaces
{
    public interface IGoalRepository
    {
        Task AddGoalAsync(Goal goal);
        Task<List<Goal>> GetAllGoalsAsync();
        Task<Goal?> GetGoalByIdAsync(int id);
        void RemoveGoal(Goal goal);
        Task SaveChangesAsync();
        void UpdateGoal(Goal goal);
    }
}