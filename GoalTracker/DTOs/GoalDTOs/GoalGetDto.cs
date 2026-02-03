using GoalTracker.DTOs.GoalTasksDTOs;
using GoalTracker.Enums;

namespace GoalTracker.DTOs.GoalDTOs
{
    public class GoalGetDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required UrgencyLevel UregencyLevel { get; set; }
        public required bool IsMissed { get; set; }
        public required bool IsCompleted { get; set; }
        public string? Icon { get; set; }
        public DateTime? Deadline { get; set; }
        public List<GoalTaskGetDto>? GoalTasks { get; set; }


    }
}
