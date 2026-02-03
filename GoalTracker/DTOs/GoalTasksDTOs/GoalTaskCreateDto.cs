namespace GoalTracker.DTOs.GoalTasksDTOs
{
    public class GoalTaskCreateDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string? Icon { get; set; }
    }
}