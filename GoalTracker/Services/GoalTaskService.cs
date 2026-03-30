using AutoMapper;
using GoalTracker.Common.Errors;
using GoalTracker.Common.Results;
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

        public async Task<Result<List<GoalTaskGetDto>>> GetAllTasksAsync(int goalId)
        {
            if (!await IsGoalExistWithGoalIdAsync(goalId))
                return Result<List<GoalTaskGetDto>>.Failure(GoalTaskErrors.GoalNotFound(goalId));

            var tasks = await _goalTaskRepository.GetAllGoalTasksAsync(goalId);
            var dtos = _mapper.Map<List<GoalTaskGetDto>>(tasks);

            return Result<List<GoalTaskGetDto>>.Success(dtos);
        }

        public async Task<Result<GoalTaskGetDto>> GetGoalTaskByIdAsync(int goalId, int goalTaskId)
        {
            if (!await IsGoalExistWithGoalIdAsync(goalId))
                return Result<GoalTaskGetDto>.Failure(GoalTaskErrors.GoalNotFound(goalId));

            var goalTask = await _goalTaskRepository.GetGoalTaskAsync(goalId, goalTaskId);

            if (goalTask is null)
                return Result<GoalTaskGetDto>.Failure(GoalTaskErrors.NotFound(goalId, goalTaskId));

            return Result<GoalTaskGetDto>.Success(_mapper.Map<GoalTaskGetDto>(goalTask));
        }

        public async Task<Result<GoalTaskGetDto>> CreateGoalTaskAsync(int goalId, GoalTaskCreateDto goalTaskDto)
        {
            if (!await IsGoalExistWithGoalIdAsync(goalId))
                return Result<GoalTaskGetDto>.Failure(GoalTaskErrors.GoalNotFound(goalId));

            var goalTask = _mapper.Map<GoalTask>(goalTaskDto);
            goalTask.GoalId = goalId;

            await _goalTaskRepository.AddGoalTaskAsync(goalTask);
            await _goalTaskRepository.SaveChangesAsync();

            return Result<GoalTaskGetDto>.Success(_mapper.Map<GoalTaskGetDto>(goalTask));
        }

        public async Task<Result> UpdateGoalTaskAsync(int goalId, int goalTaskId, GoalTaskUpdateDto goalTaskDto)
        {
            if (!await IsGoalExistWithGoalIdAsync(goalId))
                return Result.Failure(GoalTaskErrors.GoalNotFound(goalId));

            var goalTask = await _goalTaskRepository.GetGoalTaskAsync(goalId, goalTaskId);

            if (goalTask is null)
                return Result.Failure(GoalTaskErrors.NotFound(goalId, goalTaskId));

            _mapper.Map(goalTaskDto, goalTask);

            await _goalTaskRepository.SaveChangesAsync();
            await UpdateGoalCompletionStateAsync(goalId);

            return Result.Success();
        }

        public async Task<Result> RemoveGoalTaskAsync(int goalId, int goalTaskId)
        {
            if (!await IsGoalExistWithGoalIdAsync(goalId))
                return Result.Failure(GoalTaskErrors.GoalNotFound(goalId));

            var goalTask = await _goalTaskRepository.GetGoalTaskAsync(goalId, goalTaskId);

            if (goalTask is null)
                return Result.Failure(GoalTaskErrors.NotFound(goalId, goalTaskId));

            _goalTaskRepository.DeleteGoalTask(goalTask);

            await _goalTaskRepository.SaveChangesAsync();
            await UpdateGoalCompletionStateAsync(goalId);

            return Result.Success();
        }

        public async Task<Result> UpdateIsCompleteForGoalTaskAsync(int goalId, int goalTaskId, bool isComplete)
        {
            if (!await IsGoalExistWithGoalIdAsync(goalId))
                return Result.Failure(GoalTaskErrors.GoalNotFound(goalId));

            var goalTask = await _goalTaskRepository.GetGoalTaskAsync(goalId, goalTaskId);

            if (goalTask is null)
                return Result.Failure(GoalTaskErrors.NotFound(goalId, goalTaskId));

            goalTask.IsCompleted = isComplete;

            await _goalTaskRepository.SaveChangesAsync();
            await UpdateGoalCompletionStateAsync(goalId);

            return Result.Success();
        }

        private async Task<bool> IsGoalExistWithGoalIdAsync(int goalId)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(goalId);
            return goal is not null;
        }

        private async Task UpdateGoalCompletionStateAsync(int goalId)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(goalId);

            if (goal is null)
                return;

            goal.CompletedAt = goal.GoalTasks.Any() && goal.GoalTasks.All(gt => gt.IsCompleted)
                ? DateTime.Now
                : null;

            await _goalRepository.SaveChangesAsync();
        }
    }

}
