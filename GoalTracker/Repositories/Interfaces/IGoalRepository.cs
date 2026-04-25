using GoalTracker.DTOs.GoalDTOs.Query;
using GoalTracker.Models;

namespace GoalTracker.Repositories.Interfaces
{
    public interface IGoalRepository
    {
        Task AddGoalAsync(Goal goal);
        Task<List<Goal>> GetAllGoalsAsync(int userId);
        Task<Goal?> GetGoalByIdAsync(int id, int userId);
        Task<(List<Goal> Items, int TotalCount)> GetGoalsAsync(int userId, GoalQueryParams queryParams);
        void RemoveGoal(Goal goal);
        Task SaveChangesAsync();
        void UpdateGoal(Goal goal);
    }
}
