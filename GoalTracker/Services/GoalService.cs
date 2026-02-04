using AutoMapper;
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

        public async Task<GoalGetDto?> GetGoalByIdAsync(int id)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(id);
            if (goal == null) return null;
            var goalDto = _mapper.Map<GoalGetDto>(goal);
            return goalDto;
        }

        public async Task CreateGoalAsync(GoalCreateDto goalDto)
        {
            var goal = _mapper.Map<Goal>(goalDto);
            await _goalRepository.AddGoalAsync(goal);
            await _goalRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateGoalAsync(int id, GoalUpdateDto goalDto)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(id);
            if (goal == null) return false;
            _mapper.Map(goalDto, goal);
            await _goalRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteGoalAsync(int id)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(id);
            if (goal == null) return false;
            _goalRepository.RemoveGoal(goal);
            await _goalRepository.SaveChangesAsync();
            return true;
        }
    }
}
