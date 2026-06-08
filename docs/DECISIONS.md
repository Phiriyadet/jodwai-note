# Architecture Decision Records (ADR)

This document records significant architectural decisions made in the JodWai Note project. Each entry includes context, rationale, and consequences.

---

## ADR-001: Clean Architecture vs. Modular Monolith

**Status**: Accepted  
**Date**: 2026-05-28

### Context
The project needed a layered architecture that separates business logic from infrastructure concerns. Two common approaches exist:
- **Modular Monolith**: Single codebase with folders per module
- **Clean Architecture**: Strict layering with domain layer having no external dependencies

### Decision
Chose **Clean Architecture** with distinct projects:
- `JodWai.Domain` - Pure domain layer
- `JodWai.Application` - Application services (CQRS, DTOs)
- `JodWai.Infrastructure` - EF Core, repositories
- `JodWai.Api` - HTTP API layer

### Rationale
- Domain layer remains testable in isolation
- Clear boundaries prevent logic leakage
- Easier to replace infrastructure (e.g., switch databases)
- Supports future microservice extraction

### Consequences
**Positive**:
- Domain logic is pure and portable
- Easy to unit test without mocks for domain
- Clear dependency direction

**Negative**:
- Requires mapper layer (extra code)
- More files for each operation
- Slightly steeper learning curve

---

## ADR-002: Value Objects for All Domain Objects

**Status**: Accepted  
**Date**: 2026-05-28

### Context
Decided how to represent domain objects:
- **Entity-based**: Mutable objects with identity
- **Value Objects**: Immutable objects defined by value, not identity

### Decision
Use **sealed record value objects** for all domain attributes:
- `NoteId` (identifier)
- `NoteTitle` (string with validation)
- `NoteContent` (string with validation)
- `Tag` (string with normalization)
- `NoteLink` (reference object)

### Rationale
- **Single validation point**: Constructor checks once
- **Early failure**: Invalid input rejected immediately
- **Immutable**: No side effects, thread-safe
- **Equatable**: Natural equality checks

### Consequences
**Positive**:
- Catches bugs early (invalid data rejected at boundary)
- No mutable state bugs
- Clear invariants enforced

**Negative**:
- More boilerplate code
- More verbose constructors
- Slower development initially

---

## ADR-003: Repository Pattern for Data Access

**Status**: Accepted  
**Date**: 2026-05-28

### Context
How to abstract data access:
- **Direct EF Core**: Call DbContext directly
- **Repository Pattern**: Abstraction layer above DbContext

### Decision
Implement **repository pattern** with:
- `INoteRepository` interface in Application
- `NoteRepository` implementation in Infrastructure
- `Note` entities loaded as single aggregate

### Rationale
- **Testable**: Mock repository for unit tests
- **Swappable**: Change implementation (e.g., in-memory for tests)
- **Encapsulated**: Query logic hidden from Application layer

### Consequences
**Positive**:
- Easy to unit test Application layer
- Can switch to in-memory storage for tests
- Clear separation of concerns

**Negative**:
- Extra abstraction layer
- Slight performance overhead
- More code to maintain

---

## ADR-004: MediatR for CQRS Pattern

**Status**: Accepted  
**Date**: 2026-05-28

### Context
Choose a pattern for separating read and write operations:
- **Action/Result**: Generic async operations
- **CQRS (Command/Query)**: Explicit read/write separation

### Decision
Use **MediatR** for CQRS:
- `IRequest<T>` / `IRequestHandler<T>` for commands
- `IQuery<T>` / `IQueryHandler<T>` for queries
- Separate folders: `Commands/` and `Queries/`

### Rationale
- **Explicit separation**: Clear read vs write paths
- **Extensible**: Easy to add new operations
- **Type-safe**: Strongly typed requests/responses
- **Widely adopted**: Good documentation and community

### Consequences
**Positive**:
- Clear separation of concerns
- Easy to add logging/tracing per operation
- Scalable for many operations

**Negative**:
- More files per operation
- Dependency on MediatR library
- Slight verbosity

---

## ADR-005: PostgreSQL as Primary Database

**Status**: Accepted  
**Date**: 2026-05-28

### Context
Select database technology for persistence:
- **SQLite**: Simple, file-based
- **PostgreSQL**: Feature-rich, relational
- **SQL Server**: Microsoft ecosystem

### Decision
Use **PostgreSQL** with **Npgsql** provider.

### Rationale
- **Mature**: Long-standing, well-maintained
- **ORM Support**: Excellent EF Core provider
- **Feature-rich**: JSONB, full-text search, etc.
- **Open Source**: No licensing cost
- **Reliable**: Enterprise-grade stability

