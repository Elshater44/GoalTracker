using GoalTracker.Enums;

namespace GoalTracker.Models
{
    public class Goal
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required UrgencyLevel UrgencyLevel { get; set; }
        public string? Icon { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<GoalTask> GoalTasks { get; set; } = new List<GoalTask>();
        public required int UserId { get; set; }
        public required User User { get; set; }
    }
}
