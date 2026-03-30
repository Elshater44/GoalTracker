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
            var tasks = await _goalTaskService.getAllTasksAsync(goalId);
            if (tasks is null) return NotFound();
            return Ok(tasks);
        }

        [HttpGet("{goalTaskId}")]
        public async Task<ActionResult<GoalTaskGetDto>> GetGoalTaskById([FromRoute] int goalId, [FromRoute] int goalTaskId)
        {
            var task = await _goalTaskService.GetGoalTaskByIdAsync(goalId, goalTaskId);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<GoalTaskGetDto>> CreateGoalTask([FromRoute] int goalId, [FromBody] GoalTaskCreateDto goalTaskDto)
        {
            var createdTask = await _goalTaskService.CreateGoalTaskAsync(goalId, goalTaskDto);

            if (createdTask == null)
                return NotFound();

            return CreatedAtAction(
                nameof(GetGoalTaskById),
                new { goalId = goalId, goalTaskId = createdTask.Id },
                createdTask
            );
        }

        [HttpPut("{goalTaskId}")]
        public async Task<ActionResult> UpdateGoalTask([FromRoute] int goalId, [FromRoute] int goalTaskId, [FromBody] GoalTaskUpdateDto goalTaskDto)
        {
            var success = await _goalTaskService.UpdateGoalTaskAsync(goalId, goalTaskId, goalTaskDto);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{goalTaskId}/complete")]
        public async Task<ActionResult> UpdateIsCompleteGoalTask([FromRoute] int goalId, [FromRoute] int goalTaskId, [FromBody] bool isComplete)
        {
            var success = await _goalTaskService.UpdateIsCompleteForGoalTaskAsync(goalId, goalTaskId, isComplete);

            if (!success)
                return NotFound();

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