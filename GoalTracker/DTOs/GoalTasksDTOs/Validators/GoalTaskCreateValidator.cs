using FluentValidation;

namespace GoalTracker.DTOs.GoalTasksDTOs.Validators
{
    public class GoalTaskCreateValidator : AbstractValidator<GoalTaskCreateDto>
    {
        public GoalTaskCreateValidator()
        {
            RuleFor(gt => gt.Name).NotEmpty();
            RuleFor(gt => gt.Description).NotEmpty();
            RuleFor(gt => gt.Icon).MaximumLength(50);
        }
    }
}
