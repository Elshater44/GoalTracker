# Quick Reference Card - Logging

## 🚀 30-Second Summary

**Problem**: Your app has no application logging. Services, repos, and controllers don't log anything.

**Solution**: Add `ILogger<T>` to all services and repos, log CRUD operations and errors.

**Time**: 2 hours | **Impact**: Game-changer for debugging and auditing

---

## Phase 1 Quick Steps

### Step 1: Install Packages
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Enrichers.Environment
```

### Step 2: Update Program.cs
```csharp
// REPLACE this:
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

// WITH this:
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "goaltracker-.txt");
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: 
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        logPath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: 
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .CreateLogger();

// Add this after var app = builder.Build();
builder.UseSerilog();
```

### Step 3: Add ILogger to GoalService
```csharp
private readonly ILogger<GoalService> _logger;

public GoalService(IGoalRepository goalRepository, IMapper mapper, ILogger<GoalService> logger)
{
    _goalRepository = goalRepository;
    _mapper = mapper;
    _logger = logger;
}

// In CreateGoalAsync:
_logger.LogInformation("Creating goal for user {UserId}: {GoalName}", userId, goalDto.Name);
// ... code ...
_logger.LogInformation("Goal created successfully. GoalId: {GoalId}, UserId: {UserId}", goal.Id, userId);
```

### Step 4: Add ILogger to AuthService
```csharp
private readonly ILogger<AuthService> _logger;

public AuthService(IAuthRepository authRepository, TokenService tokenService, ILogger<AuthService> logger)
{
    // ...
    _logger = logger;
}

// In LoginAsync:
_logger.LogInformation("Login attempt: {Email}", loginDto.Email);
// ... code ...
_logger.LogInformation("Login successful: {Email}", loginDto.Email);
```

### Step 5: Add ILogger to GoalRepository
```csharp
private readonly ILogger<GoalRepository> _logger;

public GoalRepository(AppDbContext context, ILogger<GoalRepository> logger)
{
    _context = context;
    _logger = logger;
}

// In GetGoalsAsync:
_logger.LogDebug("Fetching goals for user {UserId} with query params: {@Params}", userId, queryParams);
var (items, totalCount) = await _goalRepository.GetGoalsAsync(userId, queryParams);
_logger.LogInformation("Retrieved {ItemCount} goals for user {UserId}", items.Count, userId);
```

### Step 6: Test It
- Run the application
- Create a goal
- Login
- Check `logs/` folder for daily log files
- Verify console shows formatted logs

---

## Common Logging Patterns

### Log Information
```csharp
_logger.LogInformation("User {UserId} created goal: {GoalName}", userId, goalName);
```

### Log Warning
```csharp
_logger.LogWarning("Goal not found. GoalId: {GoalId}, UserId: {UserId}", goalId, userId);
```

### Log Error with Exception
```csharp
try { /* code */ }
catch (Exception ex)
{
    _logger.LogError(ex, "Error creating goal for user {UserId}", userId);
    throw;
}
```

### Log Debug (complex objects)
```csharp
_logger.LogDebug("Query parameters: {@QueryParams}", queryParams);
```

---

## Log Levels Cheat Sheet

| Level | When | Example |
|-------|------|---------|
| Critical | App can't continue | Database connection lost |
| Error | Operation failed | Save failed, auth failed |
| Warning | Unexpected but ok | User not found, invalid input |
| Information | Key event | Login success, goal created |
| Debug | Detailed troubleshooting | Query executed, object serialized |
| Trace | Everything | Not used in production |

---

## Files to Modify

```
GoalTracker/
├── Program.cs ........................... ← MODIFY: Add Serilog config
├── appsettings.json ..................... ← ADD: Serilog section
├── Services/
│   ├── GoalService.cs .................. ← MODIFY: Add ILogger
│   ├── AuthService.cs .................. ← MODIFY: Add ILogger
│   ├── GoalTaskService.cs .............. ← MODIFY: Add ILogger
│   └── TokenService.cs ................. ← MODIFY: Add ILogger
├── Repositories/
│   ├── GoalRepository.cs ............... ← MODIFY: Add ILogger
│   ├── GoalTaskRepository.cs ........... ← MODIFY: Add ILogger
│   └── AuthRepository.cs ............... ← MODIFY: Add ILogger
└── logs/ .............................. ← AUTO-CREATED: Daily log files
```

---

## What Gets Logged

### After Phase 1 Implementation:

✅ User Registration
```
[2024-04-15 10:15:30.123] [INF] User registration attempt: user@example.com
[2024-04-15 10:15:31.456] [INF] User registered successfully: user@example.com
```

✅ User Login
```
[2024-04-15 10:20:00.789] [INF] Login attempt: user@example.com
[2024-04-15 10:20:01.234] [INF] Login successful: user@example.com
```

✅ Goal Creation
```
[2024-04-15 10:25:15.567] [INF] Creating goal for user 1: "Fitness Goal"
[2024-04-15 10:25:15.890] [INF] Goal created successfully. GoalId: 5, UserId: 1
```

✅ Goal Updates
```
[2024-04-15 10:30:45.123] [INF] Updating goal 5 for user 1. Fields: Name, Description
[2024-04-15 10:30:45.456] [INF] Goal 5 updated successfully for user 1
```

✅ Goal Deletion
```
[2024-04-15 10:35:20.789] [INF] Deleting goal 5 for user 1
[2024-04-15 10:35:20.234] [INF] Goal 5 deleted successfully for user 1
```

✅ Errors with Context
```
[2024-04-15 10:40:10.567] [ERR] Error creating goal for user 1
System.InvalidOperationException: Goal name cannot be empty
    at GoalTracker.Services.GoalService.CreateGoalAsync()
```

---

## Expected Directory Structure After Running

```
GoalTracker/
├── logs/
│   ├── goaltracker-20240415.txt
│   ├── goaltracker-20240416.txt
│   ├── goaltracker-20240417.txt
│   └── ... (keeps 30 days of logs)
```

Each file contains:
```
[2024-04-15 10:32:45.123 +02:00] [INF] User logged in
[2024-04-15 10:32:46.456 +02:00] [INF] Goal created
[2024-04-15 10:32:47.789 +02:00] [WRN] Goal not found
```

---

## Troubleshooting

### Logs not appearing?
1. Check `logs/` folder exists
2. Verify `builder.UseSerilog()` is called
3. Check console for any errors

### Logs not in files?
1. Verify file path in config
2. Check file permissions
3. Verify `RollingInterval.Day` is set

### Too many/few logs?
1. Adjust `MinimumLevel` in Serilog config
2. Override specific namespaces as needed

---

## Performance Impact

- **Logging Performance**: Negligible (~1% overhead)
- **Disk Space**: ~5-10 MB per day depending on traffic
- **Memory**: Minimal (logs are written immediately)

---

## Next: Phase 2 (Optional)

After Phase 1, consider:
- Request/Response logging middleware
- Exception handling middleware
- Correlation ID tracking
- Application Insights

See `LOGGING_ROADMAP.md` for details.

---

## Need Help?

📚 Documentation files in project root:
- `LOGGING_ASSESSMENT.md` - Detailed analysis
- `LOGGING_IMPLEMENTATION_GUIDE.md` - Full code examples
- `LOGGING_ROADMAP.md` - Visual roadmap
- `LOGGING_BEST_PRACTICES.md` - Patterns guide
- `LOGGING_SUMMARY.md` - Executive summary

