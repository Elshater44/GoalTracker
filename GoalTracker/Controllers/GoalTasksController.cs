using GoalTracker.Common.Results.Extensions;
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

        public GoalTasksController(GoalTaskService goalTaskService)
        {
            _goalTaskService = goalTaskService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GoalTaskGetDto>>> GetAllGoalTasks([FromRoute] int goalId)
        {
            var result = await _goalTaskService.GetAllTasksAsync(goalId);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return Ok(result.Value);
        }

        [HttpGet("{goalTaskId}")]
        public async Task<ActionResult<GoalTaskGetDto>> GetGoalTaskById([FromRoute] int goalId, [FromRoute] int goalTaskId)
        {
            var result = await _goalTaskService.GetGoalTaskByIdAsync(goalId, goalTaskId);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<ActionResult<GoalTaskGetDto>> CreateGoalTask([FromRoute] int goalId, [FromBody] GoalTaskCreateDto goalTaskDto)
        {
            var result = await _goalTaskService.CreateGoalTaskAsync(goalId, goalTaskDto);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return CreatedAtAction(
                nameof(GetGoalTaskById),
                new { goalId = goalId, goalTaskId = result.Value!.Id },
                result.Value
            );
        }

        [HttpPut("{goalTaskId}")]
        public async Task<ActionResult> UpdateGoalTask([FromRoute] int goalId, [FromRoute] int goalTaskId, [FromBody] GoalTaskUpdateDto goalTaskDto)
        {
            var result = await _goalTaskService.UpdateGoalTaskAsync(goalId, goalTaskId, goalTaskDto);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return NoContent();
        }

        [HttpPut("{goalTaskId}/complete")]
        public async Task<ActionResult> UpdateIsCompleteGoalTask([FromRoute] int goalId, [FromRoute] int goalTaskId, [FromBody] bool isComplete)
        {
            var result = await _goalTaskService.UpdateIsCompleteForGoalTaskAsync(goalId, goalTaskId, isComplete);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return NoContent();
        }

        [HttpDelete("{goalTaskId}")]
        public async Task<ActionResult> DeleteGoalTask([FromRoute] int goalId, [FromRoute] int goalTaskId)
        {
            var result = await _goalTaskService.RemoveGoalTaskAsync(goalId, goalTaskId);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return NoContent();
        }
    }
}