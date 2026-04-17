using GoalTracker.Models;

namespace GoalTracker.Repositories.Interfaces
{
    public interface IGoalTaskRepository
    {
        Task AddGoalTaskAsync(GoalTask goalTask);
        void DeleteGoalTask(GoalTask goalTask);
        Task<List<GoalTask>> GetAllGoalTasksAsync(int goalId, int userId);
        Task<GoalTask?> GetGoalTaskAsync(int goalId, int goalTaskId, int userId);
        Task SaveChangesAsync();
        void UpdateGoalTask(GoalTask goalTask);
    }
}
