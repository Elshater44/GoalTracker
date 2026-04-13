using GoalTracker.Data;
using GoalTracker.DTOs.AuthDtos;
using GoalTracker.Models;
using GoalTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GoalTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly TokenService _tokenService;

        public AuthController(UserManager<User> userManager, AppDbContext dbContext, TokenService tokenService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _tokenService = tokenService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<string>> RegisterAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                DateOfBirth = registerDto.DateOfBirth,
                UserName = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            var token = _tokenService.CreateToken(user);
            return Ok(token);
        }
        [HttpGet("users")]
        [Authorize]
        public ActionResult<List<User>> GetAllUsers()
        {
            return Ok(_dbContext.Users.ToList());
        }
    }
}
