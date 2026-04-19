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

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<List<GoalTaskGetDto>>> GetAllGoalTasks([FromRoute] int goalId)
        {
            var result = await _goalTaskService.GetAllTasksAsync(goalId, CurrentUserId);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult(this);
        }

        [HttpGet("{goalTaskId}")]
        public async Task<ActionResult<GoalTaskGetDto>> GetGoalTaskById([FromRoute] int goalId, [FromRoute] int goalTaskId)
        {
            var result = await _goalTaskService.GetGoalTaskByIdAsync(goalId, goalTaskId, CurrentUserId);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult(this);
        }

        [HttpPost]
        public async Task<ActionResult<GoalTaskGetDto>> CreateGoalTask([FromRoute] int goalId, [FromBody] GoalTaskCreateDto goalTaskDto)
        {
            var result = await _goalTaskService.CreateGoalTaskAsync(goalId, goalTaskDto, CurrentUserId);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return CreatedAtAction(
                nameof(GetGoalTaskById),
                new { goalId, goalTaskId = result.Value!.Id },
                result.Value
            );
        }

        [HttpPut("{goalTaskId}")]
        public async Task<ActionResult> UpdateGoalTask([FromRoute] int goalId, [FromRoute] int goalTaskId, [FromBody] GoalTaskUpdateDto goalTaskDto)
        {
            var result = await _goalTaskService.UpdateGoalTaskAsync(goalId, goalTaskId, goalTaskDto, CurrentUserId);

            return result.IsSuccess ? NoContent() : result.ToProblemResult(this);
        }

        [HttpPut("{goalTaskId}/complete")]
        public async Task<ActionResult> UpdateIsCompleteGoalTask([FromRoute] int goalId, [FromRoute] int goalTaskId, [FromBody] bool isComplete)
        {
            var result = await _goalTaskService.UpdateIsCompleteForGoalTaskAsync(goalId, goalTaskId, isComplete, CurrentUserId);

            return result.IsSuccess ? NoContent() : result.ToProblemResult(this);
        }

        [HttpDelete("{goalTaskId}")]
        public async Task<ActionResult> DeleteGoalTask([FromRoute] int goalId, [FromRoute] int goalTaskId)
        {
            var result = await _goalTaskService.RemoveGoalTaskAsync(goalId, goalTaskId, CurrentUserId);

            return result.IsSuccess ? NoContent() : result.ToProblemResult(this);
        }
    }
}