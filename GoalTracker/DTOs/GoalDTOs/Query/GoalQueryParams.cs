using GoalTracker.DTOs.Pagination;
using GoalTracker.Enums;

namespace GoalTracker.DTOs.GoalDTOs.Query
{
    public class GoalQueryParams : QueryParams
    {
        public bool? IsCompleted { get; set; }
        public bool? IsMissed { get; set; }
        public UrgencyLevel? UrgencyLevel { get; set; }
        public string? Search { get; set; }
        public bool? HasDeadline { get; set; }
    }
}
