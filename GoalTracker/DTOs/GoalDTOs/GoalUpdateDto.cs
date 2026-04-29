using GoalTracker.Enums;

namespace GoalTracker.DTOs.GoalDTOs
{
    public class GoalUpdateDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required UrgencyLevel UrgencyLevel{ get; set; }
        public string? Icon { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
