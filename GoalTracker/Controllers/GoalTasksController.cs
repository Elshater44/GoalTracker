using GoalTracker.Common.Results.Extensions;
using GoalTracker.DTOs.GoalTasksDTOs;
using GoalTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoalTracker.Controllers
{
    [Route("api/Goals/{goalId}/[controller]")]
    [ApiController]
    [Authorize]
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
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var result = await _goalTaskService.GetAllTasksAsync(goalId, userId.Value);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return Ok(result.Value);
        }

        [HttpGet("{goalTaskId}")]
        public async Task<ActionResult<GoalTaskGetDto>> GetGoalTaskById([FromRoute] int goalId, [FromRoute] int goalTaskId)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var result = await _goalTaskService.GetGoalTaskByIdAsync(goalId, goalTaskId, userId.Value);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<ActionResult<GoalTaskGetDto>> CreateGoalTask([FromRoute] int goalId, [FromBody] GoalTaskCreateDto goalTaskDto)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var result = await _goalTaskService.CreateGoalTaskAsync(goalId, goalTaskDto, userId.Value);

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
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var result = await _goalTaskService.UpdateGoalTaskAsync(goalId, goalTaskId, goalTaskDto, userId.Value);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return NoContent();
        }

        [HttpPut("{goalTaskId}/complete")]
        public async Task<ActionResult> UpdateIsCompleteGoalTask([FromRoute] int goalId, [FromRoute] int goalTaskId, [FromBody] bool isComplete)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var result = await _goalTaskService.UpdateIsCompleteForGoalTaskAsync(goalId, goalTaskId, isComplete, userId.Value);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return NoContent();
        }

        [HttpDelete("{goalTaskId}")]
        public async Task<ActionResult> DeleteGoalTask([FromRoute] int goalId, [FromRoute] int goalTaskId)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var result = await _goalTaskService.RemoveGoalTaskAsync(goalId, goalTaskId, userId.Value);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return NoContent();
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return null;

            return userId;
        }
    }
}
