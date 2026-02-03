namespace GoalTracker.DTOs.GoalTasksDTOs
{
    public class GoalTaskGetDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required bool IsCompleted { get; set; }
        public string? Icon { get; set; }
    }
}