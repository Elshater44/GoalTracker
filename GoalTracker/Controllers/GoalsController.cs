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
            var goal = await _goalService.GetGoalByIdAsync(id);

            if (goal is null)
                return NotFound();

            return Ok(goal);
        }

        // POST api/goals
        [HttpPost]
        public async Task<ActionResult> CreateGoal(GoalCreateDto dto)
        {
            var createdGoal = await _goalService.CreateGoalAsync(dto);

            return CreatedAtAction(
                nameof(GetGoalById),
                new { id = createdGoal.Id }
                , createdGoal
            );
        }

        // PUT api/goals/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGoal(int id, GoalUpdateDto dto)
        {
            var updated = await _goalService.UpdateGoalAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE api/goals/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGoal(int id)
        {
            var deleted = await _goalService.DeleteGoalAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
