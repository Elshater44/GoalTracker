# Logging Implementation Guide - Phase 1

## Step 1: Enhanced Serilog Configuration

### Update Program.cs with comprehensive Serilog setup:

```csharp
// BEFORE (Current):
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

// AFTER (Enhanced):
Log.Logger = new LoggerConfiguration()
    // Console output with formatted messages
    .WriteTo.Console(outputTemplate: 
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    // File output with rolling (new file daily)
    .WriteTo.File(
        "logs/goaltracker-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: 
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    // Enrich with additional properties
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "GoalTracker")
    // Set minimum log level
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .CreateLogger();

builder.UseSerilog();  // Add this line to use Serilog for built-in logging
```

---

## Step 2: Add Logging to Services

### Example: Enhanced GoalService with Logging

```csharp
using ILogger;

public class GoalService
{
    private readonly IGoalRepository _goalRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GoalService> _logger;

    public GoalService(IGoalRepository goalRepository, IMapper mapper, ILogger<GoalService> logger)
    {
        _goalRepository = goalRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<GoalGetDto>> CreateGoalAsync(GoalCreateDto goalDto, int userId)
    {
        try
        {
            _logger.LogInformation("Creating goal for user {UserId}: {GoalName}", userId, goalDto.Name);

            var goal = _mapper.Map<Goal>(goalDto);
            goal.UserId = userId;

            await _goalRepository.AddGoalAsync(goal);
            await _goalRepository.SaveChangesAsync();

            _logger.LogInformation("Goal created successfully. GoalId: {GoalId}, UserId: {UserId}", 
                goal.Id, userId);

            return Result<GoalGetDto>.Success(_mapper.Map<GoalGetDto>(goal));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating goal for user {UserId}", userId);
            return Result<GoalGetDto>.Failure(GoalErrors.CreationFailed());
        }
    }

    public async Task<Result> DeleteGoalAsync(int id, int userId)
    {
        try
        {
            _logger.LogInformation("Deleting goal {GoalId} for user {UserId}", id, userId);

            var goal = await _goalRepository.GetGoalByIdAsync(id, userId);
            if (goal is null)
            {
                _logger.LogWarning("Goal not found. GoalId: {GoalId}, UserId: {UserId}", id, userId);
                return Result.Failure(GoalErrors.NotFound(id));
            }

            _goalRepository.RemoveGoal(goal);
            await _goalRepository.SaveChangesAsync();

            _logger.LogInformation("Goal deleted successfully. GoalId: {GoalId}, UserId: {UserId}", 
                id, userId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting goal {GoalId} for user {UserId}", id, userId);
            return Result.Failure(GoalErrors.DeletionFailed());
        }
    }
}
```

---

## Step 3: Add Logging to AuthService

```csharp
public class AuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly TokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IAuthRepository authRepository, TokenService tokenService, ILogger<AuthService> logger)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<string>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            _logger.LogInformation("User registration attempt: {Email}", registerDto.Email);

            if (registerDto.Password != registerDto.PasswordConfirmation)
            {
                _logger.LogWarning("Registration failed - password mismatch for: {Email}", registerDto.Email);
                return Result<string>.Failure(AuthErrors.PasswordMismatch());
            }

            var existingUser = await _authRepository.FindByEmailAsync(registerDto.Email);
            if (existingUser is not null)
            {
                _logger.LogWarning("Registration failed - email already in use: {Email}", registerDto.Email);
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
                _logger.LogError("Registration failed for {Email}. Errors: {Errors}", 
                    registerDto.Email, errors);
                return Result<string>.Failure(AuthErrors.IdentityValidation(errors));
            }

            _logger.LogInformation("User registered successfully: {Email}", registerDto.Email);
            return Result<string>.Success(_tokenService.CreateToken(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for: {Email}", registerDto.Email);
            throw;
        }
    }

    public async Task<Result<string>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation("Login attempt: {Email}", loginDto.Email);

            var user = await _authRepository.FindByEmailAsync(loginDto.Email);

            if (user is null)
            {
                _logger.LogWarning("Login failed - user not found: {Email}", loginDto.Email);
                return Result<string>.Failure(AuthErrors.InvalidCredentials());
            }

            var isPasswordValid = await _authRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed - invalid password for: {Email}", loginDto.Email);
                return Result<string>.Failure(AuthErrors.InvalidCredentials());
            }

            _logger.LogInformation("Login successful: {Email}", loginDto.Email);
            return Result<string>.Success(_tokenService.CreateToken(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for: {Email}", loginDto.Email);
            throw;
        }
    }
}
```

---

## Step 4: Service Registration Update

In Program.cs, make sure to register ILogger:

```csharp
// Serilog is already configured above
builder.UseSerilog();  // Add this after var app = builder.Build();

// Services already auto-inject ILogger<T> in .NET 6+
// Just add to constructors as shown above
```

---

## Step 5: Required NuGet Packages

```
Serilog
Serilog.AspNetCore
Serilog.Sinks.Console
Serilog.Sinks.File
Serilog.Enrichers.Environment
```

---

## Next Steps After Phase 1:

1. **Add Middleware for Request/Response Logging** (Phase 2)
2. **Add Audit Logging for User Actions** (Phase 2)
3. **Configure Application Insights** (Phase 3)
4. **Setup Correlation IDs** (Phase 3)

