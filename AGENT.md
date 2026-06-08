# JodWai Note - AI Assistant Guide

## Project Summary

JodWai Note is a .NET 10.0 ASP.NET Core Web API application implementing a note-taking system with Clean Architecture. The application manages notes with title, content, tags, and note-to-note relationships. Uses Entity Framework Core with PostgreSQL for persistence and MediatR for CQRS pattern.

## Tech Stack

- **Language**: C# (.NET 10.0)
- **Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core 10.0.7
- **Database**: PostgreSQL
- **CQRS**: MediatR 14.1.0
- **API Docs**: Scalar

## Folder Structure

```
backend/JodWai/src/
├── JodWai.Domain/           # Pure domain layer (entities, value objects)
├── JodWai.Application/      # Commands, queries, DTOs, interfaces
├── JodWai.Infrastructure/   # EF Core, repository implementations
└── JodWai.Api/              # HTTP controllers, configuration
```

## Key Architectural Decisions

1. **Clean Architecture**: Domain layer has no dependencies on Application, Infrastructure, or external services. Dependency direction flows inward.

2. **Value Objects**: All domain objects use value objects with invariant enforcement:
   - `NoteTitle` (max 200 chars, trimmed, non-empty)
   - `NoteContent` (max 10000 chars, trimmed)
   - `Tag` (max 100 chars, lowercase, trimmed)
   - `NoteId` (GUID, non-empty)

3. **Aggregate Root**: `Note` is the aggregate root managing:
   - Title and Content via private setters
   - Internal collections of Links and Tags
   - Invariants: no self-links, at least one field must update

4. **Repository Pattern**: `INoteRepository` abstraction for data access. Implementation lives in Infrastructure.

5. **DTOs**: Application returns DTOs (e.g., `NoteDto`), not domain entities, to prevent API bloat.

## Domain Model

### Entities

| Entity | Key Attributes | Behaviors |
|--------|----------------|-----------|
| `Note` | `Id` (NoteId), `Title`, `Content`, `CreatedAt`, `UpdatedAt` | `Update()`, `SyncLinks()`, `AddTag()`, `RemoveTag()` |

### Value Objects

| Value Object | Fields | Invariants |
|--------------|--------|------------|
| `NoteId` | `Value` (Guid) | Non-empty GUID |
| `NoteTitle` | `Value` (string) | 1-200 chars, trimmed, non-empty |
| `NoteContent` | `Value` (string) | 1-10000 chars, trimmed |
| `Tag` | `Value` (string) | 1-100 chars, lowercase, trimmed |
| `NoteLink` | `TargetId` (NoteId) | No self-links |

### Aggregates

- **Note**: Root aggregate containing all links and tags.

### Domain Services

None yet defined.

## Coding Conventions

### Naming

- **Classes**: PascalCase (`Note`, `INoteRepository`)
- **Interfaces**: `I` prefix (`INoteRepository`)
- **Value Objects**: `record` with `sealed` keyword (`NoteTitle`)
- **Methods**: `CamelCase` for instance, `PascalCase` for static constructors
- **Constants**: `const` with `int` max lengths

### Files

- **Entities**: `JodWai.Domain/Entities/`
- **Value Objects**: `JodWai.Domain/ValueObjects/`
- **Commands**: `JodWai.Application/Commands/`
- **Queries**: `JodWai.Application/Queries/`
- **DTOs**: `JodWai.Application/Dtos/`

## What to Always Do

1. **Validate at Boundaries**: All external input validation happens in Value Object constructors.

2. **Use Transactions**: Repository's `SaveChangesAsync()` wraps data access operations.

3. **Return DTOs, Not Entities**: Map domain entities to DTOs before returning from API.

4. **Track Timestamps**: `CreatedAt` and `UpdatedAt` managed in domain entity.

5. **Use MediatR**: Commands for writes, Queries for reads.

6. **Throw Domain Exceptions**: `ArgumentException` for business rule violations in value objects.

## What to Never Do

1. **Never expose domain entities**: Always map to DTOs in API responses.

2. **Never bypass Value Object constructors**: Always use static `From()` factory methods.

3. **Never put business logic in controllers**: Use commands/queries instead.

4. **Never access database directly**: Use repository abstraction.

5. **Never allow empty value objects**: Validation happens in constructor.

## Testing Expectations

- **Unit tests**: Test domain logic, value object invariants
- **Integration tests**: Test repository with in-memory or test database
- **Arrange-Act-Assert**: Clear test structure
- **Test happy path and edge cases**: Empty values, length limits

## Special Constraints

1. **Note self-reference forbidden**: `NoteLink` throws on self-link creation.

2. **Note links must be re-synced**: `SyncLinks()` rebuilds links against current valid IDs.

3. **At least one field to update**: `Update()` throws if neither title nor content provided.

4. **Tags are lowercase**: Normalized on construction.

5. **.NET 10.0**: Use .NET 10.0 SDK and packages.

6. **PostgreSQL only**: No other database drivers should be added.

## Known Issues / TODO

- Application layer source files (commands, queries, mappers) are not yet populated
- Frontend implementation not yet started