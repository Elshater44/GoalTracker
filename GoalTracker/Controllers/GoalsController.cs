using GoalTracker.Common.Results.Extensions;
using GoalTracker.DTOs.GoalDTOs;
using GoalTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoalTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var goals = await _goalService.GetAllGoalsAsync(userId.Value);
            return Ok(goals);
        }

        // GET api/goals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GoalGetDto>> GetGoalById(int id)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var result = await _goalService.GetGoalByIdAsync(id, userId.Value);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return Ok(result.Value);
        }

        // POST api/goals
        [HttpPost]
        public async Task<ActionResult> CreateGoal(GoalCreateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var result = await _goalService.CreateGoalAsync(dto, userId.Value);
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
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var result = await _goalService.UpdateGoalAsync(id, dto, userId.Value);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return NoContent();
        }

        // DELETE api/goals/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGoal(int id)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var result = await _goalService.DeleteGoalAsync(id, userId.Value);

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
