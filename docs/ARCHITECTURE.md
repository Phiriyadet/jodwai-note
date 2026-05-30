# Architecture

## System Overview

JodWai Note is a note-taking REST API built with ASP.NET Core Web API. The system manages notes with title, content, tags, and note-to-note relationships (links). Users can create, read, update, and delete notes, organize them with tags, and link related notes together.

## Architectural Style

**Clean Architecture** (also known as Hexagonal Architecture) with the following layers:

``` mermaid
graph TD
    A["🌐 Presentation\n(JodWai.Api)\nHTTP API Layer"]
    B["⚙️ Application\n(JodWai.App)\nCQRS Layer"]
    C["🧠 Domain\n(JodWai.Dom)\nBusiness Rules"]
    D["🔧 Infrastructure\n(JodWai.Infra)\nEF Core, DB"]
    E["🐘 PostgreSQL"]

    A --> B
    B --> C
    C --> D
    D --> E
```
Dependency direction flows inward from outer layers to the Domain layer.

## Layer Breakdown

### Presentation Layer (`JodWai.Api`)

**Responsibility**: HTTP request handling, routing, configuration.

**Components**:
- `Controllers/` - HTTP endpoints for notes
- `Program.cs` - Application entry point, dependency injection
- `appsettings.json` - Configuration

**Technologies**:
- ASP.NET Core Web API
- Scalar for OpenAPI/Swagger documentation
- Dependency injection

### Application Layer (`JodWai.Application`)

**Responsibility**: Business workflow orchestration, CQRS commands/queries, DTOs.

**Components**:
- `Commands/` - Write operations (create, update, delete)
- `Queries/` - Read operations (get all, get by id)
- `Dtos/` - Data transfer objects for API contracts
- `Interfaces/` - Abstractions for external collaborators
- `Mappers/` - Entity-to-DTO mapping

**Technologies**:
- MediatR for CQRS pattern
- Repository pattern

### Domain Layer (`JodWai.Domain`)

**Responsibility**: Pure business logic, entities, value objects, domain services. No external dependencies.

**Components**:
- `Entities/Note.cs` - Main aggregate root
- `ValueObjects/` - NoteId, NoteTitle, NoteContent, Tag, NoteLink
- `Interfaces/` - Domain-level interfaces (optional)

**Design Principles**:
- Immutable value objects with validation
- Aggregate root manages internal relationships
- Private setters on critical fields
- Business invariants enforced at construction

### Infrastructure Layer (`JodWai.Infrastructure`)

**Responsibility**: Data persistence, external services, infrastructure concerns.

**Components**:
- `Persistence/AppDbContext.cs` - EF Core DbContext
- `Persistence/Repositories/` - Repository implementations
- `Migrations/` - Database migration files
- `Workers/` - Background jobs

**Technologies**:
- Entity Framework Core 10.0.7
- PostgreSQL via Npgsql
- Microsoft.Extensions.Hosting

## Data Flow

### Read Flow (Query)

```
1. HTTP Request → Controller
2. Controller → Issue Query via MediatR
3. Handler → QueryRepository
4. Repository → Query DbContext
5. DbContext → Execute SQL
6. DbContext → Return entities
7. Mapper → Map to DTO
8. Return HTTP Response
```

### Write Flow (Command)

```
1. HTTP Request → Controller
2. Controller → Issue Command via MediatR
3. Handler → Repository → DbContext
4. DbContext → Execute SQL (INSERT/UPDATE/DELETE)
5. SaveChanges() → Commit transaction
6. Return Result to Client
```

## Key Components

| Component | Responsibility |
|-----------|------|
| `Note` | Aggregate root: title, content, links, tags |
| `INoteRepository` | Abstraction for note data access |
| `AppDbContext` | EF Core context for Entity management |
| `NoteTitle`/`NoteContent`/`Tag` | Value objects with validation |
| `NoteLink` | Represents note-to-note relationships |

## External Dependencies

| Dependency | Purpose |
|-----------|---------|
| PostgreSQL | Primary data store |
| Scalar | API documentation (generated) |

## Deployment Model

- **API Server**: ASP.NET Core Web Application
- **Database**: PostgreSQL instance (container or managed service)
- **Migration Service**: Separate .NET console app for applying migrations
- **Host Application**: .NET 10.0 AppHost (if using composite applications)

## Architectural Decisions

### 1. Clean Architecture Over Modular Monolith

**Rationale**: Domain layer remains pure and testable without infrastructure dependencies. Clear boundaries prevent logic leakage.

**Consequence**: Requires mapper layer; slightly more boilerplate but better maintainability.

### 2. Value Objects for All Domain Objects

**Rationale**: Enforce invariants at boundaries. Validation happens once in constructor.

**Consequence**: More code for each entity field, but catches bugs early.

### 3. Repository Pattern

**Rationale**: Abstraction allows swapping implementations (e.g., in-memory for testing).

**Consequence**: Extra indirection, but enables unit testing of application layer.

### 4. MediatR for CQRS

**Rationale**: Separate concerns between read and write paths. Easy to add new operations.

**Consequence**: More files for each operation, but clearer structure.

### 5. PostgreSQL as Database

**Rationale**: Mature ORM support, rich feature set, reliable.

**Consequence**: Vendor lock-in, requires PostgreSQL license (open source for most cases).

## Known Constraints and Tradeoffs

| Constraint | Tradeoff |
|-----------|---------|
| Pure domain layer | Requires mappers, more code |
| Value objects everywhere | More verbosity, but early validation |
| Repository pattern | Additional abstraction layer |
| Single DB (PostgreSQL) | Limited to one database type |
| Scalar for API docs | Requires build step to generate |