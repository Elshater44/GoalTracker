using GoalTracker.Models;

namespace GoalTracker.Repositories.Interfaces
{
    public interface IGoalRepository
    {
        Task AddGoalAsync(Goal goal);
        Task<List<Goal>> GetAllGoalsAsync(int userId);
        Task<Goal?> GetGoalByIdAsync(int id, int userId);
        void RemoveGoal(Goal goal);
        Task SaveChangesAsync();
        void UpdateGoal(Goal goal);
    }
}
