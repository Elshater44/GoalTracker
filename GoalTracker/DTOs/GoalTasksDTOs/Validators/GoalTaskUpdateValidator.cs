using FluentValidation;

namespace GoalTracker.DTOs.GoalTasksDTOs.Validators
{
    public class GoalTaskUpdateValidator : AbstractValidator<GoalTaskUpdateDto>
    {
        public GoalTaskUpdateValidator()
        {
            RuleFor(gt => gt.Name).NotEmpty();
            RuleFor(gt => gt.Description).NotEmpty();
            RuleFor(gt => gt.Icon).MaximumLength(50);
        }
    }
}
