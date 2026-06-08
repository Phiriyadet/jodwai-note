# Testing Guide

## Testing Philosophy

JodWai Note follows a layered testing approach aligned with Clean Architecture:
- **Unit tests** for domain logic and application layer (fast, isolated)
- **Integration tests** for infrastructure layer (database interactions)
- **API tests** for HTTP endpoint integration (optional but recommended)

Tests verify behavior at each layer's boundary, not implementation details.

## Test Types and Coverage

### Unit Tests (`JodWai.Tests.Unit`)

**Purpose**: Verify domain logic, value object invariants, command/query handlers.

**What to Test**:
- Domain entity invariants and behaviors
- Value object validation (length, non-empty, format)
- Command handlers (without persistence)
- Query handlers (with mocked repository)
- Mapper correctness

**What NOT to Test**:
- Database connections
- External API calls
- HTTP middleware
- Infrastructure concerns

**Coverage Goal**: High coverage of domain logic (>80%)

### Integration Tests (`JodWai.Tests.Integration`)

**Purpose**: Verify repository implementations, DbContext, migration scripts.

**What to Test**:
- Repository CRUD operations
- DbContext query composition
- Migration correctness
- Transaction behavior
- Concurrent access (if applicable)

**What NOT to Test**:
- Pure domain logic
- Value object validation (use unit tests)

**Coverage Goal**: All infrastructure components tested end-to-end

### API Tests (Optional)

**Purpose**: Verify HTTP endpoint behavior, request/response contracts.

**What to Test**:
- HTTP status codes
- Response DTOs
- Error responses
- Authorization (if implemented)

**Tools**: Use `FluentAssertions` + `RestSharp` or `HttpClient` tests

## Folder Structure

```
JodWai.Tests.Unit/
├── JodWai.Tests.Unit.csproj
├── Domain/
│   ├── Entities/
│   │   └── NoteTests.cs
│   └── ValueObjects/
│       ├── NoteIdTests.cs
│       ├── NoteTitleTests.cs
│       ├── NoteContentTests.cs
│       ├── TagTests.cs
│       └── NoteLinkTests.cs
├── Application/
│   ├── Commands/
│   │   └── CreateNoteCommandTests.cs
│   └── Queries/
│       └── GetNotesQueryTests.cs
└── Mappers/
    └── NoteMapperTests.cs

JodWai.Tests.Integration/
├── JodWai.Tests.Integration.csproj
├── Persistence/
│   ├── AppDbContextTests.cs
│   └── NoteRepositoryTests.cs
└── Workers/
    └── BackgroundJobTests.cs (if applicable)
```

## How to Run All Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test backend/JodWai/src/JodWai.Tests.Unit/JodWai.Tests.Unit.csproj

# Run with coverage (if coverage tool installed)
dotnet test --collect:"XPlat Code Coverage"
```

## How to Write a Unit Test

### Example: Value Object Test

```csharp
[TestClass]
public class NoteTitleTests
{
    private static readonly string ValidTitle = "My Note";
    private static readonly string EmptyTitle = string.Empty;
    private static readonly string TooLongTitle = new string('A', 201);

    [TestMethod]
    public async Task WhenTitleIsValid_ThenCreatedSuccessfully()
    {
        // Arrange
        string title = ValidTitle;

        // Act
        var result = NoteTitle.From(title);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(title, result.Value);
    }

    [TestMethod]
    public async Task WhenTitleIsEmpty_ThenThrowsArgumentException()
    {
        // Arrange
        string title = EmptyTitle;

        // Act
        var action = () => NoteTitle.From(title);

        // Assert
        Assert.ThrowsException<ArgumentException>(action);
    }

    [TestMethod]
    public async Task WhenTitleTooLong_ThenThrowsArgumentException()
    {
        // Arrange
        string title = TooLongTitle;

        // Act
        var action = () => NoteTitle.From(title);

        // Assert
        Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual($"Title cannot exceed {NoteTitle.MaxLength} characters.", 
            action.Message);
    }
}
```

### Example: Entity Test

```csharp
[TestClass]
public class NoteTests
{
    private readonly INoteRepository _repository;

    public NoteTests()
    {
        _repository = new Mock<INoteRepository>(MockBehavior.Strict).Object;
    }

