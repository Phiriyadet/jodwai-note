# Conventions

This document outlines coding style and conventions specific to the JodWai Note project.

## Folder and File Naming

### Folder Structure
```
JodWai.Domain/
  Entities/           # Domain entities (aggregates)
  ValueObjects/       # Domain value objects
  Interfaces/         # Domain-level interfaces (if any)

JodWai.Application/
  Commands/           # Command handlers for writes
  Queries/            # Query handlers for reads
  Dtos/               # Data transfer objects
  Mappers/            # Entity-to-DTO mapping
  Interfaces/         # Repository and other interfaces
  Mediators/          # Command/Query mediators (if separate)

JodWai.Infrastructure/
  Persistence/        # EF Core DbContext and configurations
  Repositories/       # Repository implementations
  Migrations/         # Database migration files
  Workers/            # Background jobs (if any)

JodWai.Api/
  Controllers/        # HTTP endpoints
  Middleware/         # Custom middleware
  Extensions/         # Service collection extensions
```

### File Naming
- **Entities**: `PascalCase.cs` (e.g., `Note.cs`)
- **Value Objects**: `PascalCase.cs` (e.g., `NoteTitle.cs`)
- **Commands**: `PascalCaseCommand.cs` (e.g., `CreateNoteCommand.cs`)
- **Queries**: `PascalCaseQuery.cs` (e.g., `GetNotesQuery.cs`)
- **DTOs**: `PascalCaseDto.cs` (e.g., `NoteDto.cs`)
- **Interfaces**: `IPascalCase.cs` (e.g., `INoteRepository.cs`)
- **Implementations**: `PascalCaseImplementation.cs` (e.g., `NoteRepository.cs`)

## Class, Method, Variable Naming

### Classes
- **Entities**: Noun, PascalCase (`Note`)
- **Value Objects**: Noun, PascalCase, `sealed record` (`NoteTitle`)
- **Interfaces**: `I` prefix + Noun, PascalCase (`INoteRepository`)
- **DTOs**: Noun + `Dto`, PascalCase (`NoteDto`)

### Methods
- Instance methods: `camelCase` (`UpdateTitle()`)
- Static factory methods: `PascalCase` (`NoteTitle.From()`)
- Private methods: prefix with underscore (`_AddLink()`)

### Properties
- Public properties: `PascalCase` (`Title`, `Content`)
- Private backing fields: lowercase (`_links`, `_tags`)
- Read-only collections: `IReadOnlyList<T>`/`IReadOnlyCollection<T>`

### Constants
- `const int MaxLength = 200;`
- Meaningful names with clear purpose

## Code Style

### Record Types for Value Objects
```csharp
public sealed record NoteTitle
{
    public const int MaxLength = 200;
    public string Value { get; }
    // constructor and factory methods
}
```

### Entity with Private Setters
```csharp
public class Note
{
    public NoteId Id { get; }
    public NoteTitle Title { get; private set; }
    public NoteContent Content { get; private set; }
    // private constructor and factory method
    // public methods like Update()
}
```

### Value Object Factory Methods
```csharp
public static NoteTitle From(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException("Title cannot be empty.");
    
    value = value.Trim();
    if (value.Length > MaxLength)
        throw new ArgumentException($"Title cannot exceed {MaxLength} characters.");
    
    return new NoteTitle(value);
}
```

## Error Handling

### Domain Exceptions
```csharp
throw new ArgumentException("Title cannot be empty.");
throw new InvalidOperationException("At least one field must be updated.");
throw new InvalidOperationException("Cannot add self-reference.");
```

### Application Layer Exceptions
- Use domain exceptions for business rule violations
- Don't catch and swallow exceptions in handlers
- Return appropriate MediatR `CommandResult`/`QueryResult`

### Infrastructure Layer Exceptions
- Let EF Core exceptions propagate
- Handle connection errors appropriately
- Don't expose implementation details

## Validation

### Where Validation Lives
- **Value Objects**: Constructor-based validation (boundary check)
- **Domain Entity**: Business rule validation in public methods
- **Controllers**: Minimal validation; delegate to domain layer

### Validation Patterns
- Throw `ArgumentException` for invalid input
- Throw `InvalidOperationException` for business rule violations
- Don't validate in controllers; use value objects

### Example
```csharp
public NoteContent From(string? value)
{
    string settledValue = (value ?? string.Empty).Trim();
    
    if (settledValue.Length > MaxLength)
        throw new ArgumentException("Note content is too long.");
    
    return new NoteContent(settledValue);
}
```

