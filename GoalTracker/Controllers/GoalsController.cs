using GoalTracker.Common.Results.Extensions;
using GoalTracker.DTOs.Pagination;
using GoalTracker.DTOs.GoalDTOs;
using GoalTracker.DTOs.GoalDTOs.Query;
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

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<PagedResponse<GoalGetDto>>> GetGoals([FromQuery] GoalQueryParams queryParams)
        {
            var result = await _goalService.GetGoalsAsync(CurrentUserId, queryParams);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GoalGetDto>> GetGoalById(int id)
        {
            var result = await _goalService.GetGoalByIdAsync(id, CurrentUserId);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<ActionResult> CreateGoal(GoalCreateDto dto)
        {
            var result = await _goalService.CreateGoalAsync(dto, CurrentUserId);

            return CreatedAtAction(
                nameof(GetGoalById),
                new { id = result.Value!.Id },
                result.Value
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGoal(int id, GoalUpdateDto dto)
        {
            var result = await _goalService.UpdateGoalAsync(id, dto, CurrentUserId);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGoal(int id)
        {
            var result = await _goalService.DeleteGoalAsync(id, CurrentUserId);

            if (!result.IsSuccess)
                return result.ToProblemResult(this);

            return NoContent();
        }
    }
}