    [TestMethod]
    public async Task WhenCreatingNote_ThenCreatedAtIsSet()
    {
        // Arrange
        var noteId = NoteId.New();
        var title = NoteTitle.From("Test");
        var content = NoteContent.From("Content");

        // Act
        var note = Note.Create(title, content);

        // Assert
        Assert.IsNotNull(note);
        Assert.AreEqual(noteId, note.Id);
        Assert.IsNotNull(note.CreatedAt);
        Assert.AreEqual(note.CreatedAt, note.UpdatedAt);
    }

    [TestMethod]
    public async Task WhenUpdatingBothFields_ThenUpdatedAtIsRefreshed()
    {
        // Arrange
        var noteId = NoteId.New();
        var note = new Note(noteId, NoteTitle.From("Title"), NoteContent.From("Content"));
        var newTitle = NoteTitle.From("New Title");
        var newContent = NoteContent.From("New Content");
        var originalCreated = note.CreatedAt;

        // Act
        note.Update(newTitle, newContent);

        // Assert
        Assert.AreEqual(newTitle, note.Title);
        Assert.AreEqual(newContent, note.Content);
        Assert.AreNotEqual(note.CreatedAt, note.UpdatedAt);
    }

    [TestMethod]
    public async Task WhenUpdatingNeitherField_ThenThrowsException()
    {
        // Arrange
        var note = new Note(NoteId.New(), NoteTitle.From("Title"), NoteContent.From("Content"));

        // Act
        var action = () => note.Update();

        // Assert
        Assert.ThrowsException<InvalidOperationException>(action);
    }
}
```

### Example: Command Handler Test (with Mock Repository)

```csharp
[TestClass]
public class CreateNoteCommandTests
{
    private readonly CreateNoteCommandHandler _handler;
    private readonly Mock<INoteRepository> _repoMock;

    public CreateNoteCommandTests()
    {
        _repoMock = new Mock<INoteRepository>(MockBehavior.Strict);
        _handler = new CreateNoteCommandHandler(
            _repoMock.Object,
            new AutoMapper.MapperConfiguration().CreateMapper());
    }

    [TestMethod]
    public async Task WhenCreatingNote_ThenHandlerSucceeds()
    {
        // Arrange
        var command = new CreateNoteCommand(
            NoteTitle.From("My Note"),
            NoteContent.From("This is my note content"));
        
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Note>()))
            .ReturnsAsync(It.IsAny<Note>());
        
        _repoMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Succeeded);
    }

    [TestMethod]
    public async Task WhenCreatingNote_ThenRepositoryIsCalledOnce()
    {
        // Arrange
        var command = new CreateNoteCommand(
            NoteTitle.From("My Note"),
            NoteContent.From("Content"));
        
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Note>()))
            .ReturnsAsync(It.IsAny<Note>());

        var callCount = 0;
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Note>()))
            .Returns(() => { callCount++; return Task.FromResult(new Note()); });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(1, callCount);
    }
}
```

## How to Write an Integration Test

### Example: Repository Test with Test Database

```csharp
[TestClass]
public class NoteRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly NoteRepository _repository;
    private readonly TestDatabaseFactory _dbFactory;

    [TestInitialize]
    public void Setup()
    {
        _dbFactory = new TestDatabaseFactory();
        _context = _dbFactory.CreateDbContext();
        _repository = new NoteRepository(_context);
    }

    [TestMethod]
    public async Task WhenAddingNote_ThenNoteExistsInDatabase()
    {
        // Arrange
        var title = NoteTitle.From("Integration Test");
        var content = NoteContent.From("Integration test content");

        // Act
        var note = new Note(NoteId.New(), title, content);
        await _repository.AddAsync(note);
        await _context.SaveChangesAsync();

        // Assert
        var savedNote = await _repository.GetByIdAsync(note.Id);
        Assert.IsNotNull(savedNote);
        Assert.AreEqual("Integration Test", savedNote.Title.Value);
    }

    [TestMethod]
    public async Task WhenGettingNotesByIds_ThenAllNotesReturned()
    {
        // Arrange
        var id1 = NoteId.New();
        var id2 = NoteId.New();
        
        var note1 = new Note(id1, NoteTitle.From("Note 1"), NoteContent.From("Content 1"));
        var note2 = new Note(id2, NoteTitle.From("Note 2"), NoteContent.From("Content 2"));
        
        await _repository.AddAsync(note1);
        await _repository.AddAsync(note2);
        await _context.SaveChangesAsync();

        // Act
        var notes = await _repository.GetAllAsync([id1, id2]);

        // Assert
        Assert.AreEqual(2, notes.Count);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _dbFactory.Dispose();
    }
}
```

## What to Mock and What Not to Mock

### Mock:
- **External services**: Email senders, payment gateways, third-party APIs
- **Repository implementations** in application layer tests (use in-memory or mocked)
- **DbContext** in domain tests (use test double)

### Don't Mock:
- **Domain entities**: Test their behaviors directly
- **Value objects**: Test invariants in unit tests
- **Other domain interfaces**: Test real implementations when possible

### Example with Mocking

```csharp
[TestClass]
public class EmailServiceTests
{
    private readonly Mock<IEmailSender> _emailMock;
    private readonly Mock<INoteRepository> _noteRepoMock;
    private readonly SendNotificationCommandHandler _handler;

