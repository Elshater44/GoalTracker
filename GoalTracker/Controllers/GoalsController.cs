using GoalTracker.Common.Results.Extensions;
using GoalTracker.DTOs.GoalDTOs;
using GoalTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoalTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalsController : ControllerBase
    {
        private readonly GoalService _goalService;

        public GoalsController(GoalService goalService)
        {
            _goalService = goalService;
        }

        // GET: api/goals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoalGetDto>>> GetAllGoals()
        {
            var goals = await _goalService.GetAllGoalsAsync();
            return Ok(goals);
        }

        // GET api/goals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GoalGetDto>> GetGoalById(int id)
        {
            var result = await _goalService.GetGoalByIdAsync(id);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return Ok(result.Value);
        }

        // POST api/goals
        [HttpPost]
        public async Task<ActionResult> CreateGoal(GoalCreateDto dto)
        {
            var result = await _goalService.CreateGoalAsync(dto);
            return CreatedAtAction(
                nameof(GetGoalById),
                new { id = result.Value!.Id },
                result.Value
            );
        }

        // PUT api/goals/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGoal(int id, GoalUpdateDto dto)
        {
            var result = await _goalService.UpdateGoalAsync(id, dto);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return NoContent();
        }

        // DELETE api/goals/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGoal(int id)
        {
            var result = await _goalService.DeleteGoalAsync(id);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return NoContent();
        }
    }
}
