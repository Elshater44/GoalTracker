# Logging Best Practices & Patterns

## Structured Logging Patterns

### 1. CRUD Operations

```csharp
// CREATE
_logger.LogInformation("Creating resource of type {ResourceType} by user {UserId}", 
    nameof(Goal), userId);

// READ
_logger.LogDebug("Fetching {ResourceType} with Id {ResourceId} for user {UserId}", 
    nameof(Goal), goalId, userId);

// UPDATE
_logger.LogInformation("Updating {ResourceType} {ResourceId} for user {UserId}. Fields: {Fields}", 
    nameof(Goal), goalId, userId, "Name, Description");

// DELETE
_logger.LogInformation("Deleting {ResourceType} {ResourceId} for user {UserId}", 
    nameof(Goal), goalId, userId);
```

### 2. Authentication Events

```csharp
// Successful login
_logger.LogInformation("User authenticated successfully. UserId: {UserId}, Email: {Email}, Timestamp: {Timestamp}", 
    user.Id, user.Email, DateTime.UtcNow);

// Failed login
_logger.LogWarning("Authentication failed for email {Email}. Reason: {Reason}", 
    email, "Invalid credentials");

// Registration
_logger.LogInformation("New user registered. Email: {Email}, Timestamp: {Timestamp}", 
    email, DateTime.UtcNow);
```

### 3. Error Handling

```csharp
// Expected error
_logger.LogWarning("Resource not found. ResourceType: {ResourceType}, Id: {ResourceId}, UserId: {UserId}", 
    resourceType, id, userId);

// Unexpected error
_logger.LogError(ex, "Unexpected error during {Operation}. UserId: {UserId}, Details: {@Exception}", 
    operationName, userId, ex);

// Retry scenario
_logger.LogWarning("Operation {Operation} failed. Attempt {Attempt} of {MaxAttempts}. Error: {Error}", 
    operation, attempt, maxAttempts, errorMessage);
```

### 4. Database Operations

```csharp
// Query execution
_logger.LogDebug("Executing query for {ResourceType}. Filter: {@Filter}, Sort: {Sort}", 
    resourceType, filterObject, sortBy);

// Query performance
_logger.LogInformation("Query executed. ResourceType: {ResourceType}, Duration: {DurationMs}ms, Results: {ResultCount}", 
    resourceType, elapsedMilliseconds, resultCount);

// Database error
_logger.LogError(ex, "Database operation failed. Operation: {Operation}, Message: {Message}", 
    operationName, ex.Message);
```

### 5. Business Logic

```csharp
// Validation
_logger.LogWarning("Validation failed. Field: {Field}, Value: {@Value}, Reason: {Reason}", 
    fieldName, fieldValue, validationReason);

// State change
_logger.LogInformation("Goal status changed. GoalId: {GoalId}, OldStatus: {OldStatus}, NewStatus: {NewStatus}", 
    goalId, oldStatus, newStatus);

// Business rule violation
_logger.LogWarning("Business rule violated. Rule: {Rule}, UserId: {UserId}, Context: {@Context}", 
    ruleName, userId, contextObject);
```

---

## Log Levels Guide

### 🔴 Fatal
- Application cannot continue
- System shutdown required
- Use rarely

```csharp
_logger.LogCritical("Critical failure - application cannot continue. Error: {Error}", errorMessage);
```

### 🟠 Error
- Recoverable errors
- Operations failed
- Data integrity issues
- Use frequently for important operations

```csharp
_logger.LogError(ex, "Operation failed for {UserId}: {ErrorMessage}", userId, ex.Message);
```

### 🟡 Warning
- Potential issues
- Invalid data
- Fallback mechanisms triggered
- Retry attempts
- Use for non-critical failures

```csharp
_logger.LogWarning("Invalid input for {Field}: {@Value}", fieldName, value);
```

### 🔵 Information
- Significant state changes
- Important business events
- Start/end of operations
- Use for business-relevant events

```csharp
_logger.LogInformation("User {Email} logged in successfully", email);
```

### ⚪ Debug
- Detailed operational flow
- Variable values
- Function entry/exit (optional)
- Use for development and troubleshooting

```csharp
_logger.LogDebug("Processing query with parameters: {@Parameters}", queryParams);
```

### ⚙️ Trace
- Very verbose details
- Not typically used in production
- Use rarely

```csharp
_logger.LogTrace("Entering method {MethodName} with args: {@Args}", methodName, args);
```

---

## Anti-Patterns to Avoid

### ❌ BAD: Logging Everything
```csharp
// Don't do this - logs will be huge
_logger.LogInformation("Starting method");
_logger.LogInformation("Line 1 executed");
_logger.LogInformation("Line 2 executed");
_logger.LogInformation("Method completed");
```

### ✅ GOOD: Log Meaningful Events
```csharp
// Do this - logs key events only
_logger.LogInformation("Processing goal creation for user {UserId}", userId);
_logger.LogInformation("Goal created successfully with Id {GoalId}", goalId);
```

### ❌ BAD: String Concatenation
```csharp
_logger.LogInformation("User " + email + " logged in at " + DateTime.Now);
```

### ✅ GOOD: Structured Properties
```csharp
_logger.LogInformation("User login successful. Email: {Email}, Timestamp: {Timestamp}", 
    email, DateTime.UtcNow);
```

### ❌ BAD: Logging Sensitive Data
```csharp
_logger.LogInformation("Password hash: {Hash}", passwordHash);
```

### ✅ GOOD: Only Log Safe Data
```csharp
_logger.LogInformation("User authentication attempt for email: {Email}", email);
```

---

## Performance Considerations

### Guard Expensive Operations

```csharp
// Guard against expensive operations
if (_logger.IsEnabled(LogLevel.Debug))
{
    _logger.LogDebug("Processing collection: {@Items}", expensiveSerializeOperation);
}
```

### Use Lazy Evaluation

```csharp
// Serilog automatically handles this efficiently
_logger.LogInformation("Complex object: {@ComplexObject}", largeObject);
```

### Avoid Logging in Hot Paths

```csharp
// ❌ BAD: Logging inside a loop that runs millions of times
foreach (var item in largeCollection)
{
    _logger.LogDebug("Processing item {ItemId}", item.Id);  // Too much!
}

// ✅ GOOD: Log summary instead
_logger.LogInformation("Processing {ItemCount} items", largeCollection.Count);
foreach (var item in largeCollection)
{
    ProcessItem(item);
}
_logger.LogInformation("Batch processing completed. Processed: {ItemCount}", largeCollection.Count);
```

---

## Correlation IDs for Request Tracing

### Setup (Phase 2):

```csharp
// Add to Serilog enrichment
.Enrich.FromLogContext()

// Add to middleware
context.Items["CorrelationId"] = context.TraceIdentifier;
LogContext.PushProperty("CorrelationId", context.TraceIdentifier);

// Now all logs will include CorrelationId automatically
```

---

## Testing Logging

### Unit Test Example

```csharp
[Fact]
public async Task CreateGoal_LogsInformation()
{
    // Arrange
    var mockLogger = new Mock<ILogger<GoalService>>();
    var mockRepository = new Mock<IGoalRepository>();
    var mockMapper = new Mock<IMapper>();
    var service = new GoalService(mockRepository.Object, mockMapper.Object, mockLogger.Object);

    // Act
    await service.CreateGoalAsync(goalDto, userId);

    // Assert
    mockLogger.Verify(
        x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Creating goal")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
        Times.Once);
}
```

