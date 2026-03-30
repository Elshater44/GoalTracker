namespace GoalTracker.Common.Errors
{
    public static class GoalErrors
    {
        public static Error NotFound(int id) =>
            new("Goal.NotFound", ErrorType.NotFound, $"Goal with id {id} was not found.");
    }
}
