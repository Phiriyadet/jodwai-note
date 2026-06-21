# Architecture Decision Records

This document outlines key architectural decisions made during the development of the JodWai Note project.

## ADR-01: Clean Architecture Style

**Context:** The decision to adopt the Clean Architecture pattern was made to ensure a clear separation of concerns and maintainability. This style separates the application into distinct layers, promoting loose coupling between components.

**Decision:** The system follows the Clean Architecture pattern with the following layers:
1. **Presentation Layer (JodWai.Api):** Handles HTTP requests and responses.
2. **Application Layer (JodWai.App):** Orchestrates business workflows, including command/query handling.
3. **Domain Layer (JodWai.Dom):** Contains core business logic, entities, value objects, and domain services.
4. **Infrastructure Layer (JodWai.Infra):** Manages data persistence and external services.

**Consequences:** This decision ensures that the application is easier to test, maintain, and extend. The separation of concerns allows developers to focus on specific areas without affecting others.

## ADR-02: Repository Pattern

**Context:** To manage data access efficiently and ensure loose coupling with business logic, a repository pattern was adopted. Repositories abstract the data access layer from domain services, allowing for easy changes in persistence mechanisms.

**Decision:** The following repositories are used:
- `INoteRepository`

These repositories provide methods to interact with the database, ensuring that domain services do not directly interact with the DbContext.

**Consequences:** This decision simplifies unit testing by enabling in-memory mocks for repositories. It also improves maintainability and scalability of data access logic.

## ADR-03: MediatR Command/Query Pattern

**Context:** To decouple business logic from controllers, a MediatR command/query pattern was chosen. This pattern separates the handling of commands (requests that change state) and queries (requests for data) into distinct handlers.

**Decision:** The following handlers are used:
- `CreateNoteCommandHandler`
- `UpdateNoteCommandHandler`
- `DeleteNoteCommandHandler`

These handlers process business logic based on incoming commands or queries.

**Consequences:** This decision promotes a clean separation of concerns, making it easier to manage and test business workflows. It also enhances the reusability of command/query handling code across different parts of the application.