## Logging

### Logging Library
- `ILogger` from Microsoft.Extensions.Logging

### What to Log
- Entity creation (with ID)
- Validation errors (with message)
- Transaction failures
- External service calls (with correlation IDs)

### What NOT to Log
- PII (personal identifiable information)
- Sensitive data (passwords, tokens)
- Full stack traces without context

### Example
```csharp
_logger.LogInformation("Created note {Id}", note.Id.Value);
_logger.LogWarning("Failed to save note {Id}: {Error}", note.Id.Value, ex.Message);
```

## Tests

### Test Types

| Type | Purpose | Layer |
|---|---|---|
| Unit tests | Domain logic, value object invariants | Domain, Application |
| Integration tests | Repository, DbContext, external services | Infrastructure |
| API tests | HTTP endpoints, integration with all layers | Api |

### Test Structure
```csharp
[TestClass]
public class CreateNoteCommandTests
{
    [TestMethod]
    public async Task WhenCreatingNote_ThenNoteIsSaved()
    {
        // Arrange
        var command = new CreateNoteCommand(...);
        
        // Act
        var result = await mediator.Send(command);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Data);
    }
}
```

### Test Naming Conventions
- `When_{Action}_Then_{ExpectedOutcome}` (happy path)
- `When_{InvalidCondition}_Then_{ExpectedException}` (error case)
- Include test subject in name

### Mocking Guidelines
- **Mock**: External services (e.g., email, external APIs)
- **Don't mock**: Domain entities, value objects
- **Do test**: Repository layer with in-memory or test database

## Database Migrations

### Creating Migrations
```bash
dotnet ef migrations add MigrationName --project JodWai.Infrastructure
```

### Applying Migrations
```bash
dotnet ef database update --project JodWai.MigrationService
```

### Migration File Location
- `JodWai.Infrastructure/Migrations/`

### Migration Naming
- Descriptive name (e.g., `AddTagsColumn`)
- Avoid generic names like `20240101000000_InitialCreate`

## Environment Configuration

### Configuration Files
- `appsettings.json` - Default configuration
- `appsettings.Development.json` - Development settings
- `appsettings.{Environment}.json` - Environment-specific settings
- `appsettings.{Environment}.UserSecrets.json` - User secrets

### Environment Variables

| Variable | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `Logging:LogLevel:Default` | Default log level |
| `Logging:LogLevel:Microsoft` | EF Core log level |
| `UserSecretsId` | Unique identifier for user secrets |

### Reading Configuration
```csharp
var connection = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");
```

## Common Patterns

### Repository Pattern
```csharp
public interface INoteRepository
{
    Task<Note?> GetByIdAsync(NoteId id);
    Task<IReadOnlyList<Note>> GetAllAsync(List<NoteId> ids);
    Task AddAsync(Note note);
    Task UpdateAsync(Note note);
    Task RemoveAsync(Note note);
}
```

### MediatR Pattern
```csharp
public class CreateNoteCommand : IRequest<CommandResult<NoteDto>>
{
    public NoteTitle Title { get; set; }
    public NoteContent Content { get; set; }
}

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, CommandResult<NoteDto>>
{
    private readonly INoteRepository _repository;
    
    public async Task<CommandResult<NoteDto>> Handle(...)
    {
        // business logic
        await _repository.AddAsync(note);
        await _repository.SaveChangesAsync();
        
        var dto = _mapper.Map(note);
        return CommandResult.Success(dto);
    }
}
```

### DTO Mapping
```csharp
public class NoteDto
{
    public NoteId Id { get; set; }
    public NoteTitle Title { get; set; }
    public NoteContent Content { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
```

## What to Avoid

### Anti-Patterns
- Exposing domain entities via API
- Putting business logic in controllers
- Direct database access in application layer
- Hardcoded values or magic numbers
- Missing error handling
- Silent failures (swallowed exceptions)
- Null references without `null!`

### Bad Practices
- Generic exception handling without specific handling
- Logging PII or sensitive data
- Overly long method functions
- Tight coupling (domain depends on infrastructure)
- Multiple responsibilities in one class

## Additional Guidelines

### SOLID Principles
- **Single Responsibility**: Each class does one thing well
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Subtypes must be substitutable
- **Interface Segregation**: Small, focused interfaces
- **Dependency Inversion**: Depend on abstractions, not concretions

### Performance Considerations
- Avoid N+1 queries (use `.Include()` or projections)
- Batch insert/update when possible
- Use async/await for I/O operations
- Consider caching for read-heavy operations