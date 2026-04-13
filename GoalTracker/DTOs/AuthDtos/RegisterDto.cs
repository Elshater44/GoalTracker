namespace GoalTracker.DTOs.AuthDtos
{
    public class RegisterDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PasswordConfirmation { get; set; }
        public required DateTime DateOfBirth { get; set; }
    }
}