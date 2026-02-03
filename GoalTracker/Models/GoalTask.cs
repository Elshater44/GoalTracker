using System.ComponentModel.DataAnnotations.Schema;

namespace GoalTracker.Models
{
    public class GoalTask
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required bool IsCompleted { get; set; }
        public string? Icon { get; set; }
        public int GoalId { get; set; }
        public required Goal Goal { get; set; }

    }
}