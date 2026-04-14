using GoalTracker.Common.Errors;
using GoalTracker.Common.Results;
using GoalTracker.DTOs.AuthDtos;
using GoalTracker.Models;
using GoalTracker.Repositories.Interfaces;

namespace GoalTracker.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly TokenService _tokenService;

        public AuthService(IAuthRepository authRepository, TokenService tokenService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
        }

        public async Task<Result<string>> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto.Password != registerDto.PasswordConfirmation)
            {
                return Result<string>.Failure(AuthErrors.PasswordMismatch());
            }

            var existingUser = await _authRepository.FindByEmailAsync(registerDto.Email);
            if (existingUser is not null)
            {
                return Result<string>.Failure(AuthErrors.EmailAlreadyInUse(registerDto.Email));
            }

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                DateOfBirth = registerDto.DateOfBirth,
                UserName = registerDto.Email
            };

            var result = await _authRepository.CreateUserAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                return Result<string>.Failure(AuthErrors.IdentityValidation(errors));
            }

            return Result<string>.Success(_tokenService.CreateToken(user));
        }

        public async Task<Result<string>> LoginAsync(LoginDto loginDto)
        {
            var user = await _authRepository.FindByEmailAsync(loginDto.Email);

            if (user is null)
            {
                return Result<string>.Failure(AuthErrors.InvalidCredentials());
            }

            var isPasswordValid = await _authRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                return Result<string>.Failure(AuthErrors.InvalidCredentials());
            }

            return Result<string>.Success(_tokenService.CreateToken(user));
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return _authRepository.GetAllUsersAsync();
        }
    }
}
