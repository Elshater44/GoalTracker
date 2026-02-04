using FluentValidation;
using GoalTracker.DTOs.GoalTasksDTOs;
using GoalTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoalTracker.Controllers
{
    [Route("api/Goals/{goalId}/[controller]")]
    [ApiController]
    public class GoalTasksController : ControllerBase
    {
        private readonly GoalTaskService _goalTaskService;
        private readonly IValidator<GoalTaskCreateDto> _goalTaskCreateValidator;
        private readonly IValidator<GoalTaskUpdateDto> _goalTaskUpdateValidator;

        public GoalTasksController(GoalTaskService goalTaskService, IValidator<GoalTaskCreateDto> goalTaskCreateValidator, IValidator<GoalTaskUpdateDto> goalTaskUpdateValidator)
        {
            _goalTaskService = goalTaskService;
            _goalTaskCreateValidator = goalTaskCreateValidator;
            _goalTaskUpdateValidator = goalTaskUpdateValidator;

        }

        [HttpGet]
        public async Task<ActionResult<List<GoalTaskGetDto>>> GetAllGoalTasks([FromRoute] int goalId)
        {
            var tasks = await _goalTaskService.getAllTasksAsync(goalId);
            if (tasks is null) return NotFound();
            return Ok(tasks);
        }

        [HttpGet("{goalTaskId}")]
        public async Task<ActionResult<GoalTaskGetDto>> GetGoalTaskById([FromRoute] int goalId, [FromRoute] int goalTaskId)
        {
            var task = await _goalTaskService.GetGoalTaskByIdAsync(goalId, goalTaskId);

            if (task == null)
                return NotFound($"GoalTask {goalTaskId} not found for Goal {goalId}");

            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<GoalTaskGetDto>> CreateGoalTask([FromRoute] int goalId, [FromBody] GoalTaskCreateDto goalTaskDto)
        {
            var validationResult = _goalTaskCreateValidator.Validate(goalTaskDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
               .Select(e => new { e.PropertyName, e.ErrorMessage })
               .ToList();

                return BadRequest(errors);
            }
            var createdTask = await _goalTaskService.CreateGoalTaskAsync(goalId, goalTaskDto);

            if (createdTask == null)
                return NotFound($"Goal with id {goalId} not found");

            return CreatedAtAction(
                nameof(GetGoalTaskById),
                new { goalId = goalId, goalTaskId = createdTask.Id },
                createdTask
            );
        }

        [HttpPut("{goalTaskId}")]
        public async Task<ActionResult> UpdateGoalTask([FromRoute] int goalId, [FromRoute] int goalTaskId, [FromBody] GoalTaskUpdateDto goalTaskDto)
        {
            var validationResult = _goalTaskUpdateValidator.Validate(goalTaskDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
               .Select(e => new { e.PropertyName, e.ErrorMessage })
               .ToList();

                return BadRequest(errors);
            }
            var success = await _goalTaskService.UpdateGoalTaskAsync(goalId, goalTaskId, goalTaskDto);

            if (!success)
                return NotFound("Target task or goal not found.");

            return NoContent();
        }

        [HttpPut("{goalTaskId}/complete")]
        public async Task<ActionResult> UpdateIsCompleteGoalTask([FromRoute] int goalId, [FromRoute] int goalTaskId, [FromBody] bool isComplete)
        {
            var success = await _goalTaskService.UpdateIsCompleteForGoalTaskAsync(goalId, goalTaskId, isComplete);

            if (!success)
                return NotFound("Could not update status. Check IDs.");

            return NoContent();
        }

        [HttpDelete("{goalTaskId}")]
        public async Task<ActionResult> DeleteGoalTask([FromRoute] int goalId, [FromRoute] int goalTaskId)
        {
            var success = await _goalTaskService.RemoveGoalTaskAsync(goalId, goalTaskId);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}