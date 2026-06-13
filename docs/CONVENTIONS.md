# Project Conventions

This document outlines the naming and design patterns followed in the JodWai Note project to ensure consistency and readability.

## Naming Conventions

### Files, Folders, Classes, Interfaces, Methods, Variables, Constants
- **Files:** PascalCase (e.g., `NotesController.cs`, `NoteService.cs`)
- **Folders:** PascalCase (e.g., `Controllers`, `Services`, `Repositories`)
- **Classes:** PascalCase (e.g., `Note`, `Tag`, `NoteRepository`)
- **Interfaces:** InterfacePrefix + PascalCase (e.g., `INoteRepository`, `ITagService`)
- **Methods:** PascalCase (e.g., `GetAllNotes()`, `CreateNote(Note note)`)
- **Variables:** camelCase (e.g., `noteId`, `noteTitle`, `noteContent`)
- **Constants:** PascalCase with underscores (e.g., `MAX_NOTE_LENGTH = 256`)

## File Structure

The project follows a structured file layout as shown below:

```
JodWai.Note/
├── backend/
│   ├── JodWai/
│   │   ├── Controllers/ - HTTP endpoints for notes
│   │   ├── Services/ - Business logic services
│   │   ├── Handlers/ - MediatR command/query handlers
│   │   ├── Repositories/ - Data access layers
│   │   └── Domain/ - Entity, value object, and domain service definitions
│   ├── Program.cs - Application entry point and dependency injection
│   └── appsettings.json - Configuration
├── frontend/
│   └── JodWai-Web/
│       ├── src/
│       ├── public/
│       └── package.json (if applicable)
└── docs/
    ├── ARCHITECTURE.md
    ├── CONVENTIONS.md
    ├── DECISIONS.md
    ├── DOMAIN.md
    └── TESTING.md
```

## Error Handling

- **Error Representation:** Custom `ApiException` for all application errors.
- **Propagation:** Errors are propagated up the call stack until caught and handled by middleware or controllers.
- **Exposure:** Error messages are returned to clients with HTTP status codes (e.g., 400 Bad Request, 500 Internal Server Error).

Example:
```csharp
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNote(Guid id)
    {
        try
        {
            var note = await _noteService.GetNoteById(id);
            return Ok(note);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}
```

## Validation

Validation occurs at the application layer and is performed using DTOs and custom validation attributes.

Example:
```csharp
public class NoteDto
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] NoteDto noteDto)
    {
        var note = new Note { Title = noteDto.Title, Content = noteDto.Content };
        await _noteService.CreateNote(note);
        return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
    }
}
```

## Logging

Logging is performed using Serilog with the following conventions:

- **Log Level:** ERROR, WARNING, INFORMATION, DEBUG
- **Format:** `{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}`
- **Output:** Console and file (rolling file)

Example:
```csharp
private static readonly ILogger Logger = LoggerFactory.Create(builder =>
{
    builder.AddSerilog(new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/jodwai-note-.log", rollingInterval: RollingInterval.Day)
        .CreateLogger());
}).CreateLogger();

public async Task CreateNote(Note note)
{
    try
    {
        // Business logic
        Logger.Information("Creating new note with title: {Title}", note.Title);
    }
    catch (Exception ex)
    {
        Logger.Error(ex, "Failed to create note");
        throw;
    }
}
```

## Configuration

Configuration is managed using `appsettings.json` and injected into services.

Example:
```csharp
public class NoteService : INoteService
{
    private readonly AppDbContext _context;

    public NoteService(AppDbContext context)
    {
        _context = context;
    }

    // Business logic methods
}
```

## Migrations

Migrations are managed using Entity Framework Core and follow the conventional naming convention.

Example:
```bash
dotnet ef migrations add InitialCreate -c AppDbContext
```

## Testing

- **Unit Tests:** Located in `backend/JodWai.Tests.Unit`.
  - Naming convention: `[FeatureName]Tests.cs`
  - Example: `NoteServiceTests.cs`
- **Integration Tests:** Located in `backend/JodWai.Tests.Integration`.
  - Naming convention: `[FeatureName]IntegrationTests.cs`
  - Example: `NoteRepositoryIntegrationTests.cs`

## Patterns In Use

- **Clean Architecture:** Separation of concerns with clear layers.
- **CQRS (Command Query Responsibility Segregation):** Commands and queries are handled separately.

Example:
```csharp
public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Unit>
{
    private readonly INoteRepository _noteRepository;

    public CreateNoteCommandHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public async Task<Unit> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new Note { Title = request.Title, Content = request.Content };
        await _noteRepository.AddAsync(note);
        return Unit.Value;
    }
}
```

## What To Avoid

- **Hardcoded SQL Queries:** All database access should be abstracted through repositories.
- **Direct Access to DbContext in Domain Layer:** Business logic should not directly interact with the database.
- **Unnecessary Complexity:** Only implement features that add value, avoiding over-engineering.