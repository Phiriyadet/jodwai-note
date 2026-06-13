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
JodWai-Note/
├── AGENT.md
├── README.md
├── backend/
│   └── JodWai/
│       ├── JodWai.AppHost/
│       │   ├── AppHost.cs
│       │   ├── JodWai.AppHost.csproj
│       ├── JodWai.MigrationService/
│       │   ├── JodWai.MigrationService.csproj
│       │   ├── Program.cs
│       ├── JodWai.ServiceDefaults/
│       │   ├── Extensions.cs
│       │   └── JodWai.ServiceDefaults.csproj
│       ├── JodWai.slnx
│       ├── aspire.config.json
│       ├── src/
│       │   ├── JodWai.Api
│       │   │   ├── Controllers/
│       │   │   │   └── NotesController.cs
│       │   │   ├── Dockerfile
│       │   │   ├── JodWai.Api.csproj
│       │   │   ├── Program.cs
│       │   │   ├── appsettings.Development.json
│       │   │   └── appsettings.json
│       │   ├── JodWai.Application
│       │   │   ├── Behaviors/
│       │   │   │   ├── LoggingBehavior.cs
│       │   │   │   └── ValidationBehavior.cs
│       │   │   ├── Common/
│       │   │   │   └── Results/
│       │   │   │       ├── Error.cs
│       │   │   │       ├── Errors/
│       │   │   │       │   └── NoteErrors.cs
│       │   │   │       └── Result.cs
│       │   │   ├── Extensions/
│       │   │   │   └── ApplicationExtensions.cs
│       │   │   ├── Interfaces/
│       │   │   │   ├── INoteLinkParser.cs
│       │   │   │   ├── INoteRepository.cs
│       │   │   ├── JodWai.Application.csproj
│       │   │   ├── Mappers/
│       │   │   │   └── NoteMapper.cs
│       │   │   └── Notes/
│       │   │       ├── Commands/
│       │   │       │   ├── CreateNote/
│       │   │       │   │   ├── CreateNoteCommand.cs
│       │   │       │   │   └── CreateNoteCommandValidator.cs
│       │   │       │   ├── DeleteNote/
│       │   │       │   │   ├── DeleteNoteCommand.cs
│       │   │       │   │   └── DeleteNoteCommandValidator.cs
│       │   │       │   └── UpdateNote/
│       │   │       │       ├── UpdateNoteCommand.cs
│       │   │       │       └── UpdateNoteCommandValidator.cs
│       │   │       ├── Dtos/
│       │   │       │   ├── NoteDto.cs
│       │   │       │   ├── ParsedNoteLink.cs
│       │   │       │   └── Requests/
│       │   │       │       ├── CreateNoteRequest.cs
│       │   │       │       └── UpdateNoteRequest.cs
│       │   │       └── Queries/
│       │   │           ├── GetAllNotesQuery.cs
│       │   │           ├── GetNoteByIdQuery.cs
│       │   │           └── SearchNotesQuery.cs
│       │   ├── JodWai.Domain/
│       │   │   ├── Entities/
│       │   │   │   └── Note.cs
│       │   │   ├── JodWai.Domain.csproj
│       │   │   └── ValueObjects/
│       │   │       ├── NoteContent.cs
│       │   │       ├── NoteId.cs
│       │   │       ├── NoteLink.cs
│       │   │       ├── NoteTitle.cs
│       │   │       └── Tag.cs
│       │   └── JodWai.Infrastructure/
│       │       ├── Extensions/
│       │       │   └── InfrastructureExtensions.cs
│       │       ├── JodWai.Infrastructure.csproj
│       │       ├── Parsing/
│       │       │   └── WikiStyleNoteLinkParser.cs
│       │       └── Persistence/
│       │           ├── AppDbContext.cs
│       │           ├── Configurations/
│       │           │   └── NoteConfiguration.cs
│       │           ├── Migrations/
│       │           ├── Repositories/
│       │           │   └── NoteRepository.cs
│       │           └── Workers/
│       │               └── MigrationWorker.cs
│       └── tests/
│           ├── JodWai.Tests.Integration/
│           │   └── JodWai.Tests.Integration.csproj
│           └── JodWai.Tests.Unit/
│               ├── Application/
│               │   └── Notes/
│               │       └── Commands/
│               │           ├── CreateNoteCommandTests.cs
│               │           ├── DeleteNoteCommandTests.cs
│               │           └── UpdateNoteCommandTests.cs
│               ├── Constants/
│               │   └── NoteTestConstants.cs
│               ├── Domain/
│               │   ├── Entities/
│               │   │   └── NoteTests.cs
│               │   ├── Shared/
│               │   │   └── NoteBuilder.cs
│               │   └── ValueObjects/
│               │       ├── NoteContentTests.cs
│               │       ├── NoteIdTests.cs
│               │       ├── NoteLinkTests.cs
│               │       ├── NoteTitleTests.cs
│               │       └── TagTests.cs
│               └── JodWai.Tests.Unit.csproj
├── docs/
│   ├── ARCHITECTURE.md
│   ├── CONVENTIONS.md
│   ├── DECISIONS.md
│   ├── DOMAIN.md
│   └── TESTING.md
└── frontend/
    └── JodWai-Web/
        ├── eslint.config.js
        ├── index.html
        ├── package-lock.json
        ├── package.json
        ├── public/
        ├── src/
        │   ├── App.css
        │   ├── App.tsx
        │   ├── assets/
        │   ├── index.css
        │   └── main.tsx
        ├── tsconfig.app.json
        ├── tsconfig.json
        ├── tsconfig.node.json
        └── vite.config.ts
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