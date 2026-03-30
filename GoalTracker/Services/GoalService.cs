using AutoMapper;
using GoalTracker.Common.Errors;
using GoalTracker.Common.Results;
using GoalTracker.DTOs.GoalDTOs;
using GoalTracker.Models;
using GoalTracker.Repositories.Interfaces;

namespace GoalTracker.Services
{
    public class GoalService
    {
        private readonly IGoalRepository _goalRepository;
        private readonly IMapper _mapper;

        public GoalService(IGoalRepository goalRepository, IMapper mapper)
        {
            _goalRepository = goalRepository;
            _mapper = mapper;
        }

        public async Task<List<GoalGetDto>> GetAllGoalsAsync()
        {
            var goals = await _goalRepository.GetAllGoalsAsync();
            var goalsDto = _mapper.Map<List<GoalGetDto>>(goals);

            return goalsDto;
        }

        public async Task<Result<GoalGetDto>> GetGoalByIdAsync(int id)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(id);

            if (goal is null)
                return Result<GoalGetDto>.Failure(GoalErrors.NotFound(id));

            return Result<GoalGetDto>.Success(_mapper.Map<GoalGetDto>(goal));
        }

        public async Task<Result<GoalGetDto>> CreateGoalAsync(GoalCreateDto goalDto)
        {
            var goal = _mapper.Map<Goal>(goalDto);

            await _goalRepository.AddGoalAsync(goal);
            await _goalRepository.SaveChangesAsync();

            return Result<GoalGetDto>.Success(_mapper.Map<GoalGetDto>(goal));
        }

        public async Task<Result> UpdateGoalAsync(int id, GoalUpdateDto goalDto)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(id);

            if (goal is null)
                return Result.Failure(GoalErrors.NotFound(id));

            _mapper.Map(goalDto, goal);

            await _goalRepository.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteGoalAsync(int id)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(id);

            if (goal is null)
                return Result.Failure(GoalErrors.NotFound(id));

            _goalRepository.RemoveGoal(goal);
            await _goalRepository.SaveChangesAsync();

            return Result.Success();
        }
    }
}
