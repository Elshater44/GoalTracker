using GoalTracker.Models;
using Microsoft.AspNetCore.Identity;

namespace GoalTracker.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<User?> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<List<User>> GetAllUsersAsync();
    }
}