### Consequences
**Positive**:
- Rich feature set
- Excellent tooling
- Strong community

**Negative**:
- Vendor lock-in (minor)
- Requires PostgreSQL license (for enterprise features)
- Team may need PostgreSQL training

---

## ADR-006: AutoMapper for DTO Mapping

**Status**: Accepted  
**Date**: 2026-05-28

### Context
How to map domain entities to API response DTOs:
- **Manual mapping**: Inline `Map()` calls
- **AutoMapper**: Automatic configuration

### Decision
Use **AutoMapper** with configuration in `Application` project.

### Rationale
- **Declarative**: Map profiles are easy to read
- **Type-safe**: Compile-time validation
- **Maintainable**: Centralized mapping logic
- **EF Core Integration**: Project-to-query support

### Consequences
**Positive**:
- Less boilerplate code
- Centralized mapping rules
- Easy to change DTO structure

**Negative**:
- Additional dependency
- Mapping can hide logic (need to review profiles)
- Performance overhead (negligible)

---

## ADR-007: Scalar for API Documentation

**Status**: Accepted  
**Date**: 2026-05-28

### Context
Choose API documentation approach:
- **Swagger/OpenAPI**: Microsoft standard
- **Scalar**: Modern, lightweight alternative

### Decision
Use **Scalar** for OpenAPI/Swagger documentation.

### Rationale
- **Modern UI**: Clean, responsive interface
- **Auto-generated**: From attributes
- **Lightweight**: No heavy framework
- **Experimental features**: Early access to new OpenAPI

### Consequences
**Positive**:
- Great developer experience
- Clean UI
- Easy to update

**Negative**:
- Requires build step to generate
- Less mature than Swagger
- Smaller community

---

## ADR-008: Entity Framework Core 10.0

**Status**: Accepted  
**Date**: 2026-05-28

### Context
Choose ORM version:
- **Entity Framework 8/9**: Older stable
- **Entity Framework 10**: Latest LTS

### Decision
Use **Entity Framework Core 10.0** (or latest available).

### Rationale
- **Latest features**: Includes EF Core 10 improvements
- **Long-term support**: Stable and maintained
- **Performance**: Optimized for current use cases
- **Compatibility**: Works with .NET 10

### Consequences
**Positive**:
- Latest performance improvements
- Access to new features
- Better developer tools

**Negative**:
- May need to update frequently
- Breaking changes in future versions

---

## ADR-009: User Secrets for Local Configuration

**Status**: Accepted  
**Date**: 2026-05-28

### Context
Manage sensitive configuration locally:
- **Hardcoded**: Direct in appsettings
- **User Secrets**: Visual Studio feature
- **Environment Variables**: OS-level

### Decision
Use **User Secrets** for local development.

### Rationale
- **Secure**: Encrypted, local-only
- **Visual Studio integration**: Built-in support
- **No commit**: Not stored in Git
- **Easy**: Simple to set up

### Consequences
**Positive**:
- Sensitive data never committed
- Clean repo history
- Easy to reset

**Negative**:
- Visual Studio only (not cross-platform)
- Requires UserSecretsId setup
- Not for production

---

## ADR-010: Domain Exceptions for Business Rules

**Status**: Accepted  
**Date**: 2026-05-28

### Context
How to communicate business rule violations:
- **Generic exceptions**: Catch-all `Exception`
- **Domain exceptions**: Specific business exceptions

### Decision
Use **domain-specific exceptions**:
- `ArgumentException` for invalid input (value objects)
- `InvalidOperationException` for business rule violations
- Let exceptions propagate (don't catch/swallow)

### Rationale
- **Semantic**: Clear error meanings
- **Testable**: Can assert specific exceptions
- **Framework standard**: .NET built-in types
- **No custom exceptions**: Uses existing types

### Consequences
**Positive**:
- Clear error semantics
- Easy to test
- No custom exception types to maintain

**Negative**:
- Multiple exception types to handle
- Caller must check/convert exceptions
- Slightly more verbose error handling

---

## Summary

| ADR | Decision | Status |
|-----|----------|--------|
| ADR-001 | Clean Architecture | Accepted |
| ADR-002 | Value Objects for All | Accepted |
| ADR-003 | Repository Pattern | Accepted |
| ADR-004 | MediatR for CQRS | Accepted |
| ADR-005 | PostgreSQL Database | Accepted |
| ADR-006 | AutoMapper for Mapping | Accepted |
| ADR-007 | Scalar for Docs | Accepted |
| ADR-008 | EF Core 10.0 | Accepted |
| ADR-009 | User Secrets | Accepted |
| ADR-010 | Domain Exceptions | Accepted |

**Note**: Future decisions should be added as new entries following this template.