using GoalTracker.Enums;

namespace GoalTracker.Common.Errors
{
    public static class AuthErrors
    {
        public static Error InvalidCredentials() =>
            new("Auth.InvalidCredentials", ErrorType.Unauthorized, "Invalid email or password.");

        public static Error PasswordMismatch() =>
            new("Auth.PasswordMismatch", ErrorType.Validation, "Password and confirmation password do not match.");

        public static Error EmailAlreadyInUse(string email) =>
            new("Auth.EmailAlreadyInUse", ErrorType.Conflict, $"A user with email '{email}' already exists.");

        public static Error IdentityValidation(string details) =>
            new("Auth.IdentityValidation", ErrorType.Validation, details);
    }
}
