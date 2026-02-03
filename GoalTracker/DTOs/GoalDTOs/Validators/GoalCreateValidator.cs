using FluentValidation;
using GoalTracker.Enums;

namespace GoalTracker.DTOs.GoalDTOs.Validators
{
    public class GoalCreateValidator : AbstractValidator<GoalCreateDto>
    {
        public GoalCreateValidator()
        {
            RuleFor(g => g.Deadline).GreaterThanOrEqualTo(DateTime.Today);
            RuleFor(g => g.Name).NotEmpty();
            RuleFor(g => g.Description).NotEmpty();
            RuleFor(g => g.Icon).MaximumLength(50);
            RuleFor(g => g.UregencyLevel)
            .IsInEnum()
            .WithMessage(dto =>
            {
                var entries = Enum.GetValues(typeof(UrgencyLevel))
                    .Cast<UrgencyLevel>()
                    .Select(e => $"{(int)e} ({e})"); // Creates "1 (Low)", "2 (Medium)", etc.

                return $"Invalid Urgency Level. Valid options are: {string.Join(", ", entries)}";
            });
        }
    }
}
