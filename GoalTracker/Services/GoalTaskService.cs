using AutoMapper;
using GoalTracker.DTOs.GoalTasksDTOs;
using GoalTracker.Models;
using GoalTracker.Repositories.Interfaces;

namespace GoalTracker.Services
{
    public class GoalTaskService
    {
        private readonly IMapper _mapper;
        private readonly IGoalRepository _goalRepository;
        private readonly IGoalTaskRepository _goalTaskRepository;

        public GoalTaskService(IMapper mapper, IGoalRepository goalRepository, IGoalTaskRepository goalTasKRepository)
        {
            _mapper = mapper;
            _goalRepository = goalRepository;
            _goalTaskRepository = goalTasKRepository;
        }

        public async Task<List<GoalTaskGetDto>?> getAllTasksAsync(int goalId)
        {
            if (await IsGoalExistWithGoalIdAsync(goalId)) return null;
            var goalTasks = await _goalTaskRepository.GetAllGoalTasksAsync(goalId);
            return _mapper.Map<List<GoalTaskGetDto>>(goalTasks);
        }
        public async Task<GoalTaskGetDto?> GetGoalTaskByIdAsync(int goalId, int goalTaskId)
        {
            var goalTask = await _goalTaskRepository.GetGoalTaskAsync(goalId, goalTaskId);
            return _mapper.Map<GoalTaskGetDto>(goalTask);
        }

        public async Task<GoalTaskGetDto?> CreateGoalTaskAsync(int goalId, GoalTaskCreateDto goalTaskDto)
        {
            if (!await IsGoalExistWithGoalIdAsync(goalId)) return null;

            var goalTask = _mapper.Map<GoalTask>(goalTaskDto);
            goalTask.GoalId = goalId;
            await _goalTaskRepository.AddGoalTaskAsync(goalTask);
            await _goalTaskRepository.SaveChangesAsync();

            var goalTaskReturnDto = _mapper.Map<GoalTaskGetDto>(goalTask);
            return goalTaskReturnDto;
        }

        public async Task<bool> UpdateGoalTaskAsync(int goalId, int goalTaskId, GoalTaskUpdateDto goalTaskUpdateDto)
        {
            var goalTask = await _goalTaskRepository.GetGoalTaskAsync(goalId, goalTaskId);
            if (goalTask is null) return false;
            _mapper.Map(goalTaskUpdateDto, goalTask);
            await _goalTaskRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveGoalTaskAsync(int goalId, int goalTaskId)
        {
            var goalTask = await _goalTaskRepository.GetGoalTaskAsync(goalId, goalTaskId);
            if (goalTask is null) return false;
            _goalTaskRepository.DeleteGoalTask(goalTask);
            await _goalTaskRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateIsCompleteForGoalTaskAsync(int goalId, int goalTaskId, bool isComplete)
        {
            var goalTask = await _goalTaskRepository.GetGoalTaskAsync(goalId, goalTaskId);
            if (goalTask is null) return false;
            goalTask.IsCompleted = isComplete;
            await UpdateGoalCompletionStateAsync(goalId);
            await _goalTaskRepository.SaveChangesAsync();
            return true;
        }

        private async Task<bool> IsGoalExistWithGoalIdAsync(int goalId)
        {
            return await _goalRepository.GetGoalByIdAsync(goalId) is not null;
        }

        private async Task UpdateGoalCompletionStateAsync(int goalId)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(goalId);
            if (goal is null) return;
            goal.CompletedAt = goal.GoalTasks.All(gt => gt.IsCompleted)
                                   ? DateTime.Now
                                   : null;
        }
    }

}
