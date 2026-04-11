namespace GoalTracker.Enums
{
    public enum ErrorType
    {
        Validation = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        Internal = 500
    }
}