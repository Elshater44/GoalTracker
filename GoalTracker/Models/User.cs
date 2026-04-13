using Microsoft.AspNetCore.Identity;

namespace GoalTracker.Models
{
    public class User : IdentityUser<int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public List<Goal?> Goals { get; set; } = new();
    }
}
