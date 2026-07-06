# AI Coding Assistant Instructions

## Project Summary
JodWai Note is a note-taking REST API built with ASP.NET Core Web API. The system manages notes with title, content, tags, and note-to-note relationships (links). Users can create, read, update, and delete notes, organize them with tags, and link related notes together.

## Tech Stack
- **Backend:** .NET Core 10.0.7
- **Database:** PostgreSQL
- **Testing Framework:** xUnit + Moq

## Architecture
The architecture follows a Clean Architecture pattern with the following layers:
1. **Presentation Layer (JodWai.Api):** Handles HTTP requests and responses.
2. **Application Layer (JodWai.App):** Orchestrates business workflows, including command/query handling.
3. **Domain Layer (JodWai.Dom):** Contains core business logic, entities, value objects, and domain services.
4. **Infrastructure Layer (JodWai.Infra):** Manages data persistence and external services.

## Component Structure
```
JodWai.Note/
├── backend/
│   ├── JodWai/
│   │   ├── Controllers/ - HTTP endpoints for notes
│   │   ├── Handlers/ - MediatR command/query handlers
│   │   ├── Repositories/ - Repository implementations
│   │   └── ...
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

## Domain Summary
The domain consists of the following key components:
- **Entities:** Note, Tag
- **Value Objects:** NoteId, NoteTitle, NoteContent, Tag, NoteLink
- **Aggregates:** Note

## Coding Conventions
### Always Do
1. Follow Clean Architecture principles.
2. Use DTOs for API contracts.
3. Implement unit tests using xUnit and Moq.

### Never Do
1. Hardcode SQL queries in the application layer.
2. Store secrets directly in code or configuration files.

## Testing Expectations
- Unit tests should cover domain logic, value objects, commands, and queries.
- Integration tests should verify repository implementations and DbContext interactions.
- Always run `dotnet test` to execute all tests.

## Non-obvious Decisions
1. Use Clean Architecture for better separation of concerns and maintainability.
2. Implement value objects with strict validation to enforce invariants at boundaries.
3. Use MediatR for handling commands and queries, ensuring a clean separation between business logic and external services.