    public EmailServiceTests()
    {
        _emailMock = new Mock<IEmailSender>(MockBehavior.Strict);
        _noteRepoMock = new Mock<INoteRepository>(MockBehavior.Strict);
        
        _handler = new SendNotificationCommandHandler(
            _noteRepoMock.Object,
            _emailMock.Object);
    }

    [TestMethod]
    public async Task WhenNotificationSent_ThenEmailSentOnce()
    {
        // Arrange
        var noteId = NoteId.New();
        _noteRepoMock
            .Setup(r => r.GetByIdAsync(noteId, CancellationToken.None))
            .ReturnsAsync(new Note(noteId, NoteTitle.From("Test"), NoteContent.From("")))
            .Verifiable();
        
        _emailMock
            .Setup(m => m.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Task.CompletedTask)
            .Verifiable();

        // Act
        await _handler.Handle(new SendNotificationCommand(noteId), CancellationToken.None);

        // Assert
        _emailMock.Verify(m => m.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}
```

## Test Naming Conventions

### Format: `When_{Action}_Then_{ExpectedOutcome}`

### Examples

**Happy Path**:
- `WhenCreatingNote_ThenNoteIsSaved()`
- `WhenGettingNoteById_ThenNoteIsReturned()`
- `WhenUpdatingNote_ThenNoteIsUpdated()`

**Error Cases**:
- `WhenTitleIsEmpty_ThenThrowsArgumentException()`
- `WhenNoteNotFound_ThenReturnsNotFoundResult()`
- `WhenDuplicateContent_ThenThrowsDuplicateContentException()`

**Edge Cases**:
- `WhenTitleAtMaxLength_ThenAccepted()`
- `WhenContentAtMaxLimit_ThenAccepted()`
- `WhenTagIsEmpty_ThenLowercased()`

**Include Subject**:
- `WhenCreatingCreateNoteCommand_ThenSuccess()`
- `WhenDeletingNote_ThenNoteDeleted()`

## Coverage Expectations

- **Domain layer**: >80% coverage (value objects, entities)
- **Application layer**: >70% coverage (commands, queries)
- **Infrastructure layer**: >60% coverage (repositories, DbContext)
- **API layer**: Not measured by unit tests (use integration tests)

## Known Testing Gaps

- **API endpoint tests**: Consider adding `WebApplicationFactory` tests for full integration
- **Background worker tests**: If workers exist, add integration tests
- **Performance tests**: Add load testing scenarios if needed
- **Security tests**: Test authorization and input validation scenarios

## CI/CD Integration

### Test in CI Pipeline

```yaml
# Example GitHub Actions workflow
- name: Run tests
  run: dotnet test --no-restore --verbosity normal
  env:
    ConnectionStrings__DefaultConnection: "Host=localhost;Database=jodwai_note_test"
    Logging__LogLevel:Debug
```

### Test Report Generation

```bash
# Generate HTML test report
dotnet test --logger:"html;logfile=test-results/results.html"
```

## Additional Testing Tools

Consider adding to test project:

- **FluentAssertions**: For more fluent assertions
- **Xunit.SkippableFact**: For platform-specific tests
- **Moq**: Alternative to `Mock` for more complex mocking
- **NSubstitute**: Auto-fakes and better verification
- **BenchmarkDotNet**: Performance benchmarking
- **Testcontainers**: Docker-based integration tests

## Best Practices Summary

1. **Test layer boundaries**: Don't test implementation details
2. **Fast unit tests**: Keep setup/teardown minimal
3. **Arrange-Act-Assert**: Clear test structure
4. **One assertion per test**: Or group logically related assertions
5. **Test public APIs**: Don't test private/internal methods
6. **Use seed data**: In integration tests, don't skip seeding
7. **Parallelize tests**: Use `ParallelizeAsyncContext` or test collections
8. **Clean up resources**: Dispose DbContexts, close connections