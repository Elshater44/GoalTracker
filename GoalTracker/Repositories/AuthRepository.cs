using GoalTracker.Models;
using GoalTracker.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GoalTracker.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _userManager;

        public AuthRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public Task<User?> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public Task<bool> CheckPasswordAsync(User user, string password)
        {
            return _userManager.CheckPasswordAsync(user, password);
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return _userManager.Users.ToListAsync();
        }
    }
}
