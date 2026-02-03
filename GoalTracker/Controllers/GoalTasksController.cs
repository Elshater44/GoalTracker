using AutoMapper;
using GoalTracker.DTOs.GoalTasksDTOs;
using GoalTracker.Models;
using GoalTracker.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GoalTracker.Controllers
{
    [Route("api/Goals/{goalId}/[controller]")]
    [ApiController]
    public class GoalTasksController : ControllerBase
    {
        private readonly GoalTaskRepository _goalTaskRepository;
        private readonly GoalRepository _goalRepository;
        private readonly IMapper _mapper;

        public GoalTasksController(
            GoalTaskRepository goalTaskRepository,
            GoalRepository goalRepository,
            IMapper mapper)
        {
            _goalTaskRepository = goalTaskRepository;
            _goalRepository = goalRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> CreateGoalTask(
            GoalTaskCreateDto goalTaskDto,
            [FromRoute] int goalId)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(goalId);
            if (goal is null)
                return NotFound($"The goal with id {goalId} is not found");

            var goalTask = _mapper.Map<GoalTask>(goalTaskDto);
            goalTask.GoalId = goalId;

            await _goalTaskRepository.AddGoalTaskAsync(goalTask);
            await _goalTaskRepository.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetGoalTaskById),
                new { goalId, goalTaskId = goalTask.Id },
                _mapper.Map<GoalTaskGetDto>(goalTask)
            );
        }

        [HttpGet]
        public async Task<ActionResult<List<GoalTaskGetDto>>> GetAllGoalTasks(
            [FromRoute] int goalId)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(goalId);
            if (goal is null)
                return NotFound($"The goal with id {goalId} is not found");

            var tasks = await _goalTaskRepository.GetAllGoalTasksAsync(goalId);
            var tasksDto = _mapper.Map<List<GoalTaskGetDto>>(tasks);

            return Ok(tasksDto);
        }

        [HttpGet("{goalTaskId}")]
        public async Task<ActionResult<GoalTaskGetDto>> GetGoalTaskById(
            [FromRoute] int goalId,
            [FromRoute] int goalTaskId)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(goalId);
            if (goal is null)
                return NotFound($"The goal with id {goalId} is not found");

            var goalTask = await _goalTaskRepository.GetGoalTaskAsync(goalId, goalTaskId);
            if (goalTask is null)
                return NotFound($"The GoalTask with id {goalTaskId} is not found");

            return Ok(_mapper.Map<GoalTaskGetDto>(goalTask));
        }

        [HttpPut("{goalTaskId}")]
        public async Task<ActionResult> UpdateGoalTask(
            [FromRoute] int goalId,
            [FromRoute] int goalTaskId,
            GoalTaskUpdateDto goalTaskUpdateDto)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(goalId);
            if (goal is null)
                return NotFound($"The goal with id {goalId} is not found");

            var goalTask = await _goalTaskRepository.GetGoalTaskAsync(goalId, goalTaskId);
            if (goalTask is null)
                return NotFound($"The GoalTask with id {goalTaskId} is not found");

            _mapper.Map(goalTaskUpdateDto, goalTask);
            _goalTaskRepository.UpdateGoalTask(goalTask);
            await _goalTaskRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{goalTaskId}/complete")]
        public async Task<ActionResult> UpdateIsCompleteGoalTask(
            [FromRoute] int goalId,
            [FromRoute] int goalTaskId,
            [FromBody] bool isComplete)
        {
            var goal = await _goalRepository.GetGoalByIdAsync(goalId);
            if (goal is null)
                return NotFound($"The goal with id {goalId} is not found");

            var goalTask = await _goalTaskRepository.GetGoalTaskAsync(goalId, goalTaskId);
            if (goalTask is null)
                return NotFound($"The GoalTask with id {goalTaskId} is not found");

            goalTask.IsCompleted = isComplete;
            _goalTaskRepository.UpdateGoalTask(goalTask);
            await _goalTaskRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
