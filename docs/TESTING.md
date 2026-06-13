# Testing Philosophy

The testing strategy for the JodWai Note project is to ensure comprehensive coverage of the application's functionality. This document outlines the various test types, structures, and practices employed in the project.

## Test Types
- **Unit Tests:** Focus on individual methods or classes.
- **Integration Tests:** Verify interactions between components or services.
- **End-to-End Tests (E2E):** Simulate user workflows from start to finish.
- **Contract Tests:** Ensure adherence to external APIs and contracts.

## Testing Structure

The testing projects are organized as follows:
- `JodWai.Tests.Unit`: Contains unit tests for individual classes and methods.
- `JodWai.Tests.Integration`: Includes integration tests for various components.
- `JodWai.Tests.EndToEnd`: Simulates user workflows using Selenium or similar tools.

## Running Tests

To run the test suite, execute the following command:
```bash
dotnet test backend/JodWai.Tests.Unit
dotnet test backend/JodWai.Tests.Integration
dotnet test backend/JodWai.Tests.EndToEnd
```

## Unit Test Pattern

Unit tests follow the Arrange-Act-Assert (AAA) pattern. Each test method typically consists of three sections:
1. **Arrange:** Set up the necessary context and inputs.
2. **Act:** Execute the code under test.
3. **Assert:** Verify the expected outcomes.

Example:
```csharp
[Fact]
public async Task GetNoteById_WithValidId_ReturnsExpectedNote()
{
    // Arrange
    var noteId = Guid.NewGuid();
    var noteRepositoryMock = new Mock<INoteRepository>();
    noteRepositoryMock.Setup(repo => repo.GetByIdAsync(noteId)).ReturnsAsync(new Note { Id = noteId });

    var service = new NoteService(noteRepositoryMock.Object);

    // Act
    var result = await service.GetNoteByIdAsync(noteId);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(noteId);
}
```

## Integration Test Pattern

Integration tests verify interactions between components. They typically involve mocking external dependencies and ensuring that the system behaves as expected.

Example:
```csharp
[Fact]
public async Task AddNote_ToValidNote_Succeeds()
{
    // Arrange
    var noteRepositoryMock = new Mock<INoteRepository>();
    noteRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Note>())).Returns(Task.CompletedTask);

    var service = new NoteService(noteRepositoryMock.Object);
    var note = new Note { Title = "Test Note", Content = "This is a test." };

    // Act
    await service.CreateNoteAsync(note);

    // Assert
    noteRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Note>()), Times.Once);
}
```

## Mocking Strategy

The project uses Moq for mocking dependencies in unit and integration tests. Moq provides a flexible way to setup expectations, verify interactions, and return mock objects.

Example:
```csharp
var mockRepository = new Mock<INoteRepository>();
mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Note { Id = Guid.NewGuid() });
```

## Naming Convention

Test methods follow the pattern `[FeatureName]_Action_State`. For example, `GetNoteById_WithValidId_ReturnsExpectedNote`.

## Arrange Act Assert

The AAA pattern is consistently used in unit tests to ensure clarity and readability.

Example:
```csharp
[Fact]
public async Task GetNoteById_WithValidId_ReturnsExpectedNote()
{
    // Arrange
    var noteId = Guid.NewGuid();
    var mockRepository = new Mock<INoteRepository>();
    mockRepository.Setup(repo => repo.GetByIdAsync(noteId)).ReturnsAsync(new Note { Id = noteId });

    var service = new NoteService(mockRepository.Object);

    // Act
    var result = await service.GetNoteByIdAsync(noteId);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(noteId);
}
```

## Coverage

Coverage is monitored using the dotnet CLI:
```bash
dotnet test --logger "console;include-skipped=true" /p:CollectCoverage=True /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./coverage/coverlet.xml
```
Reports are generated using tools like ReportGenerator.

## Known Gaps

- **End-to-End Tests:** Limited E2E coverage due to time and resource constraints.
- **Contract Tests:** Not yet implemented for external APIs.