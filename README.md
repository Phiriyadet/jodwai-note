# JodWai Note

A note-taking REST API built with ASP.NET Core Web API. Features include notes with title/content, tags, and note-to-note links for organizing related content.

## Tech Stack

- **Language**: C# (.NET 10.0)
- **Framework**: ASP.NET Core Web API
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, API layers)
- **ORM**: Entity Framework Core 10.0.7
- **Database**: PostgreSQL
- **CQRS Pattern**: MediatR 14.1.0
- **API Documentation**: Scalar

## Prerequisites

- .NET 10.0 SDK
- PostgreSQL client or Docker
- `dotnet` CLI

## Local Setup

1. Restore dependencies:
   ```bash
   dotnet restore
   ```

2. Configure connection string in `backend/JodWai/src/JodWai.Api/appsettings.json` or use user secrets for development.

3. Apply migrations:
   ```bash
   dotnet ef database update --project JodWai.Infrastructure --startup JodWai.AppHost
   ```

4. Run the application:
   ```bash
   dotnet run --project backend/JodWai/src/JodWai.Api/JodWai.Api.csproj
   ```

## Run Tests

```bash
dotnet test backend/JodWai/src/JodWai.Tests.Unit/JodWai.Tests.Unit.csproj
dotnet test backend/JodWai/src/JodWai.Tests.Integration/JodWai.Tests.Integration.csproj
```

## Key Environment Variables

- `ConnectionStrings:DefaultConnection` - PostgreSQL connection string
- `Logging:LogLevel:Default` - Default log level
- `UserSecretsId` - For development credentials
- `Logging:LogLevel:Microsoft` - EF Core log level

## Project Structure

```
d:\CodingProjects\jodwai-note
├── backend/
│   └── JodWai/
│       ├── JodWai.AppHost/          # ASP.NET Host application
│       ├── JodWai.MigrationService/ # Database migrations runner
│       └── src/
│           ├── JodWai.Api/          # HTTP API layer
│           ├── JodWai.Application/  # CQRS commands, queries, DTOs
│           ├── JodWai.Domain/       # Entities, value objects, business rules
│           └── JodWai.Infrastructure/# EF Core, repository implementations
└── frontend/                         # Frontend application (if applicable)
```

## Documentation

- [Architecture](docs/ARCHITECTURE.md) - System design and layer responsibilities
- [Domain Model](docs/DOMAIN.md) - Entities, value objects, business rules
- [Conventions](docs/CONVENTIONS.md) - Code style and project guidelines