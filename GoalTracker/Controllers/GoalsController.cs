using FluentValidation;
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
        private readonly IValidator<GoalCreateDto> _goalCreateValidator;
        private readonly IValidator<GoalUpdateDto> _goalUpdateValidator;

        public GoalsController(GoalService goalService, IValidator<GoalCreateDto> goalCreateValidator, IValidator<GoalUpdateDto> goalUpdateValidator)
        {
            _goalService = goalService;
            _goalCreateValidator = goalCreateValidator;
            _goalUpdateValidator = goalUpdateValidator;
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
            var validationResult = await _goalCreateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                               .Select(e => new { e.PropertyName, e.ErrorMessage })
                               .ToList();

                // Return 400 BadRequest with the list of errors
                return BadRequest(errors);
            }

            await _goalService.CreateGoalAsync(dto);

            // Better REST practice than NoContent
            return CreatedAtAction(
                nameof(GetGoalById),
                new { id = 0 }, // you don’t return id from service yet
                null
            );
        }

        // PUT api/goals/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGoal(int id, GoalUpdateDto dto)
        {
            var validationResult = await _goalUpdateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new { e.PropertyName, e.ErrorMessage })
                    .ToList();
                return BadRequest(errors);
            }
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
