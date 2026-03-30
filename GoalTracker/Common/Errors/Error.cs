namespace GoalTracker.Common.Errors
{
    public record Error(string Id, ErrorType Type, string Description);
}