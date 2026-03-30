namespace GoalTracker.Common.Errors
{
    public static class GoalTaskErrors
    {
        public static Error NotFound(int goalId, int goalTaskId) =>
            new(
                "GoalTask.NotFound",
                ErrorType.NotFound,
                $"GoalTask with id {goalTaskId} for Goal {goalId} was not found."
            );

        public static Error GoalNotFound(int goalId) =>
            new(
                "GoalTask.GoalNotFound",
                ErrorType.NotFound,
                $"Goal with id {goalId} was not found."
            );
    }
}
